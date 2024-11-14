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
       // Condici�n para incluir "Anotaciones de Ray" solo si el bot�n est� activo.
         botonDia1.Activo ? @"Mis anotaciones para este juego misterioso.

He decidido escribir algunas notas... en caso de que algo salga mal. No s� por qu�, pero siento que lo que estoy a punto de enfrentar no ser� tan sencillo como parece.

--Advertencias para Sobrevivir al Juego Diario--

�31 d�as, 31 recompensas, 31 riesgos ocultos

    Cada d�a est� envuelto en una nueva capa de misterio. No s� exactamente qu� obtendr�, ni qu� debo hacer para conseguirlo. Al parecer, hay algo escondido tras cada desaf�o diario, pero algo me dice que estas recompensas no ser�n solo trofeos... �qu� podr�a encontrar al final?

�Un Enigma Diferente Cada D�a

    No puedo detener el tiempo. El reloj avanza, y cada d�a tiene un prop�sito. Me dijeron que debo enfrentar algo diferente cada vez, y que solo al resolverlo encontrar� la clave para desbloquear lo siguiente. Lo que parece un juego puede ser m�s que eso; quiz�s, cada desaf�o diario me est� preparando para algo mayor, m�s oscuro. Algo que a�n no he descubierto.

�Una Cuenta Atr�s hasta el �ltimo D�a

    Parece un juego de ni�os, pero hay algo inquietante en esta cuenta atr�s. Mi instinto me dice que hay algo esperando al final, y aunque no s� qu� ser�, debo mantenerme alerta. Esto no ser� una simple aventura, y cada paso hacia adelante tambi�n puede ser uno hacia el peligro..."
        : "Aun Ray no ha apuntado nada",

                // Entrada del 29 de noviembre
          @"29 de noviembre de 2024 - Expectativa de Vacaciones

Vacaciones, por fin. Solo la idea me llena de entusiasmo y me hace olvidar el estr�s de los �ltimos d�as. Tengo un billete para un lugar soleado, y ya casi puedo sentir la arena caliente bajo mis pies. �Por qu� no? Me lo he ganado.

Los casos de noviembre han sido agotadores, pero todo eso est� a punto de quedar atr�s. Llevo semanas preparando mi maleta con cada detalle: mis gafas de sol, mi protector solar, un buen libro. Mi plan es simple: relajarme y desconectar de todo. Solo paz... o eso cre�a.

Pero mientras hago los �ltimos preparativos, tengo una sensaci�n extra�a. Algo que me impide celebrar plenamente. No es nada espec�fico, solo... una corazonada. Trato de ignorarla, atribuy�ndolo al cansancio. Cierro mi libreta con una sonrisa, tratando de convencerme de que, a partir de ahora, todo ser� calma. Estoy lista para este viaje... aunque hay algo que me dice que no todo est� bajo mi control.",


                // Entrada del 30 de noviembre
                 @"30 de noviembre de 2024 - La Tarjeta Misteriosa

Hoy, mientras organizaba mis cosas para el viaje, una tarjeta inesperada se desliz� fuera de mi bolso. En ella, unas pocas palabras bastaron para hacerme olvidar el sol y la playa: 'Nos vemos pronto, Sra. detective'. Seguido de una direcci�n... en mitad de la nada.

Al verla, mi mente intenta encontrar alguna explicaci�n l�gica, pero solo siento que el suelo se vuelve inestable bajo mis pies. Esta direcci�n no me resulta familiar, y aunque preferir�a ignorarla y seguir con mis vacaciones, mi deber como detective me llama. �Es una simple coincidencia o alguien me est� buscando?

Mi instinto me dice que algo est� por comenzar, y aunque cada fibra de mi ser me dice que evite esa direcci�n, no puedo hacerlo. Cierro mi libreta, pero esta vez con una sensaci�n de incertidumbre que no puedo disipar. Ya no siento la misma emoci�n por el viaje; algo mucho m�s serio ha captado mi atenci�n. Decido llevar conmigo la libreta. Puede que la necesite. Quiz�s m�s pronto de lo que creo..."};

            // A�ade las p�ginas de diciembre
            for (int day = 1; day <= 31; day++)
            {
                string pageContent = await GetPageContentForDay(day);
                _pages.Add(pageContent);
            }

            _currentPageIndex = 0;
            DisplayCurrentPage();
        }

        // M�todo que genera el contenido de la p�gina del d�a
        private async Task<string> GetPageContentForDay(int day)
        {
            var button = await _db.GetBotonPorId(day); // Suponiendo que tienes un m�todo para obtener botones por ID
            var recompensa = await _db.GetRecompensaPorId(day); // Suponiendo que tienes un m�todo para obtener botones por ID


            if (button != null && button.Completado)
            {
                return $"D�a {day} de Diciembre - {button.MisterioDescripcion}\nHoy he conseguido: {recompensa.Nombre}"; // Aqu� 'MisterioDescripcion' es el contenido de ese d�a
            }
            else
            {
                return $"D�a {day} de Diciembre\n\nCompleta el misterio del d�a para desbloquear esta p�gina.";
            }
        }

        private void DisplayCurrentPage()
        {
            // L�gica para mostrar las tapas y las hojas seg�n el �ndice de p�gina actual
            if (_currentPageIndex == 0)
            {
                // Primera p�gina: mostrar la tapa delantera
                BackgroundImage.Source = "tapadelanteralibreta.png";
                PageText.IsVisible = false; // Ocultar texto en la tapa
            }
            else if (_currentPageIndex == _pages.Count) // La p�gina final + 1
            {
                // �ltima p�gina + 1: mostrar la tapa trasera
                BackgroundImage.Source = "tapatraseralibreta.png";
                PageText.IsVisible = false; // Ocultar texto en la tapa
            }
            else
            {
                // P�ginas intermedias: mostrar las hojas
                BackgroundImage.Source = "hojalibreta.png";
                PageText.IsVisible = true; // Mostrar texto en el resto de p�ginas
                PageText.Text = _pages[_currentPageIndex]; // Actualizar el texto de la p�gina
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
            if (_currentPageIndex <= _pages.Count) // Permitir acceder a la �ltima + 1 para la tapa trasera
            {
                _currentPageIndex++;
                DisplayCurrentPage();
            }
        }
    }
}
