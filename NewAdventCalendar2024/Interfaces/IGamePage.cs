using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewAdventCalendar2024.Interfaces
{
    // Definición de la interfaz
    public interface IGamePage
    {
        // Firma del método que debe implementar cualquier clase que implemente esta interfaz
        void InicializarTcs(TaskCompletionSource<bool> tcs);
    }

}
