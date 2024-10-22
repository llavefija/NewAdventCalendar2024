using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using NewAdventCalendar2024.Data;
using NewAdventCalendar2024.Views.PaginasPrincipales;

namespace NewAdventCalendar2024
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    // Fuente por defecto
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

                    // Fuente con aspecto de notas
                    fonts.AddFont("Libreta-Bold.ttf", "Libretabold");
                    fonts.AddFont("Libreta-Medium.ttf", "Libretamedium");
                    fonts.AddFont("Libreta-Regular.ttf", "Libretaregular");
                    fonts.AddFont("Libreta-SemiBold.ttf", "Libretasemibold");

                    // Fuente con aspecto de bloques estilo cartoon
                    fonts.AddFont("CartoonBlocksXmas-Regular.ttf", "CartoonXmasRegular");
                    fonts.AddFont("CartoonBlocksXmasSC-Regular.ttf", "CartoonXmasSC");

                    // Fuente con aspecto de caramelo navideño
                    fonts.AddFont("Candy.ttf", "CandyFont");

                    // Fuente con aspecto navideño (diseño para titulos)
                    fonts.AddFont("HomeXmas.otf", "HomeXmas"); 

                    // Fuente lobster clasica
                    fonts.AddFont("Lobster-Regular.ttf", "Lobster"); 

                    // Fuente con aspecto navideño con nieve
                    fonts.AddFont("PWHappyXmas.ttf", "HappyXmas"); 
                });

            builder.Services.AddSingleton<AppDb>(); // Registro de la base de datos como Singleton
            builder.Services.AddSingleton<InformationPage>(); // Registro de la InformationPage como Singleton
            builder.Services.AddSingleton<CalendarioPage>(); // Registro de la CalendarioPage como Singleton
            builder.Services.AddSingleton<MainPage>(); // Registro de la MainPage como Singleton



#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Construir la aplicación
            var app = builder.Build();

            // Obtener el servicio de la base de datos e inicializar botones y recompensas
            var db = app.Services.GetService<AppDb>();
            _ = Task.Run(async () => await InitDatabaseAsync(db)); // Inicializa los botones y recompensas

            return app;

        }

        private static async Task InitDatabaseAsync(AppDb db)
        {
            await db.InicializarBotonesYRecompensas();
        }
    }
}
