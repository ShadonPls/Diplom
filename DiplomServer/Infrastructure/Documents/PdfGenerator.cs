using DiplomServer.Application.DTOs.Auth;
using DiplomServer.Application.DTOs.Orders;
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
                            var Date = direction.Direction.CreatedAt;
                            var day = Date.Day;
                            if (day < 10)
                                row.ConstantColumn(20).Text($"«0{day}»");
                            else
                                row.ConstantColumn(20).Text($"«{day}»");
                            row.ConstantColumn(40)
                                .BorderBottom(0.2f)
                                .BorderColor(Colors.Black)
                                .PaddingBottom(0.5f)
                                .Text(month)
                                .FontSize(8);
                            row.ConstantColumn(40).AlignRight().Text($"{Date.Year} г.");
                        });

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

        public byte[] GenerateOrderPdf(IList<(OrdersResponseDto Details, CurrentUserDto Teacher)> items)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                foreach (var item in items)
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4); // Книжная ориентация
                        page.Margin(20, Unit.Millimetre); // Стандартные поля для документов
                        page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Times New Roman"));

                        page.Content().Element(ComposeOrderDocument(item.Details));
                    });
                }
            });

            return document.GeneratePdf();
        }

        private static Action<IContainer> ComposeOrderDocument(OrdersResponseDto details)
        {
            return container =>
            {
                container.Column(col =>
                {
                    col.Item().PaddingTop(0).Column(hcol =>
                    {
                        hcol.Item().AlignCenter().Text("Министерство образования и науки Пермского края").FontSize(12);
                        hcol.Item().AlignCenter().Text("краевое государственное автономное профессиональное образовательное учреждение").FontSize(12);
                        hcol.Item().AlignCenter().Text("«ПЕРМСКИЙ АВИАЦИОННЫЙ ТЕХНИКУМ ИМ. А.Д. ШВЕЦОВА»").FontSize(12);

                        hcol.Item().PaddingTop(8).AlignCenter().Text("УПРАВЛЕНИЕ УЧЕБНОЙ РАБОТЫ").FontSize(12);
                        hcol.Item().PaddingTop(8).AlignCenter().Text("РАСПОРЯЖЕНИЕ").FontSize(12).Bold();
                    });

                    var date = details.DateCreate;
                    var monthLow = date.ToString("MMMM", new CultureInfo("ru-RU"));

                    col.Item().PaddingTop(10).Row(row =>
                    {
                        row.ConstantColumn(30).Text($"« {(date.Day < 10 ? "0" + date.Day : date.Day)} »");
                        row.ConstantColumn(80).BorderBottom(0.5f).AlignCenter().Text(monthLow);
                        row.RelativeColumn().Text($" 20{date.ToString("yy")} г.");
                        row.ConstantColumn(20).AlignRight().Text("№");
                        row.ConstantColumn(35).BorderBottom(0.1f).AlignCenter().Text(details.Number);
                    });

                    col.Item().PaddingTop(10).Text("О ликвидации академической\nзадолженности");

                    string indent = "\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0\u00A0";

                    col.Item().PaddingTop(10).Text(txt =>
                    {
                        txt.Justify();

                        txt.Span($"{indent}На основании п. 3, 5, 6, 11 Статьи 58 Федерального Закона №273-ФЗ «Об образовании в Российской Федерации»:\n");
                        txt.Span($"{indent}3. Обучающиеся обязаны ликвидировать академическую задолженность.\n");
                        txt.Span($"{indent}5. Обучающиеся, имеющие академическую задолженность, вправе пройти промежуточную аттестацию по соответствующим учебному предмету, курсу, дисциплине (модулю) не более двух раз в сроки, определяемые организацией, осуществляющей образовательную деятельность. В указанный период не включаются время болезни обучающегося, нахождение его в академическом отпуске или отпуске по беременности и родам.\n");
                        txt.Span($"{indent}6. Для проведения промежуточной аттестации во второй раз образовательной организацией создается комиссия.\n");
                        txt.Span($"{indent}11. Обучающиеся по основным профессиональным образовательным программам, не ликвидировавшие в установленные сроки академической задолженности, отчисляются из этой организации как не выполнившие обязанностей по добросовестному освоению образовательной программы и выполнению учебного плана.\n");
                    });

                    col.Item().PaddingTop(5).Text(txt =>
                    {
                        txt.Justify();
                        txt.Span($"{indent}Неявка студента без уважительной причины ").Bold();
                        txt.Span("на заседание комиссии не дает ему право на назначение дополнительных сроков ликвидации академической задолженности и ");
                        txt.Span("приравнивается к неудовлетворительной оценке.").Bold();
                    });

                    string studentName = ((dynamic)details.StudentId).Name ?? string.Empty;
                    string groupName = ((dynamic)details.StudentId).Group ?? string.Empty;
                    string disciplineName = ((dynamic)details.DisciplineId).Name ?? string.Empty;

                    col.Item().PaddingTop(5).Text(txt =>
                    {
                        txt.Justify();
                        txt.Span($"{indent}Для студента ");
                        txt.Span($" {studentName}");
                        txt.Span(", группа ");
                        txt.Span($" {groupName}");
                        txt.Span(", установить следующий график ликвидации академической задолженности:");
                    });

                    col.Item().PaddingTop(10).Text("Прием академической задолженности преподавателем");
                    col.Item().Element(c => DrawTable(c,
                        "преподаватель",
                        disciplineName,
                        details.DateReceptionTeacher,
                        string.Join(", ", details.ReceptionTeachers.Select(t => FormatToInitials(t.TeacherName)))));

                    col.Item().PaddingTop(10).Text("Прием академической задолженности комиссией");
                    col.Item().Element(c => DrawTable(c,
                        "состав комиссии",
                        disciplineName,
                        details.DateReceptionCommission,
                        string.Join(", ", details.ReceptionCommissions.Select(t => FormatToInitials(t.TeacherName)))));

                    col.Item().PaddingTop(10).Row(row =>
                    {
                        row.RelativeColumn().Text("Руководитель управления");
                        row.ConstantColumn(150).BorderBottom(0.5f);
                        row.RelativeColumn().AlignRight().Text("А.В. Федорова");
                    });

                    col.Item().PaddingTop(10).Text("С распоряжением ознакомлены:").Underline();
                    col.Item().Element(c => DrawSignatureRow(c, "Зав. отделением"));
                    col.Item().Element(c => DrawSignatureRow(c, "Председатель ЦМК"));
                    col.Item().Element(c => DrawSignatureRow(c, "Студент"));
                });
            };
        }
        private static void DrawTable(IContainer container, string thirdColumnHeader, string discipline, DateTime? date, string teachers)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(2f);
                    cols.RelativeColumn(1f);
                    cols.RelativeColumn(1.5f);
                });

                table.Header(h =>
                {
                    h.Cell().Element(OrderHeaderCellStyle).Text("дисциплина");
                    h.Cell().Element(OrderHeaderCellStyle).Text("дата и время");
                    h.Cell().Element(OrderHeaderCellStyle).Text(thirdColumnHeader);
                });

                table.Cell().Element(OrderBodyCellStyle).Text(discipline);
                
                string dateStr = date.HasValue ? date.Value.ToString("dd.MM.yyyy" + " в " + "HH:mm") : "";
                table.Cell().Element(OrderBodyCellStyle).AlignCenter().Text(dateStr);

                table.Cell().Element(OrderBodyCellStyle).Text(teachers);
            });
        }

        private static void DrawSignatureRow(IContainer container, string roleName)
        {
            container.PaddingTop(5).Row(row =>
            {
                row.ConstantColumn(110).Text(roleName);
                row.ConstantColumn(90).BorderBottom(0.5f);
                row.ConstantColumn(5).AlignCenter().Text("/");
                row.ConstantColumn(100).BorderBottom(0.5f);
                row.ConstantColumn(5).AlignCenter().Text("/");
                row.RelativeColumn().Text(" «____» ____________ 20___ г.");
            });
        }

        private static IContainer OrderHeaderCellStyle(IContainer container)
        {
            return container.Border(0.5f).BorderColor(Colors.Black)
                .PaddingVertical(2).PaddingHorizontal(4)
                .AlignCenter().AlignMiddle();
        }

        private static IContainer OrderBodyCellStyle(IContainer container)
        {
            return container.Border(0.5f).BorderColor(Colors.Black)
                .PaddingVertical(4).PaddingHorizontal(4)
                .AlignMiddle().MinHeight(20);
        }

        private static string FormatToInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return string.Empty;

            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 3)
            {
                return $"{parts[0]} {parts[1][0]}.{parts[2][0]}.";
            }
            else if (parts.Length == 2)
            {
                return $"{parts[0]} {parts[1][0]}.";
            }
            return parts[0];
        }
    }
}