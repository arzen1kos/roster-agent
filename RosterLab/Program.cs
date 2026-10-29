using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OllamaSharp;

var b = Host.CreateApplicationBuilder(args);
b.Services.AddDbContext<RosterDb>(o => o.UseSqlite(b.Configuration.GetConnectionString("Db")));
b.Services.AddSingleton<IChatClient>(_ => new OllamaApiClient(
    new HttpClient { BaseAddress = new Uri(b.Configuration["Ollama:Url"]!), Timeout = TimeSpan.FromMinutes(5) },
    b.Configuration["Ollama:Model"]!));
b.Services.AddScoped<RosterAgent>();
b.Services.AddHostedService<RosterWorker>();

var app = b.Build();
using (var s = app.Services.CreateScope())
{
    var db = s.ServiceProvider.GetRequiredService<RosterDb>();
    db.Database.EnsureCreated();
    Seed.Run(db);
}
app.Run();