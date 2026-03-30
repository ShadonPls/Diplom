using DiplomServer.Application.DTOs.Auth;
using FluentValidation;

namespace DiplomServer.Application.Validators
{
    public class LoginDtoValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен.")
                .EmailAddress().WithMessage("Некорректный формат email.")
                .MaximumLength(255).WithMessage("Email не должен превышать 255 символов.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен.")
                .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов.")
                .MaximumLength(100).WithMessage("Пароль не должен превышать 100 символов.");
        }
    }
}