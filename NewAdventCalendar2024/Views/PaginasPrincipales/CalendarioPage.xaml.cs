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
using System.Runtime.InteropServices;
using NewAdventCalendar2024.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;


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

            NavigationPage.SetHasNavigationBar(this, false);

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
                var toolButton = new ToolBotones((Button)sender);

                await toolButton.AnimateButton();

                if (botonPulsado != null)
                {
                    // Obtener el nombre del botón pulsado
                    var botonId = botonPulsado.AutomationId;

                    // Extraer el número del botón
                    int numeroBoton = int.Parse(botonId.Substring(3));

                    // Parámetros predeterminados
                    var juegoParams = new Dictionary<int, (string, string, ContentPage)>
                    {
    { 1, ("El chalet", "Parece que esta es la dirección. Veamos si funciona al picar.\n\nJuego: MultiClicks.\nDificultad: 3 picadas.", new MultiClicksPage(3, "puerta.png", "el chalet", Color.FromArgb("#1A3A5F"))) },
    { 2, ("Una mala noche", "Llevo todo el día pensando en ese nombre, no me lo quito de la cabeza.\n\nJuego: Wordle.\nPista: Nombre.", new WordlePage("MATEO", "una mala noche")) },
    { 3, ("Un juego de mi imaginación", "Nada como hacer botar una pelota para despejar la mente.\n\nJuego: Ping Pong.\nDificultad: 3 encajes.", new PingPongPage(0.1f, 3, 3.2f, "un juego de mi imaginación")) },
    { 4, ("Algo de música", "Encuentro una radio antigua; la música me recuerda a tiempos mejores.\n\nJuego: Wordle.\nPista: Grupo.", new WordlePage("QUEEN", "algo de música")) },
    { 5, ("Venganza", "Quizás ordenar mis ideas me dé una pista sobre quién está detrás de todo esto.\n\nJuego: Tic Tac Toe.\nDificultad: 3 conexiones.", new TicTacToePage(3, "venganza")) },
    { 6, ("Una caja demasiado fuerte", "Una caja fuerte bloquea mi avance. Necesito un código o algo para abrirla.\n\nJuego: MultiClicks.\nDificultad: 250 golpeos.", new MultiClicksPage(250, "cajafuerte.png", "una caja demasiado fuerte", Color.FromArgb("#4682B4"))) },
    { 7, ("Un gran chasco", "Necesito energía, y parece que tengo una fruta por aquí.\n\nJuego: Wordle.\nPista: Fruta.", new WordlePage("MELON", "un gran chasco")) },
    { 8, ("¿El rosa es mi color?", "Tener confianza en mí misma es clave. Veamos qué tal me va en este duelo.\n\nJuego: Piedra, Papel o Tijeras.\nDificultad: gana 3 duelos.", new PiPaTiPage(3, "¿el rosa es mi color?")) },
    { 9, ("Un enorme regalo en el comedor", "Un paquete grande y sospechoso. Intentaré abrirlo con cuidado.\n\nJuego: MultiClicks.\nDificultad: 10 cortes.", new MultiClicksPage(10, "regalo.png", "un enorme regalo en el comedor", Color.FromArgb("#D3D3D3"))) },
    { 10, ("Mi fiel compañero", "Un nombre me viene a la mente, el de alguien especial.\n\nJuego: Ahorcado.\nPista: Nombre.", new AhorcadoPage("JINGLE", "mi fiel compañero")) },

    { 11, ("Una misteriosa nota", "Parece que Jingle me ha traído algo, un rastro que debo seguir.\n\nJuego: Snake.\nDificultad: 10 capturas.", new SnakePage(10, "Una misteriosa nota")) },
    { 12, ("Una cajita misteriosa en la pared", "He encontrado una pequeña caja de madera que parece necesitar un empujón.\n\nJuego: MultiClicks.\nDificultad: 100 sacudidas.", new MultiClicksPage(100, "cajita.png", "una cajita misteriosa en la pared", Color.FromArgb("#D2B48C"))) },
    { 13, ("Un caramelo de esperanza", "Algo dulce me distrae de esta situación. Quizás un juego me relaje.\n\nJuego: Ping Pong.\nDificultad: 5 encajes.", new PingPongPage(0.1f, 5, 3.2f, "un caramelo de esperanza")) },
    { 14, ("Descubrimientos oscuros en un día gris", "La vela me revela algo inesperado mientras intento ordenar mis pensamientos.\n\nJuego: Tic Tac Toe.\nDificultad: 6 conexiones.", new TicTacToePage(6, "descubrimientos oscuros en un día gris")) },
    { 15, ("La gran receta", "Una receta de paciencia y habilidad. Necesito precisión con la pelota.\n\nJuego: Ping Pong.\nDificultad: 6 encajes más rápidos.", new PingPongPage(0.2f, 6, 3.5f, "la gran receta")) },
    { 16, ("Despertando con Energía", "Me siento imparable. Esta vez iré más rápido.\n\nJuego: Ping Pong.\nDificultad: 6 encajes aún más rápidos.", new PingPongPage(0.2f, 6, 4f, "despertando con Energía")) },
    { 17, ("¿Dónde tengo la cabeza...?", "Hoy me siento despistada. Quizás algo me ayude a concentrarme.\n\nJuego: Snake.\nDificultad: 20 capturas.", new SnakePage(15, "¿dónde tengo la cabeza...?")) },
    { 18, ("Todo va mejor", "Otro duelo conmigo misma para centrarme.\n\nJuego: Piedra, Papel o Tijeras.\nDificultad: gana 6 duelos.", new PiPaTiPage(6, "todo va mejor")) },
    { 19, ("Mi taza favorita", "Una nota encriptada dentro de la taza... ¿qué intentarán decirme?\n\nJuego: Tic Tac Toe.\nDificultad: 9 conexiones.", new TicTacToePage(9, "mi taza favorita")) },
    { 20, ("El complemento perfecto", "Las dudas nublan mi mente. Quizás una palabra me aclare.\n\nJuego: Wordle.\nPista: Un deseo.", new WordlePage("CALOR", "el complemento perfecto")) },

    { 21, ("Un descubrimiento extraño", "Parece que Jingle ha notado algo extraño. Quizás me acerque al final.\n\nJuego: Snake.\nDificultad: 25 capturas.", new SnakePage(20, "un descubrimiento extraño")) },
    { 22, ("Un misterio que me tiene atrapada", "Siento que alguien me observa... pero no hay nadie más aquí.\n\nJuego: Ahorcado.\nPista: Objeto oculto.", new AhorcadoPage("CAMARA", "un misterio que me tiene atrapada")) },
    { 23, ("Un dulce descubrimiento", "Encuentro un cifrado. ¿Podría ser el famoso código César?\n\nJuego: Wordle.\nPista: Nombre del cifrado.", new WordlePage("CESAR", "un dulce descubrimiento")) },
    { 24, ("Nochebuena en el encierro", "Ya tengo una idea de dónde buscar. Espero que no me esté equivocando.\n\nJuego: Ahorcado.\nPista: Lugar oculto.", new AhorcadoPage("ARMARIO", "nochebuena en el encierro")) },
    { 25, ("El peluche oculto", "Hay algo extraño en este peluche... debería examinarlo a fondo.\n\nJuego: MultiClicks.\nDificultad: 500 punzadas.", new MultiClicksPage(500, "pingu.png", "el peluche oculto", Color.FromArgb("#E6E6FA"))) },
    { 26, ("Ansias por salir", "Mi paciencia se agota. Necesito superar este duelo.\n\nJuego: Piedra, Papel o Tijeras.\nDificultad: gana 10 duelos.", new PiPaTiPage(10, "ansias por salir")) },
    { 27, ("Un día más, un día menos", "Este material metálico me podría servir para mi plan.\n\nJuego: Wordle.\nPista: Material.", new WordlePage("METAL", "un día más, un día menos")) },
    { 28, ("Al ritmo del Rock", "La última vez que hago botar la pelota antes de tomar mi decisión final.\n\nJuego: Ping Pong.\nDificultad: 10 encajes pausados.", new PingPongPage(0.1f, 10, 3.5f, "al ritmo del Rock")) },
    { 29, ("Creo que tengo una pista", "Ya veo una posible salida. Solo necesito paciencia.\n\nJuego: Snake.\nDificultad: 30 capturas.", new SnakePage(25, "creo que tengo una pista")) },
    { 30, ("La puerta a lo desconocido", "Las llaves encajan, pero la cerradura es más dura de lo que parece.\n\nJuego: MultiClicks.\nDificultad: 500 giros con fuerza.", new MultiClicksPage(500, "grancaja.png", "la puerta a lo desconocido", Color.FromArgb("#F5F5DC "))) },

    { 31, ("El sobre rojo", "¿Qué contendrá este misterioso sobre? No sé si estoy lista.\n\nJuego: MultiClicks.\nDificultad: 1 corte.", new MultiClicksPage(1, "sobremisterioso.png", "l sobre rojo", Color.FromArgb("#98FB98 "))) },
};
                    if (numeroBoton > 1)
                    {
                        var button = await _database.GetBotonPorId(numeroBoton-1); // Suponiendo que tienes un método para obtener botones por ID

                        if (!button.Completado)
                        {
                            await DisplayAlert("Juego No Completado", "Debes completar el juego anterior antes de continuar.", "OK");
                            return; // Salir del método si el juego anterior no está completo
                        }
                    }
                   

                   
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
                bool answer = await DisplayAlert(titulo, $"{texto}\nRecompensa: {recompensaNombre}", "Jugar", "Cancelar");

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
                        // Marcar el botón como completado y desbloquear la recompensa
                        await _database.CompletarBoton(numeroBoton);
                        await _database.DesbloquearRecompensa(numeroBoton);

                        await DisplayAlert("Felicidades", "¡Has completado el juego! Has ganado: " + recompensaNombre, "OK");
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