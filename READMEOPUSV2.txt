Quiero que continúes el desarrollo de mi proyecto Unity “Refugio Nocturno”.

Actuá como un equipo senior AAA compuesto por:
game director, product manager, lead gameplay programmer, systems designer, economy designer, narrative designer, horror designer, UI/UX director, UX researcher, technical artist, tools programmer, QA lead y Unity engineer.

IMPORTANTE
No empieces desde cero.
Primero inspeccioná el proyecto actual.
No uses “Build Main Scenario” salvo que yo lo pida explícitamente.
No destruyas el layout manual.
No reemplaces sistemas existentes si podés extenderlos.
Trabajá incrementalmente, con cambios pequeños, probables de compilar y fáciles de probar.

CONTEXTO ACTUAL
El juego se llama provisionalmente “Refugio Nocturno”.
Es un juego de terror psicológico y observación, inspirado en Papers, Please.
El jugador trabaja en un refugio durante la noche. Personas llegan a una ventana pidiendo entrar. Algunas son humanas y otras son imitadores. El jugador debe observar, preguntar y decidir si permite o rechaza la entrada.

El juego ya tiene:
- 2 noches.
- 8 visitantes.
- sprites de personajes en Assets/Npcs y Assets/Resources/Npcs.
- sistema de visitantes.
- preguntas limitadas.
- decisiones permitir/rechazar.
- recursos: comida, seguridad, moral, población.
- resumen de noche.
- victoria/derrota.
- sistema de observaciones.
- sistema de sospecha.
- reglas de noche.
- tags de respuesta: Coherent, Evasive, Contradictory, Dangerous.
- feedback de contradicción.
- reloj de noche por acciones.
- eventos entre visitantes.
- registro de visitantes.
- UI estática y dinámica.
- herramientas de layout.

Scripts importantes:
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
- ExampleVisitorFactory.cs

Editor/tools:
- RefugioNocturnoGameplayUIBinder.cs
- RefugioNocturnoStaticTextBuilder.cs
- RefugioNocturnoSceneBuilder.cs

Tools permitidas:
- Refugio Nocturno > Setup Dynamic Gameplay UI
- Refugio Nocturno > Capture Dynamic Gameplay Layout
- Refugio Nocturno > Apply Captured Dynamic Gameplay Layout
- Refugio Nocturno > Sync NPC Sprites To Resources
- Refugio Nocturno > Configure NPC Sprites
- Refugio Nocturno > Cleanup Duplicate Base UI

NO usar:
- Refugio Nocturno > Build Main Scenario salvo pedido explícito.

OBJETIVO DE ESTA ETAPA
Quiero que el juego se vuelva más difícil, más profundo y más interesante.

Necesito:
1. Que al jugador realmente le cueste distinguir humanos de imitadores.
2. Más mecánicas de deducción.
3. Mejor feedback visual.
4. Sistema de compras/mejoras entre noches.
5. Más preguntas posibles.
6. Respuestas más variadas.
7. Más lore y misterio.
8. Mejor progresión de dificultad.
9. Más tensión sin depender de jumpscares.

PRINCIPIOS DE DISEÑO
Aplicar principios de UX inspirados en Celia Hodent:
- claridad sin revelar demasiado,
- feedback inmediato,
- carga cognitiva controlada,
- decisiones con consecuencias entendibles,
- atención guiada,
- accesibilidad,
- evitar frustración injusta,
- dificultad basada en lectura y deducción, no en confusión accidental.

El jugador debe sentir:
“tengo pistas suficientes para sospechar, pero nunca certeza total”.

NO quiero que el sistema diga directamente:
“este es humano” o “este es imitador”.

QUIERO que el sistema:
- sugiera,
- contradiga,
- oculte,
- cree patrones,
- haga que el jugador dude.

ROADMAP DE IMPLEMENTACIÓN

FASE 1 - DIFICULTAD REAL DE DEDUCCIÓN
Mejorar `SuspicionSystem` para que no sea demasiado obvio.

Problema actual:
Si una persona tiene muchas pistas raras, la sospecha sube mucho. Eso puede volver demasiado fácil detectar imitadores.

Objetivo:
Agregar ambigüedad real.

Implementar:
- Pistas falsas en humanos.
- Pistas humanas imitadas por imitadores.
- “ruido” de observación según seguridad del refugio.
- Observaciones que pueden fallar si seguridad es baja.
- Observaciones incompletas.
- Reglas de noche que no siempre aplican al 100%.
- Penalización por confiar demasiado en una sola pista.

Ejemplo:
Un humano puede tener:
- mirada evasiva,
- manos manchadas,
- respuestas nerviosas.

Un imitador puede tener:
- ropa mojada,
- temblor falso,
- una historia parcialmente coherente.

Crear un sistema de confianza de observación:
- Alta
- Media
- Baja

Si seguridad baja, algunas observaciones aparecen como:
- “NO CLARO”
- “INCONCLUSO”
- “FALLA DE LUZ”
- “NO VISIBLE”

FASE 2 - NUEVAS OBSERVACIONES
Expandir `ObservationProfile`.

Agregar campos:
- bodyTemperature
- breathingPattern
- voiceTone
- eyeContact
- handCondition
- memoryCoherence
- reactionToLight
- reactionToRadio
- smell
- shadowBehavior

Valores posibles:
- NORMAL
- EXTRAÑO
- INCONCLUSO
- NO VISIBLE
- HUMANO
- SOSPECHOSO
- INCONSISTENTE

La UI no debe llenarse de texto ilegible. Mostrar solo observaciones principales y permitir que algunas aparezcan como nota/lore.

FASE 3 - PREGUNTAS MÁS VARIADAS
Expandir el sistema de preguntas.

Crear categorías:
- Origen
- Evento externo
- Identidad
- Memoria
- Refugio
- Emoción
- Prueba indirecta
- Lore

Ejemplos de preguntas:
- “¿Cuál fue la última puerta que cruzaste?”
- “¿A quién viste antes de llegar?”
- “¿Qué sonido escuchaste afuera?”
- “¿Cómo se llamaba el lugar del que venís?”
- “¿Qué hay al final de la avenida principal?”
- “¿Qué le pasó a tus manos?”
- “¿Por qué no estás mojado?”
- “¿Qué recordás antes de la lluvia?”
- “¿Qué sabés del refugio norte?”
- “¿Quién te habló de este lugar?”
- “¿Qué hay detrás tuyo ahora mismo?”
- “¿Por qué repetiste esa palabra?”
- “¿Qué significa la marca en tu ropa?”
- “¿Conocés el protocolo de entrada?”

Cada respuesta debe tener:
- texto,
- tag de respuesta,
- posible modificación de sospecha,
- posible pista desbloqueada,
- posible evento narrativo.

FASE 4 - RESPUESTAS MÁS VARIADAS
Las respuestas no deben ser simples “coherente/evasiva”.
Agregar subtipos o metadata:
- truthful
- lie
- halfTruth
- panicLie
- mimicry
- memoryGap
- borrowedMemory
- repeatedPhrase
- impossibleDetail
- loreReveal

No hace falta mostrar esto al jugador directamente.
Sirve para cálculo interno y feedback.

FASE 5 - SISTEMA DE COMPRAS / MEJORAS ENTRE NOCHES
Agregar una tienda o panel de mejoras al final de cada noche.

Moneda sugerida:
- “Créditos del Refugio”
- “Suministros”
- “Vales”
- “Piezas”

La moneda se gana por:
- rechazar imitadores,
- aceptar humanos,
- terminar noche con seguridad alta,
- conservar recursos,
- tomar buenas decisiones.

Mejoras posibles:
1. Más preguntas por visitante.
   - +1 pregunta por visitante.
2. Lámpara reforzada.
   - Mejora observaciones visuales.
3. Micrófono mejorado.
   - Revela tono de voz o respiración.
4. Archivo interno.
   - Muestra historial más claro.
5. Cerradura reforzada.
   - Reduce daño al aceptar imitadores.
6. Racionamiento.
   - Reduce comida consumida por humanos aceptados.
7. Radio de onda corta.
   - Desbloquea preguntas/lore sobre refugios externos.
8. Detector térmico defectuoso.
   - Muestra temperatura, pero con margen de error.
9. Guardia extra.
   - Reduce pérdida de seguridad.
10. Café para el operador.
   - Reduce penalización de tiempo o permite una pregunta extra una vez por noche.

Sistema:
- Crear `UpgradeData`.
- Crear `UpgradeManager`.
- Crear `UpgradeShopUI`.
- Integrar con `RefugeStats`, `QuestionUI`, `SuspicionSystem`, `NightClock`.
- Debe funcionar aunque la UI sea fallback.
- Debe aparecer entre noches, antes de continuar.

FASE 6 - ECONOMÍA Y BALANCE
Evitar que el jugador compre todo.
Debe elegir.

Ejemplo:
- Al final de noche 1 puede comprar solo 1 o 2 mejoras.
- Algunas mejoras cuestan mucho.
- Algunas mejoras tienen desventajas.

Ejemplos:
- Detector térmico: ayuda, pero puede fallar con lluvia.
- Más preguntas: consume más tiempo.
- Cerradura reforzada: cara, pero salva partidas.
- Radio: agrega lore, pero a veces mete ruido/falsas pistas.

FASE 7 - MEJOR FEEDBACK VISUAL
Agregar polish visual útil, no decorativo vacío.

Implementar:
- color sutil en sospecha,
- parpadeo leve cuando sube sospecha,
- feedback cuando una respuesta contradice regla de noche,
- oscurecimiento del escenario si seguridad baja,
- texto de reloj más tenso cuando se acerca el final,
- botón de decisión con estado claro,
- animación/fade al cambiar visitante,
- feedback visual cuando se consume una pregunta,
- feedback visual cuando una mejora afecta una observación.

No sobrecargar pantalla.
Todo debe respetar estética oscura, diegética, abandonada.

FASE 8 - LORE
Agregar lore fragmentado.

Temas:
- refugios conectados por radio,
- protocolo de entrada,
- “imitadores” como fenómeno no totalmente explicado,
- lluvia como evento anómalo,
- gente que escucha voces de familiares,
- refugio norte posiblemente inexistente,
- nombres robados,
- memorias prestadas,
- una entidad o patrón que aprende del jugador.

Agregar lore en:
- respuestas,
- eventos entre visitantes,
- notas del panel derecho,
- resumen de noche,
- registro de visitantes,
- transmisiones de radio,
- mejoras de archivo/radio.

Nunca explicar todo.
El lore debe sugerir más de lo que confirma.

FASE 9 - PROGRESIÓN DE DIFICULTAD
Noche 1:
- enseñar loop,
- imitadores relativamente claros,
- humanos con alguna ambigüedad.

Noche 2:
- humanos más sospechosos,
- imitadores más coherentes,
- reglas más importantes.

Noche 3 futura:
- imitadores imitan nombres de visitantes previos,
- humanos mienten por miedo,
- observaciones pueden fallar,
- eventos entre visitantes afectan juicio.

Noche 4 futura:
- el sistema de reglas puede ser parcialmente falso,
- imitadores aprenden del jugador,
- hay consecuencias retardadas.

FASE 10 - ARQUITECTURA EXTENSIBLE
No hardcodear todo si se puede evitar.
Pero no hacer una mega migración riesgosa.

Preferir:
- clases serializables simples,
- ScriptableObjects si conviene,
- fallback en ExampleVisitorFactory,
- auto-binding por nombre,
- UI fallback si falta objeto.

FASE 11 - QA
Después de cada bloque, validar:
- compila,
- noche 1 sigue funcionando,
- noche 2 sigue funcionando,
- preguntas aparecen,
- decisiones aplican recursos,
- resumen aparece,
- ContinueButton funciona,
- no se pisan layouts,
- no aparecen UI fallback en lugares feos si ya existe UI manual.

PRIMERA TAREA RECOMENDADA
Empezar por el sistema de compras/mejoras porque agrega objetivo a medio plazo.

Orden recomendado:
1. Crear `UpgradeData`.
2. Crear `UpgradeManager`.
3. Crear `UpgradeShopUI`.
4. Agregar moneda “Suministros”.
5. Dar recompensas al final de noche.
6. Permitir comprar mejoras antes de continuar.
7. Conectar primera mejora: +1 pregunta por visitante.
8. Conectar segunda mejora: cerradura reforzada.
9. Conectar tercera mejora: lámpara reforzada para observaciones más claras.
10. Luego mejorar preguntas y dificultad.

FORMA DE RESPONDER
Antes de tocar código:
- resumí brevemente lo que encontraste,
- proponé el siguiente incremento,
- implementalo si no hay bloqueo.

Al final:
- listar archivos modificados,
- explicar cómo probar,
- mencionar limitaciones o riesgos.

IMPORTANTE FINAL
Quiero más dificultad, pero no quiero frustración injusta.
Quiero que el jugador pierda diciendo:
“me equivoqué leyendo las pistas”,
no:
“el juego me engañó sin darme herramientas”.