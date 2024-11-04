using SQLite;

namespace NewAdventCalendar2024.Models
{
    [Table("Botones")]
    public class BotonModel
    {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; } // Llave primaria

            [MaxLength(2),Unique]
            public int Numero { get; set; } // Número del botón
            public bool Activo { get; set; } // Si el botón está activo
            public bool Completado { get; set; } // Si el juego asociado se ha completado

            public string MisterioDescripcion {  get; set; } // Texto que apuntara al diario
            public int RecompensaId { get; set; } // Relación con la recompensa
        
    }
}
