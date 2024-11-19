

# 🎄 Calendario de Adviento Interactivo 2024

Este proyecto es mi primer acercamiento a .NET MAUI y busca combinar narrativa, interactividad y diseño navideño en un calendario de adviento único. A diferencia de los tradicionales, este abarca 31 días , con días destacados y desafíos personalizados.

El calendario está dedicado a mi pareja, quien inspiró cada detalle del diseño y la historia.

## 📝 Descripción del Proyecto.

Este proyecto ofrece una experiencia inmersiva que mezcla:

- **Narrativa**: Cada capítulo presenta un fragmento de la historia.
- **Interactividad**: Un minijuego en tiempo real para avanzar en la trama.
- **Diseño Temático**: Gráficos y estilo pensados ​​para la época navideña.

## 📖 Historia

El protagonista, Ray , es un detective que recibe una misteriosa tarjeta antes de lo que parecía un descanso invernal normal. Lo que comienza como unas vacaciones se convierte en un intrincado misterio. Atrapada en un chalet, Ray enfrentará desafíos diarios que le revelarán pistas sobre su situación.

El clímax llega al descubrir una puerta secreta y, con ella, quién está detrás de todo y por qué.

## 🚀 Características principales

- **Misterio y giros**: Descubre el verdadero motivo del secuestro y desvela secretos.
- **Diario y tarjetas**: Pistas y contexto se presentan en objetos interactivos.


## 🎮 Minijuegos

Cada capítulo incluye un minijuego único que permite avanzar en la historia. Aquí tienes una descripción de cada uno:

#### 🔴 MultiClics

Un minijuego sencillo pero adictivo que consiste en pulsar varias veces en una imagen específica antes de que se acabe el tiempo.

**Objetivo**: Completar el numero de cliks.

**Variación**: El número de clics requeridos cambia dependiendo del día.

#### 🟢 Serpiente

El clásico juego de Snake adaptado a la historia

**Objetivo**: Atrapa manzanas para completar el nivel.

**Variación**: El número de manzanas necesarias para ganar aumenta o disminuye según el día.

#### 🟡 Ahorcado

Pon a prueba tu vocabulario y deduce la palabra correcta letra por letra. Cada error te acerca al límite.

**Objetivo**: Resolver palabras claves relacionadas con la narrativa.

**Variación**: La palabra varía cada día.

#### 🔵 Wordle

Descifra pistas con palabras de exactamente 5 letras, basándote en las letras correctas y sus posiciones.

**Objetivo**: Encuentra la palabra clave

**Variación**: La palabra varía cada día.


#### 🟠 Piedra, Papel y Tijeras

Un duelo estratégico donde deberás anticiparte a los movimientos del oponente.

**Objetivo**: Gana dos de tres rondas para superar el desafío.

**Variación**: El numero de rondas ganadas varía cada día.


#### 🟣 Ping-pong

Un reto de habilidad y reflejos: controla tu paleta y devuelve la pelota sin fallar.

**Objetivo**: Conseguir marcar al rival un numero determinado de goles.

**Variación**: El numero de rondas ganadas varía cada día.


#### ⚫ Tres en raya

El clásico Tres en Raya , adaptado como un enigma rápido para resolver.

**Objetivo**: Consigue tres en línea antes que el oponente.

**Variación**: El numero de rondas ganadas varía cada día.


## 🎨 Diseño

- **Libreta visual**: La interfaz incluye un diario con tapas personalizadas. La historia se cuenta a traves de ella, como el diario personal de Ray.
- **Estilo navideño**: Fuentes, colores y gráficos temáticos.
- **Estilos pixelados** : Las imagenes utilizadas se han realizado a partir de pixel art.

## 🛠 Tecnologías utilizadas

- **.NET MAUI**: Para la creación de la aplicación multiplataforma.
- **C#**: Lenguaje principal de desarrollo.
- **SQLite**: Base de datos ligera para almacenar el progreso y otros datos de la aplicación.
- **XAML**: Para diseño de interfaz de usuario.
- **Visual Studio**: IDE utilizado para el desarrollo.

## 📦 Instalación.

Para ello hay dos opciones. 

**1. Abrir el proyecto en Visual Studio:**

```bash
  git clone https://github.com/llavefija/NewAdventCalendar2024.git
  cd NewAdventCalendar2024
```

**Compila e instala**: Selecciona Android y ejecuta.

**2. Descargar el APK directamente**

Encuentra el APK comprimido en la carpeta `DownloadApk`. Descomprímelo y disfruta la experiencia.

¡Disfruta la experiencia!

## 🎯 Objetivos del Proyecto.

- Crear un regalo único y personalizado.
- Aprender nuevas tecnologías y reforzar habilidades en C# y .NET MAUI.
- Experimentar con el diseño de minijuegos y narrativa interactiva.

## 👥 Contribuciones.

¡Las contribuciones son bienvenidas! Si deseas colaborar:

- Haz un fork del repositorio.

- Crea una nueva rama para tu característica o corrección.

- Realiza los cambios y haz un commit.

- Envíe una solicitud de extracción.

## 🗂 Estructura del proyecto.

```plaintext

📂 NewAdventCalendar2024

├── ⚙️ Dependencias     # Deppendencias del proyecto.

├── 📂 Data             # Base de datos botones y recompensas.

├── 📂 DownloadApk      # APK comprimido.

├── 📂 Interfaces       # Contratos para diseño modular.

├── 📂 Models           # Clases de datos y lógica.

├── 📂 Platforms        # Confuguracion de las plataformas.

├── 📂 Resources        # Archivos gráficos, fuentes y temas.

├── 📂 Tools            # Herramientas de los botones.

├── 📂 Views            # Pantallas principales (narrativa y minijuegos).

├── 📄 App.xaml         # Configuración global de la aplicación.

├── 📄 AppShell.xaml    # Navegación global.

├── 📄 Constants.cs     # Constantes de la BD.

├── 📄 MauiProgram.cs   # Inicializacion del programa.

└── 📄 README.md        # Documentación del proyecto.
```

## 📌 Estado del Proyecto.

El proyecto está finalizado , pero al ser mi primera experiencia con .NET MAUI , existen algunos detalles a pulir, especialmente en diseño y control de datos.

## ✨ Capturas de pantalla.

![App Screenshot](https://via.placeholder.com/468x300?text=App+Screenshot+Here)

¡Próximamente!

## 🙏 Créditos

- **Iconos y gráficos**: Las imagenes e iconos estan elaboradas a mano con la tecnica de pixel art.  
- **Fuentes**: Tipografía utilizada descargada de [DaFont](https://www.dafont.com/es/).

## 📜 Licencia

Este proyecto no tiene licencia ya que es un proyecto personal.

## 🔮 Futuras Mejoras

- Implementación de un sistema de pistas dinámicas para los capítulos más difíciles.  
- Mejoras en la interfaz gráfica para dispositivos con pantallas más grandes.  
- Traducción al inglés y otros idiomas.  
- Nuevos minijuegos y capítulos adicionales.

## 🖊️ Autor.

[@Abel Cumbreño Pinadero](https://www.github.com/llavefija)



