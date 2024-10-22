using SQLite;

namespace NewAdventCalendar2024.Models
{
    [Table("Recompensas")]
    public class RecompensaModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Llave primaria
        public string Nombre { get; set; } // Nombre de la recompensa
        public bool Desbloqueada { get; set; } // Estado de la recompensa
    }
}
