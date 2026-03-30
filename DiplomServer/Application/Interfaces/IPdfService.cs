namespace DiplomServer.Application.Interfaces
{
    public interface IPdfService
    {
        Task<byte[]> GenerateRetakeDirectionPdfAsync(uint retakeDirectionId);
    }
}