using System.Collections.Generic;
using Microsoft.Maui.Controls;
using NewAdventCalendar2024.Data;
using System.Threading.Tasks;

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
            _pages = new List<string>
            {
                // Entrada del 29 de noviembre
          @"29 de noviembre de 2024 - Expectativa de Vacaciones

¡Por fin! Las vacaciones están a la vuelta de la esquina. He estado soñando con este momento: un lugar caluroso, donde el sol brilla y la arena se siente suave bajo mis pies. A pesar de que el frío se siente cada vez más intenso, tengo la mente llena de imágenes de playas tropicales y cócteles refrescantes.

¡Y me lo merezco, claro que sí! Después de tantos casos, sobre todo este mes de noviembre, ¡bufff! Tengo la cabeza hecha un bombo...

Llevo días preparando mi maleta, asegurándome de no olvidar nada: gafas de sol, protector solar y un buen libro para leer mientras me relajo. Después de semanas de trabajo duro y casos estresantes, este descanso es más que merecido. ¡Diciembre será solo para mí!

Cierro mi libreta con una sonrisa, lista para emprender mi aventura. Este diciembre, solo paz y desconexión me esperan.",


                // Entrada del 30 de noviembre
                 @"30 de noviembre de 2024 - La Tarjeta Misteriosa

Hoy, mientras organizaba mis cosas, encontré algo inesperado: una tarjeta que decía “Nos vemos pronto, Sra. detective”, seguida de una dirección. Al leerla, mi corazón se hundió. Era la dirección de un chalet en medio de la nada.

¿Realmente tengo que ir allí en lugar de disfrutar de la playa? ¿Por qué me pasa esto justo cuando estoy lista para escapar del frío? Me siento frustrada, pero sé que como detective no puedo ignorarlo.

Este chalet misterioso llama a mi curiosidad, aunque preferiría estar tomando el sol. Parece que el destino tiene otros planes para mí. Tendré que enfrentar este misterio, aunque me pese. Una vez más, cierro mi libreta con la esperanza de no usarla... pero por si acaso, mejor la guardo en mi mochila."
        };

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
            PageText.Text = _pages[_currentPageIndex];
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
            if (_currentPageIndex < _pages.Count - 1)
            {
                _currentPageIndex++;
                DisplayCurrentPage();
            }
        }
    }
}
