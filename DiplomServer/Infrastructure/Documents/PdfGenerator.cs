using DiplomServer.Application.DTOs.Auth;
using DiplomServer.Application.DTOs.RetakeDirections;
using DiplomServer.Domain.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DiplomServer.Infrastructure.Documents
{
    public class PdfGenerator
    {
        public byte[] GenerateRetakeDirectionPdf(RetakeDirectionDetailsDto direction, CurrentUserDto teacher)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    // ЗАГОЛОВОК
                    page.Header()
                        .Column(column =>
                        {
                            column.Spacing(3);

                            column.Item()
                                .Text("ПЕРМСКИЙ АВИАЦИОННЫЙ ТЕХНИКУМ ИМ. А.Д. ШВЕЦОВА")
                                .FontSize(10)
                                .AlignCenter()
                                .Bold();

                            column.Item()
                                .Text("НАПРАВЛЕНИЕ НА ПЕРЕСДАЧУ")
                                .FontSize(14)
                                .AlignCenter()
                                .Bold();

                            column.Item()
                                .Text("оценки промежуточной аттестации")
                                .FontSize(11)
                                .AlignCenter();
                        });

                    page.Content()
                        .PaddingVertical(15)
                        .Column(column =>
                        {
                            column.Spacing(12);

                            // ФОРМА ПРОМЕЖУТОЧНОЙ АТТЕСТАЦИИ
                            column.Item()
                                .Column(innerColumn =>
                                {
                                    innerColumn.Item().Text("Форма промежуточной аттестации:").FontSize(10).Bold();

                                    innerColumn.Item()
                                        .PaddingTop(5)
                                        .Row(row =>
                                        {
                                            row.ConstantColumn(15).Text("☐ Экзамен");
                                            row.ConstantColumn(20).Text("                          ");
                                            row.ConstantColumn(15).Text("☐ Диф.зачет");
                                            row.ConstantColumn(20).Text("                           ");
                                            row.RelativeColumn().Text("☐ Зачет");
                                        });

                                    innerColumn.Item()
                                        .Row(row =>
                                        {
                                            row.ConstantColumn(15).Text("☐ КП/КР");
                                            row.ConstantColumn(20).Text("");
                                            row.ConstantColumn(15).Text("☐ Инд.проект");
                                            row.RelativeColumn().Text("");
                                        });
                                });

                            // ОСНОВНАЯ ИНФОРМАЦИЯ
                            column.Item()
                                .Column(infoColumn =>
                                {
                                    infoColumn.Spacing(8);

                                    infoColumn.Item()
                                        .Row(row =>
                                        {
                                            row.ConstantColumn(150).Text("Преподаватель");
                                            row.RelativeColumn()
                                                .BorderBottom(1)
                                                .BorderColor(Colors.Black)
                                                .PaddingBottom(2)
                                                .Text(teacher.TeacherName ?? "_______________")
                                                .FontSize(10);
                                        });

                                    infoColumn.Item()
                                        .Row(row =>
                                        {
                                            row.ConstantColumn(150).Text("Дисциплина");
                                            row.RelativeColumn()
                                                .BorderBottom(1)
                                                .BorderColor(Colors.Black)
                                                .PaddingBottom(2)
                                                .Text(direction.Discipline.Name)
                                                .FontSize(10);
                                        });

                                    infoColumn.Item()
                                        .Row(row =>
                                        {
                                            row.ConstantColumn(150).Text("Учебный период:");
                                            row.ConstantColumn(50).Text($"{direction.Direction.Semester} семестр").FontSize(10);
                                            row.ConstantColumn(30).Text($"20{direction.Direction.StudyYear}");
                                            row.RelativeColumn()
                                                .BorderBottom(1)
                                                .BorderColor(Colors.Black)
                                                .AlignRight()
                                                .PaddingBottom(2)
                                                .Text("уч.г.")
                                                .FontSize(9);
                                        });

                                    infoColumn.Item()
                                        .Row(row =>
                                        {
                                            row.ConstantColumn(150).Text("Группа:");
                                            row.RelativeColumn()
                                                .BorderBottom(1)
                                                .BorderColor(Colors.Black)
                                                .PaddingBottom(2)
                                                .Text(direction.Group.Name)
                                                .FontSize(10);
                                        });
                                });

                            // ТАБЛИЦА СТУДЕНТОВ
                            column.Item()
                                .PaddingTop(15)
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3f);
                                        columns.RelativeColumn(2f);
                                        columns.RelativeColumn(1.5f);
                                    });

                                    // Заголовок таблицы
                                    table.Header(header =>
                                    {
                                        header.Cell().Element(HeaderCellStyle)
                                            .Text("ФИО студента")
                                            .Bold()
                                            .FontSize(10);

                                        header.Cell().Element(HeaderCellStyle)
                                            .Text("Оценка")
                                            .Bold()
                                            .FontSize(10);

                                        header.Cell().Element(HeaderCellStyle)
                                            .Text("Подпись преподавателя")
                                            .Bold()
                                            .FontSize(10);
                                    });

                                    var students = direction.Students;

                                    for (int i = 0; i < students.Count; i++)
                                    {
                                        var item = students[i];

                                        table.Cell().Element(BodyCellStyle)
                                            .Text($"{item.StudentName}")
                                            .FontSize(10);

                                        table.Cell().Element(BodyCellStyle)
                                            .Text(item.GradeValue > 0 ? item.GradeValue.ToString() : "")
                                            .FontSize(10)
                                            .AlignCenter();

                                        table.Cell().Element(BodyCellStyle)
                                            .Text("");
                                    }

                                    // Добавить пустые строки
                                    for (int i = students.Count; i < 8; i++)
                                    {
                                        table.Cell().Element(BodyCellStyle).Text("");
                                        table.Cell().Element(BodyCellStyle).Text("");
                                        table.Cell().Element(BodyCellStyle).Text("");
                                    }
                                });

                            // ПОДПИСЬ И ДАТА
                            column.Item()
                                .PaddingTop(20)
                                .Row(row =>
                                {
                                    row.ConstantColumn(100).Text("«____»");
                                    row.ConstantColumn(150)
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Black)
                                        .PaddingBottom(2)
                                        .Text("_________________")
                                        .FontSize(9);
                                    row.ConstantColumn(50).AlignRight().Text($"{DateTime.Now.Day}__ г.");
                                });

                            // ПРИМЕЧАНИЕ
                            column.Item()
                                .PaddingTop(10)
                                .Text("Данный бланк сдаётся в учебную часть ЛИЧНО ПРЕПОДАВАТЕЛЕМ в день приёма задолженности")
                                .FontSize(9)
                                .AlignCenter()
                                .Bold();
                        });
                });
            });

            return document.GeneratePdf();
        }

        private static IContainer HeaderCellStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Black)
                .PaddingVertical(6)
                .PaddingHorizontal(5);
        }

        private static IContainer BodyCellStyle(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Black)
                .PaddingVertical(10)
                .PaddingHorizontal(5)
                .MinHeight(25);
        }
    }
}