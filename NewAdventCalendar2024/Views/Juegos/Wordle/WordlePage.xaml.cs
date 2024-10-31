using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using NewAdventCalendar2024.Interfaces;

namespace NewAdventCalendar2024.Views.Juegos.Wordle
{
    public partial class WordlePage : ContentPage, IGamePage
    {
        private List<string> wordList = new List<string> { "mango", "perro", "salud", "coche", "flore", "circo" };
        private string secretWord;
        private int maxAttempts = 6;
        public int CurrentAttempt { get; private set; }
        public List<string> Attempts { get; private set; } = new List<string>();
        private int wordLength;
        private TaskCompletionSource<bool> tcs; // TCS para manejar el resultado del juego

        public WordlePage()
        {
            InitializeComponent();
            InitializeGame(); // Inicializa el juego
            tcs = new TaskCompletionSource<bool>();
        }

        private void InitializeGame()
        {
            var random = new Random();
            secretWord = wordList[random.Next(wordList.Count)];
            wordLength = secretWord.Length;
            CurrentAttempt = 0;

            // Ajusta el Entry para que acepte el tama�o de la palabra correcta
            GuessEntry.MaxLength = wordLength;

            // Configura la cuadr�cula seg�n la longitud de la palabra
            SetupGrid();
        }

        private void SetupGrid()
        {
            // Limpia la cuadr�cula si es necesario
            WordGrid.Children.Clear();
            WordGrid.ColumnDefinitions.Clear();
            WordGrid.RowDefinitions.Clear();

            // Configura las columnas en funci�n del tama�o de la palabra
            for (int i = 0; i < wordLength; i++)
            {
                WordGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            // Configura las filas en funci�n del n�mero de intentos
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
                    ? "�Felicidades! Has adivinado la palabra."
                    : $"�Lo siento! La palabra era {secretWord}.";
                GuessEntry.IsEnabled = false; // Desactiva el campo de entrada
                tcs.SetResult(Attempts.Contains(secretWord)); // Finaliza el juego
            }
            else
            {
                MessageLabel.Text = "Contin�a adivinando...";
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
                    result[i] = 'Y'; // Correcto pero en la posici�n incorrecta
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
                    TextColor = Colors.White,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    WidthRequest = 50,
                    HeightRequest = 50
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
                'G' => Colors.Green,
                'Y' => Colors.Yellow,
                _ => Colors.Gray,
            };
        }

        public Task<bool> GetGameResultAsync() => tcs.Task; // Implementa el m�todo de la interfaz

        public void InicializarTcs(TaskCompletionSource<bool> tcs)
        {
            this.tcs = tcs; // Inicializa el TaskCompletionSource
        }
    }
}