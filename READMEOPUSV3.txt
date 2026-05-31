Necesito que continúes el desarrollo del videojuego Unity “Refugio Nocturno”.

Actuá como un equipo senior de desarrollo: game director, lead gameplay programmer, systems designer, economy designer, UI/UX designer, narrative designer, technical artist y QA.

IMPORTANTE
No empieces desde cero.
Primero inspeccioná el proyecto actual.
No uses “Build Main Scenario” salvo que se pida explícitamente.
No destruyas el layout manual.
No reemplaces sistemas existentes si podés extenderlos.
Trabajá incrementalmente y verificá compilación cuando sea posible.

CONTEXTO DEL JUEGO
“Refugio Nocturno” es un juego de terror psicológico y observación inspirado en Papers, Please.

El jugador trabaja en una cabina/refugio durante la noche. Visitantes llegan a una ventana. Algunos son humanos y otros imitadores. El jugador debe observar, hacer preguntas limitadas y decidir si permite o rechaza la entrada.

SISTEMAS ACTUALES
El proyecto ya tiene:
- 2 noches.
- 8 visitantes.
- sistema de visitantes.
- preguntas limitadas.
- permitir/rechazar.
- recursos: comida, seguridad, moral, población.
- resumen de noche.
- victoria/derrota.
- observaciones dinámicas.
- sistema de sospecha.
- reglas de noche.
- respuesta con tags: Coherent, Evasive, Contradictory, Dangerous.
- feedback de contradicción.
- reloj por acciones.
- eventos entre visitantes.
- registro de visitantes.
- tienda de mejoras entre noches.
- sprites de NPC en Assets/Npcs y Assets/Resources/Npcs.
- assets UI en Assets/Resources/UI.

SCRIPTS IMPORTANTES
Leer antes de modificar:
- VisitorData.cs
- QuestionAnswer.cs
- VisitorManager.cs
- DialogueUI.cs
- QuestionUI.cs
- DecisionController.cs
- RefugeStats.cs
- NightManager.cs
- NightClock.cs
- NightRuleData.cs
- SuspicionSystem.cs
- SuspicionUI.cs
- ContradictionFeedbackUI.cs
- InterEventSystem.cs
- InterEventUI.cs
- VisitorLog.cs
- VisitorLogUI.cs
- EndNightSummaryUI.cs
- UpgradeData.cs
- UpgradeManager.cs
- UpgradeShopUI.cs
- UIspriteLoader.cs
- ExampleVisitorFactory.cs

ESTADO ACTUAL DE LA TIENDA
La tienda ya existe, pero no todos los items tienen efecto real.

Items que actualmente funcionan:
1. Interrogatorio extendido
   - Effect: ExtraQuestion
   - Da +1 pregunta por visitante.

2. Cerradura reforzada
   - Effect: ReinforcedLock
   - Reduce en 1 la pérdida de seguridad al aceptar visitantes peligrosos.

3. Racionamiento
   - Effect: Rationing
   - Reduce en 1 el consumo de comida al aceptar visitantes.
   - Su drawback de moral todavía NO está implementado.

4. Café para el operador
   - Effect: OperatorCoffee
   - Actualmente funciona mal: da +1 pregunta por visitante.
   - Debería ser “una pregunta extra por noche” o “una pregunta extra una sola vez durante la noche”.

Items que existen pero NO están implementados todavía:
- ReinforcedLamp
- ImprovedMicrophone
- ExtraGuard
- ShortWaveRadio
- ThermalDetector
- InternalArchive

OBJETIVO PRINCIPAL
Implementar correctamente todos los items de tienda y conectar sus beneficios al gameplay real.

OBJETIVO SECUNDARIO
Expandir el juego con días/noches siguientes, más visitantes, más eventos, más lore y más progresión.

PRINCIPIOS DE DISEÑO
La dificultad debe venir de deducción, ambigüedad y consecuencias, no de confusión injusta.

El jugador debe sentir:
“Tengo pistas, pero no certeza absoluta.”

No revelar directamente si alguien es humano o imitador.

Aplicar principios de UX:
- feedback claro,
- carga cognitiva controlada,
- información importante visible,
- decisiones con consecuencias entendibles,
- tensión creciente,
- accesibilidad,
- legibilidad.

TAREA 1 - CORREGIR OPERATOR COFFEE
Cambiar `OperatorCoffee` para que no dé +1 pregunta por visitante de forma permanente.

Implementación deseada:
- Comprar café da 1 carga.
- Esa carga se consume automáticamente cuando el jugador ya no tiene preguntas disponibles y hace una pregunta extra, o se muestra como “Café disponible: 1”.
- Alternativamente: al inicio de cada noche, si el jugador compró café, gana +1 pregunta total para esa noche, no por visitante.
- Si el item es repeatable, cada compra suma una carga.
- No permitir que el jugador compre café infinitamente sin efecto.

Actualizar:
- UpgradeManager
- QuestionUI
- UpgradeShopUI si hace falta
- textos descriptivos

TAREA 2 - IMPLEMENTAR REINFORCED LAMP
Objetivo:
La lámpara reforzada mejora la calidad de observaciones.

Actualmente las observaciones son demasiado directas.
Agregar un sistema de claridad/ruido de observación:
- Si no hay lámpara, algunas observaciones pueden aparecer como:
  - “INCONCLUSO”
  - “NO VISIBLE”
  - “FALLA DE LUZ”
- Si hay ReinforcedLamp, reduce la probabilidad de observación inconclusa.
- Si ocurre un evento PartialBlackout, la lámpara puede no funcionar temporalmente.

Conectar con:
- DialogueUI
- ObservationProfile
- SuspicionSystem
- InterEventSystem si aplica

TAREA 3 - IMPLEMENTAR IMPROVED MICROPHONE
Objetivo:
El micrófono mejorado revela información auditiva.

Agregar campos opcionales en ObservationProfile:
- voiceTone
- breathingPattern

Sin micrófono:
- no mostrar esos datos o mostrarlos como “NO DISPONIBLE”.

Con micrófono:
- mostrar tono de voz y respiración.
- esos datos deben afectar sospecha.

Ejemplos:
- Voz sin aire
- Respiración irregular
- Voz demasiado calma
- Repite cadencia
- Tiembla al hablar

TAREA 4 - IMPLEMENTAR THERMAL DETECTOR
Objetivo:
Detector térmico defectuoso muestra temperatura corporal con margen de error.

Agregar campo:
- bodyTemperature

Valores posibles:
- NORMAL
- FRÍA
- IRREGULAR
- NO MEDIBLE
- ERROR

Con ThermalDetector:
- mostrar temperatura en observaciones.
- afectar sospecha, pero no de forma absoluta.
- Puede fallar con lluvia intensa o eventos de corte de luz.

TAREA 5 - IMPLEMENTAR EXTRA GUARD
Objetivo:
Guardia extra reduce daño de seguridad, pero cuesta comida.

Efecto:
- Reduce pérdida de seguridad por imitadores o eventos en 1.
- Al final de cada noche consume 1 comida adicional, o al inicio de la noche siguiente.
- Mostrar feedback claro en resumen:
  “Guardia extra consumió 1 comida.”

Conectar con:
- DecisionController
- InterEventSystem
- EndNightSummaryUI
- RefugeStats

TAREA 6 - IMPLEMENTAR SHORT WAVE RADIO
Objetivo:
Radio de onda corta desbloquea lore y preguntas especiales.

Efectos:
- Agregar preguntas opcionales tipo “radio/lore” si el jugador tiene la mejora.
- Agregar eventos especiales de radio entre visitantes.
- Algunas transmisiones pueden ser falsas.
- Puede revelar datos sobre refugio norte, protocolo de entrada, voces imitadas, nombres robados.

Conectar con:
- QuestionUI
- ExampleVisitorFactory
- InterEventSystem
- VisitorData / QuestionAnswer

TAREA 7 - IMPLEMENTAR INTERNAL ARCHIVE
Objetivo:
Archivo interno mejora el registro de visitantes.

Si el jugador compra InternalArchive:
- VisitorLogUI muestra más información:
  - respuestas dadas,
  - tags de sospecha,
  - regla activa,
  - cambios de recursos,
  - sospecha al momento de decisión.
Sin InternalArchive:
- mostrar registro más básico.

Si actualmente InternalArchive no está en la tienda default, agregarlo.

TAREA 8 - BALANCEAR COSTOS
Revisar costos para que el jugador no pueda comprar todo después de noche 1.

Sugerencia:
- ExtraQuestion: 3
- OperatorCoffee: 2, repeatable
- Rationing: 4
- ReinforcedLamp: 4
- ImprovedMicrophone: 5
- ReinforcedLock: 5
- ShortWaveRadio: 5
- ExtraGuard: 6
- ThermalDetector: 6
- InternalArchive: 3

TAREA 9 - MEJORAR FEEDBACK DE COMPRA
Cuando el jugador compra una mejora:
- mostrar feedback visual/textual:
  “Mejora adquirida: Cerradura reforzada”
- actualizar suministros inmediatamente.
- desactivar item comprado si no es repeatable.
- mostrar qué mejora está activa.

TAREA 10 - EXPANDIR NOCHES SIGUIENTES
Agregar al menos una Noche 3.

Noche 3 debe ser más difícil:
- 4 visitantes nuevos.
- 2 humanos.
- 2 imitadores.
- imitadores más sutiles.
- humanos con pistas falsas.
- regla de noche más ambigua.
- más eventos entre visitantes.
- aprovechar mejoras de tienda.

Ejemplo de regla:
“Los imitadores aprendieron a usar nombres propios, pero fallan al recordar lugares.”

Agregar eventos nuevos:
- InteriorDoorKnock
- FalseRumor
- DistantScream
- SilenceBreak

Si estos eventos necesitan imágenes y no existen, NO inventar paths silenciosamente.
Pedir o generar prompts para assets.

EVENTOS E IMÁGENES
Actualmente existen imágenes en:
Assets/Resources/UI/Events

Existentes:
- event_noise_ducts.png
- event_blackout.png
- event_blackout_alt.png
- event_protest.png

Faltan imágenes propias para:
- IntermittentRadio -> event_radio.png
- AcceptedPersonInfo -> event_person_info.png
- InteriorDoorKnock -> event_door_knock.png
- FalseRumor -> event_false_rumor.png
- DistantScream -> event_distant_scream.png
- SilenceBreak -> event_silence_break.png

Si necesitás estas imágenes:
1. Avisá explícitamente cuáles faltan.
2. Generá prompts para crear cada imagen.
3. Indicá nombre exacto de archivo.
4. Indicá carpeta destino:
   Assets/Resources/UI/Events

FORMATO DE PROMPTS PARA IMÁGENES
Cada prompt debe mantener el estilo:
- dark psychological horror,
- abandoned shelter interior,
- rainy night,
- gritty painterly realism,
- cinematic low-key lighting,
- muted colors,
- UI/event background,
- no text,
- no logos,
- no UI labels,
- 16:9 composition,
- suitable as a semi-transparent event panel background.

Ejemplo de salida esperada:
Nombre archivo: event_radio.png
Prompt: [prompt completo en inglés]

SI NECESITÁS NUEVOS PERSONAJES
Para Noche 3, si necesitás sprites nuevos:
1. Crear nombres de personajes.
2. Definir si son humanos o imitadores.
3. Generar prompt para cada sprite.
4. Indicar nombre exacto del archivo:
   Nombre.png
5. Indicar carpeta:
   Assets/Npcs
   Assets/Resources/Npcs

Formato sprite:
- full body or waist-up character,
- transparent background,
- front-facing,
- dark psychological horror,
- wet clothing if needed,
- gritty painterly realism,
- no text,
- no UI,
- consistent with existing NPCs.

TAREA 11 - ACTUALIZAR UISpriteLoader
Cuando existan nuevas imágenes, actualizar `UIspriteLoader.GetEventBackgroundPath` para mapear cada evento a su imagen específica:
- IntermittentRadio -> UI/Events/event_radio
- AcceptedPersonInfo -> UI/Events/event_person_info
- InteriorDoorKnock -> UI/Events/event_door_knock
- FalseRumor -> UI/Events/event_false_rumor
- DistantScream -> UI/Events/event_distant_scream
- SilenceBreak -> UI/Events/event_silence_break

TAREA 12 - QA
Al terminar, probar o indicar cómo probar:
- comprar cada mejora,
- verificar efecto real,
- terminar noche 1,
- abrir tienda,
- comprar mejora,
- jugar noche 2,
- confirmar que la mejora afecta gameplay,
- si agregás noche 3, confirmar que ContinueButton llega a noche 3,
- confirmar que no hay errores por assets faltantes,
- confirmar que si falta una imagen se usa fallback y se avisa.

IMPORTANTE
No solo agregues descripciones en tienda.
Cada item debe tener un beneficio real en gameplay.

Si algo no puede implementarse por falta de asset o decisión de diseño, dejalo señalado claramente y generá el prompt del asset necesario.