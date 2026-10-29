# Pixel Miner — Guardianes de Cristal

Proyecto de la primera versión descrita en Pixel_Miner_UDLA.docx.
Juego incremental para PC Windows, un jugador, controlado con clic izquierdo.
Proyecto preparado para Unity 6000.5.0f1, Universal Render Pipeline 17.5, Renderer 2D.
No necesitas crear otro proyecto ni instalar assets de pago.

## Abrir y jugar

1. Extrae completamente PixelMiner_UDLA.zip.
2. En Unity Hub, usa Add / Agregar proyecto desde disco.
3. Selecciona la carpeta PixelMiner_UDLA que contiene Assets, Packages y ProjectSettings.
4. Ábrela con Unity 6000.5.0f1. La primera apertura necesita descargar los paquetes oficiales; espera a que termine la importación. Si Unity solicita reiniciar por el sistema de entrada, acepta.
5. En Project, abre Assets/PixelMiner/Scenes/PixelMiner.unity.
6. Selecciona la pestaña Game y pulsa Play en Unity.
7. Haz clic en COMENZAR EXPEDICIÓN. Toda la partida se controla con el mouse.

No importes el ZIP como un .unitypackage. Es un proyecto completo.
La escena ya contiene la cámara, el componente del juego y la configuración Universal 2D.
La interfaz y las ilustraciones se construyen al entrar en Play: en Scene no verás la pantalla final antes de ejecutar.

Si la escena se pierde o necesitas reconstruirla, usa el menú superior:
Pixel Miner > Crear escena principal.

## Recorrido del nivel

1. Mina inicial. Cada clic en el cristal comienza generando una moneda.
   Consigue 50 cristales acumulados y compra al menos una mejora del pico.
   El contador acumulado no disminuye al comprar; el saldo de monedas sí.
2. Guardián de Piedra. Pulsa DESAFIAR GUARDIÁN y haz clic en su figura.
   Tiene 20 PV, hace 8 de daño cada 2,5 segundos y entrega 75 monedas.
3. Cueva profunda. Continúa extrayendo y comprando mejoras.
   Puedes prepararte el tiempo que necesites antes del combate final.
4. Guardián Legendario. Pulsa DESAFIAR LEYENDA.
   Tiene 100 PV, hace 12 de daño cada 2 segundos y entrega 250 monedas.
   Al vencerlo aparece la victoria y se revela el cristal legendario.

El minero empieza cada combate con 100 PV. Si pierde, aparece la pantalla
de derrota. VOLVER A LA MINA restaura la vida y mantiene saldo y mejoras.
Los enemigos recuperan su vida completa al reintentar.
La tienda y la minería quedan bloqueadas durante el combate.

Una ruta de ejemplo:
- Extrae 50 cristales y compra un nivel de minería (25) y uno de daño (20).
- Con 3 de daño por clic, vence al primer guardián en 7 clics.
- Usa la recompensa y la cueva profunda para subir minería y daño.
- Desafía al jefe cuando estés preparado. No hace falta comprar todos los niveles.

## Mejoras

Pico: producción = 1 + nivel.
Coste del siguiente nivel = 25 + 25 × nivel actual.

Filo: daño = 1 + 2 × nivel.
Coste del siguiente nivel = 20 + 20 × nivel actual.

Los valores de ataque enemigo, recompensas y costes concretos son decisiones
de implementación para completar la propuesta: el documento no los fija.
Se respetan sus valores explícitos de 1 moneda inicial, 50 acumulados y 20/100 PV.

## Funciones adicionales

- Menú principal, instrucciones y continuación de partida.
- Pausa real: detiene ataques y efectos.
- Botón de sonido y pequeños tonos de extracción, ataque y compra.
- Contadores de saldo y extracción acumulada.
- Indicador de las cuatro etapas, vida del minero y vida del enemigo.
- Barra que anticipa el siguiente ataque automático.
- Mensajes de objetivo y números flotantes al hacer clic.
- Guardado local automático de monedas, niveles y progreso.
- Confirmación antes de reemplazar la partida desde el menú.
- Diseño con proporción 1280 × 800 y adaptación al tamaño de ventana.

Al continuar una partida cerrada en pleno combate, vuelves a su zona de
preparación con la vida restaurada. No se guardan combates a mitad de un golpe.
El guardado usa PlayerPrefs con la clave PixelMiner.Save.v1.
No es un juego de plataformas: el documento especifica un minero sin movimiento
por teclado y la interacción directa con cristal y guardianes.

## Archivos importantes

Assets/PixelMiner/Scripts/MineState.cs
Reglas de economía, compras, etapas, salud, reintentos y victoria.

Assets/PixelMiner/Scripts/PixelMinerGame.cs
Interfaz, interacción, temporizador enemigo, guardado, sonidos y mensajes.
Usa IMGUI (OnGUI), disponible en Unity, sin depender de prefabs UI externos.

Assets/PixelMiner/Scripts/PixelArt.cs
Dibujo procedural original de la cueva, el minero, los cristales y guardianes.
El juego genera estas texturas con filtrado Point al iniciar.

Assets/PixelMiner/Art/
Copias PNG originales del mismo arte, disponibles para reutilizar.
Los PNG son material editable de apoyo; el juego usa las texturas procedurales.

Assets/PixelMiner/Editor/PixelMinerBuilder.cs
Herramientas para recrear la escena, verificar reglas, exportar el arte y compilar.

Assets/Settings/UniversalRP.asset y Renderer2D.asset
Configuración Universal 2D tomada de la plantilla oficial instalada de Unity.
Graphics Settings y todos los niveles de calidad apuntan a ese pipeline.

## Generar el ejecutable de Windows

1. Instala Windows Build Support para esta versión desde Unity Hub si hace falta.
2. Abre el proyecto y espera a que termine de compilar los scripts.
3. Sal de Play Mode.
4. Usa Pixel Miner > Compilar para Windows.
5. Espera a que Unity termine. El resultado se guarda en Build/Windows/PixelMiner.exe.
6. Entrega TODA la carpeta Build/Windows, incluyendo PixelMiner_Data y las DLL.
   El .exe solo no contiene todo el juego.

También puedes usar File > Build Profiles, Windows, x86_64, con la escena
Assets/PixelMiner/Scenes/PixelMiner.unity habilitada en la lista.

## Comprobaciones y límite de la verificación

El código de ejecución y del editor se compiló con Roslyn contra las DLL
oficiales de Unity 6000.5.0f1, sin errores.
Se ejecutaron 23 comprobaciones independientes de la lógica C#; consulta QA/.
El diseño de inicio, minería, combate, victoria y derrota se revisó mediante
una reproducción gráfica auxiliar. Esa revisión no fue una ejecución en Unity.

La importación completa, Play Mode y la generación del .exe NO se pudieron
verificar aquí: el servicio de licencias de Unity no tuvo acceso a WMI/BIOS
en el entorno restringido. Por eso se entrega el proyecto fuente, sin afirmar
que existe un ejecutable probado.

En tu Unity puedes repetir las comprobaciones con:
Pixel Miner > Verificar reglas.
Los resultados aparecen en Console y en QA/Reglas_VERIFICADAS.txt.
Después de una compilación exitosa se crea QA/Build_VERIFICADO.txt.

Comprobación manual recomendada antes de entregar:
- Iniciar, extraer, comprar ambas mejoras y desbloquear el guardián.
- Ver que el guardián ataca aunque no hagas clic.
- Perder un combate y comprobar que las mejoras se conservan.
- Pausar y comprobar que la vida no cambia.
- Vencer al primer guardián, entrar en la cueva profunda y vencer al jefe.
- Cerrar, volver a abrir y continuar una partida.
- Probar el ejecutable de Windows fuera del editor.

## Relación con la propuesta

Se implementa el alcance jugable de la primera versión: pantalla principal,
cristal interactivo, monedas, dos categorías de mejoras, guardián normal,
jefe final, barras de vida, derrota, reintento y victoria en cuatro etapas
dentro de una misma interfaz. La presentación mantiene la ambientación
de la civilización antigua, la energía de los cristales y el minero explorador.
Las referencias PickCrafter y Clicker Heroes son inspiración de diseño;
no se copiaron sus recursos ni se necesitan sus juegos o servicios.

Los gráficos procedurales y el código específico del juego se crearon para
este proyecto. Los archivos base del motor y del pipeline conservan las
condiciones de Unity. La propuesta original identifica al estudiante Pablo
Torres; no se añade ninguna declaración de autoría académica al código.
