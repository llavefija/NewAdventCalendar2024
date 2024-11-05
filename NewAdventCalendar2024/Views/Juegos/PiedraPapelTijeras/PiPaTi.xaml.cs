using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using NewAdventCalendar2024.Interfaces;

namespace NewAdventCalendar2024.Views.Juegos.PiedraPapelTijeras
{
    public partial class PiPaTiPage : ContentPage, IGamePage
    {
        private int playerScore = 0;
        private int machineScore = 0;
        private int scoreToWin;
        private TaskCompletionSource<bool> tcs;

        public PiPaTiPage(int minScoreToWin, string titulo)
        {
            InitializeComponent();
            scoreToWin = minScoreToWin;
            tcs = new TaskCompletionSource<bool>();
            titleLabel.Text = titulo;
            NavigationPage.SetHasNavigationBar(this, false);

            UpdateScoreLabel();
        }

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

                // Actualiza las im�genes y las etiquetas de elecci�n
                UpdateChoiceImages(playerChoice, machineChoice);

                // Muestra el resultado de la ronda
                RoundResultLabel.Text = $"T� elegiste: {playerChoice} | M�quina eligi�: {machineChoice}";
                GameStatusLabel.Text = result;

                // Actualiza el marcador basado en el resultado
                if (result == "Ganaste!")
                {
                    playerScore++;
                }
                else if (result == "Perdiste!")
                {
                    machineScore++;
                }

                UpdateScoreLabel();

                // Verifica si alguien alcanz� la puntuaci�n objetivo
                if (playerScore >= scoreToWin || machineScore >= scoreToWin)
                {
                    EndGame(playerScore >= scoreToWin);
                    await Navigation.PopAsync();
                }
            }
        }

        private void UpdateScoreLabel()
        {
            PlayerScoreLabel.Text = playerScore.ToString();
            MachineScoreLabel.Text = machineScore.ToString();
        }

        private void EndGame(bool playerWon)
        {
            // Deshabilita los botones despu�s de que alguien gane
            ButtonPiedra.IsEnabled = false;
            ButtonPapel.IsEnabled = false;
            ButtonTijeras.IsEnabled = false;

            // Muestra el estado final del juego
            GameStatusLabel.Text = playerWon ? "�Has ganado el juego!" : "La m�quina ha ganado el juego.";
            tcs.SetResult(playerWon);
        }

        private void UpdateChoiceImages(string playerChoice, string machineChoice)
        {
            // Actualiza las im�genes de acuerdo a las elecciones
            MachineChoiceImage.Source = $"{machineChoice.ToLower()}.png";
            MachineChoiceLabel.Text = machineChoice; // Muestra la elecci�n de la m�quina
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
