using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using NewAdventCalendar2024.Interfaces;

namespace NewAdventCalendar2024.Views.Juegos.Ahorcado
{
    public partial class AhorcadoPage : ContentPage, IGamePage
    {
        private const int MaxAttempts = 6; // Número máximo de intentos
        private string wordToGuess;
        private List<char> guessedLetters;
        private int incorrectGuesses;
        private TaskCompletionSource<bool> _tcs;
        private bool gameCompleted = false;

        public AhorcadoPage(string word, string titulo)
        {
            InitializeComponent();
            wordToGuess = word.ToUpper(); // Guardar la palabra en mayúsculas
            guessedLetters = new List<char>();
            incorrectGuesses = 0;

            UpdateWordDisplay();
            UpdateAttemptsDisplay();
            UpdateHangmanImage();
            titleLabel.Text = titulo;
        }

        public void InicializarTcs(TaskCompletionSource<bool> tcs)
        {
            _tcs = tcs;
        }

        private void UpdateWordDisplay()
        {
            wordLabel.Text = string.Join(" ", wordToGuess.Select(c => guessedLetters.Contains(c) ? c.ToString() : "_"));
        }

        private void UpdateAttemptsDisplay()
        {
            attemptsLabel.Text = $"Intentos restantes: {MaxAttempts - incorrectGuesses}";
            wrongLettersLabel.Text = $"Letras incorrectas: {string.Join(", ", guessedLetters.Where(l => !wordToGuess.Contains(l)))}";
        }

        private void UpdateHangmanImage()
        {
            // Actualizar la imagen del ahorcado según los intentos incorrectos
            hangmanImage.Source = $"ahorcado{incorrectGuesses}.png"; // Asegúrate de tener estas imágenes
        }

        private async void OnLetterClicked(object sender, EventArgs e)
        {
            if (gameCompleted) return; // Evitar múltiples clics si el juego ya ha terminado

            var button = sender as Button;
            char guessedLetter = button.Text[0];
            button.IsEnabled = false; // Deshabilitar el botón de letra seleccionada
            guessedLetters.Add(guessedLetter);

            if (!wordToGuess.Contains(guessedLetter))
            {
                incorrectGuesses++;
                UpdateHangmanImage();
                UpdateAttemptsDisplay();

                if (incorrectGuesses >= MaxAttempts)
                {
                    gameCompleted = true;
                    _tcs.SetResult(false);
                    await Navigation.PopAsync();

                }
            }
            else
            {
                UpdateWordDisplay();
                if (wordToGuess.All(c => guessedLetters.Contains(c)))
                {
                    gameCompleted = true;
                    _tcs.SetResult(true);
                    await Navigation.PopAsync();

                }
            }
        }
    }
}
