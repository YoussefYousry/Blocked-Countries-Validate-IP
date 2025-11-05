

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.AddSerilogLogging();

// Register services
builder.Services
    .AddHttpClient()
    .AddAppServices()
    .AddAppSwagger()
    .AddControllers();

var app = builder.Build();

// Configure middleware pipeline
app.UseAppMiddlewares();
app.UseAppSwaggerUI();

app.MapControllers();
await app.RunAsync();
