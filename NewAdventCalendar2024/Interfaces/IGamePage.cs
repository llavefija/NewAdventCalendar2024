using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewAdventCalendar2024.Interfaces
{
    // Interfaz para definir el contrato que deben seguir las páginas de juegos
    public interface IGamePage
    {
        // Inicializa el TaskCompletionSource para manejar el resultado del juego.
        // Este método permite a la página de juego comunicar si el jugador ha ganado o perdido
        void InicializarTcs(TaskCompletionSource<bool> tcs);
    }
}
