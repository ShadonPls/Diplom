namespace DiplomServer.Application.Interfaces
{
    public interface IExcelParser
    {
        List<BadGrade> GetBadGrades(string filePath);
    }
}
