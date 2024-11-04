using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using NewAdventCalendar2024.Interfaces;

namespace NewAdventCalendar2024.Views.Juegos.Wordle
{
    public partial class WordlePage : ContentPage, IGamePage
    {
        private string secretWord; // Palabra secreta que se pasa por parámetro
        private int maxAttempts = 6;
        public int CurrentAttempt { get; private set; }
        public List<string> Attempts { get; private set; } = new List<string>();
        private int wordLength;
        private TaskCompletionSource<bool> tcs; // TCS para manejar el resultado del juego

        public WordlePage(string palabra, string titulo)
        {
            InitializeComponent();
            secretWord = palabra.ToLower(); // Asigna la palabra secreta pasada por parámetro
            wordLength = secretWord.Length; // Determina la longitud de la palabra
            titleLabel.Text = titulo;
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeGame(); // Inicializa el juego
            tcs = new TaskCompletionSource<bool>();
        }

        private void InitializeGame()
        {
            CurrentAttempt = 0;

            // Ajusta el Entry para que acepte el tamaño de la palabra correcta
            GuessEntry.MaxLength = wordLength;

            // Configura la cuadrícula según la longitud de la palabra
            SetupGrid();
        }

        private void SetupGrid()
        {
            // Limpia la cuadrícula si es necesario
            WordGrid.Children.Clear();
            WordGrid.ColumnDefinitions.Clear();
            WordGrid.RowDefinitions.Clear();

            // Configura las columnas en función del tamaño de la palabra
            for (int i = 0; i < wordLength; i++)
            {
                WordGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            // Configura las filas en función del número de intentos
            for (int i = 0; i < maxAttempts; i++)
            {
                WordGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
        }

        private void OnGuessButtonClicked(object sender, EventArgs e)
        {
            string guess = GuessEntry.Text?.ToLower();

            if (guess == null || guess.Length != wordLength)
            {
                MessageLabel.Text = $"Por favor, ingresa una palabra de {wordLength} letras.";
                return;
            }

            string result = CheckGuess(guess);
            DisplayResult(guess, result);

            if (IsGameOver())
            {
                MessageLabel.Text = Attempts.Contains(secretWord)
                    ? "¡Felicidades! Has adivinado la palabra."
                    : $"¡Lo siento! No has adivinado la palabra.";
                GuessEntry.IsEnabled = false; // Desactiva el campo de entrada
                tcs.SetResult(Attempts.Contains(secretWord)); // Finaliza el juego con el resultado
                Navigation.PopAsync();
            }
            else
            {
                MessageLabel.Text = "Continúa adivinando...";
                GuessEntry.Text = string.Empty; // Limpia la entrada
            }
        }

        private string CheckGuess(string guess)
        {
            CurrentAttempt++;
            Attempts.Add(guess);
            char[] result = new char[wordLength];

            for (int i = 0; i < wordLength; i++)
            {
                if (guess[i] == secretWord[i])
                {
                    result[i] = 'G'; // Correcto
                }
                else if (secretWord.Contains(guess[i]))
                {
                    result[i] = 'Y'; // Correcto pero en la posición incorrecta
                }
                else
                {
                    result[i] = 'X'; // Incorrecto
                }
            }

            return new string(result);
        }

        private bool IsGameOver() => CurrentAttempt >= maxAttempts || Attempts.Contains(secretWord);

        private void DisplayResult(string guess, string result)
        {
            for (int i = 0; i < wordLength; i++)
            {
                Label letterLabel = new Label
                {
                    Text = guess[i].ToString().ToUpper(),
                    FontSize = 30,
                    BackgroundColor = GetResultColor(result[i]),
                    TextColor = Colors.Black,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    WidthRequest = 50,
                    HeightRequest = 50,
                    FontFamily = "Candy"
                };

                // Agrega el label al Children
                WordGrid.Children.Add(letterLabel);

                // Establece la fila y columna para el label
                Grid.SetRow(letterLabel, CurrentAttempt - 1);
                Grid.SetColumn(letterLabel, i);
            }
        }

        private Color GetResultColor(char result)
        {
            return result switch
            {
                'G' => Color.FromArgb("#388E3C"),
                'Y' => Color.FromArgb("#FFB300"),
                _ => Color.FromArgb("FFFCF2"),
            };
        }

        public Task<bool> GetGameResultAsync() => tcs.Task; // Implementa el método de la interfaz

        public void InicializarTcs(TaskCompletionSource<bool> tcs)
        {
            this.tcs = tcs; // Inicializa el TaskCompletionSource
        }
    }
}
