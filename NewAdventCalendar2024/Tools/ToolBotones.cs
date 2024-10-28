using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace NewAdventCalendar2024.Tools
{
    public class ToolBotones
    {
        private Button _button; // Cambiar a no estático para permitir múltiples instancias

        // Constructor que recibe el botón que va a manejar
        public ToolBotones(Button button)
        {
            _button = button; // Asigna el botón recibido al campo _button
        }

        // Método para aplicar el estilo
        public void ApplyStyle(Color textColor, Color backgroundColor, double borderWidth, Color borderColor)
        {
            _button.TextColor = textColor;
            _button.BackgroundColor = backgroundColor;
            _button.BorderWidth = borderWidth;
            _button.BorderColor = borderColor;
        }

        // Método para hacer la animación de agrandar y reducir
        public async Task AnimateButton()
        {
            // Aumenta el tamaño del botón
            await _button.ScaleTo(1.1, 100);
            // Regresa al tamaño normal
            await _button.ScaleTo(1.0, 100);
        }

        // Método para hacer saltitos laterales
        public async Task AnimateButtonJump()
        {
            await _button.TranslateTo(-15, 0, 50); // Mueve a la izquierda
            await _button.TranslateTo(15, 0, 50);  // Mueve a la derecha
            await _button.TranslateTo(0, 0, 50);   // Regresa al centro
        }

        // Método para desactivar el botón con estilo específico
        public void DisableButton(Color disabledColor, Color textColor)
        {
            _button.IsEnabled = false;
            _button.BackgroundColor = disabledColor;
            _button.TextColor = textColor;
        }
    }
}
