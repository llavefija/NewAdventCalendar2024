using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using NewAdventCalendar2024.Interfaces;

namespace NewAdventCalendar2024.Views.Juegos.Snake
{
    public partial class SnakePage : ContentPage, IGamePage
    {
        private int directionX;
        private int directionY;
        private List<(int X, int Y)> snakePositions;
        private (int X, int Y) foodPosition;
        private const int GridSize = 20; // Tamaño de cada cuadrícula
        private const int InitialSnakeLength = 3; // Longitud inicial de la serpiente
        private int minimumApples; // Manzanas mínimas requeridas
        private int score = 0;
        private int gameSpeed = 200; // Velocidad inicial de la serpiente en milisegundos

        private TaskCompletionSource<bool> gameCompletionSource;

        public SnakePage(int puntos, string titulo)
        {
            InitializeComponent();
            minimumApples = puntos; // Asignamos puntos al mínimo de manzanas
            titleLabel.Text = titulo;
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeGame();
        }

        public void InicializarTcs(TaskCompletionSource<bool> tcs)
        {
            gameCompletionSource = tcs;
        }

        private void InitializeGame()
        {
            score = 0;
            snakePositions = new List<(int X, int Y)>();
            for (int i = 0; i < InitialSnakeLength; i++)
            {
                snakePositions.Add((InitialSnakeLength - i, 5)); // Inicializa en una línea horizontal
            }
            directionX = 1; // Dirección inicial a la derecha
            directionY = 0;
            PlaceFood();
            DrawGame(); // Dibuja el estado inicial del juego
            UpdateScoreLabel();
        }

        private void PlaceFood()
        {
            var random = new Random();
            foodPosition = (random.Next(0, (int)(gameView.Width / GridSize)),
                            random.Next(0, (int)(gameView.Height / GridSize)));
            while (snakePositions.Contains(foodPosition))
            {
                foodPosition = (random.Next(0, (int)(gameView.Width / GridSize)),
                                random.Next(0, (int)(gameView.Height / GridSize)));
            }
        }

        private void MoveUp()
        {
            if (directionY == 0) // Evita moverse hacia atrás
                (directionX, directionY) = (0, -1);
        }

        private void MoveDown()
        {
            if (directionY == 0)
                (directionX, directionY) = (0, 1);
        }

        private void MoveLeft()
        {
            if (directionX == 0)
                (directionX, directionY) = (-1, 0);
        }

        private void MoveRight()
        {
            if (directionX == 0)
                (directionX, directionY) = (1, 0);
        }

        private void UpdateSnakePosition()
        {
            (int X, int Y) head = snakePositions.First();
            (int X, int Y) newHead = (head.X + directionX, head.Y + directionY);

            // Verifica si la serpiente choca con los límites o con su propia cola
            if (newHead.X < 0 || newHead.X >= gameView.Width / GridSize ||
                newHead.Y < 0 || newHead.Y >= gameView.Height / GridSize ||
                snakePositions.Contains(newHead))
            {
                EndGame(); // Finaliza el juego
                Navigation.PopAsync();
                return;
            }

            // Verifica si la serpiente ha comido la comida
            if (newHead == foodPosition)
            {
                snakePositions.Insert(0, newHead);
                score++;
                UpdateScoreLabel();
                PlaceFood();
            }
            else
            {
                snakePositions.Insert(0, newHead);
                snakePositions.RemoveAt(snakePositions.Count - 1);
            }

            DrawGame();
        }

        private void UpdateScoreLabel()
        {
            scoreLabel.Text = $"Manzanas recogidas: {score}/{minimumApples}";
        }

        private void DrawGame()
        {
            gameView.Drawable = new GameDrawable(snakePositions, foodPosition);
            gameView.Invalidate(); // Redibuja el GraphicsView
        }

        private void EndGame()
        {
            // Si el puntaje alcanza o supera el mínimo de manzanas, el juego se considera superado
            if (score >= minimumApples)
            {
                gameCompletionSource?.SetResult(true); // Marca el juego como completado
            }
            else
            {
                gameCompletionSource?.SetResult(false); // Marca el juego como fallido
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Dispatcher.StartTimer(TimeSpan.FromMilliseconds(gameSpeed), () =>
            {
                UpdateSnakePosition();
                return gameCompletionSource == null || !gameCompletionSource.Task.IsCompleted; // Detiene el timer si el juego terminó
            });
        }

        private void OnMoveUpClicked(object sender, EventArgs e) => MoveUp();
        private void OnMoveDownClicked(object sender, EventArgs e) => MoveDown();
        private void OnMoveLeftClicked(object sender, EventArgs e) => MoveLeft();
        private void OnMoveRightClicked(object sender, EventArgs e) => MoveRight();
    }

    public class GameDrawable : IDrawable
    {
        private readonly List<(int X, int Y)> snakePositions;
        private readonly (int X, int Y) foodPosition;

        public GameDrawable(List<(int X, int Y)> snakePositions, (int X, int Y) foodPosition)
        {
            this.snakePositions = snakePositions;
            this.foodPosition = foodPosition;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.White; // Fondo blanco
            canvas.FillRectangle(dirtyRect); // Rellenar el fondo

            // Dibuja la serpiente
            canvas.FillColor = Colors.Green;
            foreach (var position in snakePositions)
            {
                canvas.FillRectangle(position.X * 20, position.Y * 20, 20, 20);
            }

            // Dibuja la comida
            canvas.FillColor = Colors.Red;
            canvas.FillRectangle(foodPosition.X * 20, foodPosition.Y * 20, 20, 20);
        }
    }
}
