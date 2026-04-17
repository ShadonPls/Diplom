using DiplomServer.Application.DTOs.Auth;
using DiplomServer.Application.DTOs.RetakeDirections;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace DiplomServer.Infrastructure.Documents
{
    public class PdfGenerator
    {
        // PdfGenerator.cs

        public byte[] GenerateRetakeDirectionPdf(
            IList<(RetakeDirectionDetailsDto Direction, CurrentUserDto Teacher)> items)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                for (int i = 0; i < items.Count; i += 2)
                {
                    var left = items[i];
                    var hasRight = i + 1 < items.Count;
                    var right = hasRight ? items[i + 1] : default;

                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(10);
                        page.DefaultTextStyle(x => x.FontSize(8).FontFamily("Arial"));
                        page.Header().Height(0);

                        page.Content()
                            .Padding(5)
                            .Row(row =>
                            {
                                row.RelativeColumn(0.5f)
                                   .PaddingRight(5)
                                   .Element(ComposeDocument(left.Direction, left.Teacher));

                                if (hasRight)
                                {
                                    row.RelativeColumn(0.5f)
                                       .PaddingLeft(5)
                                       .Element(ComposeDocument(right.Direction, right.Teacher));
                                }
                                else
                                {
                                    // Пустая правая половина
                                    row.RelativeColumn(0.5f).Element(c => c.Text(""));
                                }
                            });
                    });
                }
            });

            return document.GeneratePdf();
        }

        private static Action<IContainer> ComposeDocument(RetakeDirectionDetailsDto direction, CurrentUserDto teacher)
        {
            return container =>
            {
                container
                    .Column(col =>
                    {
                        var monthLow = DateTime.Today.ToString("MMMM", new CultureInfo("ru-RU"));
                        var month = char.ToUpper(monthLow[0]) + monthLow[1..];

                        col.Spacing(6);

                        // Заголовок
                        col.Item().Column(hcol =>
                        {
                            hcol.Spacing(2);
                            hcol.Item().Text("ПЕРМСКИЙ АВИАЦИОННЫЙ ТЕХНИКУМ ИМ. А.Д. ШВЕЦОВА")
                                .FontSize(8).AlignCenter().Bold();
                            hcol.Item().Text("НАПРАВЛЕНИЕ НА ПЕРЕСДАЧУ")
                                .FontSize(10).AlignCenter().Bold();
                            hcol.Item().Text("оценки промежуточной аттестации")
                                .FontSize(10).AlignCenter();
                        });

                        col.Item().Text("Форма промежуточной аттестации:").FontSize(8).Bold();

                        var attestTypeName = direction.AttestationType?.Name ?? string.Empty;

                        col.Item().Row(row =>
                        {
                            row.ConstantColumn(70).Text(attestTypeName == "Экзамен" ? "☑ Экзамен" : "☐ Экзамен");
                            row.ConstantColumn(15).Text("");
                            row.ConstantColumn(70).Text(attestTypeName == "Диф.зачёт" ? "☑ Диф.зачет" : "☐ Диф.зачет");
                            row.ConstantColumn(15).Text("");
                            row.RelativeColumn().Text(attestTypeName == "Зачёт" ? "☑ Зачет" : "☐ Зачет");
                        });

                        col.Item().Row(row =>
                        {
                            row.ConstantColumn(70).Text(attestTypeName == "КП/КР" ? "☑ КП/КР" : "☐ КП/КР");
                            row.ConstantColumn(15).Text("");
                            row.ConstantColumn(70).Text(attestTypeName == "Инд.проект" ? "☑ Инд.проект" : "☐ Инд.проект");
                            row.RelativeColumn().Text("");
                        });

                        // Основная информация
                        col.Item().Column(infoCol =>
                        {
                            infoCol.Spacing(5);
                            infoCol.Item().Row(row =>
                            {
                                row.ConstantColumn(70).Text("Преподаватель");
                                row.RelativeColumn()
                                    .BorderBottom(0.5f)
                                    .BorderColor(Colors.Black)
                                    .PaddingBottom(0.5f)
                                    .Text(teacher.TeacherName ?? "_______________")
                                    .FontSize(8);
                            });
                            infoCol.Item().Row(row =>
                            {
                                row.ConstantColumn(70).Text("Дисциплина");
                                row.RelativeColumn()
                                    .BorderBottom(0.5f)
                                    .BorderColor(Colors.Black)
                                    .PaddingBottom(0.5f)
                                    .Text(direction.Discipline.Name)
                                    .FontSize(8);
                            });
                            infoCol.Item().Row(row =>
                            {
                                row.ConstantColumn(80).Text("Учебный период:");
                                row.ConstantColumn(30).Text($"{direction.Direction.Semester} сем.").FontSize(8);
                                row.ConstantColumn(70).Text($"{direction.Direction.StudyYear} уч.г.");
                            });
                            infoCol.Item().Row(row =>
                            {
                                row.ConstantColumn(70).Text("Группа:");
                                row.RelativeColumn()
                                    .BorderBottom(0.5f)
                                    .BorderColor(Colors.Black)
                                    .PaddingBottom(0.5f)
                                    .Text(direction.Group.Name)
                                    .FontSize(8);
                            });
                        });

                        // Таблица
                        col.Item().PaddingTop(8).Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(3f);
                                cols.RelativeColumn(1.5f);
                                cols.RelativeColumn(2f);
                            });

                            table.Header(h =>
                            {
                                h.Cell().Element(HeaderCellStyle).Text("ФИО студента").Bold().FontSize(8).AlignCenter();
                                h.Cell().Element(HeaderCellStyle).Text("Оценка").Bold().FontSize(8).AlignCenter();
                                h.Cell().Element(HeaderCellStyle).Text("Подпись").Bold().FontSize(8).AlignCenter();
                            });

                            var students = direction.Students;
                            int rowCount = Math.Min(students.Count, 6);
                            for (int i = 0; i < rowCount; i++)
                            {
                                var item = students[i];
                                table.Cell().Element(BodyCellStyle).Text(item.StudentName).FontSize(8).AlignCenter();
                                table.Cell().Element(BodyCellStyle)
                                    .Text(item.GradeValue > 0 ? item.GradeValue.ToString() : "")
                                    .FontSize(8).AlignCenter();
                                table.Cell().Element(BodyCellStyle).Text("");
                            }
                            for (int i = students.Count; i < 6; i++)
                            {
                                table.Cell().Element(BodyCellStyle).Text("");
                                table.Cell().Element(BodyCellStyle).Text("");
                                table.Cell().Element(BodyCellStyle).Text("");
                            }
                        });

                        col.Item().PaddingTop(12).Row(row =>
                        {
                            row.ConstantColumn(15).Text($"«{DateTime.Today.Day}»");
                            row.ConstantColumn(40)
                                .BorderBottom(0.2f)
                                .BorderColor(Colors.Black)
                                .PaddingBottom(0.5f)
                                .Text(month)
                                .FontSize(8);
                            row.ConstantColumn(40).AlignRight().Text($"{DateTime.Today.Year} г.");
                        });

                        // Примечание
                        col.Item().PaddingTop(10)
                            .Text("Данный бланк сдаётся в учебную часть ЛИЧНО ПРЕПОДАВАТЕЛЕМ")
                            .FontSize(8).Bold();
                    });
            };
        }

        private static IContainer HeaderCellStyle(IContainer container)
        {
            return container.Border(0.5f).BorderColor(Colors.Black)
                .PaddingVertical(3).PaddingHorizontal(2);
        }

        private static IContainer BodyCellStyle(IContainer container)
        {
            return container.Border(0.5f).BorderColor(Colors.Black)
                .PaddingVertical(2).PaddingHorizontal(2)
                .MinHeight(8);
        }
    }
}