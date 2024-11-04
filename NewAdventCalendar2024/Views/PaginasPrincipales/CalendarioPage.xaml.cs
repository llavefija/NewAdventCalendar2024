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
    { 1, ("El chalet", "Parece que esta es la dirección. Veamos si funciona al picar.\n\nJuego: MultiClicks.\nDificultad: 3 picadas.", new MultiClicksPage(3, "puerta.png", "El chalet")) },
    { 2, ("Una mala noche", "Llevo todo el día pensando en ese nombre, no me lo quito de la cabeza.\n\nJuego: Wordle.\nPista: Nombre.", new WordlePage("MATEO", "Una mala noche")) },
    { 3, ("Un juego de mi imaginación", "Nada como hacer botar una pelota para despejar la mente.\n\nJuego: Ping Pong.\nDificultad: 3 encajes.", new PingPongPage(0.1f, 3, 3.2f, "Un juego de mi imaginación")) },
    { 4, ("Algo de música", "Encuentro una radio antigua; la música me recuerda a tiempos mejores.\n\nJuego: Wordle.\nPista: Grupo.", new WordlePage("QUEEN", "Algo de música")) },
    { 5, ("Venganza", "Quizás ordenar mis ideas me dé una pista sobre quién está detrás de todo esto.\n\nJuego: Tic Tac Toe.\nDificultad: 3 conexiones.", new TicTacToePage(3, "Venganza")) },
    { 6, ("Una caja demasiado fuerte", "Una caja fuerte bloquea mi avance. Necesito un código o algo para abrirla.\n\nJuego: MultiClicks.\nDificultad: 250 golpeos.", new MultiClicksPage(250, "cajafuerte.png", "Una caja demasiado fuerte")) },
    { 7, ("Un gran chasco", "Necesito energía, y parece que tengo una fruta por aquí.\n\nJuego: Wordle.\nPista: Fruta.", new WordlePage("MELON", "Un gran chasco")) },
    { 8, ("¿El rosa es mi color?", "Tener confianza en mí misma es clave. Veamos qué tal me va en este duelo.\n\nJuego: Piedra, Papel o Tijeras.\nDificultad: gana 3 duelos.", new PiPaTiPage(3, "¿El rosa es mi color?")) },
    { 9, ("Un enorme regalo en el comedor", "Un paquete grande y sospechoso. Intentaré abrirlo con cuidado.\n\nJuego: MultiClicks.\nDificultad: 10 cortes.", new MultiClicksPage(10, "regalo.png", "Un enorme regalo en el comedor")) },
    { 10, ("Mi fiel compañero", "Un nombre me viene a la mente, el de alguien especial.\n\nJuego: Ahorcado.\nPista: Nombre.", new AhorcadoPage("JINGLE", "Mi fiel compañero")) },

    { 11, ("Una misteriosa nota", "Parece que Jingle me ha traído algo, un rastro que debo seguir.\n\nJuego: Snake.\nDificultad: 10 capturas.", new SnakePage(10, "Una misteriosa nota")) },
    { 12, ("Una cajita misteriosa en la pared", "He encontrado una pequeña caja de madera que parece necesitar un empujón.\n\nJuego: MultiClicks.\nDificultad: 100 sacudidas.", new MultiClicksPage(100, "cajita.png", "Una cajita misteriosa en la pared")) },
    { 13, ("Un caramelo de esperanza", "Algo dulce me distrae de esta situación. Quizás un juego me relaje.\n\nJuego: Ping Pong.\nDificultad: 5 encajes.", new PingPongPage(0.1f, 5, 3.2f, "Un caramelo de esperanza")) },
    { 14, ("Descubrimientos oscuros en un día gris", "La vela me revela algo inesperado mientras intento ordenar mis pensamientos.\n\nJuego: Tic Tac Toe.\nDificultad: 6 conexiones.", new TicTacToePage(6, "Descubrimientos oscuros en un día gris")) },
    { 15, ("La gran receta", "Una receta de paciencia y habilidad. Necesito precisión con la pelota.\n\nJuego: Ping Pong.\nDificultad: 6 encajes más rápidos.", new PingPongPage(0.2f, 6, 3.5f, "La gran receta")) },
    { 16, ("Despertando con Energía", "Me siento imparable. Esta vez iré más rápido.\n\nJuego: Ping Pong.\nDificultad: 6 encajes aún más rápidos.", new PingPongPage(0.2f, 6, 4f, "Despertando con Energía")) },
    { 17, ("¿Dónde tengo la cabeza...?", "Hoy me siento despistada. Quizás algo me ayude a concentrarme.\n\nJuego: Snake.\nDificultad: 20 capturas.", new SnakePage(15, "¿Dónde tengo la cabeza...?")) },
    { 18, ("Todo va mejor", "Otro duelo conmigo misma para centrarme.\n\nJuego: Piedra, Papel o Tijeras.\nDificultad: gana 6 duelos.", new PiPaTiPage(6, "Todo va mejor")) },
    { 19, ("Mi taza favorita", "Una nota encriptada dentro de la taza... ¿qué intentarán decirme?\n\nJuego: Tic Tac Toe.\nDificultad: 9 conexiones.", new TicTacToePage(9, "Mi taza favorita")) },
    { 20, ("El complemento perfecto", "Las dudas nublan mi mente. Quizás una palabra me aclare.\n\nJuego: Wordle.\nPista: Un deseo.", new WordlePage("CALOR", "El complemento perfecto")) },

    { 21, ("Un descubrimiento extraño", "Parece que Jingle ha notado algo extraño. Quizás me acerque al final.\n\nJuego: Snake.\nDificultad: 25 capturas.", new SnakePage(20, "Un descubrimiento extraño")) },
    { 22, ("Un misterio que me tiene atrapada", "Siento que alguien me observa... pero no hay nadie más aquí.\n\nJuego: Ahorcado.\nPista: Objeto oculto.", new AhorcadoPage("CAMARA", "Un misterio que me tiene atrapada")) },
    { 23, ("Un dulce descubrimiento", "Encuentro un cifrado. ¿Podría ser el famoso código César?\n\nJuego: Wordle.\nPista: Nombre del cifrado.", new WordlePage("CESAR", "Un dulce descubrimiento")) },
    { 24, ("Nochebuena en el encierro", "Ya tengo una idea de dónde buscar. Espero que no me esté equivocando.\n\nJuego: Ahorcado.\nPista: Lugar oculto.", new AhorcadoPage("ARMARIO", "Nochebuena en el encierro")) },
    { 25, ("El peluche oculto", "Hay algo extraño en este peluche... debería examinarlo a fondo.\n\nJuego: MultiClicks.\nDificultad: 500 punzadas.", new MultiClicksPage(500, "pingu.png", "El peluche oculto")) },
    { 26, ("Ansias por salir", "Mi paciencia se agota. Necesito superar este duelo.\n\nJuego: Piedra, Papel o Tijeras.\nDificultad: gana 10 duelos.", new PiPaTiPage(10, "Ansias por salir")) },
    { 27, ("Un día más, un día menos", "Este material metálico me podría servir para mi plan.\n\nJuego: Wordle.\nPista: Material.", new WordlePage("METAL", "Un día más, un día menos")) },
    { 28, ("Al ritmo del Rock", "La última vez que hago botar la pelota antes de tomar mi decisión final.\n\nJuego: Ping Pong.\nDificultad: 10 encajes pausados.", new PingPongPage(0.1f, 10, 3.5f, "Al ritmo del Rock")) },
    { 29, ("Creo que tengo una pista", "Ya veo una posible salida. Solo necesito paciencia.\n\nJuego: Snake.\nDificultad: 30 capturas.", new SnakePage(25, "Creo que tengo una pista")) },
    { 30, ("La puerta a lo desconocido", "Las llaves encajan, pero la cerradura es más dura de lo que parece.\n\nJuego: MultiClicks.\nDificultad: 500 giros con fuerza.", new MultiClicksPage(500, "grancaja.png", "La puerta a lo desconocido")) },

    { 31, ("El sobre rojo", "¿Qué contendrá este misterioso sobre? No sé si estoy lista.\n\nJuego: MultiClicks.\nDificultad: 1 corte.", new MultiClicksPage(1, "sobremisterioso.png", "El sobre rojo")) },
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