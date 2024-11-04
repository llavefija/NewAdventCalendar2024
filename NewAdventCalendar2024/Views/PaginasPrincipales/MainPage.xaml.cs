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
            NavigationPage.SetHasNavigationBar(this, false);
            _database = db;
        }

        // Evento para navegar a la página del calendario
        private async void OnCalendarButtonClicked(object sender, EventArgs e)
        {
            await HandleButtonClickAsync(sender, new CalendarioPage(_database));
        }

        // Evento para navegar a la página de información
        private async void OnInformationButtonClicked(object sender, EventArgs e)
        {
            await HandleButtonClickAsync(sender, new InformationPage(_database));
        }

        // Evento para cerrar la aplicación
        private void OnExitButtonClicked(object sender, EventArgs e)
        {
            try
            {
                var toolButton = new ToolBotones((Button)sender);
#if ANDROID
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
#elif IOS
                UIKit.UIApplication.SharedApplication.PerformSelector(new ObjCRuntime.Selector("terminateWithSuccess"), null, 0f);
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cerrar la aplicación: {ex}");
            }
        }

        // Método genérico para manejar eventos de clic y navegación
        private async Task HandleButtonClickAsync(object sender, Page targetPage)
        {
            try
            {
                var toolButton = new ToolBotones((Button)sender);
                await toolButton.AnimateButton();
                await Navigation.PushAsync(targetPage, true);
            }
            catch (Exception ex)
            {
                await DisplayAlert("ERROR", "Ocurrió un error al navegar.", "OK");
                Console.WriteLine($"Error en la navegación: {ex}");
            }
        }

        // Animación de aparición en los botones al cargar la página
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await AnimateButtonsAsync();
        }

        // Método para animar los botones de aparición secuencialmente
        private async Task AnimateButtonsAsync()
        {
            try
            {
                await BtCalendario.FadeTo(1, 500);
                await BtInformacion.FadeTo(1, 500);
                await BtSalir.FadeTo(1, 500);
            }
            catch (Exception ex)
            {
                await DisplayAlert("ERROR", "Error en la animación de los botones.", "OK");
                Console.WriteLine($"Error en la animación: {ex}");
            }
        }
    }
}
