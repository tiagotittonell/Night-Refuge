Quiero que actúes como un equipo completo de desarrollo AAA de al menos 20 especialistas: director creativo, product manager, game director, lead gameplay programmer, UI/UX director, narrative designer, technical artist, Unity engineer, systems designer, QA lead, audio designer, economy designer, accessibility designer, tools programmer, producer, UX researcher, horror designer, content designer, art director, build engineer y refactor lead.

PROYECTO
Estoy desarrollando en Unity un prototipo llamado “Refugio Nocturno”.

Es un juego de terror psicológico y observación, inspirado en Papers, Please, pero con una interfaz fija en primera persona dentro de una cabina/refugio. El jugador recibe visitantes durante la noche. Algunos son humanos reales y otros imitadores. Debe observar, preguntar y decidir si permite o rechaza la entrada.

IMPORTANTE
No empieces desde cero.
Primero inspeccioná el proyecto y respetá la estructura existente.
No destruyas el layout manual.
No uses “Build Main Scenario” salvo que yo lo pida explícitamente, porque puede pisar la escena.
Priorizá mejoras incrementales, probables de compilar, con bajo riesgo.

ESTADO ACTUAL DEL PROYECTO
Unity 6.
Escena principal: Assets/Scenes/SampleScene.unity.
Escenario visual principal ya está puesto como imagen de fondo.
Canvas contiene:
- MainScenarioBackground
- VisitorImage
- StaticTextLayer
- DynamicGameplayLayer
- AllowButton
- RejectButton
- EndNightSummaryPanel
- RulePanel

Scripts principales:
- VisitorData.cs
- QuestionAnswer.cs
- VisitorManager.cs
- DialogueUI.cs
- QuestionUI.cs
- DecisionController.cs
- RefugeStats.cs
- NightManager.cs
- EndNightSummaryUI.cs
- ExampleVisitorFactory.cs

Editor/tools:
- RefugioNocturnoGameplayUIBinder.cs
- RefugioNocturnoStaticTextBuilder.cs
- RefugioNocturnoSceneBuilder.cs

Tools útiles:
- Refugio Nocturno > Setup Dynamic Gameplay UI
- Refugio Nocturno > Capture Dynamic Gameplay Layout
- Refugio Nocturno > Apply Captured Dynamic Gameplay Layout
- Refugio Nocturno > Sync NPC Sprites To Resources
- Refugio Nocturno > Configure NPC Sprites
- Refugio Nocturno > Cleanup Duplicate Base UI

NO usar:
- Refugio Nocturno > Build Main Scenario, salvo pedido explícito.

FLUJO ACTUAL
El juego ya tiene 2 noches y 8 visitantes:
Noche 1:
- Elena
- Tomas
- Mara
- Andres

Noche 2:
- Julia
- Nadie
- Bruno
- Clara

Sprites están en:
- Assets/Npcs
- Assets/Resources/Npcs

Gameplay actual:
1. Inicia noche.
2. Aparece visitante.
3. Se muestra sprite, nombre, diálogo, observaciones y preguntas.
4. El jugador puede hacer preguntas limitadas.
5. Decide PERMITIR o NO PERMITIR.
6. Se aplican consecuencias a comida, seguridad, moral y población.
7. Se muestra feedback narrativo breve.
8. Avanza al siguiente visitante.
9. Al terminar la noche aparece resumen.
10. Botón CONTINUAR pasa a noche 2.
11. Hay victoria si sobrevive las dos noches.
12. Hay derrota si seguridad o moral llegan a 0, o comida llega a 0 al final de la noche.

OBJETIVO DE DESARROLLO
Llevar este prototipo hacia una experiencia mucho más completa, pulida y cercana a un juego comercial premium/AAA, pero manteniendo alcance realista para Unity y el proyecto actual.

Usá principios de UI/UX inspirados en Celia Hodent:
- reducir carga cognitiva innecesaria,
- hacer que la información importante sea clara,
- dar feedback inmediato y legible,
- guiar la atención del jugador,
- evitar confusión accidental,
- diseñar decisiones tensas pero entendibles,
- reforzar motivación, agencia y claridad,
- mejorar accesibilidad y legibilidad,
- usar affordances claras en botones, paneles y estados.

PILARES DE DISEÑO
1. Tensión por información incompleta.
2. Deducción clara pero nunca obvia.
3. Consecuencias acumulativas.
4. Interfaz diegética: todo debe sentirse como parte del refugio.
5. Decisiones morales ambiguas.
6. Terror psicológico sobrio, sin depender de jumpscares.
7. Partidas cortas pero rejugables.
8. El jugador debe sentir: “creo que sé qué hacer, pero puedo estar equivocado”.

ROADMAP DE IMPLEMENTACIÓN

FASE 1 - ESTABILIDAD Y UX BASE
Primero revisar:
- que no haya objetos duplicados activos en Canvas,
- que todos los textos runtime estén conectados,
- que EndNightSummaryPanel funcione,
- que ContinueButton avance,
- que VisitorImage muestre todos los sprites,
- que NightStatic y ClockStatic cambien por noche.

Mejorar:
- feedback visual de botones al hover/click,
- bloqueo de botones mientras se resuelve una decisión,
- estado claro de preguntas restantes,
- mejor legibilidad de diálogo, observaciones y resumen,
- evitar textos cortados o superpuestos.

FASE 2 - OBSERVACIÓN COMO MECÁNICA REAL
Expandir ObservationProfile.
Cada visitante debe tener campos estructurados:
- ropa mojada,
- temblor,
- mirada evasiva,
- heridas visibles,
- comportamiento,
- respuestas,
- temperatura corporal,
- respiración,
- reacción al micrófono,
- coherencia de memoria,
- riesgo estimado.

El panel de observaciones debe cambiar dinámicamente.
Agregar señales ambiguas:
- humanos pueden parecer sospechosos,
- imitadores pueden imitar algunas señales humanas,
- algunas reglas de noche pueden contradecir intuición básica.

FASE 3 - SISTEMA DE SOSPECHA
Agregar un sistema visible o semi-visible de sospecha:
- no debe decir “es imitador”,
- debe ayudar a ordenar pistas,
- puede tener estados: Bajo, Medio, Alto, Desconocido.
La sospecha debe calcularse a partir de observaciones, respuestas y regla de la noche.
Debe ser configurable, no hardcodeada de forma rígida.

FASE 4 - REGLAS DE NOCHE MÁS FUERTES
Cada noche debe tener una regla jugable real, no solo texto.
Ejemplos:
- “Los imitadores evitan nombres propios.”
- “Los imitadores no tiemblan bajo la lluvia.”
- “Los imitadores repiten estructuras de frase.”
- “Algunos humanos heridos mienten por miedo.”
- “Si alguien menciona un refugio inexistente, sospechá.”

La regla debe influir en la deducción y en el score de sospecha.

FASE 5 - SISTEMA DE CONSECUENCIAS MÁS DRAMÁTICO
Además de modificar recursos, cada decisión debería producir:
- evento narrativo corto,
- cambio de estado del refugio,
- posible efecto retardado,
- impacto en población,
- impacto en confianza interna.

Ejemplos:
- si entra un imitador, puede dañar seguridad más tarde,
- si rechazás demasiados humanos, baja moral,
- si aceptás demasiada gente, baja comida más rápido,
- si seguridad es baja, algunas observaciones fallan.

FASE 6 - EVENTOS ENTRE VISITANTES
Agregar eventos breves entre visitantes:
- ruido en los ductos,
- alguien del refugio protesta,
- radio intermitente,
- corte de luz parcial,
- golpe en una puerta interior,
- persona aceptada da información,
- rumor falso se propaga.

Estos eventos deben afectar:
- moral,
- seguridad,
- comida,
- reglas percibidas,
- tensión narrativa.

FASE 7 - MEJORAR EL SISTEMA DE PREGUNTAS
Las preguntas deben tener más peso.
Agregar:
- categorías de preguntas,
- preguntas desbloqueadas por noche,
- preguntas que revelan contradicciones,
- preguntas que consumen tiempo,
- límite por visitante,
- respuestas con tags internos: coherente, evasiva, contradictoria, peligrosa.

El jugador no debe poder preguntar todo. Debe elegir.

FASE 8 - TIEMPO Y PRESIÓN
Agregar reloj funcional por noche.
Cada visitante consume tiempo.
Cada pregunta consume minutos.
Cada decisión consume minutos.
Al llegar a cierta hora termina la noche o aumenta el peligro.

El reloj no debe apurar de forma arcade al principio. Debe aumentar tensión gradualmente.

FASE 9 - ARCHIVO / REGISTRO
Crear un panel de registro:
- visitantes vistos,
- decisión tomada,
- pistas observadas,
- consecuencias,
- noche,
- resultado.
Esto ayuda al jugador a aprender y mejora la sensación de sistema profundo.

FASE 10 - CONTENIDO
Expandir a:
- 3 o 4 noches,
- 12 a 20 visitantes,
- visitantes con arcos,
- personajes que mencionan eventos previos,
- imitadores que copian nombres de humanos anteriores,
- humanos que regresan o son mencionados después.

No hacer todo de golpe. Primero preparar la arquitectura para extender.

FASE 11 - ARQUITECTURA
Migrar progresivamente datos hardcodeados de ExampleVisitorFactory a ScriptableObjects o una base de datos editable.
Pero hacerlo con cuidado:
- mantener ExampleVisitorFactory como fallback,
- no romper Play Mode,
- permitir configurar visitantes desde Inspector,
- permitir crear noches desde assets.

FASE 12 - AUDIO Y ATMÓSFERA
Agregar sistema simple de audio:
- lluvia loop,
- golpe en ventana,
- click de botones,
- radio,
- zumbido eléctrico,
- sonido al aceptar/rechazar,
- sonido especial si entra un imitador.

Debe ser opcional y tolerante a clips faltantes.

FASE 13 - JUICE VISUAL
Agregar polish sin depender de assets externos:
- fade entre visitantes,
- parpadeo leve de luz,
- leve shake al golpe,
- oscurecimiento si baja seguridad,
- color de UI según peligro,
- animación sutil del visitante al aparecer,
- transición de resumen.

Todo debe respetar la estética oscura actual.

FASE 14 - ACCESIBILIDAD
Agregar:
- opción de texto grande,
- contraste alto,
- desactivar parpadeos,
- velocidad de texto instantánea,
- controles claros con mouse,
- estados visuales además de color.

FASE 15 - QA Y BALANCE
Crear checklist de prueba:
- noche 1 completa,
- noche 2 completa,
- victoria,
- derrota por seguridad,
- derrota por moral,
- derrota por comida,
- todos los visitantes muestran sprite,
- todas las preguntas funcionan,
- resumen muestra datos correctos,
- ContinueButton funciona,
- no hay textos superpuestos.

FORMA DE TRABAJO
Trabajá por incrementos.
Para cada cambio:
1. Inspeccioná archivos existentes.
2. Decí qué vas a cambiar.
3. Implementá el cambio.
4. Evitá refactors innecesarios.
5. No rompas layout manual.
6. Si agregás UI dinámica, hacerla compatible con Capture Dynamic Gameplay Layout.
7. Si creás objetos nuevos, nombrarlos claramente.
8. Si hay riesgo de romper escena, preferir auto-binding por nombre.
9. Verificá referencias y compilación cuando sea posible.
10. Explicá al final cómo probarlo.

PRIORIDAD INMEDIATA
Primero mejorar la jugabilidad central, no hacer cosmética vacía.

Orden sugerido para empezar:
1. Sistema de sospecha por visitante.
2. Reglas de noche que afecten sospecha.
3. Feedback claro de pistas contradictorias.
4. Registro de visitantes.
5. Eventos entre visitantes.
6. Reloj funcional.
7. Expandir visitantes/noches.
8. Audio y polish visual.

CRITERIO DE CALIDAD
El resultado debe sentirse:
- oscuro,
- legible,
- tenso,
- claro,
- jugable,
- extensible,
- sin sobrecargar al jugador,
- con decisiones que importan.

Recordá: el objetivo no es solo agregar features. Es convertir el prototipo en un videojuego con loop fuerte, claridad de UX, tensión creciente, decisiones significativas y arquitectura preparada para crecer.