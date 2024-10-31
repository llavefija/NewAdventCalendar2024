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
                        { 1, ("¿Quién está ahí?", "¿Hay alguien en casa? Voy a picar varias veces a ver si tengo suerte.\nRequiere 3 picadas.", new MultiClicksPage(3, "puerta.png")) },
                        { 2, ("Ese nombre...", "Llevo todo el día pensando en ese nombre, ¿y si fuera él?\nPista: Nombre.", new WordlePage("MATEO")) },
                        { 3, ("La pelota como terapia", "Nada como hacer botar una pelota para despejar la mente. Quizás pueda usarla para darle un poco de orden a este caos.\nDificultad: 3 encajes.", new PingPongPage(0.1f, 3, 3.2f)) },
                        { 4, ("Música en el aire", "¡Qué suerte, una radio que funciona! Me encanta lo que suena.\nPista: Grupo.", new WordlePage("QUEEN")) },
                        { 5, ("Ordenando pensamientos", "Quizás necesite ordenar mis ideas. Paso a paso, como un juego de estrategia... algo debe haber por aquí que me guíe.\nDificultad: 3 conexiones.", new TicTacToePage(3)) },
                        { 6, ("Frente a la caja fuerte", "Vaya... una caja fuerte. Pero no me sé el código. ¿Quién dijo que iba a ser fácil?\nRequiere 500 golpeos.", new MultiClicksPage(500, "cajafuerte.png")) },
                        { 7, ("Algo para comer", "No puedo pensar con el estómago vacío. Quizás algo dulce me venga bien.\nPista: Fruta.", new WordlePage("MELON")) },
                        { 8, ("Una charla conmigo misma", "Siempre va bien tomarse tiempo para conocerse mejor.\nDificultad: gana 3 duelos.", new PiPaTiPage(3)) },
                        { 9, ("Un regalo misterioso", "Lo abriré cuidadosamente con la navaja... parece que algo se mueve dentro.\nRequiere 10 cortes.", new MultiClicksPage(10, "regalo.png")) },
                        { 10, ("Una gran decisión", "Nunca imaginé tomar esta decisión, pero es el momento de dar el paso.\nPista: Nombre.", new AhorcadoPage("JINGLE")) },

                        { 11, ("El rastro", "Parece que Jingle tiene algo. ¡Buen chico!\nDificultad: 10 capturas.", new SnakePage(10)) },
                        { 12, ("Una caja empotrada", "He encontrado una caja de madera, pero no se abre. Quizás haya que forzarla un poco.\nRequiere 250 sacudidas.", new MultiClicksPage(250, "cajita.png")) },
                        { 13, ("El día de hoy es una mierda", "Solo hay una palabra para describir el día de hoy...\nPista: Adjetivo.", new AhorcadoPage("DESASTROSO")) },
                        { 14, ("Un agradable olor", "No todo es malo. He podido crear una pequeña vela a partir de esos frutos secos.\nPista: Frutos secos.", new AhorcadoPage("ALMENDRA")) },
                        { 15, ("A seguir jugando un poco más", "He estado pensando en una receta sanadora mientras hacía rebotar la pelota.\nDificultad: 6 encajes más rápidos.", new PingPongPage(0.2f, 6, 3.5f)) },
                        { 16, ("Más rápido", "Estoy que me salgo, he dominado mucho control con la pelota.\nDificultad: 6 encajes aún más rápidos.", new PingPongPage(0.2f, 6, 4f)) },
                        { 17, ("Despiste", "Hoy no estoy en mis cabales, me siento muy despistada.\nDificultad: 3 conexiones.", new TicTacToePage(3)) },
                        { 18, ("Me centro", "Tengo un duelo conmigo misma para centrarme un poco más.\nDificultad: gana 3 duelos.", new PiPaTiPage(6)) },
                        { 19, ("Descodificación", "Así que era esto lo que quería decir la nota...\nPista: Mueble.", new AhorcadoPage("ARMARIO")) },
                        { 20, ("Eureka", "Es algo sencillo pero interesante, a tener en cuenta.\nPista: Lo que me gustaría tener.", new WordlePage("CALOR")) },

                        { 21, ("Jingle lo ha vuelto a hacer", "Parece que Jingle se ha dado cuenta de algo, nunca le he dado importancia.\nDificultad: 20 capturas.", new SnakePage(20)) },
                        { 22, ("Decepción o alivio", "No era lo que parecía... y menos mal.\nPista: Criatura.", new AhorcadoPage("DUENDE")) },
                        { 23, ("Un imán", "Pero no uno cualquiera, este era raro, tenía algo. Podría ser un imán _____\nPista: Profesión secreta.", new WordlePage("ESPIA")) },
                        { 24, ("Otro duelo más", "Me voy a perder Navidad.\nDificultad: gana 9 duelos.", new PiPaTiPage(9)) },
                        { 25, ("Un peluche", "Yo solo veía un inofensivo peluche, hasta que decidí abrirlo cuidadosamente.\nRequiere 500 punzadas.", new MultiClicksPage(500, "pingu.png")) },
                        { 26, ("Frustración", "Me siento frustrada ahora mismo.\nDificultad: 10 conexiones.", new TicTacToePage(10)) },
                        { 27, ("Es la hora", "He decidido realizar mi plan más macabro para salir de aquí.\nPista: Material.", new WordlePage("METAL")) },
                        { 28, ("Calma antes de la guerra", "Decido botar por última vez la pelota antes de soltar toda mi ira.\nDificultad: 10 encajes más pausados.", new PingPongPage(0.1f, 10, 3.5f)) },
                        { 29, ("Otra ronda de destrucción", "Ya solo me queda la mitad del chalet por descubrir, hoy será un día largo.\nDificultad: 30 capturas.", new SnakePage(30)) },
                        { 30, ("Abrir la gran puerta", "Parece que las llaves encajan a la perfección, pero cuesta más de lo que parece.\nRequiere 10000 giros con fuerza.", new MultiClicksPage(10000, "grancaja.png")) },

                        { 31, ("El sobre", "Abel me ha dado este sobre, estoy entre emocionada y enfadada.\nRequiere 1 corte.", new MultiClicksPage(1, "sobremisterioso.png")) },



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