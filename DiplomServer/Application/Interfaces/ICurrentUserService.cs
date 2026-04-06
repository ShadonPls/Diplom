namespace DiplomServer.Application.Interfaces
{
    public interface ICurrentUserService
    {
        uint UserId { get; }
        uint? TeacherId { get; }
        string? Login { get; }
        bool IsAuthenticated { get; }
    }
}