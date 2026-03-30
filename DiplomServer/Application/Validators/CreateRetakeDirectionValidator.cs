using DiplomServer.Application.DTOs.RetakeDirections;
using FluentValidation;

namespace DiplomServer.Application.Validators
{
    public class CreateRetakeDirectionValidator : AbstractValidator<CreateRetakeDirectionRequestDto>
    {
        public CreateRetakeDirectionValidator()
        {
            RuleFor(x => x.GroupId)
                .GreaterThan(0u).WithMessage("GroupId должен быть больше 0.");

            RuleFor(x => x.DisciplineId)
                .GreaterThan(0u).WithMessage("DisciplineId должен быть больше 0.");

            RuleFor(x => x.AttestTypeId)
                .GreaterThan(0u).WithMessage("AttestTypeId должен быть больше 0.");

            RuleFor(x => x.Semester)
                .InclusiveBetween(1, 2).WithMessage("Семестр должен быть 1 или 2.");

            RuleFor(x => x.StudyYear)
                .NotEmpty().WithMessage("Учебный год обязателен.")
                .MaximumLength(20).WithMessage("Учебный год не должен превышать 20 символов.");

            RuleFor(x => x.Students)
                .NotEmpty().WithMessage("Нужно добавить хотя бы одного студента.");

            RuleForEach(x => x.Students)
                .SetValidator(new RetakeDirectionStudentRequestValidator());
        }
    }
}