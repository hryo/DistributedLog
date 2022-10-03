using DistributedLog.Master.Application;
using DistributedLog.Master.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ReplicasFactory>();
builder.Services.AddSingleton<LogStorage>();
builder.Services.AddSingleton<ReplicationService>();
builder.Services.AddSingleton<ReplicatedStorage>();

var replicationConfigSection = builder.Configuration.GetSection("Secondaries");
builder.Services.Configure<ReplicationConfig>(replicationConfigSection);
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
