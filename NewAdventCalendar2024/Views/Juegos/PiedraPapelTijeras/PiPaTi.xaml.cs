namespace NewAdventCalendar2024.Views.Juegos.PiedraPapelTijeras
{
    public partial class PiPaTiPage : ContentPage
    {
        private int playerScore = 0;
        private int machineScore = 0;
        private int roundsToWin;

        public PiPaTiPage(int puntos)
        {
            InitializeComponent();
            UpdateScore();
            roundsToWin = puntos;
        }

        private async void OnPiedraClicked(object sender, EventArgs e)
        {
            await PlayRound("Piedra");
        }

        private async void OnPapelClicked(object sender, EventArgs e)
        {
            await PlayRound("Papel");
        }

        private async void OnTijerasClicked(object sender, EventArgs e)
        {
            await PlayRound("Tijeras");
        }

        private async Task PlayRound(string playerChoice)
        {
            string machineChoice = GetMachineChoice();
            string winner = DetermineWinner(playerChoice, machineChoice);

            resultLabel.Text = $"Máquina: {machineChoice} - Jugador: {playerChoice}\nEl ganador es: {winner}";

            // Actualiza el marcador
            if (winner == "Jugador")
            {
                playerScore++;
            }
            else if (winner == "Máquina")
            {
                machineScore++;
            }

            UpdateScore();
            machineChoiceImage.Source = $"{machineChoice}.png"; // Muestra la elección de la máquina

            // Espera un momento para que el jugador vea la elección
            await Task.Delay(2000);

            // Verifica si alguien ha ganado
            if (playerScore >= roundsToWin || machineScore >= roundsToWin)
            {
                resultLabel.Text += $"\n¡Partida Finalizada! Ganador: {(playerScore > machineScore ? "Jugador" : "Máquina")}";
                ResetGame();
                return; // Sale del método para evitar que se reinicie la ronda
            }

            ResetRound();
        }

        private string GetMachineChoice()
        {
            Random random = new Random();
            int choice = random.Next(3);
            return choice switch
            {
                0 => "Piedra",
                1 => "Papel",
                2 => "Tijeras",
                _ => "Piedra",
            };
        }

        private string DetermineWinner(string playerChoice, string machineChoice)
        {
            if (playerChoice == machineChoice)
            {
                return "Nadie";
            }
            else if ((playerChoice == "Piedra" && machineChoice == "Tijeras") ||
                     (playerChoice == "Papel" && machineChoice == "Piedra") ||
                     (playerChoice == "Tijeras" && machineChoice == "Papel"))
            {
                return "Jugador";
            }
            else
            {
                return "Máquina";
            }
        }

        private void UpdateScore()
        {
            scoreLabel.Text = $"Puntos - Jugador: {playerScore} | Máquina: {machineScore}";
        }

        private void ResetRound()
        {
            machineChoiceImage.Source = null; // Limpia la elección de la máquina
        }

        private void ResetGame()
        {
            playerScore = 0;
            machineScore = 0;
            UpdateScore();
            ResetRound();
        }
    }
}
