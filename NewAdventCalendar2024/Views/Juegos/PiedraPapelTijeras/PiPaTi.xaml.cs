using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using NewAdventCalendar2024.Interfaces;

namespace NewAdventCalendar2024.Views.Juegos.PiedraPapelTijeras
{
    public partial class PiPaTiPage : ContentPage, IGamePage
    {
        private int playerScore = 0;         // Puntuación del jugador
        private int machineScore = 0;         // Puntuación de la máquina
        private int scoreToWin;                // Puntuación para ganar
        private TaskCompletionSource<bool> tcs;

        // Constructor que acepta la puntuación mínima como parámetro
        public PiPaTiPage(int minScoreToWin)
        {
            InitializeComponent();
            scoreToWin = minScoreToWin; // Asigna la puntuación mínima
            tcs = new TaskCompletionSource<bool>();
            UpdateScoreLabel();
        }

        // Implementación del método de la interfaz
        public void InicializarTcs(TaskCompletionSource<bool> tcs)
        {
            this.tcs = tcs;
        }

        private async void OnPlayerChoice(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                string playerChoice = button.CommandParameter.ToString();
                string machineChoice = GetMachineChoice();
                string result = GetResult(playerChoice, machineChoice);

                // Actualiza las imágenes basadas en las elecciones
                UpdateChoiceImages(playerChoice, machineChoice);

                // Muestra el resultado de la ronda
                RoundResultLabel.Text = $"Tú elegiste: {playerChoice} | Máquina eligió: {machineChoice}";
                GameStatusLabel.Text = result;

                // Actualiza el marcador según el resultado
                if (result == "Ganaste!")
                {
                    playerScore++;
                }
                else if (result == "Perdiste!")
                {
                    machineScore++;
                }

                UpdateScoreLabel();

                // Verifica si alguien ha alcanzado la puntuación objetivo
                if (playerScore >= scoreToWin || machineScore >= scoreToWin)
                {
                    EndGame(playerScore >= scoreToWin);
                }
            }
        }

        private void UpdateScoreLabel()
        {
            ScoreLabel.Text = $"Tu Puntuación: {playerScore} | Máquina: {machineScore}";
        }

        private void EndGame(bool playerWon)
        {
            // Deshabilita los botones después de que alguien gane
            ButtonPiedra.IsEnabled = false;
            ButtonPapel.IsEnabled = false;
            ButtonTijeras.IsEnabled = false;

            // Muestra el estado final del juego
            GameStatusLabel.Text = playerWon ? "¡Has ganado el juego!" : "La máquina ha ganado el juego.";
            tcs.SetResult(playerWon); // Finaliza el juego con base en la puntuación
        }

        private void UpdateChoiceImages(string playerChoice, string machineChoice)
        {
            // Asegúrate de tener las imágenes correctamente nombradas
            MachineChoiceImage.Source = $"machine_{machineChoice.ToLower()}.png";
            PlayerChoiceImage.Source = $"player_{playerChoice.ToLower()}.png";
        }

        private string GetMachineChoice()
        {
            Random random = new Random();
            int choice = random.Next(3); // 0: Piedra, 1: Papel, 2: Tijeras
            return choice switch
            {
                0 => "Piedra",
                1 => "Papel",
                _ => "Tijeras"
            };
        }

        private string GetResult(string playerChoice, string machineChoice)
        {
            if (playerChoice == machineChoice)
            {
                return "Empate!";
            }
            else if ((playerChoice == "Piedra" && machineChoice == "Tijeras") ||
                     (playerChoice == "Papel" && machineChoice == "Piedra") ||
                     (playerChoice == "Tijeras" && machineChoice == "Papel"))
            {
                return "Ganaste!";
            }
            else
            {
                return "Perdiste!";
            }
        }

        public Task<bool> GetGameResultAsync() => tcs.Task;
    }
}
