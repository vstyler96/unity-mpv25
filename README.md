# Unity Fundamentals [Mini Fútbol 3D (3v3)]

Mini fútbol 3D 3 contra 3 hecho en Unity. Controlas a un jugador del equipo local (azul) con dos compañeros manejados por la IA, contra tres rivales de la IA (rojo). El partido dura 3 minutos; al final pulsa cualquier tecla para jugar otra vez.

## Requisitos

- Unity **6000.6.0f1** (Unity 6)
- Universal Render Pipeline (URP) 17.6
- Input System 1.20
- Cinemachine 3

## Cómo ejecutarlo

1. Clona el repositorio.
2. Abre la carpeta del proyecto con Unity Hub usando la versión indicada arriba.
3. Abre la escena `Assets/Scenes/SampleScene.unity`.
4. Pulsa **Play**.

## Controles

| Acción | Teclado | Mando |
|--------|---------|-------|
| Moverse | WASD / flechas | Stick izquierdo |
| Correr (mantener) | Shift izquierdo | Pulsar el stick izquierdo |
| Chutar | Clic izquierdo / Enter | Botón oeste (X / Cuadrado) |

## Partido

- **Campo**: 60 m de largo (eje Z) por 30 m de ancho (eje X). Las porterías miden 9 m y están centradas en x = 0.
- **Equipos**: el local defiende la portería de z = -30 y ataca hacia +Z; el visitante, al revés.
- **Alineaciones**: el objeto `Match` de la escena tiene las listas `homeSpawns` y `awaySpawns` (hijos de `Spawns`). El primer punto de `homeSpawns` es el jugador humano. Mueve o añade puntos para cambiar la formación.
- **IA**: cada equipo tiene tres roles fijos durante todo el partido, según el orden de `homeSpawns` / `awaySpawns`: goleador (el primero; en el local eres tú), portero y defensa. La IA es muy básica: el goleador va directo a la pelota y chuta a portería; el portero y el defensa se quedan quietos en su punto de salida y chutan si la pelota les llega.
- **Gol**: aparece **¡Goool!** en el centro de la pantalla, del color del equipo que ha marcado. El juego se congela 1,5 s y todos vuelven a su posición inicial con la pelota en el centro.

## Guía paso a paso

Abre [`Docs/football-3v3-guide.html`](Docs/football-3v3-guide.html) en el navegador: explica cómo reconstruir el juego desde cero (campo, prefabs, los cinco scripts, los controles con el Input System y cómo cambiar teclas).

## Estructura

```
Assets/
├── Materials/   Materiales (pelota, suelo, madera) y material físico rebotante
├── Prefab/      Prefabs de Player (futbolista), Ball y Goal
├── Scenes/      SampleScene, la escena principal
├── Scripts/     Lógica del juego
├── Settings/    Configuración de URP (PC y móvil)
└── Textures/    Texturas PBR 4K
```

### Scripts

- **`Match.cs`**: crea los 6 jugadores, asigna la cámara al humano, lleva el marcador y el tiempo, muestra **¡Goool!** y hace el saque tras cada gol.
- **`Player.cs`**: un jugador (humano o IA): movimiento con Rigidbody, chute y reinicio de posición.
- **`PlayerAI.cs`**: la IA básica: el goleador persigue la pelota; el portero y el defensa se quedan en su sitio.
- **`Ball.cs`**: la pelota; `ResetPhysics(posición)` la detiene y la coloca en el punto indicado.
- **`Goal.cs`**: al entrar un objeto con la etiqueta `Ball` en el trigger, avisa a `Match` de qué equipo ha marcado.

## Estilo de código

El archivo `.editorconfig` de la raíz aplica reglas al estilo PSR-2 al código C#. Rider, Visual Studio y VS Code las leen solas y avisan cuando se incumplen:

- Llaves de clases y métodos en su propia línea; las de `if`, `for` y `else` en la misma línea (`} else {`).
- Llaves siempre, también en los `if` de una línea.
- Visibilidad explícita en todos los miembros (`private void Awake()`).
- Una declaración por línea, líneas de 120 caracteres como máximo, 4 espacios de sangría y finales de línea LF.

`InputSystem_Actions.cs` queda fuera porque Unity lo genera. Para comprobar el formato desde la terminal:

```sh
dotnet format whitespace Assets/Scripts --folder --verify-no-changes
```

## Licencia

Este proyecto se distribuye bajo la licencia [GNU Affero General Public License v3.0](LICENSE).
