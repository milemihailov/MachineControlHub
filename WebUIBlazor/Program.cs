using System.Diagnostics;
using MudBlazor.Services;
using WebUI;
using WebUI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<SerialConnectionService>();
builder.Services.AddSingleton<HotendTemperatureService>();
builder.Services.AddSingleton<BedTemperatureService>();
builder.Services.AddSingleton<ChamberTemperatureService>();
builder.Services.AddSingleton<PrintingService>();
builder.Services.AddSingleton<ControlPanelService>();
builder.Services.AddSingleton<BedLevelingService>();
builder.Services.AddSingleton<PrinterDataService>();
builder.Services.AddSingleton(BackgroundTimer.Instance);
builder.Services.AddSingleton<PrinterManagerService>();
builder.Services.AddMudServices();
builder.Services.AddHttpClient();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();



    Util.OpenUrl("http://localhost:5000");
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");



app.Run();
