namespace NewAdventCalendar2024.Views.Juegos.Snake
{
    public partial class SnakePage : ContentPage
    {
        private int directionX;
        private int directionY;
        private List<(int X, int Y)> snakePositions;
        private (int X, int Y) foodPosition;
        private const int GridSize = 20; // Tamaño de cada cuadrícula
        private const int InitialSnakeLength = 3; // Longitud inicial de la serpiente

        public SnakePage(int puntos)
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            snakePositions = new List<(int X, int Y)>();
            for (int i = 0; i < InitialSnakeLength; i++)
            {
                snakePositions.Add((InitialSnakeLength - i, 5)); // Inicializa en una línea horizontal
            }
            directionX = 1; // Dirección inicial a la derecha
            directionY = 0; // Dirección inicial a la derecha
            PlaceFood();
            DrawGame(); // Dibuja el estado inicial del juego
        }

        private void PlaceFood()
        {
            foodPosition = (new Random().Next(0, (int)(gameView.Height / GridSize)),
                            new Random().Next(0, (int)(gameView.Width / GridSize)));
            while (snakePositions.Contains(foodPosition))
            {
                foodPosition = (new Random().Next(0, (int)(gameView.Height / GridSize)),
                                new Random().Next(0, (int)(gameView.Width / GridSize)));
            }
        }

        private void MoveUp() => (directionX, directionY) = (0, -1);
        private void MoveDown() => (directionX, directionY) = (0, 1);
        private void MoveLeft() => (directionX, directionY) = (-1, 0);
        private void MoveRight() => (directionX, directionY) = (1, 0);

        private void UpdateSnakePosition()
        {
            (int X, int Y) head = snakePositions.First();
            (int X, int Y) newHead = (head.X + directionX, head.Y + directionY);

            if (newHead == foodPosition)
            {
                snakePositions.Insert(0, newHead);
                PlaceFood();
            }
            else
            {
                snakePositions.Insert(0, newHead);
                snakePositions.RemoveAt(snakePositions.Count - 1);
            }

            DrawGame();
        }

        private void DrawGame()
        {
            // Crea un nuevo GameDrawable y lo asigna al GraphicsView
            gameView.Drawable = new GameDrawable(snakePositions, foodPosition);
            gameView.Invalidate(); // Redibuja el GraphicsView
        }

        private void OnMoveUpClicked(object sender, EventArgs e) => MoveUp();
        private void OnMoveDownClicked(object sender, EventArgs e) => MoveDown();
        private void OnMoveLeftClicked(object sender, EventArgs e) => MoveLeft();
        private void OnMoveRightClicked(object sender, EventArgs e) => MoveRight();

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Dispatcher.StartTimer(TimeSpan.FromMilliseconds(200), () =>
            {
                UpdateSnakePosition();
                return true; // Repetir el timer
            });
        }
    }

    public class GameDrawable : IDrawable
    {
        private List<(int X, int Y)> snakePositions;
        private (int X, int Y) foodPosition;

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
                canvas.FillRectangle(position.X * 20, position.Y * 20, 20, 20); // Usar GridSize aquí
            }

            // Dibuja la comida
            canvas.FillColor = Colors.Red;
            canvas.FillRectangle(foodPosition.X * 20, foodPosition.Y * 20, 20, 20); // Usar GridSize aquí
        }
    }
}
