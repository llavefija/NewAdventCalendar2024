using System.Threading.Tasks;
using NewAdventCalendar2024.Interfaces;

namespace NewAdventCalendar2024.Views.Juegos.TicTacToe
{
    public partial class TicTacToePage : ContentPage, IGamePage
    {
        private const int SIZE = 3;
        private string[,] board = new string[SIZE, SIZE];
        private string currentPlayer = "X"; // El jugador siempre comienza con "X"
        private int playerScore = 0;
        private int aiScore = 0;
        private TaskCompletionSource<bool> tcs; // Variable para el TaskCompletionSource
        private const int minScoreToWin = 3; // Establecer el puntaje mínimo para ganar

        public TicTacToePage(int puntos, string titulo)
        {
            InitializeComponent();
            ResetBoard();
            NavigationPage.SetHasNavigationBar(this, false);
            titleLabel.Text = titulo;
        }

        // Método requerido por la interfaz
        public void InicializarTcs(TaskCompletionSource<bool> taskCompletionSource)
        {
            tcs = taskCompletionSource; // Asignar el TaskCompletionSource a la variable local
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
                UpdateScores(currentPlayer);
                return; // Salir si hay un ganador
            }

            // Cambiar turno a la máquina
            currentPlayer = "O";
            StatusLabel.Text = "Turno de la Máquina";

            MakeAIMove();
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
                UpdateScores(currentPlayer);
                return; // Salir si hay un ganador
            }

            // Cambiar turno de vuelta al jugador
            currentPlayer = "X";
            StatusLabel.Text = "Turno del Jugador";
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
                StatusLabel.Text = "¡Empate!";
                ResetBoard(); // Reinicia el tablero en caso de empate sin cambiar los puntajes
                return false; // No llamar a UpdateScores en caso de empate
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

        private void UpdateScores(string winner)
        {
            if (winner == "X")
            {
                playerScore++;
                StatusLabel.Text = "¡Jugador gana la ronda!";
            }
            else if (winner == "O")
            {
                aiScore++;
                StatusLabel.Text = "¡Máquina gana la ronda!";
            }

            RoundLabel.Text = $"Jugador: {playerScore} - Máquina: {aiScore}";

            // Verificar si alguno de los jugadores ha alcanzado la puntuación mínima para ganar
            if (playerScore >= minScoreToWin || aiScore >= minScoreToWin)
            {
                tcs.SetResult(playerScore >= minScoreToWin); // Devuelve true si el jugador ha ganado
                DisableGame(); // Deshabilitar el juego
                Navigation.PopAsync();
            }
            else
            {
                // Reiniciar el tablero para la siguiente ronda si alguien ganó
                ResetBoard();
            }
        }

        private void DisableGame()
        {
            foreach (var child in GameGrid.Children)
            {
                if (child is Label label)
                {
                    label.GestureRecognizers.Clear(); // Eliminar cualquier gesto de reconocimiento para deshabilitar la interacción
                    label.TextColor = Colors.Gray; // Cambiar el color del texto para indicar que está deshabilitado
                }
            }
        }
    }
}
