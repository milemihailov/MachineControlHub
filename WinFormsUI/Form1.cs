using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using WebUI;
using WebUI.Data;

namespace WinFormsUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddWindowsFormsBlazorWebView();
            serviceCollection.AddBlazorWebView();
            serviceCollection.AddSingleton<ConnectionServiceSerial>();
            serviceCollection.AddScoped<HotendTemperatureService>();
            serviceCollection.AddScoped<BedTemperatureService>();
            serviceCollection.AddScoped<ChamberTemperatureService>();
            serviceCollection.AddScoped<PrintingService>();
            serviceCollection.AddSingleton<ControlPanelService>();
            serviceCollection.AddSingleton<BedLevelingService>();
            serviceCollection.AddScoped<PrinterDataServiceTest>();
            serviceCollection.AddSingleton(BackgroundTimer.Instance);
            serviceCollection.AddMudServices();
            serviceCollection.AddHttpClient();
            InitializeComponent();
            blazorWebView1.Dock = DockStyle.Fill;
            blazorWebView1.HostPage = "wwwroot/index.html";
            blazorWebView1.Services = serviceCollection.BuildServiceProvider();
            blazorWebView1.RootComponents.Add<App>("#app");
            WindowState = FormWindowState.Maximized;
            Icon = new Icon(@"wwwroot\3dprinter.ico");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
