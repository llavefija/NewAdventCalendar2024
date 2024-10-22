using NewAdventCalendar2024.Tools;
using NewAdventCalendar2024.Data;

namespace NewAdventCalendar2024.Views.PaginasPrincipales
{

    public partial class MainPage : ContentPage
    {

        private readonly AppDb _database;

        public MainPage(AppDb db)
        {
            InitializeComponent();
            _database = db;
        }

       

        // Evento para navegar a la página del calendario
        private async void OnCalendarButtonClicked(object sender, EventArgs e)
        {
            try
            {
                // Instanciar ToolBotones con el botón clicado
                var toolButton = new ToolBotones((Button)sender); 

                // Animación de aumentar tamaño
                await toolButton.AnimateButton();

                // Navegar a CalendarioPage
                await Navigation.PushAsync(new CalendarioPage(_database), true); 
            }
            // Captura de la excepción
            catch (Exception ex) 
            {
                // Mostrar el error en una alerta al usuario
                await DisplayAlert("ERROR", $"Ocurrió un error con el botón.", "OK");
                Console.WriteLine($"Ha ocurrido un error: {ex}");

            }
        }


        // Evento para navegar a la página de información
        private async void OnInformationButtonClicked(object sender, EventArgs e)
        {
            try
            {
                // Instanciar ToolBotones con el botón clicado
                var toolButton = new ToolBotones((Button)sender);

                // Animación de aumentar tamaño
                await toolButton.AnimateButton();

                // Navegar a InformationPage
                await Navigation.PushAsync(new InformationPage(_database), true);
            }
            // Captura de la excepción
            catch (Exception ex) 
            {
                // Mostrar el error en una alerta al usuario
                await DisplayAlert("ERROR", $"Ocurrió un error con el botón.", "OK");
                Console.WriteLine($"Ha ocurrido un error: {ex}");

            }
        }

        // Evento para cerrar la aplicación
        private void OnExitButtonClicked(object sender, EventArgs e)
        {
            try
            {
                // Instanciar ToolBotones con el botón clicado
                var toolButton = new ToolBotones((Button)sender); 

                // Cierra la aplicación en Android e iOS
#if ANDROID
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
#elif IOS
                UIKit.UIApplication.SharedApplication.PerformSelector(new ObjCRuntime.Selector("terminateWithSuccess"), null, 0f);

#endif

            }
            // Captura de la excepción
            catch (Exception ex) 
            {
                Console.WriteLine($"Ha ocurrido un error: {ex}");
            }
        }

        // Metodo para realizar una animacion de inicio en los botones.
        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                // Establecer opacidad inicial de los botones a 0
                BtCalendario.Opacity = 0;
                BtInformacion.Opacity = 0;
                BtSalir.Opacity = 0;

                // Animación de fade-in para cada botón
                await BtCalendario.FadeTo(1, 500); // Aparece en 500ms
                await BtInformacion.FadeTo(1, 500);
                await BtSalir.FadeTo(1, 500);
            }
            // Captura de la excepción
            catch (Exception ex) 
            {
                // Mostrar el error en una alerta al usuario
                await DisplayAlert("ERROR", $"Ocurrió un error las animaciones del botoón.", "OK");
                Console.WriteLine($"Ha ocurrido un error: {ex}");

            }
        }


    }

}


