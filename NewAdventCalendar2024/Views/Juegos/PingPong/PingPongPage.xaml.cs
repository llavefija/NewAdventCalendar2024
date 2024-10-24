using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using NewAdventCalendar2024.Interfaces;

namespace NewAdventCalendar2024.Views.Juegos.PingPong
{
    public partial class PingPongPage : ContentPage, IDrawable
    {
        private float ballX, ballY; // Definición de la ubicación de la pelota
        private float paddle1Y, paddle2Y; // Definición de la ubicación de las palas

        private float ballSpeedX = 3f; // Velocidad inicial de la pelota
        private float ballSpeedY = 3f; // Velocidad inicial de la pelota

        private const float paddleWidth = 10, paddleHeight = 60; // Ancho y altura de las palas
        private float fieldWidth, fieldHeight; // Ancho y alto del campo

        private int playerScore = 0; // Puntuación del jugador
        private int machineScore = 0; // Puntuación de la máquina

        private int minScoreToWin = 5; // Puntuación mínima para ganar

        private bool isMovingUp = false; // Controlador del movimiento hacia arriba
        private bool isMovingDown = false; // Controlador del movimiento hacia abajo

        private float speedIncrement = 1f; // Incremento de velocidad de la pelota
        private float paddleSpeed = 8f; // Velocidad de movimiento de las palas

        // TCS para comprobar el estado de la partida
        private TaskCompletionSource<bool> tcs;

        public PingPongPage(float SpeedIncrement, int MinScoretoWin, float PaddleSpeed)
        {
            InitializeComponent();

            // Ajuste del ancho y alto del campo
            float screenWidth = (float)(DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density);
            float fieldAspectRatio = 4f / 3f; // Proporción 4:3
            fieldWidth = screenWidth * 0.9f; // 90% del ancho de la pantalla
            fieldHeight = fieldWidth / fieldAspectRatio + 100; // Aumentar altura

            // Asignar los valores pasados por el juego
            this.paddleSpeed = PaddleSpeed;
            this.speedIncrement = SpeedIncrement;
            this.minScoreToWin = MinScoretoWin;

            // Ajusta la pelota al centro
            ballX = fieldWidth / 2;
            ballY = fieldHeight / 2;

            // Ajusta las palas al centro
            paddle1Y = fieldHeight / 2 - paddleHeight / 2;
            paddle2Y = fieldHeight / 2 - paddleHeight / 2;

            // Muestra el campo, pelota y raquetas
            GameView.Drawable = this;

            MinScoreLabel.Text = $"Puntuación mínima para ganar: {minScoreToWin}";

            // Iniciar un loop hasta que el juego pare
            tcs = new TaskCompletionSource<bool>(); // Inicializar TCS
            Dispatcher.StartTimer(TimeSpan.FromMilliseconds(20), () =>
            {
                UpdateGame();

                // Comprobar si se ha alcanzado la puntuación mínima
                if (playerScore == minScoreToWin)
                {
                    tcs.SetResult(true); // Jugador gana
                    return false; // Detener el juego
                }
                else if (machineScore == minScoreToWin)
                {
                    tcs.SetResult(false); // Máquina gana
                    return false; // Detener el juego
                }

                return true;
            });
        }

        // Método para dibujar el campo y los componentes
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float offsetX = (dirtyRect.Width - fieldWidth) / 2;
            float offsetY = (dirtyRect.Height - fieldHeight) / 2;

            canvas.FillColor = Colors.Black;
            canvas.FillRectangle(dirtyRect);

            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize = 2;
            canvas.DrawLine(offsetX, offsetY, offsetX + fieldWidth, offsetY); // Superior
            canvas.DrawLine(offsetX, offsetY + fieldHeight, offsetX + fieldWidth, offsetY + fieldHeight); // Inferior
            canvas.DrawLine(offsetX, offsetY, offsetX, offsetY + fieldHeight); // Izquierda
            canvas.DrawLine(offsetX + fieldWidth, offsetY, offsetX + fieldWidth, offsetY + fieldHeight); // Derecha
            canvas.DrawLine(offsetX + fieldWidth / 2, offsetY, offsetX + fieldWidth / 2, offsetY + fieldHeight); // Línea central

            canvas.FillColor = Colors.White;
            canvas.FillCircle(offsetX + ballX, offsetY + ballY, 10);

            canvas.FillColor = Colors.Cyan;
            canvas.FillRectangle(offsetX + 50, offsetY + paddle1Y, paddleWidth, paddleHeight); // Pala del jugador
            canvas.FillColor = Colors.Red;
            canvas.FillRectangle(offsetX + fieldWidth - 60, offsetY + paddle2Y, paddleWidth, paddleHeight); // Pala de la máquina
        }

        // Método para actualizar el juego
        private void UpdateGame()
        {
            // Movimiento de la pelota
            ballX += ballSpeedX;
            ballY += ballSpeedY;

            // Comprobar si colisiona con el techo o el suelo
            if (ballY <= 0 || ballY >= fieldHeight)
                ballSpeedY = -ballSpeedY;

            // Colisión con la pala del jugador
            if (ballX <= 60 && ballY >= paddle1Y && ballY <= paddle1Y + paddleHeight)
            {
                ballSpeedX = -ballSpeedX;
                ballSpeedX += speedIncrement; // Incrementar velocidad
                ballSpeedY += speedIncrement; // Incrementar velocidad verticalmente
            }

            // Colisión con la pala de la máquina
            if (ballX >= fieldWidth - 60 && ballY >= paddle2Y && ballY <= paddle2Y + paddleHeight)
            {
                ballSpeedX = -ballSpeedX;
                ballSpeedX += speedIncrement; // Incrementar velocidad
                ballSpeedY += speedIncrement; // Incrementar velocidad verticalmente
            }

            // Gol del jugador
            if (ballX <= 0)
            {
                machineScore++;
                ResetBallAfterGoal();
            }
            // Gol de la máquina
            else if (ballX >= fieldWidth)
            {
                playerScore++;
                ResetBallAfterGoal();
            }

            // Movimiento automático de la pala de la máquina
            if (paddle2Y < ballY - paddleHeight / 2)
                paddle2Y += paddleSpeed; // Ajustar velocidad hacia abajo

            if (paddle2Y > ballY - paddleHeight / 2)
                paddle2Y -= paddleSpeed; // Ajustar velocidad hacia arriba

            if (isMovingUp && paddle1Y > 0)
                paddle1Y -= paddleSpeed; // Ajustar velocidad
            if (isMovingDown && paddle1Y < fieldHeight - paddleHeight)
                paddle1Y += paddleSpeed; // Ajustar velocidad

            // Actualiza el panel de puntuación
            ScoreLabel.Text = $"Jugador: {playerScore} | Máquina: {machineScore}";

            GameView.Invalidate();
        }

        // Movimiento de la pala del jugador
        private void MoveUp(object sender, EventArgs e)
        {
            isMovingUp = true; // Activar movimiento hacia arriba
        }

        private void MoveDown(object sender, EventArgs e)
        {
            isMovingDown = true; // Activar movimiento hacia abajo
        }

        private void OnButtonReleased(object sender, EventArgs e)
        {
            // Desactivar movimiento en función del botón liberado
            if (sender is Button button)
            {
                if (button.Text == "UP")
                {
                    isMovingUp = false; // Desactivar movimiento hacia arriba
                }
                else if (button.Text == "DOWN")
                {
                    isMovingDown = false; // Desactivar movimiento hacia abajo
                }
            }
        }

        private async void ResetBallAfterGoal()
        {
            ballX = fieldWidth / 2;
            ballY = fieldHeight / 2;
            ballSpeedX = 3f; // Restablecer velocidad
            ballSpeedY = 3f; // Restablecer velocidad

            await Task.Delay(1000);
        }

        // Método para obtener el resultado del juego
        public Task<bool> GetGameResultAsync()
        {
            return tcs.Task; // Retorna el resultado del juego
        }
    }
}
