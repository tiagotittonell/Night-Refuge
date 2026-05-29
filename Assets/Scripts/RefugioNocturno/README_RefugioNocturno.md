# Refugio Nocturno - Configuracion del prototipo

## Escena sugerida

Crear una escena llamada `Scene_RefugioNocturno` con esta jerarquia:

- GameManager
  - NightManager
  - VisitorManager
  - RefugeStats
- Canvas
  - WindowArea
  - VisitorImage
  - DialoguePanel
  - QuestionsPanel
  - DecisionPanel
    - AllowButton
    - RejectButton
  - StatsPanel
  - RulePanel
  - EndNightSummaryPanel
- Audio
  - RainAmbience
  - KnockSound
- Background
  - RefugeInterior

## Componentes

1. Agregar `NightManager`, `VisitorManager`, `RefugeStats` y `DecisionController` al objeto `GameManager`.
2. Agregar `DialogueUI` al `DialoguePanel`.
3. Agregar `QuestionUI` al `QuestionsPanel`.
4. Agregar `EndNightSummaryUI` al `EndNightSummaryPanel` o a un objeto controlador dentro del panel.
5. Conectar todas las referencias serializadas desde el Inspector.
6. Crear un prefab de boton para preguntas con componente `Button` y un texto `TMP_Text` como hijo. Asignarlo a `QuestionUI`.
7. `NightManager` puede arrancar con datos de ejemplo si `Create Example Data On Start` esta activo y la lista `Nights` esta vacia.

## UI minima

- `VisitorImage`: `Image` grande centrada detras de una ventana.
- `DialoguePanel`: textos TMP para nombre, dialogo y pistas observables.
- `QuestionsPanel`: contenedor vertical para botones de preguntas y texto de preguntas restantes.
- `DecisionPanel`: botones `PERMITIR` y `RECHAZAR`.
- `StatsPanel`: textos TMP para comida, seguridad, moral y poblacion.
- `RulePanel`: texto TMP con la regla de la noche.
- `EndNightSummaryPanel`: panel oculto al inicio, texto TMP de resumen y boton continuar.

## Flujo jugable

1. `NightManager` inicia la noche y muestra la regla.
2. `VisitorManager` muestra el visitante actual.
3. `QuestionUI` genera preguntas y limita su uso.
4. `DecisionController` aplica consecuencias con `RefugeStats`.
5. `NightManager` registra aciertos/errores y avanza.
6. Al terminar visitantes, `EndNightSummaryUI` muestra el resumen.

## Extension

Para produccion conviene crear assets `VisitorData` desde `Create > Refugio Nocturno > Visitor Data` y asignarlos a cada `NightData` del `NightManager`. El prototipo ya incluye 2 noches y 8 visitantes generados en runtime como ejemplo.
