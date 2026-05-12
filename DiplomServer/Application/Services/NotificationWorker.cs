using DiplomServer.Application.DTOs.Common;
using DiplomServer.Application.Interfaces;
using DiplomServer.Infrastructure.Data;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Linq;
using System.Text;

public class NotificationWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<NotificationWorker> _logger;
    private readonly EmailSettings _emailSettings;

    public NotificationWorker(
        IServiceProvider services,
        ILogger<NotificationWorker> logger,
        IOptions<EmailSettings> emailSettings)
    {
        _services = services;
        _logger = logger;
        _emailSettings = emailSettings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Сервис уведомлений запущен.");

        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await SendRemindersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверке расписания.");
            }
        }
    }

    private async Task SendRemindersAsync()
    {
        using IServiceScope scope = _services.CreateScope();
        var dbOrders = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var dbUsers = scope.ServiceProvider.GetRequiredService<ScheduleDbContext>();
        

        var targetTime = DateTime.Now.AddHours(1);
        var start = targetTime.AddMinutes(-1);
        var end = targetTime.AddMinutes(1);

        var teacherSchedules = await dbOrders.ReceptionTeachers
            .Where(rt => rt.Order.DateReceptionTeacher >= start && rt.Order.DateReceptionTeacher <= end &&
                         rt.Order.DateReceptionTeacher.Minute == targetTime.Minute)
            .Select(rt => new TeacherScheduleInfo
            {
                TeacherId = rt.TeacherId,
                OrderId = rt.OrderId,
                OrderType = "Прием преподавателя",
                ScheduleDate = rt.Order.DateReceptionCommission,
                StudentId = rt.Order.StudentId
            })
            .Union(dbOrders.ReceptionCommissions
                .Where(rc => rc.Order.DateReceptionCommission >= start && rc.Order.DateReceptionCommission <= end)
                .Select(rc => new TeacherScheduleInfo
                {
                    TeacherId = rc.TeacherId,
                    OrderId = rc.OrderId,
                    OrderType = "Прием комиссии",
                    ScheduleDate = rc.Order.DateReceptionCommission,
                    StudentId = rc.Order.StudentId
                }))
            .Distinct()
            .ToListAsync();

        if (!teacherSchedules.Any()) return;
        var teacherGroups = teacherSchedules
        .GroupBy(t => t.TeacherId)
        .ToDictionary(g => g.Key, g => g.ToList());

        var teacherIds = teacherGroups.Keys.Select(id => (uint)id).ToList();

        var emails = await dbUsers.Users
            .Where(u => teacherIds.Contains((uint)u.IdUser))
            .Select(u => new { u.IdUser, u.Login })
            .ToListAsync();

        foreach (var item in emails)
        {
            try
            {
                var schedules = teacherGroups[item.IdUser];

                var message = await BuildEmailMessage(schedules, targetTime);

                await SendEmail(
                    item.Login,
                    $"Напоминание о приеме - {schedules.Count} запись(ей)",
                    message
                );

                _logger.LogInformation($"Уведомление отправлено преподавателю {item.IdUser} на {item.Login} (Приемов: {schedules.Count})");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка отправки письма преподавателю {item.IdUser}");
            }
        }
    }


    private async Task<string> BuildEmailMessage(List<TeacherScheduleInfo> schedules, DateTime targetTime)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Уважаемый преподаватель!");
        sb.AppendLine();
        sb.AppendLine($"У вас запланирован(ы) прием(ы) через час (в {targetTime:HH:mm}).");
        sb.AppendLine();
        sb.AppendLine("Детали:");
        sb.AppendLine("-------");

        foreach (var schedule in schedules)
        {
            // Получаем информацию о студенте
            var studentName = "Не указан";
            var studentGroup = "Не указан";
            try
            {
                var student = await GetStudent(schedule.StudentId);
                studentName = student?.Name ?? "Не указан";
                studentGroup = student.Group;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Не удалось получить данные студента ID: {schedule.StudentId}");
            }

            sb.AppendLine($"Тип: {schedule.OrderType}");
            sb.AppendLine($"Студент: {studentName}");
            sb.AppendLine($"Группа: {studentGroup}");
            sb.AppendLine("-------");
        }

        sb.AppendLine();
        sb.AppendLine("Пожалуйста, подготовьтесь к приему заранее.");

        return sb.ToString();
    }

    private async Task<StudentTypeDto> GetStudent(uint id)
    {
        using IServiceScope scope = _services.CreateScope();
        var dbStudents = scope.ServiceProvider.GetRequiredService<VrDbContext>();
        var students = await dbStudents.Students
                .Where(s => s.Id == id)
                .Select(s => new StudentTypeDto
                {
                    Id = (int)s.Id,
                    Name = $"{s.Lastname} {s.Firstname} {s.Surname}",
                    Group = $"{s.Group}"
                })
                .FirstOrDefaultAsync();
        return students;
    }

    private async Task SendEmail(string email, string subject, string message)
    {
        try
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };

            using var client = new SmtpClient();

            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email отправлен на {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке email на {Email}", email);
            throw;
        }
    }
}
public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string UseSsl { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
}

public class TeacherScheduleInfo
{
    public uint TeacherId { get; set; }
    public uint OrderId { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public DateTime ScheduleDate { get; set; }
    public uint StudentId { get; set; }
}