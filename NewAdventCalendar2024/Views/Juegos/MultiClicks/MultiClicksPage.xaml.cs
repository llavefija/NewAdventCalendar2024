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

        public MultiClicksPage(int initialClicks, string imageSource, string titulo, Color backgroundColor)
        {
            InitializeComponent();
            RemainingClicks = initialClicks;
            ImageSource = imageSource;
            titleLabel.Text = titulo;

            this.BackgroundColor = backgroundColor;

            NavigationPage.SetHasNavigationBar(this, false);


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

                // Si el contador llega a 0, termina el juego
                if (RemainingClicks <= 0)
                {
                    gameCompleted = true; // Evitar múltiples mensajes
                    clickableImage.IsEnabled = false;
                    // Indica que el juego ha sido completado
                    _tcs.SetResult(true);
                    await Navigation.PopAsync();
                }
            }
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
