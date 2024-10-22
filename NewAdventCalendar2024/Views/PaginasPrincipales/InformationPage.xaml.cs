using System.Collections.Generic;
using Microsoft.Maui.Controls;
using NewAdventCalendar2024.Data; // Aseg�rate de importar el espacio de nombres correcto

namespace NewAdventCalendar2024.Views.PaginasPrincipales
{
    public partial class InformationPage : ContentPage
    {
        private List<string> _pages;
        private int _currentPageIndex;
        private readonly AppDb _db; // Instancia de AppDb

        public InformationPage(AppDb db) // Constructor con AppDb como par�metro
        {
            InitializeComponent();
            _db = db; // Guarda la instancia de la base de datos
            InitializePages();
            DisplayCurrentPage();
        }

        private async void InitializePages()
        {
            _pages = new List<string>
            {
@"NORMAS DEL JUEGO:

1. Cada d�a se desbloquea un nuevo boton.
2. cada bot�n corresponde a un minijuego.
3. Si completas el minijuego desbloquearas una recompensa unica.
4. Para abrir el dia 31 hara falta completar todos los anteriores dias.",
@"C�MO JUGAR:

1. Abre la pesta�a de calendario seleciona el d�a actual.
2. Cada d�a tiene un minijuego distinto y si no logras superarlo puedes volverlo a intentar mas tarde.
3. "
            };

            // Obtiene la lista de recompensas y la convierte a un formato de cadena
            var recompensas = await ListaDeRecompensas();
            _pages.Add($@"RECOMPENSAS: 
{string.Join("\n ", recompensas)}"); // Une los nombres de las recompensas en una sola cadena

            _currentPageIndex = 0;
            DisplayCurrentPage(); // Muestra la p�gina inicial

        }

        // Devuelve una lista de nombres de recompensas
        public async Task<List<string>> ListaDeRecompensas()
        {
            // Obtiene la lista de recompensas desde la base de datos
            var recompensas = await _db.GetRecompensas();

            // Crea una lista para almacenar los nombres de las recompensas
            List<string> nombresRecompensas = new List<string>();

            // Recorre la lista de recompensas y modifica el nombre si no est� desbloqueada
            foreach (var recompensa in recompensas)
            {
                // Si la recompensa no est� desbloqueada, se a�ade "DESCONOCIDO"
                if (!recompensa.Desbloqueada)
                {
                    nombresRecompensas.Add($"Recompensa {recompensa.Id}: DESCONOCIDO");
                }
                else
                {
                    // Si est� desbloqueada, se a�ade su nombre
                    nombresRecompensas.Add($"Recompensa {recompensa.Id}: {recompensa.Nombre}");
                }
            }

            return nombresRecompensas;
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
