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
        private const int MaxAttempts = 6; // N�mero m�ximo de intentos
        private string wordToGuess;
        private List<char> guessedLetters;
        private int incorrectGuesses;
        private TaskCompletionSource<bool> _tcs;
        private bool gameCompleted = false;

        public AhorcadoPage(string word)
        {
            InitializeComponent();
            wordToGuess = word.ToUpper(); // Guardar la palabra en may�sculas
            guessedLetters = new List<char>();
            incorrectGuesses = 0;

            UpdateWordDisplay();
            UpdateAttemptsDisplay();
            UpdateHangmanImage();
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
            // Actualizar la imagen del ahorcado seg�n los intentos incorrectos
            hangmanImage.Source = $"ahorcado{incorrectGuesses}.png"; // Aseg�rate de tener estas im�genes
        }

        private async void OnLetterClicked(object sender, EventArgs e)
        {
            if (gameCompleted) return; // Evitar m�ltiples clics si el juego ya ha terminado

            var button = sender as Button;
            char guessedLetter = button.Text[0];
            button.IsEnabled = false; // Deshabilitar el bot�n de letra seleccionada
            guessedLetters.Add(guessedLetter);

            if (!wordToGuess.Contains(guessedLetter))
            {
                incorrectGuesses++;
                UpdateHangmanImage();
                UpdateAttemptsDisplay();

                if (incorrectGuesses >= MaxAttempts)
                {
                    gameCompleted = true;
                    await DisplayAlert("Fin", "�Has perdido!", "OK");
                    resultLabel.Text = "Perdiste. La palabra era: " + wordToGuess;
                    _tcs.SetResult(false); // Indica que el juego termin� sin �xito
                }
            }
            else
            {
                UpdateWordDisplay();
                if (wordToGuess.All(c => guessedLetters.Contains(c)))
                {
                    gameCompleted = true;
                    await DisplayAlert("Fin", "�Has ganado!", "OK");
                    resultLabel.Text = "�Ganaste!";
                    _tcs.SetResult(true); // Indica que el juego termin� con �xito
                }
            }
        }
    }
}
