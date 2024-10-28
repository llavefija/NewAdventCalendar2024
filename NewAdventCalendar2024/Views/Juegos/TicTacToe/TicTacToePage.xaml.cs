namespace NewAdventCalendar2024.Views.Juegos.TicTacToe
{
    public partial class TicTacToePage : ContentPage
    {
        private const int SIZE = 3;
        private string[,] board = new string[SIZE, SIZE];
        private string currentPlayer = "X"; // El jugador siempre comienza con "X"
        private int playerScore = 0;
        private int aiScore = 0;
        private int rounds = 1;

        public TicTacToePage(int puntos)
        {
            InitializeComponent();
            ResetBoard();
        }

        private void ResetBoard()
        {
            for (int row = 0; row < SIZE; row++)
            {
                for (int col = 0; col < SIZE; col++)
                {
                    board[row, col] = null;
                    GetLabel(row, col).Text = string.Empty;
                    GetLabel(row, col).TextColor = Colors.Black; // Asegura que el color sea negro al reiniciar
                }
            }
            StatusLabel.Text = "Turno del Jugador";
        }

        private Label GetLabel(int row, int col)
        {
            return (Label)GameGrid.Children[row * SIZE + col];
        }

        private void OnCellTapped(object sender, EventArgs e)
        {
            var label = sender as Label;
            int row = Grid.GetRow(label);
            int col = Grid.GetColumn(label);

            // Comprobar si la celda ya está ocupada
            if (board[row, col] != null)
                return;

            // Jugador juega
            board[row, col] = currentPlayer;
            label.Text = currentPlayer;

            // Cambiar color de la ficha
            label.TextColor = currentPlayer == "X" ? Colors.Red : Colors.Cyan;

            if (CheckWinner(currentPlayer))
            {
                if (currentPlayer == "X")
                {
                    playerScore++;
                    UpdateScore();
                    DisplayWinner("Jugador");
                }
                else
                {
                    aiScore++;
                    UpdateScore();
                    DisplayWinner("Máquina");
                }
                return; // Salir si hay un ganador
            }

            // Cambiar turno a la máquina
            if (currentPlayer == "X")
            {
                currentPlayer = "O";
                StatusLabel.Text = "Turno de la Máquina";
                MakeAIMove();
            }
            else
            {
                currentPlayer = "X";
                StatusLabel.Text = "Turno del Jugador";
            }
        }

        private void MakeAIMove()
        {
            Random rand = new Random();
            int row, col;

            do
            {
                row = rand.Next(SIZE);
                col = rand.Next(SIZE);
            } while (board[row, col] != null);

            board[row, col] = currentPlayer;
            var label = GetLabel(row, col);
            label.Text = currentPlayer;
            label.TextColor = Colors.Cyan; // Color de la máquina

            if (CheckWinner(currentPlayer))
            {
                aiScore++;
                UpdateScore();
                DisplayWinner("Máquina");
            }
            else
            {
                currentPlayer = "X";
                StatusLabel.Text = "Turno del Jugador";
            }
        }

        private bool CheckWinner(string player)
        {
            // Comprobar filas y columnas
            for (int i = 0; i < SIZE; i++)
            {
                if (board[i, 0] == player && board[i, 1] == player && board[i, 2] == player)
                    return true;

                if (board[0, i] == player && board[1, i] == player && board[2, i] == player)
                    return true;
            }

            // Comprobar diagonales
            if (board[0, 0] == player && board[1, 1] == player && board[2, 2] == player)
                return true;

            if (board[0, 2] == player && board[1, 1] == player && board[2, 0] == player)
                return true;

            // Comprobar empate
            if (IsBoardFull())
            {
                DisplayWinner("Empate");
            }

            return false;
        }

        private bool IsBoardFull()
        {
            foreach (var cell in board)
            {
                if (cell == null) return false;
            }
            return true;
        }

        private void DisplayWinner(string winner)
        {
            StatusLabel.Text = $"{winner} gana!";
            rounds++;
            if (rounds > 3)
            {
                // Reiniciar la partida
                DisplayAlert("Juego Terminado", $"El Jugador gana {playerScore} - {aiScore} de 3", "Aceptar");
                playerScore = 0;
                aiScore = 0;
                rounds = 1;
                RoundLabel.Text = "Ronda: 1";
                ResetBoard();
            }
            else
            {
                RoundLabel.Text = $"Ronda: {rounds}";
                ResetBoard();
            }
        }

        private void UpdateScore()
        {
            StatusLabel.Text = $"Jugador: {playerScore} - Máquina: {aiScore}";
        }
    }
}
