using System.Collections.Generic;
using Microsoft.Maui.Controls;
using NewAdventCalendar2024.Data;
using System.Threading.Tasks;
using NewAdventCalendar2024.Models;

namespace NewAdventCalendar2024.Views.PaginasPrincipales
{
    public partial class InformationPage : ContentPage
    {
        private List<string> _pages;
        private int _currentPageIndex;
        private readonly AppDb _db;

        public InformationPage(AppDb db)
        {
            InitializeComponent();
            _db = db;
            NavigationPage.SetHasNavigationBar(this, false);
            InitializePages();
        }

        private async void InitializePages()
        {
            var botonDia1 = await _db.GetBotonPorId(1);
            _pages = new List<string>
            {"", 
       // Condición para incluir "Anotaciones de Ray" solo si el botón está activo.
         botonDia1.Activo ? @"Mis anotaciones para este juego misterioso.

He decidido escribir algunas notas... en caso de que algo salga mal. No sé por qué, pero siento que lo que estoy a punto de enfrentar no será tan sencillo como parece.

--Advertencias para Sobrevivir al Juego Diario--

·31 días, 31 recompensas, 31 riesgos ocultos

    Cada día está envuelto en una nueva capa de misterio. No sé exactamente qué obtendré, ni qué debo hacer para conseguirlo. Al parecer, hay algo escondido tras cada desafío diario, pero algo me dice que estas recompensas no serán solo trofeos... ¿qué podría encontrar al final?

·Un Enigma Diferente Cada Día

    No puedo detener el tiempo. El reloj avanza, y cada día tiene un propósito. Me dijeron que debo enfrentar algo diferente cada vez, y que solo al resolverlo encontraré la clave para desbloquear lo siguiente. Lo que parece un juego puede ser más que eso; quizás, cada desafío diario me esté preparando para algo mayor, más oscuro. Algo que aún no he descubierto.

·Una Cuenta Atrás hasta el Último Día

    Parece un juego de niños, pero hay algo inquietante en esta cuenta atrás. Mi instinto me dice que hay algo esperando al final, y aunque no sé qué será, debo mantenerme alerta. Esto no será una simple aventura, y cada paso hacia adelante también puede ser uno hacia el peligro..."
        : "Aun Ray no ha apuntado nada",

                // Entrada del 29 de noviembre
          @"29 de noviembre de 2024 - Expectativa de Vacaciones

Vacaciones, por fin. Solo la idea me llena de entusiasmo y me hace olvidar el estrés de los últimos días. Tengo un billete para un lugar soleado, y ya casi puedo sentir la arena caliente bajo mis pies. ¿Por qué no? Me lo he ganado.

Los casos de noviembre han sido agotadores, pero todo eso está a punto de quedar atrás. Llevo semanas preparando mi maleta con cada detalle: mis gafas de sol, mi protector solar, un buen libro. Mi plan es simple: relajarme y desconectar de todo. Solo paz... o eso creía.

Pero mientras hago los últimos preparativos, tengo una sensación extraña. Algo que me impide celebrar plenamente. No es nada específico, solo... una corazonada. Trato de ignorarla, atribuyéndolo al cansancio. Cierro mi libreta con una sonrisa, tratando de convencerme de que, a partir de ahora, todo será calma. Estoy lista para este viaje... aunque hay algo que me dice que no todo está bajo mi control.",


                // Entrada del 30 de noviembre
                 @"30 de noviembre de 2024 - La Tarjeta Misteriosa

Hoy, mientras organizaba mis cosas para el viaje, una tarjeta inesperada se deslizó fuera de mi bolso. En ella, unas pocas palabras bastaron para hacerme olvidar el sol y la playa: 'Nos vemos pronto, Sra. detective'. Seguido de una dirección... en mitad de la nada.

Al verla, mi mente intenta encontrar alguna explicación lógica, pero solo siento que el suelo se vuelve inestable bajo mis pies. Esta dirección no me resulta familiar, y aunque preferiría ignorarla y seguir con mis vacaciones, mi deber como detective me llama. ¿Es una simple coincidencia o alguien me está buscando?

Mi instinto me dice que algo está por comenzar, y aunque cada fibra de mi ser me dice que evite esa dirección, no puedo hacerlo. Cierro mi libreta, pero esta vez con una sensación de incertidumbre que no puedo disipar. Ya no siento la misma emoción por el viaje; algo mucho más serio ha captado mi atención. Decido llevar conmigo la libreta. Puede que la necesite. Quizás más pronto de lo que creo..."};

            // Añade las páginas de diciembre
            for (int day = 1; day <= 31; day++)
            {
                string pageContent = await GetPageContentForDay(day);
                _pages.Add(pageContent);
            }

            _currentPageIndex = 0;
            DisplayCurrentPage();
        }

        // Método que genera el contenido de la página del día
        private async Task<string> GetPageContentForDay(int day)
        {
            var button = await _db.GetBotonPorId(day); // Suponiendo que tienes un método para obtener botones por ID
            var recompensa = await _db.GetRecompensaPorId(day); // Suponiendo que tienes un método para obtener botones por ID


            if (button != null && button.Completado)
            {
                return $"Día {day} de Diciembre - {button.MisterioDescripcion}\nHoy he conseguido: {recompensa.Nombre}"; // Aquí 'MisterioDescripcion' es el contenido de ese día
            }
            else
            {
                return $"Día {day} de Diciembre\n\nCompleta el misterio del día para desbloquear esta página.";
            }
        }

        private void DisplayCurrentPage()
        {
            // Lógica para mostrar las tapas y las hojas según el índice de página actual
            if (_currentPageIndex == 0)
            {
                // Primera página: mostrar la tapa delantera
                BackgroundImage.Source = "tapadelanteralibreta.png";
                PageText.IsVisible = false; // Ocultar texto en la tapa
            }
            else if (_currentPageIndex == _pages.Count) // La página final + 1
            {
                // Última página + 1: mostrar la tapa trasera
                BackgroundImage.Source = "tapatraseralibreta.png";
                PageText.IsVisible = false; // Ocultar texto en la tapa
            }
            else
            {
                // Páginas intermedias: mostrar las hojas
                BackgroundImage.Source = "hojalibreta.png";
                PageText.IsVisible = true; // Mostrar texto en el resto de páginas
                PageText.Text = _pages[_currentPageIndex]; // Actualizar el texto de la página
            }
        }

        private void OnPreviousPageClicked(object sender, EventArgs e)
        {
            if (_currentPageIndex > 0)
            {
                _currentPageIndex--;
                DisplayCurrentPage();
            }
        }

        private void OnNextPageClicked(object sender, EventArgs e)
        {
            if (_currentPageIndex <= _pages.Count) // Permitir acceder a la última + 1 para la tapa trasera
            {
                _currentPageIndex++;
                DisplayCurrentPage();
            }
        }
    }
}
