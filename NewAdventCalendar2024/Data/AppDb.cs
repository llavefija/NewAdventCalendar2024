using SQLite;
using NewAdventCalendar2024.Models;
using NewAdventCalendar2024.Tools;

namespace NewAdventCalendar2024.Data;

public class AppDb
{
    SQLiteAsyncConnection Database;

    // Constructor de la clase
    public AppDb()
    {
    }

    // Conecta la base de datos
    async Task Init()
    {
        if (Database is not null)
            return;

        Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

        try
        {
            // Crea la primera tabla
            var result1 = await Database.CreateTableAsync<RecompensaModel>();

            // Verifica si la tabla se creó correctamente antes de continuar
            if (result1 == CreateTableResult.Created || result1 == CreateTableResult.Migrated)
            {
                var result2 = await Database.CreateTableAsync<BotonModel>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al inicializar la base de datos: {ex.Message}");
        }
    }


    // Devuelve una lista de todos los botones
    public async Task<List<BotonModel>> GetBotones()
    {
        await Init();
        return await Database.Table<BotonModel>().ToListAsync();
    }

    public async Task<bool> CompletarBoton(int id)
    {
        await Init(); // Inicializa la base de datos

        // Busca el botón por ID
        var boton = await Database.Table<BotonModel>().Where(i => i.Id == id).FirstOrDefaultAsync();

        // Verifica si el botón existe
        if (boton != null)
        {
            // Marca el botón como completado
            boton.Completado = true;

            // Actualiza el botón en la base de datos
            await Database.UpdateAsync(boton);

            // Puedes retornar true si la operación fue exitosa
            return true;
        }

        // Retorna false si el botón no fue encontrado
        return false;
    }


    // Devuelve una lista de todas las recompensas
    public async Task<List<RecompensaModel>> GetRecompensas()
    {
        await Init();
        return await Database.Table<RecompensaModel>().ToListAsync();
    }

    // Devuelve una lista de todos los botones no completados
    public async Task<List<BotonModel>> GetBotonesNoCompletados()
    {
        await Init();
        return await Database.Table<BotonModel>().Where(t => !t.Completado).ToListAsync();
    }

    // Devuelve una lista de todas las recompensas no desbloqueadas
    public async Task<List<RecompensaModel>> GetRecompensasNoDesbloqueadas()
    {
        await Init();
        return await Database.Table<RecompensaModel>().Where(t => !t.Desbloqueada).ToListAsync();
    }

    // Devuelve una lista de todos los botones buscados por ID
    public async Task<BotonModel> GetBotonPorId(int id)
    {
        await Init();
        return await Database.Table<BotonModel>().Where(i => i.Id == id).FirstOrDefaultAsync();
    }

    // Devuelve una lista de todas las recompensas buscadas por ID
    public async Task<RecompensaModel> GetRecompensaPorId(int id)
    {
        await Init();
        return await Database.Table<RecompensaModel>().Where(i => i.Id == id).FirstOrDefaultAsync();
    }

    // Desbloquea la recompensa
    public async Task DesbloquearRecompensa(int botonId)
    {
        await Init();

        // Busca el botón por su ID
        var boton = await GetBotonPorId(botonId);

        // Verifica que el botón existe y está completado
        if (boton?.Completado == true)
        {
            // Busca la recompensa asociada al botón, si no está desbloqueada
            var recompensa = await GetRecompensaPorId(boton.RecompensaId);
            if (recompensa?.Desbloqueada == false)
            {
                // Desbloquea la recompensa
                recompensa.Desbloqueada = true;
                await SaveRecompensa(recompensa);
            }
        }
    }

    // Guarda el boton
    public async Task<int> SaveBoton(BotonModel item)
    {
        await Init();
        if (item.Id != 0)
        {
            return await Database.UpdateAsync(item);
        }
        else
        {
            return await Database.InsertAsync(item);
        }
    }

    // Guarda la recompensa
    public async Task<int> SaveRecompensa(RecompensaModel item)
    {
        await Init();
        if (item.Id != 0)
        {
            return await Database.UpdateAsync(item);
        }
        else
        {
            return await Database.InsertAsync(item);
        }
    }

    // Inicializa los botones y las recompensas
    public async Task InicializarBotonesYRecompensas()
    {
        await Init();

        // Verifica si ya existen botones y recompensas
        var botones = await GetBotones();
        var recompensas = await GetRecompensas();

        if (botones.Count == 0 && recompensas.Count == 0)
        {
            for (int i = 1; i <= 31; i++)
            {
                // Crear un nuevo botón
                var nuevoBoton = new BotonModel
                {
                    Numero = i,
                    Activo = false,
                    Completado = false,
                    RecompensaId = i // Se vincula con la recompensa de igual Id
                };

                // Crear una nueva recompensa
                var nuevaRecompensa = new RecompensaModel
                {
                    Nombre = $"Recompensa {i}",
                    Desbloqueada = false
                };

                // Guardar el botón y la recompensa en la base de datos
                await SaveBoton(nuevoBoton);
                await SaveRecompensa(nuevaRecompensa);
            }
        }
    }

    public async Task ActualizarBotonesYHabilitar(ContentPage page)
    {


        // Obtiene la fecha actual
        DateTime fechaActual = DateTime.Now;

        // Obtiene todos los botones de la base de datos
        var botones = await Database.Table<BotonModel>().ToListAsync();

        // Recorre los botones y verifica las condiciones
        foreach (var boton in botones)
        {
            // Encuentra el control de botón en la página basado en el número del botón
            var botonControl = page.FindByName<Button>($"Btn{boton.Numero}");
            ToolBotones b = new ToolBotones(botonControl);

            if (botonControl != null)
            {
                // Comprobación de si el botón debe estar activo
                if ((fechaActual.Day >= boton.Numero && fechaActual.Month == 10) || fechaActual.Year > 2024)
                {
                    boton.Activo = true;
                    botonControl.IsEnabled = true; // Activa el botón en la interfaz

                    // Si es el mismo día, independientemente de si está completado o no, borde dorado
                    if (fechaActual.Day == boton.Numero && fechaActual.Month == 10 && fechaActual.Year == 2024)
                    {
                        // Si está completado, borde dorado e interior negro
                        if (boton.Completado)
                        {
                            b.ApplyStyle(Colors.Black, botonControl.BackgroundColor, 1, Colors.Green);
                        }
                        else
                        {
                            b.ApplyStyle(Colors.Black, botonControl.BackgroundColor, 1, Colors.Red);

                        }
                    }
                    else
                    {
                        // Si está completado, borde dorado e interior negro
                        if (boton.Completado)
                        {
                            b.ApplyStyle(Colors.Black, Colors.Green, 1, Colors.Gold);
                        }
                        // Si no está completado, borde negro e interior rojo
                        else
                        {
                            b.ApplyStyle(Colors.Black, Colors.Red, 1, Colors.Gold);

                        }
                    }
                }
                else
                {

                    // Desactivar el botón si no cumple las condiciones
                    boton.Activo = false;
                    botonControl.IsEnabled = false; // Desactiva el botón en la interfaz
                }

                // Actualiza el botón en la base de datos
                await Database.UpdateAsync(boton);
            }
        }
    }



}