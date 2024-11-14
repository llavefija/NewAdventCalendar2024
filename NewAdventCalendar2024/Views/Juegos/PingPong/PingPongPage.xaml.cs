using System;
using System.Diagnostics;
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
        private bool ballDirectionToRight; // Determina la dirección inicial después de cada gol

        private bool isPaused = false; // Nueva variable para controlar la pausa

        public PingPongPage(float SpeedIncrement, int MinScoretoWin, float PaddleSpeed, string titulo)
        {
            InitializeComponent();

            // Guardar las configuraciones iniciales de juego
            this.paddleSpeed = PaddleSpeed;
            this.speedIncrement = SpeedIncrement;
            this.minScoreToWin = MinScoretoWin;

            // Configurar la vista y otros elementos
            GameView.Drawable = this;
            MinScoreLabel.Text = $"Puntuación mínima para ganar: {minScoreToWin}";
            titleLabel.Text = titulo;

            // Inicializar tcs
            InicializarTcs(new TaskCompletionSource<bool>());

            // Ocultar barra de navegación
            NavigationPage.SetHasNavigationBar(this, false);

            // Llamar al método de inicialización asíncrona
            _ = InicializarJuegoAsync(); // Ignoramos el resultado ya que no necesitamos esperar aquí
        }

        private async Task InicializarJuegoAsync()
        {

            // Calcular dimensiones del campo de juego
            float screenWidth = (float)(DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density);
            float fieldAspectRatio = 4f / 3f;
            fieldWidth = screenWidth * 0.9f;
            fieldHeight = fieldWidth / fieldAspectRatio + 100;

            // Ubicar pelota y palas en sus posiciones iniciales
            ballX = fieldWidth / 2;
            ballY = fieldHeight / 2;
            paddle1Y = fieldHeight / 2 - paddleHeight / 2;
            paddle2Y = fieldHeight / 2 - paddleHeight / 2;

            // Comenzar con la dirección hacia la derecha
            ballDirectionToRight = true;

            // Asignar una dirección inicial aleatoria a la pelota
            SetRandomInitialBallDirection();

            // Iniciar el temporizador para la actualización del juego
            Dispatcher.StartTimer(TimeSpan.FromMilliseconds(20), () =>
            {
                UpdateGame();

                if (playerScore >= minScoreToWin)
                {
                    tcs.SetResult(true);
                    Navigation.PopAsync();
                    return false;
                }
                else if (machineScore >= minScoreToWin)
                {
                    tcs.SetResult(false);
                    Navigation.PopAsync();
                    return false;
                }

                return true;
            });
        }

        private void SetRandomInitialBallDirection()
        {
            // Generar dirección inicial aleatoria en una de las cuatro diagonales
            Random rnd = new Random();
            int direction = rnd.Next(4); // Valores 0-3
            float initialSpeed = 3f;

            switch (direction)
            {
                case 0: // Derecha hacia arriba
                    ballSpeedX = initialSpeed;
                    ballSpeedY = -initialSpeed;
                    break;
                case 1: // Derecha hacia abajo
                    ballSpeedX = initialSpeed;
                    ballSpeedY = initialSpeed;
                    break;
                case 2: // Izquierda hacia arriba
                    ballSpeedX = -initialSpeed;
                    ballSpeedY = -initialSpeed;
                    break;
                case 3: // Izquierda hacia abajo
                    ballSpeedX = -initialSpeed;
                    ballSpeedY = initialSpeed;
                    break;
            }
        }

        private async void ResetBallAfterGoal()
        {
            // Centrar la pelota después de cada gol
            ballX = fieldWidth / 2;
            ballY = fieldHeight / 2;

            // Configurar velocidad base y elegir dirección según el último gol
            float baseSpeed = 3f;
            ballSpeedX = ballDirectionToRight ? -baseSpeed : baseSpeed;

            // Evitar un movimiento completamente vertical
            Random rnd = new Random();
            ballSpeedY = rnd.Next(0, 2) == 0 ? -baseSpeed : baseSpeed;

            // Reiniciar las palas a su posición inicial
            paddle1Y = fieldHeight / 2 - paddleHeight / 2;
            paddle2Y = fieldHeight / 2 - paddleHeight / 2;

            // Pausar el juego por 3 segundos
            isPaused = true;
            await Task.Delay(3000); // Pausa de 3 segundos
            isPaused = false;
        }


        public void InicializarTcs(TaskCompletionSource<bool> tcs)
        {
            this.tcs = tcs;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float offsetX = (dirtyRect.Width - fieldWidth) / 2;
            float offsetY = (dirtyRect.Height - fieldHeight) / 2;

            canvas.FillColor = Colors.ForestGreen;
            canvas.FillRectangle(dirtyRect);

            canvas.StrokeColor = Colors.SandyBrown;
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
            // Si el juego está pausado, no actualizamos nada
            if (isPaused) return;

            ballX += ballSpeedX;
            ballY += ballSpeedY;

            if (ballY <= 0 || ballY >= fieldHeight) ballSpeedY = -ballSpeedY;

            if (ballX <= 60 && ballY >= paddle1Y && ballY <= paddle1Y + paddleHeight)
            {
                float impactPoint = (ballY - paddle1Y) / paddleHeight;
                ballSpeedX = Math.Abs(ballSpeedX) + speedIncrement;
                ballSpeedY = (impactPoint - 0.5f) * 10;
            }

            if (ballX >= fieldWidth - 60 && ballY >= paddle2Y && ballY <= paddle2Y + paddleHeight)
            {
                float impactPoint = (ballY - paddle2Y) / paddleHeight;
                ballSpeedX = -Math.Abs(ballSpeedX) - speedIncrement;
                ballSpeedY = (impactPoint - 0.5f) * 10;
            }

            if (ballX <= 0)
            {
                machineScore++;
                ballDirectionToRight = true;
                ResetBallAfterGoal();
            }
            else if (ballX >= fieldWidth)
            {
                playerScore++;
                ballDirectionToRight = false;
                ResetBallAfterGoal();
            }

            if (paddle2Y < ballY - paddleHeight / 2 && paddle2Y < fieldHeight - paddleHeight)
                paddle2Y += paddleSpeed;
            else if (paddle2Y > ballY - paddleHeight / 2 && paddle2Y > 0)
                paddle2Y -= paddleSpeed;

            if (isMovingUp && paddle1Y > 0) paddle1Y -= paddleSpeed;
            if (isMovingDown && paddle1Y < fieldHeight - paddleHeight) paddle1Y += paddleSpeed;

            ScoreLabel.Text = $"Jugador: {playerScore} | Máquina: {machineScore}";

            GameView.Invalidate();
        }

        private void MoveUp(object sender, EventArgs e)
        {
            isMovingUp = true;
            Debug.WriteLine("MoveUp pressed");
        }

        private void MoveDown(object sender, EventArgs e)
        {
            isMovingDown = true;
            Debug.WriteLine("MoveDown pressed");
        }


        private void OnButtonReleased(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Text == "B")
                {
                    isMovingUp = false;
                    Debug.WriteLine("MoveUp released");
                }
                else if (button.Text == "b")
                {
                    isMovingDown = false;
                    Debug.WriteLine("MoveDown released");
                }
            }
        }

        public Task<bool> GetGameResultAsync() => tcs.Task;
    }
}
