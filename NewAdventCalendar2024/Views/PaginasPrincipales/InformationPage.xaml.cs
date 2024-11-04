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

�Por fin! Las vacaciones est�n a la vuelta de la esquina. He estado so�ando con este momento: un lugar caluroso, donde el sol brilla y la arena se siente suave bajo mis pies. A pesar de que el fr�o se siente cada vez m�s intenso, tengo la mente llena de im�genes de playas tropicales y c�cteles refrescantes.

�Y me lo merezco, claro que s�! Despu�s de tantos casos, sobre todo este mes de noviembre, �bufff! Tengo la cabeza hecha un bombo...

Llevo d�as preparando mi maleta, asegur�ndome de no olvidar nada: gafas de sol, protector solar y un buen libro para leer mientras me relajo. Despu�s de semanas de trabajo duro y casos estresantes, este descanso es m�s que merecido. �Diciembre ser� solo para m�!

Cierro mi libreta con una sonrisa, lista para emprender mi aventura. Este diciembre, solo paz y desconexi�n me esperan.",


                // Entrada del 30 de noviembre
                 @"30 de noviembre de 2024 - La Tarjeta Misteriosa

Hoy, mientras organizaba mis cosas, encontr� algo inesperado: una tarjeta que dec�a �Nos vemos pronto, Sra. detective�, seguida de una direcci�n. Al leerla, mi coraz�n se hundi�. Era la direcci�n de un chalet en medio de la nada.

�Realmente tengo que ir all� en lugar de disfrutar de la playa? �Por qu� me pasa esto justo cuando estoy lista para escapar del fr�o? Me siento frustrada, pero s� que como detective no puedo ignorarlo.

Este chalet misterioso llama a mi curiosidad, aunque preferir�a estar tomando el sol. Parece que el destino tiene otros planes para m�. Tendr� que enfrentar este misterio, aunque me pese. Una vez m�s, cierro mi libreta con la esperanza de no usarla... pero por si acaso, mejor la guardo en mi mochila."
        };

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
