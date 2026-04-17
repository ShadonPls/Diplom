namespace DiplomServer.Application.Interfaces
{
    public interface IPdfService
    {
        Task<byte[]> GenerateRetakeDirectionPdfAsync(IList<uint> retakeDirectionIds);
    }
}