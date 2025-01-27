using CommunityToolkit.Maui;
using CommunityToolkit.Maui.ApplicationModel;
using Microsoft.Extensions.Logging;
using SmartHome.Db;
using SmartHome.Interfaces;
using SmartHome.Models;
using SmartHome.Services;
using Plugin.LocalNotification;

namespace SmartHome
{
    public static class MauiProgram
    {
        private static string _ReceiverQueueName = "relayQueue";
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder

                .UseMauiApp<App>()
                //.UseLocalNotification()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .Services
                .AddSingleton<SmartRelayDatabase>()
                .AddScoped<IServerConfiguration, ServerConfiguration>()
                .AddScoped<IMQTTConnection, MQTTConnection>()
                .AddSingleton<IBadge>(Badge.Default)
                ;

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

    }
}
