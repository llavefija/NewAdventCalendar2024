using NewAdventCalendar2024.Tools;
using NewAdventCalendar2024.Data;
using NewAdventCalendar2024.Interfaces;
using NewAdventCalendar2024.Views.Juegos.MultiClicks;
using NewAdventCalendar2024.Views.Juegos.PingPong;
using NewAdventCalendar2024.Views.Juegos.Ahorcado;
using NewAdventCalendar2024.Views.Juegos.PiedraPapelTijeras;
using NewAdventCalendar2024.Views.Juegos.TicTacToe;
using NewAdventCalendar2024.Views.Juegos.Snake;
using NewAdventCalendar2024.Views.Juegos.Wordle;


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
                    var botonId = botonPulsado.AutomationId;

                    // Extraer el número del botón
                    int numeroBoton = int.Parse(botonId.Substring(3));

                    // Parámetros predeterminados
                    var juegoParams = new Dictionary<int, (string, string, ContentPage)>
                    {
                        { 1, ("Multi Clicks", "Pulsa varias veces a la imagen hasta cumplir todos los clics.\nDificultad: 100 clicks.", new MultiClicksPage(100, "vudu.png")) },
                        { 2, ("Ping Pong", "Vence consiguiendo 3 puntos a favor antes que el rival en una batalla de ping pong.\nDificultad: Facil.", new PingPongPage(0.1f, 3, 3.2f)) },
                        { 3, ("Ahorcado", "Descubre la palabra oculta antes de perder la cabeza.\nDificultad: 7 letras.", new AhorcadoPage("MISTERIO")) },
                        { 4, ("Piedra, papel o tijeras", "Gana 3 duelos al juego de piedra, papel o tijeras.\nDificultad: Facil.", new PiPaTiPage(3)) },
                        { 5, ("Wordle", "Descubre la palabra oculta sin quedarte sin intentos.\nDificultad: 5 letras.", new WordlePage("PITON")) },
                        { 7, ("TicTacToe", "Gana al rival 3 partidas al tic tac toe.\nDificultad: Facil.", new TicTacToePage(3)) },
                        { 8, ("Snake", "Come 10 manzanas minimo para completar el desafio.\nDificultad: Facil.", new SnakePage(10)) },

                    };

                    // Verificar si hay parámetros para el botón pulsado
                    if (juegoParams.TryGetValue(numeroBoton, out var parametros))
                    {
                        await ManejarBotonPulsado(parametros.Item1, parametros.Item2, numeroBoton, parametros.Item3);
                    }
                    else
                    {
                        await DisplayAlert("Botón Pulsado", $"Has pulsado un botón desconocido: {numeroBoton}", "OK");
                    }
                }
            }
            catch (FormatException ex)
            {
                await DisplayAlert("ERROR", "Formato incorrecto en el botón pulsado.", "OK");
                Console.WriteLine($"Error de formato: {ex}");
            }
            catch (Exception ex)
            {
                await DisplayAlert("ERROR", $"Ocurrió un error al pulsar el botón: {ex.Message}", "OK");
                Console.WriteLine($"Ha ocurrido un error: {ex}");
            }
            ActualizarBotones();
        }



        private async Task ManejarBotonPulsado(string titulo, string texto, int numeroBoton, ContentPage paginaJuego)
        {
            try
            {
                // Recompensa del botón pulsado
                string recompensaNombre = await NombreRecompensa(numeroBoton);

                // Mostrar alerta con el título, texto personalizado y la recompensa
                bool answer = await DisplayAlert(titulo, $"{texto}\nRecompensa: {recompensaNombre}.", "Jugar", "Cancelar");

                if (answer)
                {
                    // Inicializar TaskCompletionSource
                    var tcs = new TaskCompletionSource<bool>();

                    // Pasar tcs al constructor de la página del juego
                    if (paginaJuego is IGamePage juegoConTcs)
                    {
                        juegoConTcs.InicializarTcs(tcs);
                    }

                    // Navegar a la página del juego
                    await Navigation.PushAsync(paginaJuego, true);

                    // Esperar a que el juego termine
                    bool juegoCompletado = await tcs.Task;

                    // Acciones basadas en si el juego fue completado o no
                    if (juegoCompletado)
                    {
                        await DisplayAlert("Felicidades", "¡Has completado el juego! Has ganado: " + recompensaNombre, "OK");

                        // Marcar el botón como completado y desbloquear la recompensa
                        await _database.CompletarBoton(numeroBoton);
                        await _database.DesbloquearRecompensa(numeroBoton);
                    }
                    else
                    {
                        await DisplayAlert("Juego no completado", "No has completado el juego. Inténtalo más tarde.", "OK");
                    }
                }
              
            }
            catch (Exception ex)
            {
                await DisplayAlert("ERROR", $"Ocurrió un error: {ex.Message}", "OK");
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