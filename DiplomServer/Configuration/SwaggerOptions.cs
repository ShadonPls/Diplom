namespace DiplomServer.Configuration
{
    public class SwaggerOptions
    {
        public const string SectionName = "Swagger";

        public string Title { get; set; } = "DiplomServer API";
        public string Version { get; set; } = "v1";
    }
}