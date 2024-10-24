using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using NewAdventCalendar2024.Interfaces;

namespace NewAdventCalendar2024.Views.Juegos.MultiClicks
{
    public partial class MultiClicksPage : ContentPage, IGamePage
    {
        public string ImageSource { get; set; }
        public int RemainingClicks { get; set; }
        private TaskCompletionSource<bool> _tcs;
        private bool gameCompleted = false;

        public MultiClicksPage(int initialClicks, string imageSource)
        {
            InitializeComponent();
            RemainingClicks = initialClicks;
            ImageSource = imageSource;

            UpdateBindings();
        }

        public void InicializarTcs(TaskCompletionSource<bool> tcs)
        {
            _tcs = tcs;
        }

        private async void OnImageClicked(object sender, EventArgs e)
        {
            if (RemainingClicks > 0 && !gameCompleted)
            {
                RemainingClicks--;
                UpdateClickCount();

                // Reducir progresivamente el tamaño de la imagen con límites
                UpdateImageSize();

                // Si el contador llega a 0, termina el juego
                if (RemainingClicks <= 0)
                {
                    gameCompleted = true; // Evitar múltiples mensajes
                    clickableImage.IsEnabled = false;
                    await DisplayAlert("Fin", "¡Se acabaron los clics!", "OK");

                    // Indica que el juego ha sido completado
                    _tcs.SetResult(true);
                    await Navigation.PopAsync();
                }
            }
        }

        private void UpdateImageSize()
        {
            // Definir el tamaño máximo y mínimo
            double maxSize = 400; // Tamaño máximo (en píxeles)
            double minSize = 50;  // Tamaño mínimo (en píxeles)

            // Calcular el nuevo tamaño de la imagen basado en los clics restantes
            double scaleFactor = (double)RemainingClicks / 100; // Ajusta el factor de escala según sea necesario
            double newSize = maxSize * scaleFactor;

            // Asegurarse de que la nueva imagen esté dentro de los límites
            if (newSize < minSize)
            {
                newSize = minSize; // Aplica el tamaño mínimo
            }
            else if (newSize > maxSize)
            {
                newSize = maxSize; // Aplica el tamaño máximo
            }

            clickableImage.WidthRequest = newSize;
            clickableImage.HeightRequest = newSize;
        }

        private void UpdateClickCount()
        {
            clickCountLabel.Text = $"Clicks restantes: {RemainingClicks}"; // Corregir la visualización
        }

        private void UpdateBindings()
        {
            BindingContext = this;
        }
    }
}
