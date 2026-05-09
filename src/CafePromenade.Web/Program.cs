using CafePromenade.Core.Services;
using CafePromenade.Web.Services;
using CafePromenade.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(_ => true);
    });
});

builder.Services.AddSingleton<IRepositoryService, RepositoryService>();
builder.Services.AddSingleton<IVeraCryptService, VeraCryptService>();
builder.Services.AddSingleton<INtliteService, NtliteService>();
builder.Services.AddSingleton<IMessagingService, MessagingService>();
builder.Services.AddSingleton<ITelegramService, TelegramService>();
builder.Services.AddSingleton<IDockerService, DockerService>();
builder.Services.AddSingleton<ISystemService, SystemService>();
builder.Services.AddSingleton<ICredentialVaultService, CredentialVaultService>();
builder.Services.AddSingleton<IWindowsTweakService, WindowsTweakService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.MapGet("/", () => "CafePromenade SuperApp API is running! Swagger: /swagger");
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();
