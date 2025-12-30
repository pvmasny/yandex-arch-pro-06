

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Console.WriteLine("Test: simplest-agent.observability.svc.cluster.local");
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();


builder.Services.AddOpenTelemetry()
    .WithTracing(traceBuilder =>
    {
        traceBuilder.AddAspNetCoreInstrumentation()
        .AddJaegerExporter(opt =>
        {
            opt.AgentHost = "simplest-agent.observability.svc.cluster.local";
            opt.AgentPort = 6831;
        })
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("OrderService"))
        .AddSource("OrderService");
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
