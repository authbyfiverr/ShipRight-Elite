using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShipRight
{
	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var builder = new HostBuilder()
			  .ConfigureServices((hostContext, services) =>
			  {
				  services.AddScoped<MainForm>();
				  services.AddSingleton<IConfiguration, Configuration>();
				  services.AddScoped<IBoardReader, BoardReader>();
				  services.AddScoped<IScreenshotService, ScreenshotService>();
				  services.AddScoped<IMouseMovement, MouseMovement>();
				  services.AddScoped<IRunner, Runner>();
				  services.AddScoped<IPuzzler, Puzzler>();
				  services.AddScoped<IAction, Action>();
				  services.AddSingleton<IManualOverlay, ManualOverlay>();
			  });

			var host = builder.Build();

			using (var serviceScope = host.Services.CreateScope())
			{
				var services = serviceScope.ServiceProvider;
				try
				{
					var service = services.GetRequiredService<MainForm>();
					Application.Run(service);
					return;
				}
				catch
				{
					Environment.Exit(0);
					return;
				}

			}
		}
	}
}
