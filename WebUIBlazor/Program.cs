using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using WebUI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<ConnectionServiceSerial>();
builder.Services.AddSingleton<PrinterData>();
builder.Services.AddSingleton<HotendTemperatureService>();
builder.Services.AddSingleton<BedTemperatureService>();
builder.Services.AddSingleton<PrintingService>();
builder.Services.AddSingleton<ControlPanelService>();



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
