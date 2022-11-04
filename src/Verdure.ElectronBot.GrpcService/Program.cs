using System.Device.Gpio;
using ElectronBot.DotNet;
using Verdure.ElectronBot.GrpcService;
using Verdure.ElectronBot.GrpcService.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddSingleton<IElectronLowLevel, ElectronLowLevel>();


var app = builder.Build();

EmojiPlayHelper.Current.ElectronLowLevel = app.Services.GetRequiredService<IElectronLowLevel>();

EmojiPlayHelper.Current.Start();

// Configure the HTTP request pipeline.
app.MapGrpcService<ElectronBotActionService>();
app.MapGet("/abc", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
