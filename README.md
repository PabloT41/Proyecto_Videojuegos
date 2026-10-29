# Pixel Miner — Guardianes de Cristal

Proyecto fuente de un juego clicker de minería y combate, desarrollado en Unity Universal 2D con C#.

Este repositorio contiene la versión editable normal del proyecto: escenas, scripts, gráficos, materiales y configuración de Unity. No incluye la exportación Web de itch.io ni ejecutables compilados.

## Abrir el proyecto

1. Clona el repositorio.
2. En Unity Hub, usa **Add project from disk** y selecciona la carpeta que contiene Assets, Packages y ProjectSettings.
3. Abre con **Unity 6000.5.0f1** y espera la importación de paquetes.
4. Abre Assets/PixelMiner/Scenes/PixelMiner.unity.
5. Pulsa **Play** para jugar.

Para preparar una compilación normal de escritorio, selecciona **Windows** en **File → Build Profiles**, activa esa plataforma y utiliza la escena PixelMiner.unity.

## Jugabilidad

- Clic izquierdo en el cristal para obtener monedas.
- Mejoras del pico para aumentar la extracción y del filo para aumentar el daño.
- Cuatro etapas: mina inicial, Guardián de Piedra, cueva profunda y Guardián Legendario.
- Guardianes de 20 y 100 PV con ataques automáticos y animaciones.
- Derrota con reintento, victoria, pausa y guardado local de progreso.
- Arte pixel art 2D, animación de minería y respiración con los pies fijos.

## Archivos principales

- Assets/PixelMiner/Scripts/MineState.cs: reglas, economía y progresión.
- Assets/PixelMiner/Scripts/PixelMinerGame.cs: interfaz, interacción, animaciones y guardado.
- Assets/PixelMiner/Scripts/PixelArt.cs: dibujos y fotogramas procedurales.
- Assets/PixelMiner/Art: gráficos y material de la vista previa.
- Assets/PixelMiner/Editor: herramientas del editor y vista previa del nivel.

El menú **Pixel Miner → Vista previa** permite visualizar la mina y los guardianes fuera de Play Mode. Es una referencia visual del editor; durante la partida la presentación se dibuja desde el código.

El menú **Pixel Miner → Verificar reglas** ejecuta las comprobaciones de lógica y muestra los resultados en Console.

## Control de versiones

Se conservan los archivos .meta para mantener las referencias de Unity. Las cachés Library, Temp, Logs, las preferencias locales, los resultados de compilación y el perfil de exportación Web se excluyen mediante .gitignore.
