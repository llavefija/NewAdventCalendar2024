﻿using System.Reflection;

namespace NewAdventCalendar2024.Tools
{
    public class ToolBotones
    {
        private static Button _button = new Button();

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
            await _button.ScaleTo(1.1, 100); // Aumenta tamaño
            await _button.ScaleTo(1, 100);   // Regresa al tamaño normal
        }

        // Método para hacer saltitos laterales
        public async Task AnimateButtonJump()
        {
            await _button.TranslateTo(-15, 0, 50); // Mueve a la izquierda
            await _button.TranslateTo(15, 0, 50);  // Mueve a la derecha
            await _button.TranslateTo(0, 0, 50);   // Regresa al centro
        }

        // Método para desactivar el botón con estilo específico
        public static void DisableButton(Color disabledColor, Color textColor)
        {
            _button.IsEnabled = false;
            _button.BackgroundColor = disabledColor;
            _button.TextColor = textColor;
        }
    }
}