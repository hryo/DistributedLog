using DistributedLog.Master.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<SecondariesFactory>();
builder.Services.AddSingleton<ILogStorage, LogStorage>();
builder.Services.AddSingleton<ReplicationProtocol>();
builder.Services.AddSingleton<ReplicatedStorage>();
builder.Services.Configure<SecondariesConfig>(builder.Configuration);

var secondaries = builder.Configuration.GetSection("Secondaries").Get<SecondariesConfig>();
foreach (var secondaryUrl in secondaries.Urls)
{
    builder.Services.AddHttpClient<SecodaryClient>(client =>
    {
        client.BaseAddress = new Uri(secondaryUrl);
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();