using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using NewAdventCalendar2024.Interfaces;

namespace NewAdventCalendar2024.Views.Juegos.PiedraPapelTijeras
{
    public partial class PiPaTiPage : ContentPage, IGamePage
    {
        private int playerScore = 0;         // Puntuaci�n del jugador
        private int machineScore = 0;         // Puntuaci�n de la m�quina
        private int scoreToWin;                // Puntuaci�n para ganar
        private TaskCompletionSource<bool> tcs;

        // Constructor que acepta la puntuaci�n m�nima como par�metro
        public PiPaTiPage(int minScoreToWin)
        {
            InitializeComponent();
            scoreToWin = minScoreToWin; // Asigna la puntuaci�n m�nima
            tcs = new TaskCompletionSource<bool>();
            UpdateScoreLabel();
        }

        // Implementaci�n del m�todo de la interfaz
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

                // Actualiza las im�genes basadas en las elecciones
                UpdateChoiceImages(playerChoice, machineChoice);

                // Muestra el resultado de la ronda
                RoundResultLabel.Text = $"T� elegiste: {playerChoice} | M�quina eligi�: {machineChoice}";
                GameStatusLabel.Text = result;

                // Actualiza el marcador seg�n el resultado
                if (result == "Ganaste!")
                {
                    playerScore++;
                }
                else if (result == "Perdiste!")
                {
                    machineScore++;
                }

                UpdateScoreLabel();

                // Verifica si alguien ha alcanzado la puntuaci�n objetivo
                if (playerScore >= scoreToWin || machineScore >= scoreToWin)
                {
                    EndGame(playerScore >= scoreToWin);
                }
            }
        }

        private void UpdateScoreLabel()
        {
            ScoreLabel.Text = $"Tu Puntuaci�n: {playerScore} | M�quina: {machineScore}";
        }

        private void EndGame(bool playerWon)
        {
            // Deshabilita los botones despu�s de que alguien gane
            ButtonPiedra.IsEnabled = false;
            ButtonPapel.IsEnabled = false;
            ButtonTijeras.IsEnabled = false;

            // Muestra el estado final del juego
            GameStatusLabel.Text = playerWon ? "�Has ganado el juego!" : "La m�quina ha ganado el juego.";
            tcs.SetResult(playerWon); // Finaliza el juego con base en la puntuaci�n
        }

        private void UpdateChoiceImages(string playerChoice, string machineChoice)
        {
            // Aseg�rate de tener las im�genes correctamente nombradas
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
