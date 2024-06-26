using MachineControlHub.PrinterConnection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using WebUI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<SerialConnectionService>();
builder.Services.AddScoped<HotendTemperatureService>();
builder.Services.AddScoped<BedTemperatureService>();
builder.Services.AddScoped<ChamberTemperatureService>();
builder.Services.AddScoped<PrintingService>();
builder.Services.AddScoped<ControlPanelService>();
builder.Services.AddSingleton<BedLevelingService>();
builder.Services.AddScoped<PrinterDataServiceTest>();
builder.Services.AddSingleton(BackgroundTimer.Instance);
builder.Services.AddSingleton<PortConnectionManager>();
builder.Services.AddSingleton<SerialDataProcessor>();
builder.Services.AddMudServices();
builder.Services.AddHttpClient();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
