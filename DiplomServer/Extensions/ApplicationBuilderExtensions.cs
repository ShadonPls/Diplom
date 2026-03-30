using DiplomServer.Middleware;

namespace DiplomServer.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static WebApplication UseAppPipeline(this WebApplication app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            return app;
        }
    }
}