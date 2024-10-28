using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using NewAdventCalendar2024.Interfaces;

namespace NewAdventCalendar2024.Views.Juegos.PingPong
{
    public partial class PingPongPage : ContentPage, IDrawable, IGamePage
    {
        private float ballX, ballY; // Ubicación de la pelota
        private float paddle1Y, paddle2Y; // Ubicación de las palas
        private float ballSpeedX, ballSpeedY; // Velocidad de la pelota
        private const float paddleWidth = 10, paddleHeight = 60;
        private float fieldWidth, fieldHeight;
        private int playerScore = 0, machineScore = 0;
        private int minScoreToWin = 5;
        private bool isMovingUp = false, isMovingDown = false;
        private float speedIncrement = 1f, paddleSpeed = 8f;

        private TaskCompletionSource<bool> tcs;

        public PingPongPage(float SpeedIncrement, int MinScoretoWin, float PaddleSpeed)
        {
            InitializeComponent();

            float screenWidth = (float)(DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density);
            float fieldAspectRatio = 4f / 3f;
            fieldWidth = screenWidth * 0.9f;
            fieldHeight = fieldWidth / fieldAspectRatio + 100;

            this.paddleSpeed = PaddleSpeed;
            this.speedIncrement = SpeedIncrement;
            this.minScoreToWin = MinScoretoWin;

            ballX = fieldWidth / 2;
            ballY = fieldHeight / 2;
            paddle1Y = fieldHeight / 2 - paddleHeight / 2;
            paddle2Y = fieldHeight / 2 - paddleHeight / 2;

            ballSpeedX = 3f; // Velocidad inicial de la pelota
            ballSpeedY = 3f; // Velocidad inicial de la pelota

            GameView.Drawable = this;
            MinScoreLabel.Text = $"Puntuación mínima para ganar: {minScoreToWin}";

            InicializarTcs(new TaskCompletionSource<bool>());

            Dispatcher.StartTimer(TimeSpan.FromMilliseconds(20), () =>
            {
                UpdateGame();

                if (playerScore >= minScoreToWin)
                {
                    tcs.SetResult(true);
                    return false;
                }
                else if (machineScore >= minScoreToWin)
                {
                    tcs.SetResult(false);
                    return false;
                }

                return true;
            });
        }

        public void InicializarTcs(TaskCompletionSource<bool> tcs)
        {
            this.tcs = tcs;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float offsetX = (dirtyRect.Width - fieldWidth) / 2;
            float offsetY = (dirtyRect.Height - fieldHeight) / 2;

            canvas.FillColor = Colors.Black;
            canvas.FillRectangle(dirtyRect);

            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize = 2;
            canvas.DrawLine(offsetX, offsetY, offsetX + fieldWidth, offsetY);
            canvas.DrawLine(offsetX, offsetY + fieldHeight, offsetX + fieldWidth, offsetY + fieldHeight);
            canvas.DrawLine(offsetX, offsetY, offsetX, offsetY + fieldHeight);
            canvas.DrawLine(offsetX + fieldWidth, offsetY, offsetX + fieldWidth, offsetY + fieldHeight);
            canvas.DrawLine(offsetX + fieldWidth / 2, offsetY, offsetX + fieldWidth / 2, offsetY + fieldHeight);

            canvas.FillColor = Colors.White;
            canvas.FillCircle(offsetX + ballX, offsetY + ballY, 10);

            canvas.FillColor = Colors.Cyan;
            canvas.FillRectangle(offsetX + 50, offsetY + paddle1Y, paddleWidth, paddleHeight);
            canvas.FillColor = Colors.Red;
            canvas.FillRectangle(offsetX + fieldWidth - 60, offsetY + paddle2Y, paddleWidth, paddleHeight);
        }

        private void UpdateGame()
        {
            ballX += ballSpeedX;
            ballY += ballSpeedY;

            // Rebotar al llegar al borde superior o inferior
            if (ballY <= 0 || ballY >= fieldHeight)
                ballSpeedY = -ballSpeedY;

            // Colisiones con las palas
            if (ballX <= 60 && ballY >= paddle1Y && ballY <= paddle1Y + paddleHeight)
            {
                // Ajuste de rebote basado en la posición de la pala
                float impactPoint = (ballY - paddle1Y) / paddleHeight;
                ballSpeedX = -ballSpeedX; // Cambiar dirección
                ballSpeedY = (impactPoint - 0.5f) * 10; // Aumentar/reducir velocidad en Y basado en el impacto
                ballSpeedX = Math.Abs(ballSpeedX) + speedIncrement; // Mantener dirección positiva y añadir incremento
            }

            if (ballX >= fieldWidth - 60 && ballY >= paddle2Y && ballY <= paddle2Y + paddleHeight)
            {
                float impactPoint = (ballY - paddle2Y) / paddleHeight;
                ballSpeedX = -ballSpeedX;
                ballSpeedY = (impactPoint - 0.5f) * 10;
                ballSpeedX = -Math.Abs(ballSpeedX) - speedIncrement; // Mantener dirección negativa y añadir incremento
            }

            // Puntos y reinicio de la pelota
            if (ballX <= 0)
            {
                machineScore++;
                ResetBallAfterGoal();
            }
            else if (ballX >= fieldWidth)
            {
                playerScore++;
                ResetBallAfterGoal();
            }

            // Movimiento de la pala del rival (IA simple)
            if (paddle2Y < ballY - paddleHeight / 2 && paddle2Y < fieldHeight - paddleHeight)
                paddle2Y += paddleSpeed;
            else if (paddle2Y > ballY - paddleHeight / 2 && paddle2Y > 0)
                paddle2Y -= paddleSpeed;

            // Movimiento de la pala del jugador
            if (isMovingUp && paddle1Y > 0)
                paddle1Y -= paddleSpeed;
            if (isMovingDown && paddle1Y < fieldHeight - paddleHeight)
                paddle1Y += paddleSpeed;

            ScoreLabel.Text = $"Jugador: {playerScore} | Máquina: {machineScore}";

            GameView.Invalidate();
        }

        private void MoveUp(object sender, EventArgs e) => isMovingUp = true;
        private void MoveDown(object sender, EventArgs e) => isMovingDown = true;

        private void OnButtonReleased(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Text == "UP")
                    isMovingUp = false;
                else if (button.Text == "DOWN")
                    isMovingDown = false;
            }
        }

        private async void ResetBallAfterGoal()
        {
            ballX = fieldWidth / 2;
            ballY = fieldHeight / 2;
            ballSpeedX = 3f; // Reiniciar velocidad
            ballSpeedY = 3f;
            await Task.Delay(1000);
        }

        public Task<bool> GetGameResultAsync() => tcs.Task;
    }
}
