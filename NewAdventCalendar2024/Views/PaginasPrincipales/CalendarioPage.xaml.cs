using NewAdventCalendar2024.Tools;
using NewAdventCalendar2024.Data;
using NewAdventCalendar2024.Views.Juegos.MultiClicks;

namespace NewAdventCalendar2024.Views.PaginasPrincipales
{
    public partial class CalendarioPage : ContentPage
    {

        private readonly AppDb _database;


        //Constructor de la clase CalendarioPage
        public CalendarioPage(AppDb db)
        {
            InitializeComponent();

            this.Dispatcher.StartTimer(TimeSpan.FromSeconds(1), ActualizarContador); // Inicia un contador y lo actualiza hasta el dia siguiente
            FechaActualLabel.Text = $"Hoy es  {DateTime.Now.ToString("dddd d 'de' MMMM yyyy")}. Proximo dia en...";

            _database = db;

            ActualizarBotones();

        }

        private async void ActualizarBotones()
        {
            await _database.ActualizarBotonesYHabilitar(this);
        }

        // Actualiza el contador del dia siguiente
        bool ActualizarContador()
        {
            var now = DateTime.Now; // Coje el dia actual
            var nextDay = now.Date.AddDays(1); // Próximo día a las 00:00
            var timeLeft = nextDay - now; // Mira cuanto falta

            CountdownLabel.Text = $"{timeLeft.Hours:D2}:{timeLeft.Minutes:D2}:{timeLeft.Seconds:D2}"; // Crea el contador y lo muestra

            return true; // Repite el temporizador cada segundo
        }

        private async void BotonPulsado(object sender, EventArgs e)
        {
            try
            {
                // Cast el sender al tipo Button
                var botonPulsado = sender as Button;

                if (botonPulsado != null)
                {
                    // Obtener el nombre del botón pulsado
                    var botonId = botonPulsado.AutomationId; // AutomationId, e.g., Btn1, Btn2, etc.

                    // Extraer el número del botón
                    int numeroBoton = int.Parse(botonId.Substring(3)); // Elimina "Btn" para obtener el número

                    var toolButton = new ToolBotones(botonPulsado); // Instanciar ToolBotones con el botón clicado

                    //await toolButton.PlayAudioFromResourceAsync("SonidoBoton.mp3"); // Reproducir sonido (comentado por si está desactivado)
                    await toolButton.AnimateButton(); // Animación de aumentar tamaño

                    // Inicializar TaskCompletionSource
                    var tcs = new TaskCompletionSource<bool>();

                    // Realiza acciones específicas dependiendo del botón
                    switch (numeroBoton)
                    {
                        case 2:
                            bool answer = await DisplayAlert("Multi Clicks", $@"Pulsa varias veces a la imagen hasta cumplir todos los clicks.
Dificultad: 50 clicks.
Recompensa: {NombreRecompensa(numeroBoton)}.", "Jugar", "Cancelar");

                            if (answer)
                            {
                                // Pasar tcs al constructor
                                var multiClicksPage = new MultiClicksPage(tcs);
                                await Navigation.PushAsync(multiClicksPage, true);

                                // Esperar a que el juego termine y obtener el resultado
                                bool juegoCompletado = await tcs.Task;

                                if (juegoCompletado)
                                {
                                    // Acción si el jugador ha completado el juego
                                    await DisplayAlert("Felicidades", "¡Has completado el juego de Multi Clicks!", "OK");

                                   await _database.CompletarBoton(numeroBoton);

                                    await _database.DesbloquearRecompensa(numeroBoton);

                                }
                                else
                                {
                                    // Acción si no ha completado el juego
                                    await DisplayAlert("Juego no completado", "No has completado los 50 clics. Inténtalo más tarde.", "OK");
                                }
                            }
                            else
                            {
                                await Navigation.PopToRootAsync(); // Regresa a la página principal
                            }
                            break;

                        // Resto de los casos (1, 3, 4, ..., 31)
                        // Aquí puedes simplificar o manejar la lógica similar a los botones 1-31 si es necesario

                        default:
                            await DisplayAlert("Botón Pulsado", $"Has pulsado un botón desconocido: {numeroBoton}", "OK");
                            break;
                    }
                }
            }
            catch (Exception ex) // Captura de la excepción
            {
                // Mostrar el error en una alerta al usuario
                await DisplayAlert("ERROR", $"Ocurrió un error al pulsar el botón: {ex.Message}", "OK");
                Console.WriteLine($"Ha ocurrido un error: {ex}");
            }
        }
        public async Task<string> NombreRecompensa(int numero)
        {

            // Obtiene la lista de recompensas desde la base de datos
            var recompensa = await _database.GetRecompensaPorId(numero);




            // Si la recompensa no está desbloqueada, se añade "DESCONOCIDO"
            if (!recompensa.Desbloqueada)
            {
                return "DESCONOCIDO";
            }
            else
            {
                return recompensa.Nombre;
            }


        }
    }


}