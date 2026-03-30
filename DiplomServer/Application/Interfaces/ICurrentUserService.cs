namespace DiplomServer.Application.Interfaces
{
    public interface ICurrentUserService
    {
        uint UserId { get; }
        uint? TeacherId { get; }
        string? Email { get; }
        string? Role { get; }
        bool IsAuthenticated { get; }
    }
}