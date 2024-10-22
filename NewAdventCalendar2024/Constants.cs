namespace NewAdventCalendar2024
{
    public static class Constants
    {
        public const string DatabaseFilename = "adventcalendar.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // Abre la base de datos en modo read/write
            SQLite.SQLiteOpenFlags.ReadWrite |
            // Crear la base de datos si no existe
            SQLite.SQLiteOpenFlags.Create |
            // Establece una base de dato multi-threadeds 
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }
}