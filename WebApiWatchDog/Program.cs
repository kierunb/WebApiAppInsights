using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ServiceProcess;
using WebApiWatchDog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecksUI()
    .AddInMemoryStorage();
    // .AddSqlServerStorage("connectionString");


builder.Services.AddHealthChecks()
    .AddCheck<RandomHealthCheck>("Random Health Check")
    .AddCheck<AlwaysGoodCheck>("Good Check")
    .AddCheck("AlwaysHealthyToo", () => HealthCheckResult.Healthy(), tags: new[] { "Tag1" })
    .AddDiskStorageHealthCheck(s => s.AddDrive("C:\\", 1024)) // 1024 MB (1 GB) free minimum
    .AddProcessAllocatedMemoryHealthCheck(512) // 512 MB max allocated memory
    .AddUrlGroup(new Uri("http://httpbin.org/status/200"), name: "HttpBin Uri")
    .AddProcessHealthCheck("MsMpEng.exe", p => p.Length > 0, name: "Antivirus running")
    .AddWindowsServiceHealthCheck("Winmgmt", s => s.Status == ServiceControllerStatus.Running);


//.AddApplicationInsightsPublisher() // push to Application Insights

//.AddSqlServer(
//            connectionString: Configuration["Data:ConnectionStrings:Sql"],
//            healthQuery: "SELECT 1;",
//            name: "sql",
//            failureStatus: HealthStatus.Degraded,
//            tags: new string[] { "db", "sql", "sqlserver" });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("healthz", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.MapHealthChecksUI(); // /healthchecks-ui#/healthchecks


app.Run();
