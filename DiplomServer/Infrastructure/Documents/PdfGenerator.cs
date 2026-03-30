using DiplomServer.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DiplomServer.Infrastructure.Documents
{
    public class PdfGenerator
    {
        public byte[] GenerateRetakeDirectionPdf(RetakeDirection direction)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Column(column =>
                        {
                            column.Item().Text("Направление на пересдачу")
                                .FontSize(18)
                                .Bold()
                                .AlignCenter();

                            column.Item().PaddingTop(10).Text($"Номер: {direction.Number ?? "-"}");
                            column.Item().Text($"Дата пересдачи: {direction.RetakeDate:dd.MM.yyyy}");
                            column.Item().Text($"Статус: {direction.Status}");
                        });

                    page.Content()
                        .PaddingVertical(20)
                        .Column(column =>
                        {
                            column.Spacing(8);

                            column.Item().Text($"Группа: {direction.GroupDiscipline.Group.Name}");
                            column.Item().Text($"Дисциплина: {direction.GroupDiscipline.Discipline.Name}");
                            column.Item().Text($"Тип аттестации: {direction.GroupDiscipline.AttestType.Name}");
                            column.Item().Text($"Семестр: {direction.GroupDiscipline.Semester}");
                            column.Item().Text($"Учебный год: {direction.GroupDiscipline.StudyYear}");

                            column.Item().PaddingTop(15).Text("Список студентов").Bold();

                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30);
                                    columns.RelativeColumn(4);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("№").Bold();
                                    header.Cell().Element(CellStyle).Text("Студент").Bold();
                                    header.Cell().Element(CellStyle).Text("Оценка").Bold();
                                    header.Cell().Element(CellStyle).Text("Сдал").Bold();
                                    header.Cell().Element(CellStyle).Text("Дата").Bold();
                                });

                                var students = direction.RetakeDirectionStudents
                                    .OrderBy(x => x.Student.LastName)
                                    .ThenBy(x => x.Student.FirstName)
                                    .ToList();

                                for (int i = 0; i < students.Count; i++)
                                {
                                    var item = students[i];

                                    table.Cell().Element(CellStyle).Text((i + 1).ToString());
                                    table.Cell().Element(CellStyle).Text($"{item.Student.LastName} {item.Student.FirstName} {item.Student.Surname}".Trim());
                                    table.Cell().Element(CellStyle).Text(item.RetakeGradeValue.ToString());
                                    table.Cell().Element(CellStyle).Text(item.RetakeIsPassed ? "Да" : "Нет");
                                    table.Cell().Element(CellStyle).Text(item.RetakeGradeDate.ToString("dd.MM.yyyy"));
                                }
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Сгенерировано: ");
                            x.Span(DateTime.Now.ToString("dd.MM.yyyy HH:mm"));
                        });
                });
            });

            return document.GeneratePdf();
        }

        private static IContainer CellStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(5)
                .PaddingHorizontal(6);
        }
    }
}