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
            MisterioDescripcion = @"Llegada al chalet

Hoy me he asegurado de llevar de todo en mi mochila. Quiero descubrir qué hay detrás de esa misteriosa tarjeta sin que me tome demasiado tiempo. He cogido mi teléfono, unas cuantas golosinas (el azúcar me ayuda a pensar), un cargador, mi cartera, mi libreta y una pequeña navaja multiusos.

Tal como indicaba la tarjeta de ayer, parece que acabo de llegar a la dirección correcta: número 12 (un chalet enumerado en mitad de la nada; curioso, cuanto menos) y una puerta de madera robusta. El chalet es más grande de lo que imaginaba, con paredes de madera oscura que crujen con el viento, pero al menos está decorado con un estilo navideño que lo hace ver menos tenebroso.
Nada más llegar, he mirado por las ventanas, pero no parecía verse el interior; todo está envuelto en sombras.

Es entonces cuando, después de aporrear cuidadosamente la puerta, esta se ha abierto. Entro y... ¡¡PUM!! Con un portazo detrás de mí, la puerta se ha cerrado sin manera de poder abrirla. ¿¿¿Qué puedo hacer???
Durante un instante, los nervios me comen por dentro, pero es entonces cuando me digo: “Ray, basta ya, ¡tranquila!”. He cerrado los ojos y he contado 1, 2 y 3. Al abrirlos, me siento más centrada, así que busco en mi bolsillo una piruleta que tanto me ayuda a pensar y me la llevo a la boca.

A oscuras, busco el interruptor de la luz y, al dar con él, la habitación se ilumina tenuemente. Me encuentro en una entrada polvorienta, con muebles cubiertos por sábanas blancas que parecen fantasmas esperando a ser descubiertos.
Las ventanas también están tapiadas y un olor a humedad y madera vieja llena el aire.

Un escalofrío me recorre la espalda al darme cuenta de que no hay señales de vida. Me esperan unas largas horas por delante... y una creciente sensación de que hay más en este chalet de lo que parece a simple vista. ¿Qué secretos guarda este lugar?
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
            MisterioDescripcion = @"Malos días

Creo que no hay peor manera de pasar una noche... en un chalet misterioso lleno de polvo, mucho frío, y joder... esos muebles dan miedo. Siento escalofríos solo al verlos. 
Se nota que lleva tiempo sin pasar nadie por aquí: la calefacción no funciona y hace falta una limpieza profunda. 

Lo primero que he hecho ha sido quitar todas las sábanas blancas de los muebles en busca de algo que me pueda ayudar a solucionar este misterio o abrir la puerta, pero mis primeras búsquedas no han tenido éxito. 
Lo único que he encontrado es una especie de calentador de manos; quizás me ayude a no morir de frío.

“Mateo”, solo se me venía ese nombre a la cabeza. ¿Será cosa suya? Imposible, hace años que le perdí la pista. ¿¿Entonces quién?? 
Ayer dije que me esperaban horas largas, pero al ritmo en el que voy no voy a ver las vacaciones de Navidad este año...

P.D.: Misteriosamente hay comida en la despensa y en la nevera. Creo que tendré suficiente para unos días, pero... ¿quién la dejó aquí?
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
            MisterioDescripcion = @"Me comienzo a estresar

Creo que me estoy volviendo loca. Nada tiene sentido hoy.

He encontrado algo debajo de la “cama” donde estoy durmiendo, si se le puede llamar asi. Es mas bien un pequeño colchon sucio encima de un somier de hierro.
Lo que he encontrado a sido una pequeña pelota, de color rojo brillante. 

La he hecho rebotar varias veces contra la pared porque eso a veces me ayuda a pensar, pero no tengo suerte. Hoy estoy en blanco. 

¿Tercer día y ya estoy así? Espero que sea un bajón puntual. 
Quizás esta pelota me ayude a encontrar algo más, porque por ahora, solo siento que estoy atrapada en un laberinto sin salida...

He aprovechado para investigar un poco este espacio. He dibujado un pequeño plano que quizás me ayude en un futuro.

En el chalet, un lugar acogedor de dos plantas, he explorado el amplio comedor conectado directamente a la cocina.
La chimenea que adorna el comedor, aunque sin salida, le da un toque cálido al ambiente.
En la segunda planta, se encuentran los dormitorios y el baño, y desde el pasillo ubicado en la parte superior, puedo ver el comedor, recordándome que estoy en un lugar con más secretos de los que imagino.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Hoy no he encontrado nada interesante; mis bolsillos están vacíos por ahora.",
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
            MisterioDescripcion = @"Las cosas mejoran (Poco a poco, o al menos eso creo)

¡Hoy estoy a tope! He encontrado una pequeña radio en uno de los cajones de la cocina, una vieja y sucia pero extrañamente, ¡funciona! Parece que aún recibe algunas ondas a través de la antena. 

La música me ha motivado tanto que pensé que tendría la fuerza suficiente para quitar la dura madera que tapa las ventanas... pero va a ser que no. Supongo que hace falta algo más que Queen para romper una ventana.

Sin embargo nada me desmotiva hoy. Dormí mucho mejor anoche gracias a un antifaz improvisado, hecho para dormir sin temer al exterior, ¡me ayuda a calmar los nervios! 

A ver qué tal mañana... ya llevaré cinco días aquí, sin pistas aún ni una manera clara de salir.
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
            MisterioDescripcion = @"Venganza

He descartado a Mateo de mi lista negra. No creo que esto sea cosa suya; su motivación eran las estafas informáticas. Montar algo así es mucho más retorcido… incluso perverso. 

¡Estamos a 20 días de Navidad! Solo alguien malvado haría algo así. Me llevo la piruleta a la boca de nuevo, pensando qué está sucediendo aquí.

Fue entonces cuando caí en la cuenta: ¡una maldita venganza! ¿Podría todo ser cosa de David Hernández? Su caso fue un secuestro hace cinco años, tan bien preparado que debe ser la envidia entre los criminales. Disfraces, falsificación de documentos, trampillas secretas… secretos y más secretos. Si no hubiera sido por mí, aquello habría terminado peor, y quizás por eso quiere vengarse.

Pero… no estoy segura, y no puedo aferrarme a algo sin pruebas. ¿Hacia dónde me llevará este misterio?
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
            MisterioDescripcion = @"Una caja demasiado fuerte

Bien, bien y bien. La cosa no podría ir mejor.

He estado botando la pelota todos estos días y, en una de esas, un parquet del suelo se ha levantado. Parece que no sabes esconder cosas, ¿eh? ¡Ja! Seas quien seas, esto es un punto para Ray.
¿¿El código de la caja fuerte?? Mmmm... no he tenido tiempo de descubrirlo, así que le he dado golpes contra todo: pared, suelo, muebles… incluso a puño limpio.

Se puede decir que el esfuerzo tiene su recompensa. Es un paso adelante. Dentro he encontrado un anillo con unos brillantes de tono verde. No sé si es esmeralda, peridoto, jade u olivina, pero desde luego es precioso. Ojalá alguien me hubiera pedido matrimonio con algo así... echo de menos a mi pareja. :(

Bueno, bueno, que si me pongo a pensar, me dejo lo más importante. Aparte del anillo, había una preciosa llave verde con un muñeco de jengibre en el mango, con una punta demasiado plana y alargada.

Sinceramente, nunca había visto una igual; era bastante pesada y reflejaba toda la luz. Era casi tan preciosa como el anillo. No sé dónde puede encajar, pero de seguro que ¡¡voy a descubrirlo pronto!!
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Una pesada llave alargada verde y un anillo con joya verde",
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
            MisterioDescripcion = @"Un gran chasco y un gran melón

He probado la llave en todas las puertas y en todos los cajones que tienen cerradura, ¡y nada!

Parece ser que esta llave sirve para algo más que no estoy viendo. Algo se me escapa. Es cierto que una llave de este tamaño, color y diseño no se ve todos los días.
Estoy intentando ver más allá, pensar a lo grande. Me hago preguntas sobre este lugar y todo este misterio, pero no encuentro la respuesta.

Por ejemplo, el misterio de la comida “infinita”. ¿Qué sucede aquí? No lo sé...

De todas maneras, no se puede pensar con el estómago vacío, así que voy a comerme ese melón que lleva días mirándome; ya que está ahí, habrá que hacer algo con él.

P.D.: Parece que mi humor va mejorando cada día, aunque no debería acostumbrarme.
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

Llevo horas dándole vueltas a todo; no solo a lo que ocurre en este extraño chalet, sino también a mis propias cosas, a mi vida y a quién soy.

De alguna manera, este lugar me ha obligado a estar más tranquila, casi sin quererlo. Mañana cumplo 30 años, y la verdad es que añoro aquella época en la que tenía 20. Todo era más sencillo y no me daba cuenta. Valorar las cosas en su momento es un arte que casi nadie domina, y quizás ahora mismo, en medio de este misterio, estoy empezando a aprenderlo.

Es curioso cómo han cambiado mis prioridades, aunque también es inevitable sentir nostalgia por las cosas pequeñas. Aquella chica de 20 años, siempre apurada y llena de energía, era tan distinta. Yo, ahora, intento quejarme menos, disfrutar más y darme un respiro cuando las cosas no salen como espero. Pero, a decir verdad, extraño esa chispa, esa espontaneidad que tenía sin pensar demasiado.

Entre esos recuerdos, uno tonto me viene a la mente: mi funda de móvil de entonces, de un rosa suave y bonito. Nunca le presté demasiada atención, pero ahora me doy cuenta de que el rosa siempre ha estado en mi vida, de una manera u otra. Hoy llevo una mochila de ese mismo color, y me pregunto... ¿será que el rosa es mi color favorito y yo sin saberlo?

Quizás sí. Quizás hay detalles de nosotros mismos que llevan años con nosotros y que solo entendemos mucho después. Aunque haya cosas que no puedo cambiar, estoy empezando a apreciar quién soy hoy, con 30 años y todo lo vivido. Eso, en sí mismo, es todo un descubrimiento.
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
            MisterioDescripcion = @"¡¡Un enorme regalo en el comedor!!

Estoy desconcertada. No entiendo nada. ¿¿Por qué?? ¿¿Cómo??

Al despertar, me encuentro un gran paquete en el centro de la sala principal del chalet. Sigo sin comprender cómo ha llegado aquí sin que yo me diera cuenta, y menos en un lugar tan apartado como este. Hoy es mi cumpleaños, y, aunque nunca habría imaginado pasarlo aquí, entre estas paredes y sin compañía, esto supera cualquier sorpresa que pudiera esperar...

Desde el momento en que lo vi, supe que algo extraño ocurría. Una etiqueta colgaba del paquete, y en ella estaba escrito mi nombre. Eso significa que quien haya dejado este “regalo” sabe algo sobre mí… quizás más de lo que quisiera admitir. Y luego, para colmo de rarezas, el regalo no dejaba de moverse ligeramente, como si algo o alguien en su interior intentara liberarse.

Decidí abrirlo,entre la curiosidad y la cautela, y lo primero que encontré fue algo inesperado: una llave de color rosa chicle, ¡super cuqui,
cabe decir! Al igual que la primera llave verde que encontré, esta también tiene en el mango un muñeco de jengibre,
solo que esta vez la punta de la llave parece formar una letra o un símbolo extraño,
como si estuviera destinada a usarse junto con la otra.Quizás juntas puedan abrir algo importante en este lugar.Así que, de momento,
las guardaré en mi mochila hasta que descubra cómo funcionan.

Pero eso no es lo más sorprendente… Dentro del paquete había ¡un perro! Un cachorro de mirada brillante, juguetón y sin ningún miedo en absoluto.Aunque me pilló por sorpresa(¿qué se supone que voy a hacer con un perro en este lugar ?),
me siento aliviada de tener algo de compañía.La idea de compartir este misterio con alguien más,
aunque sea un cachorro,
hace que este sitio se sienta menos solitario y tenebroso.

Sin embargo, la inquietud sigue ahí.Quien sea que haya preparado esto para mí, no solo sabe que hoy es mi cumpleaños,
sino que tiene acceso directo a este chalet y me vigila de cerca.La sensación es escalofriante,
aunque también estoy más decidida que nunca a desentrañar qué secretos esconde este lugar y qué papel juego yo en todo esto.

Por ahora, sumo esta llave misteriosa a mi colección y, con mi nuevo compañero a mi lado, quizás esté un paso más cerca de resolver el rompecabezas que este lugar esconde.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Un pequeño compañero canino y una pesada llave de color rosa.",
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
            MisterioDescripcion = @"Presentación de Jingle

Siempre me ha gustado el nombre de Jingle; tiene un sonido alegre, como campanas tintineantes, y con las fechas tan cercanas a la Navidad, no pude resistirme a llamarlo así. Parecía la opción perfecta para un compañero tan especial.

Jingle es un adorable perro salchicha de pelaje negro con unas manchas de color café claro que le dan un toque encantador y único. Su mirada curiosa y su energía contagiosa han llenado de vida este extraño lugar. No puedo evitar sentirme agradecida por tenerle aquí; en medio de esta soledad y misterio, su presencia hace que todo sea un poco más soportable.

Sin embargo, esta noche he sentido que, aunque tengo a Jingle a mi lado, no estamos completamente solos. Hay una sensación en el aire, una presencia que parece moverse más allá de las paredes que puedo ver. Mientras investigaba cada rincón del chalet, tratando de descubrir más pistas, escuché ruidos apagados que venían desde algún lugar en el suelo, como si hubiera una habitación secreta debajo de nosotros, justo fuera de mi alcance. Eran pasos suaves, resonando apenas, pero lo suficiente como para que la piel se me erizara.

No sé si son imaginaciones mías o si realmente alguien —o algo— está allí abajo, observándonos o esperando. Sea como sea, siento que este lugar esconde algo más profundo, algo que aún no puedo ver. Con Jingle a mi lado, me siento menos vulnerable, pero también más alerta que nunca.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Nuevamente hoy no he encontrado nada interesante.",
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
            MisterioDescripcion = @"Una misteriosa nota

¡¡Me encanta!! ¿Quién diría que Jingle tiene más ambición por salir de aquí que yo? Bueno, para ser justa, es una ambición dirigida a cualquier cosa que huela a basura… pero hoy su curiosidad ha servido para algo.

Mientras me acompañaba en la cocina, Jingle se lanzó directo a la papelera y, cuando pensé que tendría que limpiar el desastre, vi que había sacado un papel arrugado, uno que nunca había visto antes. Lo extraño es que parecía recién arrugado, como si alguien lo hubiera dejado a toda prisa, como si hubiera cambiado sus planes en el último momento. Quien esté vigilándonos parece tener todo perfectamente calculado, y hasta ahora, me tiene en jaque.

Con un nudo en la garganta, leí la nota y descubrí que era algo más que una burla. Al principio, parecía un mensaje inconexo, escrito casi sin sentido, como si cada palabra estuviera colocada al azar:

“Ll3ev2as un d1ía tras oTro pErdida eNtro iEste ‘chalet’. cReo quE al fiN4al ya no podRás vErlo más. PiEnsO qUe3 Cuand0 cReas quE eStás yA poR saLir, volveráS a eMpezar dEsde ciEro. ¡jaja!”

¿Se está riendo de mí? ¿¿De mí?? ¿¡EN SERIO!? ¡Qué desfachatez! ¡Claro que voy a salir, y lo haré resolviendo cada pequeño acertijo! Nadie subestima a Ray y se queda tan tranquilo.

Pero entonces noté algo. Las letras mayúsculas y los números parecían fuera de lugar, como si escondieran otro mensaje. Probé varias combinaciones y técnicas para descifrarlo, pero todavía no consigo que tenga sentido. Puede ser algún tipo de código numérico, o incluso una clave oculta. Solo sé que lleva tiempo y paciencia.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Un poco de basura esparcida por el suelo y una misteriosa nota.",
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
            MisterioDescripcion = @"Una Cajita Misteriosa en la Pared

Hoy me he topado con una caja inesperada. La he encontrado incrustada en la pared, perfectamente disimulada como si fuera un cuadro. En todo este tiempo no había notado su presencia, como si alguien la hubiera colocado ahí a propósito, esperando que pasara desapercibida.

La caja en sí es preciosa, de aspecto antiguo, con una perla roja en forma de corazón en la parte delantera que le da un toque elegante, aunque el óxido y el polvo revelan que lleva demasiado tiempo olvidada. Al intentar abrirla, me di cuenta de que el cierre estaba atascado; debía de estar completamente oxidada por dentro. Me desesperé tanto que, después de varios intentos, la lancé al aire en un arrebato de frustración.

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
            MisterioDescripcion = @"No podia ser de otra manera

Pues ya he perdido la esperanza de nuevo... hoy no tengo motivación para nada. 

La radio no tiene pilas ya. A mí me duele la garganta. Como siga así acabo enferma... que poco me ha durado las buenas vibes.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Un resfriado de la ostia.",
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
            MisterioDescripcion = @"Ya llevo dos semanas aquí!!

¿Qué te voy a contar pequeño diario? Creo que has pasado días mejores... Con los casos que hemos resueltos juntos y ahora me debes aguantar pachucha...

Sobretodo este estilo monocolor en los apuntes... ¡¡solo me he traido el boli negro!!

Jingle parece que se preocupa por como estoy, pobrecito.

Fatal. :(

P.D.: He creado una especie de vela a partir de almendras. No huele mal (al menos lo que yo puedo oler).
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Una vela aromatica.",
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
            MisterioDescripcion = @"La formula

Quizás esta página la arranque del diario, es un poco desastroso y vergonzoso incluso estoy que estoy haciendo, pero tengo receta para un nuevo té:

1. Una pizca de moho que hay al final de los cajones de madera (No huele mal).
2. Algunos hierbajos que he encontrado en una maceta, quiero pensar que son de antiguas especias.
3. Esencia de vainilla, que casualmente quedaba en la cocina.
4. Esperanza, y mucha...

No es el mejor té, pero me encuentro mejor, ya veremos mañana.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Una nueva receta innovadora de té",
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
            MisterioDescripcion = @"¡¡Ya está despertando Ray!!

¡Estoy en una nube! De verdad que me siento mil veces mejor, a pesar de que, honestamente, aún me dan arcadas al recordar algunas de las cosas que he hecho... Pero eso es un pequeño precio a pagar por seguir avanzando.

Lo importante ahora es que tengo un delicioso té caliente junto a unas gominolas que encontré en mi mochila. ¡No está ni tan mal! ¡¡Toma ya, Ray!! Esto es lo que necesitaba para empezar el día con energía.

Con mi mente despejada, he vuelto a reflexionar sobre la nota del otro día. Su contenido me dejó intrigada, y ahora que me siento mejor, creo que debería buscar alguna técnica de codificación que conozca. Hay tantas formas de encriptar un mensaje, desde el cifrado César hasta métodos más complejos, y estoy segura de que he aprendido algo que puede ayudarme a desentrañar lo que dice.

Así que hoy me he propuesto revisar mis apuntes de la libreta. No quiero dejar piedra sin remover en mi búsqueda de pistas. Quizás esa nota tenga un significado oculto que, al entender, me acerque aún más a la verdad. Si puedo descifrar el mensaje, podría revelarme un camino hacia la salida o darme más información sobre este misterioso lugar.

Dibujo un esquema mental de las posibilidades. Una cifra, un símbolo, tal vez un patrón… la clave podría estar en los pequeños detalles. Cada minuto cuenta, así que me pondré manos a la obra. Quiero que Ray se despierte con buenas noticias. ¡Hoy es un nuevo día lleno de posibilidades!
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Gominilas para sanar costipados.",
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

Sin embargo, debo admitir que el catarro sigue aquí conmigo, así que seguiré la receta del doctor (o sea, yo) por un día más. ¡Es increíble cómo los síntomas pueden hacer que incluso los mejores planes se desmoronen! Con lo bien que iba ayer, sintiéndome animada y con la mente despierta, ahora es un reto volver a concentrarme.

Me pregunto si este estado de encierro me ha llevado a ver las cosas de manera diferente. Quizás la falta de estrés exterior ha abierto mi mente a nuevas posibilidades. Necesito recordar que no estoy sola; tengo a Jingle y el misterio de las llaves. Las pequeñas cosas son las que realmente importan, incluso si estoy atrapada aquí. ¡Y quién sabe! Tal vez esta experiencia me brinde la oportunidad de crecer y aprender más sobre mí misma. Así que hoy, en vez de dejarme llevar por la desesperación, intentaré aprovechar al máximo este tiempo. ¡Vamos a seguir adelante!
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Mas gominolas sanadoras...",
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
            MisterioDescripcion = @"¡Vamos a recuperarnos al máximo!

¡Venga, Ray! Hoy nos despedimos del resfriado, ¡ya verás! Solo necesito preparar una última receta de ese té, y con un poco de suerte, estarás activa como siempre.

¡Tú puedes! ¡¡¡Ánimo!!! La energía positiva es clave, y con un poco de optimismo, estoy segura de que superaré esto en un abrir y cerrar de ojos.

Además, he hecho un descubrimiento que me ha sacado una sonrisa: ¡he encontrado unas pilas! Ja, ja. Eso significa que podré disfrutar de música a todo volumen, siempre y cuando funcionen, claro está.

La música siempre ha sido un buen antídoto para el ánimo. Así que, entre el té y mis melodías favoritas, ¡no hay resfriado que me detenga! Hoy es un nuevo día, y estoy decidida a aprovecharlo al máximo. ¡Vamos a darle un buen empujón a esta recuperación!
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Ultimas gominolas sanadoras.",
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
            MisterioDescripcion = @"Mi taza favorita

19 días aquí dentro y, sorprendentemente, me acabo de dar cuenta de esta taza tan rechulona. ¿¿De verdad estoy hecha para buscar cosas?? ¡Es increíble lo que uno puede pasar por alto cuando está atrapado en sus pensamientos!

De todas maneras, a partir de hoy voy a disfrutar de los desayunos como nunca. Esta taza se merece ser parte de mis rituales matutinos.

Mientras estaba en la tarea de resolver el acertijo de la nota, algo comenzó a encajar. Tras días releyendo y probando distintas técnicas, al enfocar solo las letras mayúsculas y juntar los números, finalmente logré descifrar un mensaje que decía algo como:

“Busca en lo más alto del armario, ese es el lugar por donde empezar. La primera llave para encontrar el mayor de los tesoros. Esto no es un crimen, es un misterio que te propone alguien conocido.”

He logrado entender el mensaje y eso es un gran avance. No sé qué tesoro pueda haber detrás de todo esto, pero no pienso detenerme hasta descubrirlo.

Gracias, Jingle, por darme el primer paso hacia algo grande. Tu curiosidad me ha inspirado a seguir adelante y no rendirme. ¡El misterio está en marcha!
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Una taza rechulona.",
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
            MisterioDescripcion = @"El complemento perfecto

He encontrado la manera de mantener el calor gracias a algo muy sencillo: una tapa para tazas. Es curioso cómo algo tan simple puede hacer una gran diferencia. Con esto, el calor de la taza no se va, lo cual significa que si yo me tapo con mantas, mi calor tampoco se escapa.

Un poco boba sí soy, la verdad. Ya podría haber caído en esto antes, pero es que aquí no hay lavadora y taparme da un poco de cosa. Aunque, sinceramente, cuando has pasado 20 días entre polvo, ya te acostumbras a las incomodidades. A veces es necesario encontrar soluciones ingeniosas en situaciones inesperadas, ¿no?

Ahora que tengo este tapa tazas, me siento un poco más cómoda. Quizás sea un buen momento para ir a mirar al armario y ver qué más sorpresas me esperan. Cada pequeño descubrimiento me acerca un paso más a resolver este misterio y salir de aquí. ¡Vamos a ello!
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
            MisterioDescripcion = @"¿Qué es eso que cuelga?

Jingle ha sido el primero en darse cuenta, y yo después. Hay algo colgando del techo del armario... ¿podría ser un tipo de pista? La curiosidad me pica, y aunque no puedo verlo claramente, siento que es algo importante.

De todas maneras, no llego. La distancia es demasiado y mi estiramiento no es suficiente. Voy a intentar tirar cosas hasta lograr que caiga. A lo mejor me lleva toda la tarde, pero no pasa nada, hoy tengo ganas de atacar a las gominolas que me quedan en la mochila.

Cada pequeño momento cuenta, y si consigo sacar algo del techo del armario, puede ser la clave para resolver un nuevo misterio. ¡Vamos a ver qué pasa! Con un poco de suerte, este será el descubrimiento que me acerque un paso más a salir de aquí. ¡A la carga! así que hay tiempo de sobras.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Una vez más, más gominolas a las que atacar.",
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
            MisterioDescripcion = @"No era un duende...

Al principio pensé que lo que asomaba del techo del armario era un duende. La verdad, creo que tantos días encerrada aquí me están pasando factura; me distraigo con facilidad. Pero al acercarme, me di cuenta de que era un muñeco raro con forma de duende. Era extraño y feo, pero a la vez gracioso, como sacado de un cuento infantil.

Ahora bien, esa figurita colgante no sirve para nada. Lo que realmente me intriga es el armario en sí. No puedo creer que solo haya un adorno allí arriba. Debe de haber algo más que me ayude a resolver este misterio y descubrir el secreto que se oculta en estas cuatro paredes.

Mi curiosidad se enciende. ¿Qué habrá detrás de esa puerta? ¿Un compartimento secreto? ¿Algún objeto olvidado que me dará pistas sobre cómo salir de aquí? Siento que hay algo esperando a ser descubierto, y no pienso rendirme hasta encontrarlo.

Con un poco de suerte me lanzaré a la tarea de investigar el armario en busca de la respuesta. ¡Es hora de desentrañar este misterio!
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Una figura con aspecto de duende.",
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
            MisterioDescripcion = @"El misterio del imán espía

Hoy es el día en que me pongo las pilas. Mi familia me estará esperando para Navidad, y no puedo quedarme aquí un día más. Así que decidí empezar el día con energía, con la determinación de salir de este encierro.

Primero, revisé el armario donde encontré un peluche escondido en la parte superior. Era un poco polvoriento, pero al cogerlo no noté nada raro. Sin embargo, al mirarlo de nuevo, me di cuenta de que su mirada parecía contener un secreto. Tal vez fue solo mi imaginación, pero sentí que había más detrás de esa tierna fachada.

Luego, mi atención se desvió hacia la cocina. Fui a la nevera y, de repente, un imán raro capturó mi mirada. ¿¿Tiene pies?? Era una figura extraña, casi ridícula, que parecía tener una pequeña cámara incrustada. En ese momento, mi corazón se detuvo por un instante. ¡Qué mierdas! Me están espiando.

Sin pensarlo dos veces, quité el imán de la nevera. El resto del día lo pasé con la mente llena de preguntas. ¿Quién está detrás de esto? ¿Por qué me están observando? Mi curiosidad se ha despertado una vez más, y estoy decidida a descubrir qué se esconde tras este nuevo misterio. No puedo dejar que esto me frene. Hay algo más grande en juego, y estoy decidida a desentrañarlo.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Un misterioso imán Espía.",
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
            MisterioDescripcion = @"Nochebuena en el encierro

Ríete, sí, te lo mereces. Es Nochebuena y aquí estoy, encerrada, sin más pistas que me ayuden a salir. ¡Tonta que soy! Imbécil.

Buffff,qué rabia.La frustración empieza a apoderarse de mí, y la desesperación se asoma cada vez más.Me va a dar algo.No estoy preparada para lo que pueda haber ahí.

Con cada minuto que pasa,la idea de enfrentarme a lo desconocido se vuelve más abrumadora. ¿Por qué no puedo simplemente estar en casa,
con mi familia, celebrando como todos los demás? En lugar de eso, estoy aquí, rodeada de polvo y misterio, con el estómago revuelto y la mente llena de preguntas sin respuesta.

Me voy a por más gominolas.Quizás eso me ayude a calmar los nervios.No puedo permitir que esto me consuma.Necesito encontrar una manera de seguir adelante,
pero a medida que miro hacia el armario, la ansiedad se mezcla con un resquicio de esperanza.Tal vez haya algo allí que me ayude a resolver este enigma.O tal vez solo me encontraré con más confusión. ¡No sé qué pensar!
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Un poco de gominolas.",
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
            MisterioDescripcion = @"El peluche oculto

Así que, en medio de esta soledad y desesperación, decidí jugar un poco con Jingle. Es Navidad y me lo estoy perdiendo... Había encontrado un peluche con forma de pingüino oculto en la parte de arriba del armario. Al lanzarlo para ver si podía hacerlo rebotar, me di cuenta de que no rebotaba ni un mínimo. Este peluche pesaba bastante, lo que me pareció extraño.

Con cuidado, fui descosiendo un poco la costura del peluche. Me parece muy mono y no quiero dañarlo. Y ole él... Al abrirlo, ¡dentro había una llave más! Era del mismo estilo que las otras, con su famoso muñeco de jengibre, pero esta era mucho más gorda y dorada.

Es curioso. El pingüino sostenía un cartel que indicaba la ubicación de dos de las otras llaves... ¿¿En serio?? Me estás diciendo que desde el primer día podría haber encontrado estas tres llaves, y el día de mi cumpleaños, otra más...

Me asomo a pensar en el significado de esto. No sé quién está organizando esto, pero o le falta imaginación o lo ha organizado mal. Pero, sinceramente, ¡0 gracia! ¿Qué se supone que debo hacer con todas estas llaves? Es una broma de mal gusto, y mientras más descubro, más frustrante se vuelve. Este juego se está tornando en un verdadero enigma, y cada vez se siente más como una trampa.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Una llave de tono dorado grande y un peluche con forma de pingüino",
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
            MisterioDescripcion = @"Ansias por salir

No puedo más, deseo salir... Solo diré eso. He conseguido reunir cuatro llaves, y aunque debería sentirme emocionada por ello, en realidad no tengo idea de qué hacer con ellas. Me estoy volviendo loca.

La frustración se acumula, y ya he pensado varias veces en intentar destruir todo. No es una opción muy racional, lo sé, pero la idea de romper esas llaves, ese peluche y hasta el armario que me ha mantenido atrapada, de alguna manera me alivia un poco.

Sin embargo, ahí estoy, tratando de calmar mi mente mientras como gominolas. La dulzura temporal me ofrece un respiro momentáneo, pero no es suficiente para ahogar este deseo de libertad. ¿Qué significan estas llaves? ¿A dónde me llevarán? Si al menos pudiera tener una pista más clara... ¡No sé cuánto tiempo más podré soportar esto!
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "¿MAS GOMINOLAS?",
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
            MisterioDescripcion = @"Un día más, un día menos

Me quedan pocas gominolas en mi mochila... yo no puedo más. Estoy al borde de la desesperación. He perdido Navidad y sigo aquí, y a este paso, me pierdo Año Nuevo también.

Así que he tomado una decisión drástica. Mirando el somier de la cama, he decidido arrancar uno de los barrotes de metal. No sé por qué no lo hice antes. Este barrote será mi nuevo aliado en mi misión por salir. Lo voy a mejorar; le haré una especie de empuñadura, algo que me permita usarlo para romper cosas al día siguiente.

Al menos tengo un plan, aunque suene un poco loco. Es mejor que quedarme aquí sentada, sintiéndome cada vez más atrapada en esta pesadilla. Un día más, un día menos, y este encierro no podrá durar para siempre. Pero no puedo evitar que la ansiedad me consuma. ¿Qué pasará cuando salga? ¿Seguiré sintiéndome así? Espero que las gominolas me ayuden a resistir un poco más...
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Unas gominolas...",
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
            MisterioDescripcion = @"Viva Master of Puppets

Que esta mañana suene Master of Puppets en la radio es lo mejor que me podía pasar hoy. La energía de esa canción me ha invadido, y no he podido resistirme: me he zampado otra bolsa de gominolas y he cogido una sartén grande. Hoy es el día de la destrucción.

He comenzado a reventar todo. Paredes, suelos, columnas... ¡TODO! No hay nada que me detenga en mi búsqueda de pistas. Cada golpe de la sartén es un grito de desesperación y determinación, como si estuviera reclamando mi libertad a gritos.

Cada vez que un trozo de pared se desploma, siento que recupero un poco de control. ¿Quién necesita la calma en un momento como este? Estoy aquí, desafiando a la desesperación y a la locura.

P.D.: Jingle está a salvo. Al menos él no tiene que lidiar con este caos. Suerte que tiene un corazón de acero, porque yo, sinceramente, me estoy volviendo loca. ¡Vamos, que nada me detenga!
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Dosis de gominolas.",
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
            MisterioDescripcion = @"Creo que tengo una pista

La mañana ha empezado con energía. He estado reventando todo a mi alrededor al ritmo de Master of Puppets, y la frustración se convierte en motivación. Mientras destrozo paredes, suelos y columnas, mi mente se llena de pensamientos sobre lo que podría estar oculto. Pero no solo el caos me rodea, sino que también mis últimas gominolas me dan un empujón extra.
Tras una sesión intensa de destrucción, algo llama mi atención en el suelo. Entre los escombros, asoman unas escaleras. La emoción se mezcla con la ansiedad; esto podría ser lo que he estado buscando.

Con cuidado, empiezo a despejar el camino hacia las escaleras. Cada paso me acerca más a la verdad. Con un último golpe de la sartén, me abro paso hasta la entrada. La oscuridad de las escaleras invita a la aventura.

Desciendo con cautela, cada peldaño cruje bajo mi peso. Cuando llego al final, me encuentro ante una puerta con cuatro cerraduras. Mis pulsaciones aumentan. ¿Qué se esconde detrás de ella? ¿Serán las respuestas que tanto busco?
Me quedo mirando la puerta, sintiendo la mezcla de desesperación y emoción. Hoy ha sido un día largo, y esto se siente como un hito. Sin embargo, no puedo abrirla, al menos no todavía. Siento que la historia está a punto de dar un giro inesperado. Tendré que volver a pensar en cómo usar esas llaves que he encontrado.

Mañana será otro día, y estoy más cerca que nunca de descubrir lo que hay detrás de esta puerta.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Penúltimas gominolas.",
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
            MisterioDescripcion = @"La puerta a lo desconocido

Después de una larga noche de pensamientos agitados, me despierto decidida. Hoy es el día en que tengo que abrir esa puerta. Detrás de ella podría haber horror o, tal vez, una salida. No lo sabré hasta que me atreva a dar el paso.

Me acerco a la puerta, y la observación me deja sin aliento. Es una sala grande, su apariencia es metálica, pero el sonido que emite no es el de metal, sino más bien un suave eco como si fuera madera, quizás con un barniz especial que oculta su verdadera naturaleza.

Las cuatro cerraduras brillan, perfectamente alineadas, encajando con el color y la forma de las llaves que he ido encontrando. Cada una parece contar una historia, un pedazo de este enigma que he estado resolviendo. La pintura de la puerta está dividida en dos mitades: una blanca, otra negra. La imagen que se forma es la de un ángel y un demonio, un curioso detalle que me recuerda a un cierto artista...

Siento una mezcla de adrenalina y miedo mientras introduzco la primera llave. La cerradura gira, pero no se abre. Así que continúo con la segunda, luego la tercera, y finalmente, la cuarta. Todo mi esfuerzo me lleva a un punto en el que la puerta comienza a ceder, pero aún así, me cuesta mucho abrirla.

Con un último empujón, la puerta se abre de par en par. Lo que veo al otro lado me deja paralizada: “¡Abel! ¿Qué haces tú aquí?” La incredulidad me inunda mientras un torrente de emociones me invade al reconocer a la persona que menos esperaba encontrar en este lugar.
"
        };

        // Crear una nueva recompensa
        nuevaRecompensa = new RecompensaModel
        {
            Nombre = "Últimas gominolas.",
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
            MisterioDescripcion = @"Un regalo inesperado

Al abrir la puerta del cobertizo, me encuentro con Abel, mi novio. Todo esto fue una broma. No era una caja, ni Mateo, ni David... Abel había planeado una serie de acertijos y desafíos con un regalo original en mente, con la esperanza de que pudiera resolver el misterio antes de que llegara el día de Navidad.

Con una sonrisa traviesa, me dice: “Lo siento mucho por hacerte perder tus vacaciones, pero quería enseñarte cuánto vales y lo importante que es valorarse a uno mismo. Nunca imaginé que te llevaría tan lejos, pero sé que todo esto, aunque caótico, tiene un propósito. Aprender a disfrutar del camino, a veces complicado, pero lleno de sorpresas.”

En su mano, sostiene un pequeño paquete envuelto con cuidado. Lo abro con ansiedad y dentro hay un vale de viaje. Mis ojos brillan al leer las opciones: “He planeado un viaje para nosotros, con varias opciones de destino. Podemos ir a donde siempre has querido: una escapada a la playa, una montaña llena de aventura, o una ciudad que nunca duerme. La decisión es tuya.”

“Este es mi regalo para ti,” dice, con una mirada llena de cariño. “Espero que disfrutes la sorpresa y que esta aventura nos haga olvidar el caos de estos días. ¡Feliz Navidad!”

Me siento abrumada de alegría. Aunque todo lo vivido ha sido una montaña rusa de emociones, he aprendido a valorar más que nunca lo que tengo: a mí misma, a Abel, y a la vida. Con una sonrisa radiante, lo abrazo, sintiendo que este es el inicio de una nueva aventura.
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