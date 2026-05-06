using DiplomServer.Application.Interfaces;
using DiplomServer.Domain.Entities;
using DiplomServer.Infrastructure.Data;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace DiplomServer
{
    public class ExcelParser : IExcelParser
    {
        private readonly ScheduleDbContext _scheduleContext;
        private readonly VrDbContext _vrContext;
        private readonly AppDbContext _appDbContext;

        public ExcelParser(
            ScheduleDbContext scheduleContext,
            VrDbContext vrContext,
            AppDbContext appDbContext)
        {
            _scheduleContext = scheduleContext;
            _vrContext = vrContext;
            _appDbContext = appDbContext;
        }
        public List<BadGrade> GetBadGrades(string filePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var dataset = reader.AsDataSet();
                var table = dataset.Tables[0];

                // Поиск строки с заголовком "Студент"
                int headerRowIndex = -1;
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var firstCell = table.Rows[i][0]?.ToString();
                    if (!string.IsNullOrWhiteSpace(firstCell) && firstCell == "Студент")
                    {
                        headerRowIndex = i;
                        break;
                    }
                }
                if (headerRowIndex == -1)
                    throw new Exception("Заголовок 'Студент' не найден");

                var headerRow = table.Rows[headerRowIndex];

                int[] disciplineIndexes = { 3, 7, 9, 11, 13, 15, 17, 19, 21, 23 };
                int[] gradeIndexes = { 3, 7, 10, 12, 14, 16, 18, 20, 22, 24 };

                List<string> subjectNames = new List<string>();
                foreach (int idx in disciplineIndexes)
                {
                    string subject = headerRow[idx]?.ToString();
                    if (string.IsNullOrEmpty(subject)) subject = "—";
                    subjectNames.Add(subject);
                }

                var badGrades = new List<BadGrade>();
                int autoId = 1;

                int startRow = headerRowIndex + 1;
                for (int i = startRow; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    string fio = row[0]?.ToString();
                    if (string.IsNullOrEmpty(fio) || fio == "Студент") continue;

                    for (int j = 0; j < gradeIndexes.Length; j++)
                    {
                        string gradeStr = row[gradeIndexes[j]]?.ToString();
                        bool isBad = string.IsNullOrEmpty(gradeStr) ||
                                         gradeStr == "Не введено" ||
                                         gradeStr == "2" ||
                                         gradeStr == "2,0";

                        if (isBad)
                        {
                            int gradeValue = 0;
                            if (gradeStr == "2" || gradeStr == "2,0")
                                gradeValue = 2;

                            string discipline = subjectNames[j];

                            badGrades.Add(new BadGrade
                            {
                                Id = autoId++,
                                FIO = fio,
                                NameDiscipline = discipline,
                                Grade = gradeValue
                            });
                        }
                    }
                }
                ToDataBase(badGrades);
                return badGrades;
            }
        }
        private void ToDataBase(List<BadGrade> badGrades)
        {
            // 1. Загружаем уже существующие долги, чтобы не добавлять их снова
            var existingDebts = _appDbContext.AcademicDebts
                .Select(d => new { d.StudentId, d.DisciplineId })
                .ToList();

            var studentDict = _vrContext.Students
                .AsEnumerable()
                .ToDictionary(
                    s => $"{s.Lastname} {s.Firstname} {s.Surname}".Trim(),
                    s => s.Id,
                    StringComparer.OrdinalIgnoreCase
                );

            var allDisciplines = _scheduleContext.Disciplines.ToList();
            var debts = new List<AcademicDebts>();

            foreach (var item in badGrades)
            {
                if (studentDict.TryGetValue(item.FIO.Trim(), out uint studentId))
                {
                    int disciplineId = GetBestMatchDisciplineId(item.NameDiscipline, allDisciplines);

                    if (disciplineId != -1)
                    {
                        uint discId = (uint)disciplineId;

                        // 2. Проверка на дубликат в БД (сравниваем через Any)
                        bool exists = existingDebts.Any(d => d.StudentId == studentId && d.DisciplineId == discId);

                        // Также проверяем, не добавили ли мы этот долг уже в текущем цикле
                        bool alreadyInList = debts.Any(d => d.StudentId == studentId && d.DisciplineId == discId);

                        if (!exists && !alreadyInList)
                        {
                            debts.Add(new AcademicDebts
                            {
                                StudentId = studentId,
                                DisciplineId = discId
                            });
                        }
                    }
                }
            }

            if (debts.Any())
            {
                _appDbContext.AcademicDebts.AddRange(debts);
                _appDbContext.SaveChanges();
            }
        }


        private int GetBestMatchDisciplineId(string excelName, List<ScheduleDiscipline> allDisciplines)
        {
            string normalizedExcel = NormalizeName(excelName);

            ScheduleDiscipline bestMatch = null;
            int minDistance = int.MaxValue;
            int threshold = 10;

            foreach (var discipline in allDisciplines)
            {
                string normalizedDb = NormalizeName(discipline.Name);

                // 1. Приоритет вхождения (самое точное совпадение)
                if (normalizedExcel == normalizedDb) return (int)discipline.Id;

                // 2. Считаем расстояние
                int dist = CalculateLevenshteinDistance(normalizedExcel, normalizedDb);

                // 3. Запоминаем лучший вариант, но не возвращаем сразу
                if (dist < minDistance)
                {
                    minDistance = dist;
                    bestMatch = discipline;
                }
            }

            // Возвращаем ID только если нашли что-то достаточно близкое
            return (bestMatch != null && minDistance <= threshold) ? (int)bestMatch.Id : -1;
        }
        private string NormalizeName(string name)
        {
            return name.ToLower()
                       .Replace("программного обеспечения", "по")
                       .Replace("информационные технологии", "ит")
                       .Replace("  ", " ") 
                       .Trim();
        }
        public static int CalculateLevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 0; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }

    }

    
    public class BadGrade
    {
        public int Id { get; set; }
        public string FIO { get; set; }
        public string NameDiscipline { get; set; }
        public int Grade { get; set; }
    }
}
