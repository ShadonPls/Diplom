using DiplomServer.Application.DTOs.Auth;
using FluentValidation;

namespace DiplomServer.Application.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен.")
                .EmailAddress().WithMessage("Некорректный формат email.")
                .MaximumLength(255).WithMessage("Email не должен превышать 255 символов.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен.")
                .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов.")
                .MaximumLength(100).WithMessage("Пароль не должен превышать 100 символов.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Роль обязательна.")
                .MaximumLength(50).WithMessage("Роль не должна превышать 50 символов.");
        }
    }
}