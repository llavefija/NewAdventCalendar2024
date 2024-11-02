using SQLite;
using NewAdventCalendar2024.Models;
using NewAdventCalendar2024.Tools;
using Microsoft.Maui.Animations;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System;
using static Microsoft.Maui.ApplicationModel.Permissions;
using System.Diagnostics.Metrics;

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

        #region Dia 1
        var nuevoBoton = new BotonModel
        {
            Numero = 1,
            Activo = false,
            Completado = false,
            RecompensaId = 1,
            MisterioDescripcion = @"El chalet.

Tal como indicaba la tarjeta de ayer, parece que he llegado a la dirección correcta: el número 12 en dorado. Un chalet en medio de la nada; curioso, cuanto menos. El número brilla en una robusta puerta de madera, y el chalet es más grande de lo que imaginaba. Sus paredes de madera oscura crujen con el viento, pero al menos la decoración navideña le quita algo de lo tenebroso.
Hoy me he asegurado de llevar de todo en la mochila: el teléfono, el cargador, mi cartera, la famosa libreta de casos, una pequeña navaja multiusos y unas cuantas golosinas (el azúcar me ayuda a pensar).

Nada más llegar, he mirado por las ventanas, pero no logro ver nada en el interior; está envuelto en sombras. Tras aporrear cuidadosamente la puerta, esta se abre y... ¡PUM! Un portazo a mis espaldas la cierra de golpe. Sin posibilidad de abrirla de nuevo, me invade el pánico. 
Por un instante, los nervios me consumen, pero me digo: “Ray, basta, ¡tranquila!”. Cierro los ojos, respiro hondo y cuento hasta tres. Al abrirlos, me siento más centrada. Busco en mi bolsillo una piruleta y la saboreo, tratando de encontrar calma.

A oscuras, tanteo hasta dar con el interruptor de la luz. La habitación se ilumina tenuemente, revelando una entrada polvorienta con muebles cubiertos por sábanas blancas, como fantasmas en espera de ser descubiertos. Las ventanas están tapiadas y un olor a humedad y madera vieja llena el aire.
Un escalofrío me recorre al notar que no hay señales de vida. Me esperan largas horas... y la creciente sensación de que este lugar guarda más secretos de los que se ven a simple vista.

Al dar un paso más en la penumbra, un susurro lejano parece provenir de algún rincón oculto. ¿Será el viento, o algo más? La intriga crece, y en el fondo sé que, pase lo que pase, debo seguir adelante.
"
        };

        // Crear una nueva recompensa
        var nuevaRecompensa = new RecompensaModel
        {
            Nombre = $"Una piruleta que tenía en el bolsillo.",
            Desbloqueada = false
        };


        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 2

        nuevoBoton = new BotonModel
        {
            Numero = 2,
            Activo = false,
            Completado = false,
            RecompensaId = 2,
            MisterioDescripcion = @"Una mala noche.

No hay peor manera de pasar una noche... en un chalet misterioso, lleno de polvo, con un frío que cala hasta los huesos. Y esos muebles... dan escalofríos solo de verlos. Se nota que hace mucho tiempo que nadie pasa por aquí: la calefacción no funciona y sería necesaria una limpieza a fondo.

Lo primero que he hecho ha sido quitar las sábanas blancas de los muebles, buscando algo que me ayude a resolver este misterio o a abrir la puerta, pero mis primeras búsquedas no han dado resultados. Para mi sorpresa, descubrí que el chalet está decorado con adornos navideños e incluso tiene un bonito árbol en el comedor. Está totalmente abandonado, pero alguien se ha asegurado de darle un toque festivo.

Lo único útil que he encontrado es una especie de calentador de manos; espero que al menos me ayude a soportar el frío. 

En mi cabeza no dejo de pensar en un nombre: “Mateo”. ¿Será cosa suya? Imposible, hace años que le perdí la pista. ¿Entonces quién está detrás de todo esto? 

Ayer me dije que me esperaban largas horas, pero a este ritmo... parece que me perderé las vacaciones de Navidad este año.

P.D.: De forma misteriosa, la despensa y la nevera están llenas de comida. Creo que será suficiente para unos días, pero... ¿quién se encargó de dejarla aquí?

Al anochecer, mientras intento calentarme con el artefacto que encontré, observé cómo una pequeña fuente de luz provenía del piso de abajo. No sé si es mi imaginación, pero el chalet parece guardar más de lo que aparenta.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = $"Artefacto de calor.",
            Desbloqueada = false
        };


        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 3
        nuevoBoton = new BotonModel
        {
            Numero = 3,
            Activo = false,
            Completado = false,
            RecompensaId = 3,
            MisterioDescripcion = @"Un juego de mi imaginación.

Creo que me estoy volviendo loca. Nada tiene sentido hoy. Anoche intenté seguir aquella luz extraña, pero desapareció cuando estaba a mitad de camino bajando las escaleras. Ya no sé si fue real o solo un juego de mi imaginación.

Lo único que he encontrado al volver a la “cama” —si se le puede llamar así— ha sido una pequeña pelota blanca brillante. Mi cama improvisada es en realidad un viejo colchón sucio sobre un somier de hierro, aunque he reutilizado las sábanas de los muebles para cubrirlo y hacerlo algo más soportable.

No estoy segura de si la pelota estaba allí antes, pero de todas formas me sirve. La he hecho rebotar varias veces contra la pared; a veces eso me ayuda a pensar, pero hoy no tengo suerte. Estoy en blanco.

¿Tercer día y ya estoy así? Espero que sea un bajón puntual. Quizás esta pelota pueda ayudarme a encontrar algo más, porque ahora mismo solo siento que estoy atrapada en un laberinto sin salida...

He aprovechado el tiempo para investigar un poco más a fondo este espacio y he esbozado un pequeño plano, por si me es útil en el futuro.

El chalet, un lugar acogedor de dos plantas, cuenta con un amplio comedor conectado directamente a la cocina. La chimenea que adorna el comedor, aunque parece no tener salida, le da un toque cálido al ambiente, y en su repisa hay varios elementos decorativos, además de una vela con aroma a almendras tostadas. En la segunda planta, he encontrado varios dormitorios y un baño, y desde el pasillo que conecta las habitaciones, puedo ver el comedor en la planta baja. Todo esto me recuerda que estoy en un lugar con más secretos de los que imaginaba.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Una vela aromatica.",
            Desbloqueada = false
        };


        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 4

        // Inicializa el cuarto botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 4,
            Activo = false,
            Completado = false,
            RecompensaId = 4,
            MisterioDescripcion = @"Algo de musica.

¡Hoy estoy a tope! Revisando los cajones de la cocina, he encontrado una pequeña radio. Es vieja y está sucia, pero, extrañamente... ¡funciona! Después de ajustar la antena, he logrado captar algunas ondas. No deja de parecerme raro, porque en todos estos días que llevo aquí encerrada, mi móvil no tiene ni una sola barra de cobertura; parece servir solo para tomar fotos y poco más. Sin embargo, esta radio sigue recibiendo algo, aunque sea con mucha estática.

La música me ha dado energía, tanta que pensé que tendría la fuerza suficiente para quitar la madera que bloquea las ventanas... pero va a ser que no. Supongo que se necesita algo más que un poco de Queen para romper una ventana.

Sin embargo, nada me desmotiva hoy. Dormí mucho mejor anoche gracias a un antifaz improvisado; me ayuda a calmar los nervios y a dormir sin preocuparme de lo que pueda haber afuera.

Mientras ajustaba la radio, escuché algo extraño: una voz entrecortada que decía números... como una frecuencia: '1...7...2...3'. No estoy segura de lo que significa, pero intentaré recordar esos números. Tal vez sea solo una interferencia, o quizás... una pista.

A ver qué tal mañana. Ya van cinco días aquí, sin una salida clara ni respuestas, pero al menos ahora tengo algo en lo que pensar.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Un antifaz gracioso improvisado.",
            Desbloqueada = false
        };

        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 5

        // Inicializa el quinto botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 5,
            Activo = false,
            Completado = false,
            RecompensaId = 5,
            MisterioDescripcion = @"Venganza.

He descartado a Mateo de mi lista negra. No creo que esto sea cosa suya; su motivación eran las estafas informáticas. Montar algo así es mucho más retorcido… incluso perverso.

¡Estamos a 20 días de Navidad! Solo alguien malvado haría algo así. Me llevo la piruleta a la boca, pensando qué está sucediendo aquí.

Entonces, una idea me golpea: ¡esto es una venganza! ¿Podría todo ser cosa de David Hernández? Hace cinco años, su caso fue uno de los secuestros mejor planeados que jamás vi: disfraces, falsificación de documentos, trampillas secretas… secretos y más secretos. Si no hubiera sido por mí, aquella historia habría terminado mucho peor. ¿Y si está resentido? Quizás quiere hacerme pasar por algo similar.

Pero... aunque todo encaje, algo me detiene. Un caso así requiere preparación y recursos, y David se mantiene vigilado. Además, dudo que haya alguien de su círculo cerca. ¿Es realmente capaz de montar algo así estando bajo custodia? Me da vueltas la cabeza.

Sin embargo, por ahora no tengo nada más a lo que aferrarme.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Una piruleta que tenia en la mochila.",
            Desbloqueada = false
        };


        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 6

        // Inicializa el quinto botón y recompensa
        // Inicializa el sexto botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 6,
            Activo = false,
            Completado = false,
            RecompensaId = 6,
            MisterioDescripcion = @"Una caja demasiado fuerte.

La cosa no podría ir mejor. Todos estos días he estado haciendo rebotar la pelota que me encontré el otro día. Aunque no me da una pista directa, al menos me ayuda a pensar. Y es entonces cuando, ¡plash!, una tabla de madera del parquet del pasillo ha cedido y se ha levantado, revelando una caja fuerte. Una caja fuerte de color gris como el acero, con una pequeña pantalla que decía “LOCK” y una pequeña rueda de color rojo en el centro rodeado de números.

No parecía tener gran cosa dentro, solo un par de piezas metálicas que resonaban en su interior. Lo primero que he intentado es forzarla con la navaja, pero al no tener resultado, he decidido estrellarla contra varios muebles del chalet. Fue entonces cuando, entre el estruendo, me vino a la mente un número: ¡1723! La adrenalina me ha jugado una mala pasada, pero debe de ser el código, el mismo que escuché en la radio hace varios días.

¡¡Eureka!! Ese era el codigo. Dentro de la caja he encontrado un anillo con brillantes de tono verde. No sé si es esmeralda, peridoto, jade u olivina, pero desde luego es precioso. Ojalá alguien me hubiera pedido matrimonio con algo así... echo de menos a mi pareja. :(

Pero eso no es todo. Junto al anillo había una hermosa llave verde con un muñeco de jengibre en el mango. Tenía una punta demasiado plana y alargada, y nunca había visto una igual. Era bastante pesada y reflejaba toda la luz, casi tanto como el anillo. No sé dónde puede encajar, pero estoy segura de que ¡¡voy a descubrirlo pronto!!
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Una pesada llave alargada de color verde y un anillo preciso con una joya verde.",
            Desbloqueada = false
        };

        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 7


        // Inicializa el quinto botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 7,
            Activo = false,
            Completado = false,
            RecompensaId = 7,
            MisterioDescripcion = @"Un gran chasco.

He probado la llave en todas las puertas y en todos los cajones con cerradura, ¡y nada!

Parece que esta llave sirve para algo más de lo que puedo ver. Algo se me escapa, una sensación de que estoy cerca de descubrir algo importante. Es cierto que una llave de este tamaño, color y diseño no se encuentra todos los días, y su misteriosa apariencia me hace sospechar que su propósito va más allá de lo evidente.

Mientras intento ver más allá, pensando a lo grande, me hago preguntas sobre este lugar y todo este misterio. El silencio del chalet se siente opresivo, como si las paredes guardaran secretos que esperan ser revelados.

Por ejemplo, el enigma de la comida “infinita”. ¿Quién la deja aquí? ¿Qué sucede realmente? Cada bocado que tomo me deja con más dudas que respuestas, como si estuviera atrapada en un juego del que aún no comprendo las reglas.

De todos modos, no se puede pensar con el estómago vacío, así que voy a comerme ese melón que lleva días mirándome.

P.D.: Mi humor va mejorando cada día, aunque el entorno me advierte que no debo acomodarme. La tensión en el aire es palpable, y algo me dice que aún no he visto lo peor.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Una buena merienda en forma de melón",
            Desbloqueada = false
        };


        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 8

        // Inicializa el octavo botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 8,
            Activo = false,
            Completado = false,
            RecompensaId = 8,
            MisterioDescripcion = @"¿El rosa es mi color?

Llevo horas dándole vueltas a todo: no solo a lo que ocurre en este extraño chalet, sino también a mis propias cosas, a mi vida y a quién soy.

Este lugar, con su atmósfera inquietante, ha tenido un efecto curioso en mí; me ha forzado a estar más tranquila, casi sin quererlo. Mañana cumplo 30 años, y la verdad es que añoro aquella época en la que tenía 20. Todo era más sencillo y despreocupado. Valorar las cosas en su momento es un arte que casi nadie domina, y quizás ahora, en medio de este misterio, estoy comenzando a aprenderlo.

A medida que el tiempo pasa, mis prioridades han cambiado, y la nostalgia se convierte en una sombra persistente. Aquella chica de 20 años, siempre apurada y llena de energía, era tan distinta. Yo, ahora, intento quejarme menos, disfrutar más y darme un respiro cuando las cosas no salen como espero. Sin embargo, a veces, esa chispa de espontaneidad se siente lejana, como un eco de un pasado más vibrante.

Entre esos recuerdos, uno tonto me viene a la mente: mi funda de móvil de entonces, de un rosa suave y bonito. Nunca le presté demasiada atención, pero ahora me doy cuenta de que el rosa siempre ha estado en mi vida, como un hilo invisible que une mis experiencias. Hoy llevo una mochila de ese mismo color, y me pregunto... ¿será que el rosa es mi color favorito y yo sin saberlo?

Esta revelación provoca una mezcla de emociones; quizás hay detalles de nosotros mismos que llevan años con nosotros y que solo entendemos mucho después. Mientras la oscuridad de este chalet me rodea, pienso en cómo las cosas pequeñas pueden llevar consigo grandes significados. Aunque haya aspectos de mi vida que no puedo cambiar, estoy empezando a apreciar quién soy hoy, con 30 años y todo lo vivido. Eso, en sí mismo, es todo un descubrimiento.

A medida que las horas pasan, una inquietud crece en mí, un presentimiento de que el rosa puede ser más que solo un color; tal vez sea una señal, una conexión con un pasado que podría contener las respuestas que busco en este lugar. ¿Qué otras verdades se esconden en este chalet, y qué secretos guardan las sombras que me observan?
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Un recuerdo fugaz de mi funda rosa.",
            Desbloqueada = false
        };

        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 9

        // Inicializa el noveno botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 9,
            Activo = false,
            Completado = false,
            RecompensaId = 9,
            MisterioDescripcion = @"Un enorme regalo en el comedor.

Estoy desconcertada. No entiendo nada. ¿¿Por qué?? ¿¿Cómo??

Al despertar, me encuentro un gran paquete en el centro de la sala principal del chalet. Sigo sin comprender cómo ha llegado aquí sin que yo me diera cuenta, y mucho menos en un lugar tan apartado como este. Hoy es mi cumpleaños, y aunque nunca habría imaginado pasarlo aquí, entre estas paredes y en soledad, esto supera cualquier sorpresa que pudiera esperar...

Desde el momento en que lo vi, una sensación extraña me invadió. Una etiqueta colgaba del paquete, y en ella estaba escrito mi nombre. Eso significa que quien haya dejado este ""regalo"" sabe algo sobre mí... quizás más de lo que quisiera admitir. Y, para colmo de rarezas, el paquete no dejaba de moverse ligeramente, como si algo o alguien en su interior intentara liberarse.

Decidí abrirlo, entre la curiosidad y la cautela. Lo primero que encontré fue algo inesperado: una llave de color rosa chicle, ¡super cuqui, cabe decir! Al igual que la primera llave verde que encontré, esta también tiene en el mango un muñeco de jengibre, pero esta vez la punta de la llave parece formar una letra o un símbolo extraño, como si estuviera destinada a usarse junto con la otra. Quizás juntas puedan abrir algo importante en este lugar. Así que, de momento, las guardaré en mi mochila hasta que descubra cómo funcionan.

Pero eso no es lo más sorprendente... Dentro del paquete había ¡un perro! Un cachorro de mirada brillante, juguetón y sin ningún miedo en absoluto. Aunque me pilló por sorpresa (¿qué se supone que voy a hacer con un perro en este lugar?), me siento aliviada de tener algo de compañía. La idea de compartir este misterio con alguien más, aunque sea un cachorro, hace que este sitio se sienta menos solitario y tenebroso.

Sin embargo, la inquietud sigue ahí. Quien sea que haya preparado esto para mí no solo sabe que hoy es mi cumpleaños, sino que tiene acceso directo a este chalet y me vigila de cerca. Esa sensación es escalofriante, como si estuviera atrapada en un juego del que no entiendo las reglas. Pero, extrañamente, también me siento más decidida que nunca a desentrañar qué secretos esconde este lugar y qué papel juego yo en todo esto.

Por ahora, sumo esta llave misteriosa a mi colección y, con mi nuevo compañero a mi lado, quizás esté un paso más cerca de resolver el rompecabezas que este lugar esconde. La combinación de la llave rosa y la presencia del cachorro me hace preguntarme si el misterio se está desvelando o si, por el contrario, solo me estoy adentrando en un laberinto aún más complicado.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Una pesada llave de color rosa y un cariñoso compañero canino.",
            Desbloqueada = false
        };



        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 10

        // Inicializa el décimo botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 10,
            Activo = false,
            Completado = false,
            RecompensaId = 10,
            MisterioDescripcion = @"Mi fiel compañero.

Siempre me ha gustado el nombre de Jingle; tiene un sonido alegre, como campanas tintineantes, y con las fechas tan cercanas a la Navidad, no pude resistirme a llamarlo así. Parecía la opción perfecta para un compañero tan especial.

Jingle es un adorable perro salchicha de pelaje negro con unas manchas de color café claro que le dan un toque encantador y único. Su mirada curiosa y su energía contagiosa han llenado de vida este extraño lugar. No puedo evitar sentirme agradecida por tenerle aquí; en medio de esta soledad y misterio, su presencia hace que todo sea un poco más soportable.

Mientras disfrutaba de un magnífico caramelo dorado de sabor a miel, no he parado de pensar en cómo puede estar conectado todo esto, especialmente mis reflexiones de ayer sobre los colores y la aparición de la llave rosa. La dulzura del caramelo me acompañaba, contrastando con la inquietud que sentía. La coincidencia es inquietante: justo cuando me detuve a pensar en el rosa y su significado, aparece una llave de ese color. Esto me lleva a dos conclusiones, y ambas son perturbadoras. La opción más realista es que alguien con mucho tiempo libre se haya dedicado a estudiarme y sabe cómo influir en mis pensamientos. La segunda opción, más fantasiosa, es que este chalet esté de alguna manera conectado conmigo, como si mis pensamientos y mis experiencias se entrelazaran con los secretos que esconde.

Esta noche, mientras he explorado cada rincón del chalet en busca de más pistas, he sentido que, aunque tengo a Jingle a mi lado, no estamos completamente solos. Hay una sensación en el aire, una presencia que parece moverse más allá de las paredes que puedo ver. Escuché ruidos apagados que venían desde algún lugar en el suelo, como si hubiera una habitación secreta debajo de nosotros, justo fuera de mi alcance. Eran pasos suaves, resonando apenas, pero lo suficiente como para que la piel se me erizara.

No sé si son imaginaciones mías o si realmente alguien —o algo— está allí abajo, observándonos o esperando. Sea como sea, siento que este lugar esconde algo más profundo, algo que aún no puedo ver. Con Jingle a mi lado, me siento menos vulnerable, pero también más alerta que nunca. La inquietud de lo desconocido me acompaña, y no puedo dejar de preguntarme qué revelaciones me esperan en este enigma, un laberinto donde cada color, cada sonido y cada susurro parecen tener un significado oculto.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Un dulce caramelo con sabor a miel.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 11

        // Inicializa el undécimo botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 11,
            Activo = false,
            Completado = false,
            RecompensaId = 11,
            MisterioDescripcion = @"Una misteriosa nota.

¡¡Me encanta!! ¿Quién diría que Jingle tiene más ambición por salir de aquí que yo? Bueno, para ser justa, es una ambición dirigida a cualquier cosa que huela a basura… pero hoy su curiosidad ha servido para algo.

Mientras me acompañaba en la cocina, Jingle se lanzó directo a la papelera y, cuando pensé que tendría que limpiar el desastre, vi que había sacado un papel arrugado, uno que nunca había visto antes. Lo extraño es que parecía recién arrugado, como si alguien lo hubiera dejado a toda prisa, como si hubiera cambiado sus planes en el último momento. Quien esté vigilándonos parece tener todo perfectamente calculado, y hasta ahora, me tiene en jaque.

Con un nudo en la garganta, leí la nota y descubrí que era algo más que una burla. 

“Cuidado donde el arte se asoma; detrás del marco, el secreto se asoma”.

¿Se está riendo de mí? ¿¿De mí?? ¿¡EN SERIO!? ¡Qué desfachatez! La idea de que haya algo oculto detrás de un cuadro me inquieta, pero también despierta mi curiosidad. Sé que hay más en este lugar de lo que parece, y tengo que encontrarlo.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Una pequeña nota misteriosa y una basura que recoger.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 12

        // Inicializa el duodécimo botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 12,
            Activo = false,
            Completado = false,
            RecompensaId = 12,
            MisterioDescripcion = @"Una cajita misteriosa en la pared.

Hoy me he topado con una caja inesperada. La he encontrado incrustada en la pared, perfectamente disimulada como si fuera un cuadro. En todo este tiempo no había notado su presencia, como si alguien la hubiera colocado ahí a propósito, esperando que pasara desapercibida.

La caja en sí es preciosa, de aspecto antiguo, con una perla roja en forma de corazón en la parte delantera que le da un toque elegante. Sin embargo, el óxido y el polvo revelan que lleva demasiado tiempo olvidada. Al intentar abrirla, me di cuenta de que el cierre estaba atascado; debía estar completamente oxidada por dentro. Me desesperé tanto que, después de varios intentos, la lancé al aire en un arrebato de frustración.

¡¡PAAAAM!! La caja estalló en mil pedazos al caer al suelo. El ruido fue tan fuerte que Jingle salió corriendo, asustado y con las orejas bien bajas. ¡Pobre! Pero el golpe ha sido toda una suerte porque, al romperse, reveló un nuevo tesoro: una tercera llave, esta vez de un azul profundo y brillante, probablemente la más sencilla de todas, pero igualmente fascinante. La pequeña figura de jengibre sigue presente en el diseño, como en las anteriores. Parece que este detalle tiene un significado que aún debo desentrañar.

Junto a la llave, la perla roja también se desprendió, rodando hasta mis pies. La recogí con cuidado; tiene un tono carmesí tan intenso que parece latir con vida propia. Quizás tiene un valor simbólico, o tal vez solo sea otro capricho del creador de este lugar, pero por si acaso, la guardaré como un talismán.

Este lugar empieza a revelarse poco a poco, pero mi mente no deja de pensar en el tiempo que llevo aquí encerrada. Hoy es el día 12, una fecha especial para mí porque es nuestro aniversario, el día que cada año celebro con mi pareja. Nos queda solo un mes para alcanzar los 13 años juntos... y todo lo que quiero es salir de aquí y darle un abrazo. Nada más. Me pregunto si quien me ha dejado estas pistas sabría la importancia de esta fecha para mí. Cada detalle nuevo que encuentro parece desafiarme, recordándome que no puedo rendirme ahora.

Este avance es algo esperanzador. Ahora tengo tres llaves, cada una con su propio color y misterioso muñeco de jengibre. La idea de descubrir su función es el siguiente paso hacia la salida. ¡Ya lo verás! Cada pista me acerca un poco más. No pienso perder la esperanza, porque cada descubrimiento es un paso adelante... aunque sea en esta jaula dorada que me mantiene alejada de lo que más amo.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Una llave de color azul brillante y un talismán rojo con forma de corazón.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 13

        // Inicializa el decimotercer botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 13,
            Activo = false,
            Completado = false,
            RecompensaId = 13,
            MisterioDescripcion = @"Un caramelo de esperanza.

Hoy me encuentro en un estado de ánimo bastante gris. Ayer, después de los emocionantes descubrimientos, esperaba que esta energía positiva continuara, pero parece que no podía ser de otra manera. De hecho, he decidido comerme el último caramelo de miel que me queda, con la esperanza de que al menos su sabor dulce me ayude a mejorar. No quiero recurrir a los de mentol, no aún.

Sin embargo, la verdad es que he perdido la esperanza de nuevo… hoy no tengo motivación para nada. La radio, que había sido mi compañía, ya no tiene pilas. A mí me duele la garganta, y si sigo así, acabaré enferma. Me pesa el corazón y la mente. La alegría que había encontrado en los pequeños hallazgos parece haberse desvanecido. Me pregunto cómo es posible que, en un lugar lleno de secretos, me sienta tan sola y abatida.

Es frustrante, porque quiero seguir luchando, pero el cansancio y la incertidumbre se están apoderando de mí. Tal vez solo necesite un momento para respirar y recordar que cada día es una nueva oportunidad, aunque hoy me cuesta verlo.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Un último caramelo de miel.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 14

        // Inicializa el decimocuarto botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 14,
            Activo = false,
            Completado = false,
            RecompensaId = 14,
            MisterioDescripcion = @"Descubrimientos oscuros en un dia gris.

¿Qué te voy a contar, pequeño diario? Creo que has pasado días mejores… Con todos los casos que hemos resuelto juntos, y ahora me debes aguantar pachucha. Sobretodo con este estilo monocolor en mis apuntes... ¡¡solo me he traído el boli negro!!

A pesar de que hoy me siento fatal, mi mente no puede evitar seguir pensando en las pistas que quedan por descubrir. En medio de esta niebla de desánimo, me ha sorprendido una nueva revelación. Después de encender la vela que estaba sobre la chimenea cada día, hoy he notado algo extraño: una pequeña llave negra ha aparecido.

No es como las otras tres; esta es super simple y diminuta, y no encaja en absoluto con el estilo de las demás. Además, ¡le falta el muñeco de jengibre! Es un detalle que me intriga, pero no puedo evitar pensar que, aunque sea diferente, seguramente tiene su utilidad entre estas paredes que forman el chalet.

Jingle parece que se preocupa por cómo estoy; pobrecito, él no entiende lo que pasa aquí. Esta situación me tiene muy desanimada, y no puedo evitar sentir que he perdido el rumbo. Así que, tras muchas dudas, finalmente he decidido comer mi primer caramelo de mentol. Quiero mantenerme alerta y con la mente despejada, especialmente ahora que llevo dos semanas aquí encerrada. Necesito recordar que, a pesar de lo mal que me siento, cada día trae consigo la posibilidad de un nuevo descubrimiento.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Una llave negra pequeña y un caramelo de mentol.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 15

        // Inicializa el decimoquinto botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 15,
            Activo = false,
            Completado = false,
            RecompensaId = 15,
            MisterioDescripcion = @"La gran receta.

Hoy, en medio del misterio del chalet, decidí que no podía dejar que el desánimo me consumiera. Así que me embarqué en una pequeña aventura culinaria: ¡preparar un rooibos de vainilla improvisado!

Ingredientes:

Té rooibos: La base de mi creación, que encontré en la despensa.
Esencia de vainilla: Un hallazgo afortunado que le dará un toque especial.
Azúcar moreno: Para añadir dulzura a la mezcla.
Canela en polvo: Para un giro cálido y acogedor.
Pizca de sal: Para realzar los sabores.

Elaboración:

Cocción del rooibos: Caliento agua y añado una cucharada de rooibos, dejando que hierva durante 5 minutos.

Mezcla mágica: Añado unas gotas de esencia de vainilla, una cucharadita de azúcar moreno, una pizca de sal y un toque de canela a la cacerola. ¡El aroma es increíble!

¡A disfrutar!: Cuelo el rooibos en una taza y lo saboreo. El sabor cálido de la vainilla me envuelve y me hace olvidar un poco la situación.

Y así, con mi improvisado rooibos de vainilla, celebro los pequeños placeres de la vida, incluso en este extraño lugar. ¡Salud por los descubrimientos!
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Una nueva receta innovadora.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 16

        // Inicializa el decimosexto botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 16,
            Activo = false,
            Completado = false,
            RecompensaId = 16,
            MisterioDescripcion = @"Despertando con Energía.

Hoy me siento en una nube. De verdad que estoy mil veces mejor. Lo importante ahora es que tengo un delicioso té caliente, que está haciendo maravillas, junto a un caramelo de mentol que finalmente he probado. ¡No está ni tan mal! ¡¡Toma ya, Ray!! Esto es lo que necesitaba para empezar el día con energía.

Además, no puedo dejar de pensar en la llave que encontré antes de ayer, esa pequeña y simple llave negra que parece no encajar con el estilo de las demás. Me pregunto qué puertas podría abrir. Tal vez haya algo escondido en alguna parte del chalet, algo que me ayude a entender mejor todo este enredo.

Así que hoy me he propuesto revisar mis apuntes en la libreta. No quiero dejar piedra sin remover en mi búsqueda de pistas. Cada detalle cuenta y puede acercarme más a la verdad.

¡Vamos a por ello!
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Último caramelo de mentol.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 17

        // Inicializa el decimoséptimo botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 17,
            Activo = false,
            Completado = false,
            RecompensaId = 17,
            MisterioDescripcion = @"¿Dónde tengo la cabeza...?

He estado... bueno, he hecho de todo, excepto pensar en que estoy encerrada. A veces parece que el tiempo se detiene aquí. Y, a decir verdad, no se está tan mal; hay comida, agua, y estoy viviendo una experiencia completamente nueva. Quizás me he vuelto demasiado positiva. Si me lo hubieran preguntado hace 20 años, estoy segura de que ya habría resuelto este enigma.

Sin embargo, debo admitir que el catarro sigue aquí conmigo, así que seguiré la receta del doctor (o sea, yo) por un día más. ¡Es increíble cómo los síntomas pueden hacer que incluso los mejores planes se desmoronen! Con lo bien que iba ayer, sintiéndome animada y con la mente despierta, ahora es un reto volver a concentrarme. Para hacer el bajón más ameno, me estoy comiendo unas gominolas de color rosa que encontré en mi mochila. Son dulces y me hacen sonreír un poco, así que no me quejo.

Mientras buscaba alguna cerradura donde encajara la llave negra, ¡sorpresa! Encontré unas pilas. Eso es sinónimo de música de nuevo. ¡Por fin podré encender la radio y disfrutar de mis melodías favoritas! Puede que la música sea el mejor aliado en este encierro, un recordatorio de que hay algo más allá de estas paredes.

Me pregunto si este estado de encierro me ha llevado a ver las cosas de manera diferente. Quizás la falta de estrés exterior ha abierto mi mente a nuevas posibilidades. Necesito recordar que no estoy sola; tengo a Jingle y el misterio de las llaves. Las pequeñas cosas son las que realmente importan, incluso si estoy atrapada aquí. Así que hoy, en vez de dejarme llevar por la desesperación, intentaré aprovechar al máximo este tiempo. ¡Vamos a seguir adelante!
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Unas gominolas rosas.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 18

        // Inicializa el decimoctavo botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 18,
            Activo = false,
            Completado = false,
            RecompensaId = 18,
            MisterioDescripcion = @"Todo va mejor.

¡Venga, Ray! Todo parece estar mejorando. Entre el té caliente, los caramelos, el azúcar y, sobre todo, la compañía de Jingle, todo es perfecto por ahora. Él siempre sabe cómo hacerme sonreír y me da fuerzas para seguir adelante.

Sin embargo, anoche, mientras me acomodaba en la cama, escuché susurros, pasos e incluso el sonido de la puerta principal abriéndose. ¿Será eso posible? Al levantarme esta mañana, corrí a comprobar, pero todo estaba super cerrado. La idea de que haya alguien más aquí, aunque sea un eco de mi imaginación, me mantiene alerta.

¡Vamos a recuperarnos al máximo! Hoy nos despedimos del resfriado, ¡ya verás! Solo necesito preparar una última receta de ese té, y con un poco de suerte, estaré activa como siempre. ¡Tú puedes! ¡¡¡Ánimo!!! La energía positiva es clave, y con un poco de optimismo, estoy segura de que superaré esto en un abrir y cerrar de ojos.

La música siempre ha sido un buen antídoto para el ánimo. Así que, entre el té y mis melodías favoritas, ¡no hay resfriado que me detenga! Hoy es un nuevo día, y estoy decidida a aprovecharlo al máximo. ¡Vamos a darle un buen empujón a esta recuperación!
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Gominolas con forma de oso.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 19

        // Inicializa el decimonoveno botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 19,
            Activo = false,
            Completado = false,
            RecompensaId = 19,
            MisterioDescripcion = @"Mi taza favorita.

¡19 días aquí dentro! Y, sorprendentemente, acabo de darme cuenta de esta taza tan espectacular. ¿De verdad estoy hecha para buscar cosas? ¡Es increíble lo que uno puede pasar por alto cuando está atrapado en sus pensamientos! Esta taza destaca entre las demás, parece como si alguien la hubiera puesto recientemente. ¿Cómo es posible que no la hubiera notado antes?

Aprovecho para desayunar con ella, ya que se merece ser parte de mis rituales matutinos. Sin embargo, al mirar en su interior, encuentro una nota encriptada. Mis ojos se iluminan con la posibilidad de otro enigma.

Finalmente, logré descifrar el mensaje que dice:

“Bvxq wkh ohd, xvh wkd wSoh wkh vlph uh wkh pdeoh vpruh sxwfrqg.”

Ahora me quedo desconcertada. Otra nota más. ¿Qué significará esto? La idea de que podría haber algo escondido en el armario me hace sentir una mezcla de emoción y ansiedad.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Una nota misteriosa y una taza espectacular.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 20

        // Inicializa el vigésimo botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 20,
            Activo = false,
            Completado = false,
            RecompensaId = 20,
            MisterioDescripcion = @"El complemento perfecto.

Hoy es el segundo día desde que descubrí la nota en mi taza favorita y, a pesar de lo que me gustaría, todavía no he logrado entender su significado. He estado pensando y pensando, dándole vueltas al enigma. Quizás haya algo más sencillo en todo esto de lo que parece.

Mientras observaba la tapa de tazas que encontré, se me ocurrió que las letras desordenadas de la nota podrían tener un sentido oculto. ¿Y si esas letras sin orden no son más que un juego de palabras que simplemente necesitan ser reorganizadas? Pero... ¿sería demasiado básico? Me cuesta creer que algo tan evidente haya pasado desapercibido.

A veces me pregunto si mi mente se está volviendo demasiado compleja al buscar soluciones complicadas a problemas que pueden ser simples. Mirar la tapa de tazas me hace pensar en cómo algo tan trivial puede cambiar mi día. Tal vez la clave para resolver este misterio no sea más que encontrar el orden correcto.

Cada pequeño descubrimiento, incluso los más insignificantes, puede ser un paso más hacia la salida. ¡Vamos a ello!
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Una tapa para tazas.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 21

        // Inicializa el vigésimo primer botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 21,
            Activo = false,
            Completado = false,
            RecompensaId = 21,
            MisterioDescripcion = @"Un descubrimiento extraño.

Jingle ha sido el primero en darse cuenta de algo peculiar. Al mirar bajo el mueble de la cocina, algo ha captado su atención. Curiosa, me agacho para ver qué es. Mis ojos se posan en un objeto colorido, verde y blanco, que a simple vista parece navideño, incluso un poco como un duende. Al acercarme, meto la mano y, para mi sorpresa, lo que encuentro no es más que un pato de goma, ¡pero con un disfraz de duende! ¿Qué hacía esto aquí? No me parece sospechoso, simplemente raro.

La extraña visión del pato me hace sonreír, pero me intriga saber cómo ha llegado a este lugar. ¿Hay alguna conexión con el misterio que estoy tratando de resolver? La idea de un pato disfrazado de duende es bastante divertida, pero también me deja pensando.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Pato de goma.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 22

        // Inicializa el vigésimo segundo botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 22,
            Activo = false,
            Completado = false,
            RecompensaId = 22,
            MisterioDescripcion = @"Un misterio que me tiene atrapada.

Hoy me he pasado el día intentando resolver la nota, y de verdad, ¿me está costando tanto? Sin internet, tengo que basarme en recuerdos y en los apuntes que he tomado sobre técnicas de codificación. Me pregunto si hay alguna que sea la indicada para este enigma, o si realmente está todo al azar.

Necesito un poco de café para aclarar mis ideas. Mientras busco en la cocina, algo me llama la atención: ese imán raro que parecía tener unos pies no son lo que parecen. Al acercarme, descubro que son unos cables. Pero lo más inquietante son sus ojos, que brillan latentemente de color rojo. ¿Una cámara? No podía ser.

Sin pensarlo dos veces, decido arrancarlo. Y efectivamente, parece que alguien me está viendo a través de unas cámaras. La sensación es macabra. Después de dar una vuelta a la casa, me doy cuenta de que no es solo uno: ¡estoy rodeada de ellas!

La inquietud se apodera de mí. No puedo esconderme y desconectar todas sería una locura. ¿Alguien se ríe de mí? Es un juego cruel y me siento más atrapada que nunca. Este descubrimiento añade una nueva capa al misterio que me rodea, y ahora me pregunto qué más me están ocultando.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Imán espía.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 23

        // Inicializa el vigésimo tercer botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 23,
            Activo = false,
            Completado = false,
            RecompensaId = 23,
            MisterioDescripcion = @"Un dulce descubrimiento.

Hoy ha sido un día de altos y bajos. Después de haber estallado algunas cámaras con la pelota en un intento de esconder mi privacidad de ellas lo más posible, he decidido que es el momento de tomar unas cuantas gominolas rosas extra para aclarar mis ideas. Munch, munch... ¡son tan deliciosas!

Mientras saboreo el dulce sabor, empiezo a pensar y pensar... ¡Aja! Creo que tengo la solución. Recuerdo que Abel me comentó una vez sobre una codificación llamada César. Es una codificación simple, solo hay que mover tres letras hacia un lado. ¿Cómo se me ha podido escapar algo tan básico?

Con un nuevo aire de determinación, me pongo a trabajar en el mensaje. Ahora tiene sentido. Lo que decía la nota es:

“La llave oscura úsala en el lugar donde el silencio es más profundo.”

Ahora que tengo esta información, estoy más cerca que nunca de desentrañar el misterio que me rodea. ¡No puedo esperar para ver qué sorpresas me esperan!
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Unas gominolas rosas.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 24

        // Inicializa el vigésimo cuarto botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 24,
            Activo = false,
            Completado = false,
            RecompensaId = 24, // Se debe corregir el ID para que sea 24
            MisterioDescripcion = @"Nochebuena en el encierro.

Nochebuena en el encierro

Ríete, sí, te lo mereces. Es Nochebuena y aquí estoy, encerrada, sin más pistas que me ayuden a salir. ¡Tonta que soy! Imbécil.

Buffff, qué rabia. La frustración empieza a apoderarse de mí, y la desesperación se asoma cada vez más. Me va a dar algo. No estoy preparada para lo que pueda haber ahí.

Hoy he estado dándole vueltas a la nota que dice: “La llave oscura, úsala en el lugar donde el silencio es más profundo.” Pero, ¿qué significa en verdad? He pasado todo el día pensando, dándole vueltas a cada palabra, hasta que, de repente, la revelación me golpea. ¡El armario! Claro, ese es el lugar donde el silencio es más profundo. Cuando cierro las puertas, el mundo exterior desaparece y me encuentro en un vacío donde solo el eco de mis pensamientos resuena.

Con cada minuto que pasa, la idea de enfrentarme a lo desconocido se vuelve más abrumadora. ¿Por qué no puedo simplemente estar en casa, con mi familia, celebrando como todos los demás? En lugar de eso, estoy aquí, rodeada de polvo y misterio, con el estómago revuelto y la mente llena de preguntas sin respuesta.

Voy a por más gominolas. Quizás eso me ayude a calmar los nervios. No puedo permitir que esto me consuma. Necesito encontrar una manera de seguir adelante, pero a medida que miro hacia el armario, la ansiedad se mezcla con un resquicio de esperanza. Tal vez haya algo allí que me ayude a resolver este enigma. O tal vez solo me encontraré con más confusión. ¡No sé qué pensar!

Con una gominola en la mano y el corazón acelerado, me acerco al armario, lista para descubrir lo que el silencio profundo tiene reservado para mí esta Nochebuena.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Unas gominolas con forma de oso.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 25

        // Inicializa el vigésimo quinto botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 25,
            Activo = false,
            Completado = false,
            RecompensaId = 25, // ID correcto para el día 25
            MisterioDescripcion = @"El peluche oculto.

Así que, en medio de esta soledad y desesperación, decidí jugar un poco con Jingle. Es Navidad y me lo estoy perdiendo... Mientras revisaba el armario, algo llamó mi atención. En la oscuridad, había una pequeña puerta secreta pintada de negro, oculta por el vacío del armario. Con un poco de nerviosismo, me acerqué a ella. El eco resonaba dentro, y aunque aún no había metido nada en el armario, podía sentir que algo esperaba ser descubierto.

Con la llave negra en la mano, la encajé en la cerradura. Para mi sorpresa, la puerta se abrió con un suave clic, revelando un extraño peluche con forma de pingüino. Era adorable, con grandes ojos brillantes y una pequeña sonrisa, pero lo que realmente captó mi atención fue el cartel que sostenía entre sus patas. En letras grandes y enigmáticas decía:

“4 llaves hay, cuando las tengas ven a buscarme, ya estarás preparada.”

La idea de enfrentarte a alguien peligroso me aterraba, pero, curiosamente, había sentido que esta presencia desconocida me había cuidado bien durante estos días. Tenía comida, agua y, aunque podía haberme atacado mientras dormía, nunca lo había hecho. ¿Qué estaba sucediendo?

Mientras sostenía el peluche en mis manos, noté que pesaba más de lo que debería. Con cuidado, decidí descoser un poco la costura. No quería dañar a mi nuevo amigo. Al abrirlo, ¡sorpresa! Dentro había una llave más, ¡y era dorada! Esta era más grande que el resto y mantenía el mismo detalle del muñeco de jengibre.

Mientras me asomo a pensar en el significado de esto, no puedo evitar sentir una mezcla de incredulidad y frustración. No sé quién está organizando este juego macabro, pero no tiene nada de gracia. ¿Qué se supone que debo hacer con todas estas llaves? Cada vez que descubro algo nuevo, este juego se siente más como una trampa, y me pregunto qué me espera al final de este enigmático laberinto.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Una llave grande de tono dorado y un peluche con forma de pingüino",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 26

        // Inicializa el vigésimo sexto botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 26,
            Activo = false,
            Completado = false,
            RecompensaId = 26, // ID correcto para el día 26
            MisterioDescripcion = @"Ansias por salir.

No puedo más, deseo salir... Eso es todo lo que puedo decir. He logrado reunir cuatro llaves, y aunque debería sentirme emocionada, la realidad es que no tengo ni idea de qué hacer con ellas. Cada día se siente como una eternidad, y estoy empezando a sentir que esta situación me está consumiendo.

La frustración se acumula dentro de mí, como una olla a presión lista para estallar. He pensado varias veces en la locura de intentar destruir todo lo que me rodea: esas llaves que parecen burlarse de mí, el peluche que ha sido mi único compañero y hasta el armario que me ha mantenido atrapada. La idea de romperlos me brinda, extrañamente, un pequeño alivio. Pero también sé que eso no solucionará nada; solo sería una explosión de rabia sin sentido.

Así que aquí estoy, sentada con un puñado de gominolas, intentando calmar mi mente. La dulzura es un respiro momentáneo en este mar de desesperación, pero no es suficiente para ahogar este creciente deseo de libertad. Cada vez que miro las llaves, un nudo se forma en mi estómago. ¿Qué significan realmente? ¿A dónde me llevarán? Todo este juego se ha vuelto un laberinto sin salida, y no sé cuánto tiempo más podré soportar esta incertidumbre.

Las horas se alargan, y el silencio se siente como una broma cruel. La noche anterior, me desperté de un sueño inquietante en el que corría sin rumbo, buscando una puerta que nunca llegaba a encontrar. Esa imagen se ha grabado en mi mente, y ahora es un recordatorio constante de que debo encontrar una salida. No quiero que esta experiencia me defina; necesito recuperar el control.

Es hora de tomar una decisión. Debo investigar esas llaves, un intento desesperado por encontrar sentido a este caos. Tal vez, solo tal vez, la respuesta que tanto anhelo esté más cerca de lo que pienso. Mañana, me prometo a mí misma, tomaré el primer paso. Ya no puedo quedarme aquí sentada, atrapada en esta espiral de ansiedad y frustración. ¡Necesito una salida!
       "
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Unas gominolas rosas.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 27

        // Inicializa el vigésimo séptimo botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 27,
            Activo = false,
            Completado = false,
            RecompensaId = 27, // ID correcto para el día 27
            MisterioDescripcion = @"Un día más, un día menos.

Me quedan pocas gominolas en mi mochila... No puedo más. Estoy al borde de la desesperación. He perdido la Navidad y, a este paso, me perderé también el Año Nuevo. La idea de pasar otra celebración atrapada en esta habitación me resulta insoportable.

Así que he tomado una decisión drástica. Al observar el somier de la cama, la frustración se ha convertido en acción. Sin pensarlo dos veces, arranqué uno de los barrotes de metal. ¿Por qué no lo hice antes? Este barrote será mi nuevo aliado, mi herramienta para forzar una salida. Planeo transformarlo en una especie de maza improvisada, algo que me permita romper cualquier cosa que me impida avanzar.

Es un plan descabellado, lo sé. Pero, sinceramente, es mejor que quedarme sentada aquí, sintiéndome cada vez más atrapada en esta pesadilla. Cada momento que pasa, la ansiedad crece, y con ella, una oleada de incertidumbre. ¿Qué pasará cuando salga? ¿Seguiré sintiéndome así, perdida y desorientada?

Con cada golpe que le doy al barrote para darle forma, siento que me empodero un poco más. A medida que me concentro en esta tarea, las gominolas se convierten en un pequeño consuelo, un dulce alivio que me ayuda a resistir un poco más.

La esperanza y la locura se entrelazan en mi mente. Un día más, un día menos, y este encierro no podrá durar para siempre. Esta acción, por más caótica que parezca, me recuerda que tengo el control. Es hora de prepararme para lo que venga. Y aunque el futuro sea incierto, estoy decidida a dar el primer paso hacia la libertad.
        "
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Unas gominolas rosas.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 28

        // Inicializa el vigésimo octavo botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 28,
            Activo = false,
            Completado = false,
            RecompensaId = 28, // ID correcto para el día 28
            MisterioDescripcion = @"Al ritmo del Rock.

Que esta mañana suene Master of Puppets en la radio es lo mejor que me podía pasar hoy. La energía de esa canción me ha invadido por completo, y no he podido resistirme: me he zampado otra bolsa de gominolas y he cogido mi maza improvisada. Hoy es el día de la destrucción.

He comenzado a reventar todo. Paredes, suelos, columnas... ¡TODO! No hay nada que me detenga en mi búsqueda de pistas. Cada golpe de la maza es un grito de desesperación y determinación, como si estuviera reclamando mi libertad a gritos. El ritmo de la música se mezcla con el eco de los impactos, creando una sinfonía caótica que resuena en cada rincón.

Con cada trozo de pared que se desploma, siento que recupero un poco de control. La adrenalina recorre mis venas, y la locura se convierte en mi aliada. ¿Quién necesita la calma en un momento como este? Estoy aquí, desafiando a la desesperación y a la locura, empujando los límites de mi propia resistencia.

Y mientras desato este caos, no puedo evitar sonreír. La música, las gominolas y el ruido de la destrucción son una mezcla explosiva que me impulsa hacia adelante. Siento que estoy luchando no solo por mi libertad, sino también por mi sanidad.

P.D.: Jingle está a salvo. Al menos él no tiene que lidiar con este caos. Suerte que tiene un corazón de acero, porque yo, sinceramente, me estoy volviendo loca. ¡Vamos, que nada me detenga!
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Unas gominolas con forma de oso.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 29

        // Inicializa el vigésimo noveno botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 29,
            Activo = false,
            Completado = false,
            RecompensaId = 29, // ID correcto para el día 29
            MisterioDescripcion = @"Creo que tengo una pista.

La mañana ha comenzado con la resaca del caos de ayer. Mirando a mi alrededor, no puedo evitar sentirme abrumada por la destrucción que he causado. El comedor se asemeja a un campo de batalla, con escombros y trozos de pared esparcidos por todas partes. No sé si debo sentirme aliviada o culpable por lo que he hecho.

Mientras evalúo el desastre, algo llama mi atención: una pequeña rendija en la pared que antes había pasado desapercibida. La curiosidad se apodera de mí, y me acerco para inspeccionarla más de cerca. Con un empujón firme, la sección de la pared se desplaza, revelando una puerta oculta que lleva a unas escaleras.

La emoción se mezcla con la ansiedad; esto podría ser lo que he estado buscando. La oscuridad que emana de las escaleras promete una nueva aventura, un paso más hacia la libertad.

Con cuidado, empiezo a despejar el camino, retirando escombros y trozos de madera. Cada paso me acerca más a la verdad. Con un último esfuerzo, empujo la puerta y me asomo a las escaleras que se hunden en la penumbra.

Desciendo con cautela, cada peldaño cruje bajo mi peso. Cuando llego al final, me encuentro ante una puerta con cuatro cerraduras. Mis pulsaciones aumentan. ¿Qué se esconde detrás de ella? ¿Serán las respuestas que tanto busco?

Contemplo la puerta, sintiendo la mezcla de desesperación y emoción. Hoy ha sido un día largo, y este descubrimiento se siente como un hito. Sin embargo, no puedo abrirla, al menos no todavía. La ansiedad burbujea en mi interior, pero sé que estoy más cerca que nunca de descubrir lo que hay detrás de esta puerta.

Siento un poco de culpa por el propietario del chalet; debe estar horrorizado al ver lo que he hecho y las reparaciones que le tocará pagar. Pero, sinceramente, la violencia de ayer me ha llevado a un paso más cerca de la libertad.

Mañana será otro día, y estoy decidida a desentrañar el misterio que se esconde tras esas cuatro cerraduras.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Unas gominolas rosas.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 30

        // Inicializa el trigésimo botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 30,
            Activo = false,
            Completado = false,
            RecompensaId = 30, // ID correcto para el día 30
            MisterioDescripcion = @"La puerta a lo desconocido.

Después de una larga noche de pensamientos agitados, me despierto decidida. Hoy es el día en que tengo que abrir esa puerta. Detrás de ella podría haber horror o, tal vez, una salida. No lo sabré hasta que me atreva a dar el paso.

Para darme un último empujón, me acerco a mi mochila y saco la última piruleta. La sostengo entre mis manos, su color brillante me da un pequeño consuelo en medio de esta incertidumbre. La como lentamente, saboreando cada instante, con la esperanza de que no necesite más dulces para enfrentar lo que está por venir.

Al acercarme a la puerta, mi respiración se hace más rápida. La observación me deja sin aliento. Es una sala grande, su apariencia es metálica, pero el sonido que emite no es el de metal, sino más bien un suave eco como si fuera madera, quizás con un barniz especial que oculta su verdadera naturaleza.

Las cuatro cerraduras brillan, perfectamente alineadas, encajando con el color y la forma de las llaves que he ido encontrando. Cada una parece contar una historia, un pedazo de este enigma que he estado resolviendo. La pintura de la puerta está dividida en dos mitades: una blanca, otra negra. La imagen que se forma es la de un ángel y un demonio, un curioso detalle que me recuerda a un cierto artista...

Siento una mezcla de adrenalina y miedo mientras introduzco la primera llave. La cerradura gira, pero no se abre. Así que continúo con la segunda, luego la tercera, y finalmente, la cuarta. Todo mi esfuerzo me lleva a un punto en el que la puerta comienza a ceder, pero aún así, me cuesta mucho abrirla.

Con un último empujón, la puerta se abre de par en par. Lo que veo al otro lado me deja paralizada: “¡Abel! ¿Qué haces tú aquí?” La incredulidad me inunda mientras un torrente de emociones me invade al reconocer a la persona que menos esperaba encontrar en este lugar.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "La sagrada piruleta.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

        #region Dia 31

        // Inicializa el trigésimo primer botón y recompensa
        nuevoBoton = new BotonModel
        {
            Numero = 31,
            Activo = false,
            Completado = false,
            RecompensaId = 31, // ID correcto para el día 31
            MisterioDescripcion = @"El sobre rojo.

Al abrir la puerta del cobertizo, me encuentro con Abel, mi novio. La incredulidad me inunda mientras mis emociones se agolpan. Todo esto fue una broma. No era una caja, ni Mateo, ni David... Abel había planeado una serie de acertijos y desafíos con un regalo original en mente, con la esperanza de que pudiera resolver el misterio antes de que llegara el día de Navidad.

La sala en la que me encuentro es grande y luminosa, iluminada con una luz suave que contrasta con la oscuridad de los días anteriores. Las paredes están adornadas con pantallas que muestran cada una de las cámaras que me han estado observando. En cada una, puedo ver fragmentos de la destrucción que he causado en las plantas superiores, un recordatorio visual de mi lucha por salir.

Y ahí, en un rincón, está Jingle, el pingüino de peluche que había encontrado en el armario. Él también parece estar en paz, como si supiera que su papel en este juego había sido vital. Abel se agacha y acaricia a Jingle, sonriendo. “Él ha estado contigo todo el tiempo, ayudándote a mantener el espíritu. Jingle también es parte de esta aventura.”

Con una sonrisa traviesa, Abel dice: “Lo siento mucho por hacerte perder tus vacaciones, pero quería enseñarte cuánto vales y lo importante que es valorarse a uno mismo. Nunca imaginé que te llevaría tan lejos, pero sé que todo esto, aunque caótico, tiene un propósito. Aprender a disfrutar del camino, a veces complicado, pero lleno de sorpresas.”

En su mano, sostiene un pequeño paquete envuelto con cuidado. Lo abro con ansiedad y dentro hay un vale de viaje. Mis ojos brillan al leer las opciones: “He planeado un viaje para nosotros, con varias opciones de destino. Podemos ir a donde siempre has querido: una escapada a la playa, una montaña llena de aventura, o una ciudad que nunca duerme. La decisión es tuya.”

“Este es mi regalo para ti,” dice, con una mirada llena de cariño. “Espero que disfrutes la sorpresa y que esta aventura nos haga olvidar el caos de estos días. ¡Feliz Navidad!”

Me siento abrumada de alegría. Aunque todo lo vivido ha sido una montaña rusa de emociones, he aprendido a valorar más que nunca lo que tengo: a mí misma, a Abel y a la vida. Con una sonrisa radiante, lo abrazo, sintiendo que este es el inicio de una nueva aventura. Jingle, en su suave pelaje, también parece compartir mi alegría, como un fiel compañero que ha estado a mi lado en esta odisea.
       "
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "El viaje de nuestros sueños.",
            Desbloqueada = false
        };

        // Guardar el nuevo botón y la recompensa
        await SaveBoton(nuevoBoton);
        await SaveRecompensa(nuevaRecompensa);

        #endregion

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
                if ((fechaActual.Day >= boton.Numero && fechaActual.Month >= 9) || fechaActual.Year > 2024)
                {
                    boton.Activo = true;
                    botonControl.IsEnabled = true; // Activa el botón en la interfaz

                    // Si es el mismo día, independientemente de si está completado o no, borde dorado
                    if (fechaActual.Day == boton.Numero && fechaActual.Month >= 9 && fechaActual.Year == 2024)
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