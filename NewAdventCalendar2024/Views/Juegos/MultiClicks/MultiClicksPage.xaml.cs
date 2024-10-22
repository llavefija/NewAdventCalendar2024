using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace NewAdventCalendar2024.Views.Juegos.MultiClicks
{
    public partial class MultiClicksPage : ContentPage
    {
        private int remainingClicks = 50; // Cambia el número inicial si deseas
        private TaskCompletionSource<bool> _tcs; // TCS para controlar el resultado del juego

        public MultiClicksPage(TaskCompletionSource<bool> tcs)
        {
            InitializeComponent();
            _tcs = tcs; // Guardar la referencia a tcs
            UpdateClickCount();
        }

        private async void OnImageClicked(object sender, EventArgs e)
        {
            if (remainingClicks > 0)
            {
                remainingClicks--;

                // Actualiza el contador de clics
                UpdateClickCount();

                // Inicia la animación de tambaleo
                await Tambalear();
            }

            // Si el contador llega a 0, se puede mostrar un mensaje o desactivar el botón
            if (remainingClicks <= 0)
            {
                clickableImage.IsEnabled = false; // Desactiva el botón al llegar a 0
                await DisplayAlert("Fin", "¡Se acabaron los clics!", "OK");

                // Indica que el juego ha sido completado
                _tcs.SetResult(true); // Marca el juego como completado
                await Navigation.PopAsync(); // Regresa a la página anterior
            }
        }

        private async Task Tambalear()
        {
            var originalRotation = clickableImage.Rotation;
            // Añade una animación simple de tambaleo
            clickableImage.Rotation = originalRotation - 30; // Rota a la izquierda
            await Task.Delay(100); // Espera para ver la rotación
            clickableImage.Rotation = originalRotation + 30; // Rota a la derecha
            await Task.Delay(100); // Espera para ver la rotación
            clickableImage.Rotation = originalRotation; // Regresa a la posición original
        }

        private void UpdateClickCount()
        {
            clickCountLabel.Text = $"Clicks restantes: {remainingClicks}";
        }
    }
}
