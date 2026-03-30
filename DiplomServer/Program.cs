using DiplomServer.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppOptions(builder.Configuration);
builder.Services.AddAppDb(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAppServices();
builder.Services.AddAppRepositories();
builder.Services.AddAppSwagger(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseAppPipeline();

app.Run();