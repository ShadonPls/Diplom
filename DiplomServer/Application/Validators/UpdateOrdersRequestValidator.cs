using FluentValidation;
using DiplomServer.Application.DTOs.Orders;

namespace DiplomServer.Application.Validators
{
    public class UpdateOrdersRequestValidator : AbstractValidator<UpdateOrdersRequestDto>
    {
        public UpdateOrdersRequestValidator()
        {
            RuleFor(x => x.Number)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Number))
                .WithMessage("Номер заказа не должен превышать 100 символов.");

            RuleFor(x => x.StudentId)
                .GreaterThan(0u)
                .When(x => x.StudentId.HasValue)
                .WithMessage("StudentId должен быть больше 0.");

            RuleFor(x => x.DisciplineId)
                .GreaterThan(0u)
                .When(x => x.DisciplineId.HasValue)
                .WithMessage("DisciplineId должен быть больше 0.");

            RuleFor(x => x.DateCreate)
                .NotEmpty()
                .When(x => x.DateCreate.HasValue)
                .WithMessage("DateCreate обязателен при указании.");

            RuleFor(x => x.DateReceptionTeacher)
                .GreaterThanOrEqualTo(DateTime.UtcNow.AddDays(-365 * 10))
                .When(x => x.DateReceptionTeacher.HasValue)
                .WithMessage("Дата приема преподавателем должна быть разумной.");

            RuleFor(x => x.DateReceptionCommission)
                .GreaterThanOrEqualTo(DateTime.UtcNow.AddDays(-365 * 10))
                .When(x => x.DateReceptionCommission.HasValue)
                .WithMessage("Дата приема комиссией должна быть разумной.");

            RuleFor(x => x.ReceptionTeachers)
            .NotNull()
            .WithMessage("ReceptionTeachers не должен быть null.");

            RuleForEach(x => x.ReceptionTeachers!)
                .ChildRules(teachers =>
                {
                    teachers.RuleFor(t => t.TeacherId)
                        .GreaterThan(0u)
                        .WithMessage("TeacherId в ReceptionTeachers должен быть больше 0.");
                })
                .When(x => x.ReceptionTeachers != null && x.ReceptionTeachers.Any());

            RuleFor(x => x.ReceptionCommission)
                .NotNull()
                .WithMessage("ReceptionCommissions не должен быть null.");

            RuleForEach(x => x.ReceptionCommission!)
                .ChildRules(commissions =>
                {
                    commissions.RuleFor(c => c.TeacherId)
                        .GreaterThan(0u)
                        .WithMessage("TeacherId в ReceptionCommissions должен быть больше 0.");
                })
                .When(x => x.ReceptionCommission != null && x.ReceptionCommission.Any());
        }
    }
}