using NewAdventCalendar2024.Tools;
using NewAdventCalendar2024.Data;
using NewAdventCalendar2024.Interfaces;
using NewAdventCalendar2024.Views.Juegos.MultiClicks;
using NewAdventCalendar2024.Views.Juegos.PingPong;


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
                    int numeroBoton = int.Parse(botonId.Substring(3)); // "Btn1" => 1

                    int numeroDeClicks = 0;
                    string imagen = "";

                    int puntos = 0;
                    float incremento = 0;
                    float velocidad = 0;


                    // Instanciar ToolBotones con el botón clicado
                    var toolButton = new ToolBotones(botonPulsado);
                    await toolButton.AnimateButton(); // Animación de botón

                    // Manejar el botón basado en su número
                    switch (numeroBoton)
                    {
                        case 1:
                            // Parámetros personalizados para el juego MultiClicks
                            numeroDeClicks = 100; // Por ejemplo
                            imagen = "vudu.png"; // Imagen por defecto o personalizada

                            await ManejarBotonPulsado("Multi Clicks",
                                "Pulsa varias veces a la imagen hasta cumplir todos los clics.\nDificultad: " + numeroDeClicks + " clics.",
                                numeroBoton,
                                new MultiClicksPage(numeroDeClicks, imagen));
                            break;

                        case 2:
                            // Parámetros personalizados para el juego MultiClicks
                            puntos = 5; // Por ejemplo
                            incremento = 0.5f; // Imagen por defecto o personalizada
                            velocidad = 8f;

                            await ManejarBotonPulsado("Ping Pong",
                                "Vence consiguiendo " + puntos + " puntos a favor antes que el rival en una batalla de ping pong.\nDificultad: Facil." + numeroDeClicks + " clics.",
                                numeroBoton,
                                new PingPongPage(incremento, puntos, velocidad));
                            break;

                        case 10:
                            // Parámetros personalizados para el juego MultiClicks
                            numeroDeClicks = 250; // Por ejemplo
                            imagen = "regalo.png"; // Imagen por defecto o personalizada

                            await ManejarBotonPulsado("Multi Clicks",
                                "Pulsa varias veces a la imagen hasta cumplir todos los clics.\nDificultad: " + numeroDeClicks + " clics.",
                                numeroBoton,
                                new MultiClicksPage(numeroDeClicks, imagen));
                            break;

                        case 11:
                            // Parámetros personalizados para el juego MultiClicks
                            puntos = 7; // Por ejemplo
                            incremento = 1f; // Imagen por defecto o personalizada
                            velocidad = 9f;

                            await ManejarBotonPulsado("Ping Pong",
                                "Vence consiguiendo " + puntos + " puntos a favor antes que el rival en una batalla de ping pong.\nDificultad: Media." + numeroDeClicks + " clics.",
                                numeroBoton,
                                new PingPongPage(incremento, puntos, velocidad));
                            break;

                        case 18:
                            // Parámetros personalizados para el juego MultiClicks
                            numeroDeClicks = 1000; // Por ejemplo
                            imagen = "oldcorazongorro.png"; // Imagen por defecto o personalizada

                            await ManejarBotonPulsado("Multi Clicks",
                                "Pulsa varias veces a la imagen hasta cumplir todos los clics.\nDificultad: " + numeroDeClicks + " clics.",
                                numeroBoton,
                                new MultiClicksPage(numeroDeClicks, imagen));
                            break;

                        case 19:
                            // Parámetros personalizados para el juego MultiClicks
                            puntos = 10; // Por ejemplo
                            incremento = 1.5f; // Imagen por defecto o personalizada
                            velocidad = 9f;

                            await ManejarBotonPulsado("Ping Pong",
                                "Vence consiguiendo " + puntos + " puntos a favor antes que el rival en una batalla de ping pong.\nDificultad: Dificil." + numeroDeClicks + " clics.",
                                numeroBoton,
                                new PingPongPage(incremento, puntos, velocidad));
                            break;


                        // Otros casos hasta el botón 31
                        default:
                            await DisplayAlert("Botón Pulsado", $"Has pulsado un botón desconocido: {numeroBoton}", "OK");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("ERROR", $"Ocurrió un error al pulsar el botón: {ex.Message}", "OK");
                Console.WriteLine($"Ha ocurrido un error: {ex}");
            }
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