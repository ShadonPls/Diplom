using DiplomServer.Application.DTOs.RetakeDirections;
using FluentValidation;

namespace DiplomServer.Application.Validators
{
    public class RetakeDirectionStudentRequestValidator : AbstractValidator<RetakeDirectionStudentRequestDto>
    {
        public RetakeDirectionStudentRequestValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0u).WithMessage("StudentId должен быть больше 0.");

            RuleFor(x => x.GradeValue)
                .InclusiveBetween(2, 5).WithMessage("Оценка должна быть в диапазоне от 2 до 5.");

            RuleFor(x => x.GradeDate)
                .NotEmpty().WithMessage("Дата оценки обязательна.");
        }
    }
}