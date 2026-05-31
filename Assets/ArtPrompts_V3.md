# Art Prompts - Refugio Nocturno V3
## Imagenes faltantes para eventos, personajes de Noche 3 e icono de mejora

Estos prompts mantienen coherencia con el escenario principal del juego: una cabina/refugio vista desde una interfaz fija, con ventana central, lluvia nocturna, madera vieja, metal gastado, paneles oscuros, luz amarilla sucia y atmosfera de terror psicologico.

### Referencia visual principal
Usar como referencia el panel actual del videojuego, donde ocurre el loop principal:
- cabina interior abandonada,
- ventana grande con lluvia y calle oscura afuera,
- escritorio de madera,
- radio vieja,
- paneles metalicos,
- lampara calida,
- botones fisicos verdes/rojos,
- textura gastada, humedad y oxido,
- composicion oscura, legible y cinematica.

Los eventos NO deben parecer escenas genericas de otro juego. Deben sentirse como momentos que ocurren dentro del mismo refugio, en la misma noche, con la misma direccion de arte. Algunos eventos pueden mostrar otros rincones del refugio, como pasillos, ductos, puertas interiores o radio, pero siempre deben compartir el mismo lenguaje visual del panel principal.

---

## FONDOS DE EVENTOS
Destino: `Assets/Resources/UI/Events/`

Formato recomendado:
- PNG.
- 1920x1080.
- 16:9.
- Sin texto.
- Sin logos.
- Sin UI dibujada.
- Deben funcionar como fondo de panel semitransparente.
- Dejar zonas oscuras o de bajo detalle donde pueda ir texto encima.
- Evitar caras demasiado definidas salvo que el evento lo pida.
- Mantener la imagen legible incluso con overlay oscuro encima.

---

### 1. event_radio.png
**Uso en juego:** evento `IntermittentRadio`. La radio del refugio emite estatica y parece decir un nombre.

**Prompt:**
Dark psychological horror event background set inside the same abandoned night shelter control booth as the main gameplay panel, using the main game scene as visual reference. Close-up cinematic view of an old battered radio sitting on a worn wooden desk, its analog display glowing faint green through dust and scratches. Thin radio wires crawl into darkness, a chipped enamel mug and damp papers nearby, rain reflections trembling on the tabletop from the large window behind it. The room is dim, with dirty amber lamp light from one side and cold blue rain light from the window, creating strong low-key contrast. The radio should feel alive but not supernatural in an obvious way, as if a voice is almost forming inside the static. Gritty painterly realism, muted yellow-green and blue tones, wet surfaces, rusted metal, dust, cracked shelter walls, cinematic composition with empty dark space on the right for text overlay, no text, no symbols, no logos, no UI labels, 16:9, suitable as a semi-transparent in-game event panel background.

---

### 2. event_person_info.png
**Uso en juego:** evento `AcceptedPersonInfo`. Alguien aceptado antes se acerca y advierte que el ultimo visitante no era normal.

**Prompt:**
Dark psychological horror event background inside the same abandoned shelter as the main gameplay panel, visually consistent with the control booth scene. A narrow interior hallway just behind the reception booth, lit by a weak amber emergency bulb and cold rain light leaking through a small barred window. In the foreground, a half-visible survivor silhouette leans close as if whispering urgent information, while another blurred figure listens in fear. The figures must remain shadowy and anonymous, not detailed portraits. Worn concrete walls, peeling paint, water stains, exposed cables, old notice boards, wet footprints leading back toward the booth. Mood of paranoia and mistrust, like a secret warning exchanged between visitors. Gritty painterly realism, cinematic low-key lighting, muted browns, sickly amber and cold blue, strong negative space on the upper right for text overlay, no text, no logos, no UI, 16:9, suitable as a semi-transparent event panel background.

---

### 3. event_door_knock.png
**Uso en juego:** evento `InteriorDoorKnock`. Algo golpea una puerta interior del refugio.

**Prompt:**
Dark psychological horror event background set inside the same shelter universe as the main gameplay panel. A heavy interior metal door viewed from inside the refuge, close to the control booth, with old bolts, rust, scratched paint and a small reinforced observation slit. Fresh dents and subtle vibration marks suggest something is knocking from the other side, but the source is unseen. The floor is damp, reflecting dim red emergency light and cold blue rain glow from a distant window. Include small environmental details from the main booth style: industrial metal trim, worn wood edge, dirty concrete, hanging cable, dust and moisture. The composition should create dread through restraint, not a monster reveal. Leave the lower third slightly darker for text overlay. Gritty painterly realism, cinematic low-key lighting, muted red, blue and brown palette, no text, no logos, no UI labels, 16:9, suitable as a semi-transparent event panel background.

---

### 4. event_false_rumor.png
**Uso en juego:** evento `FalseRumor`. Un rumor falso empieza a circular entre la gente del refugio.

**Prompt:**
Dark psychological horror event background inside the same abandoned refuge, using the main gameplay panel as style reference. A cramped common corner of the shelter, with several shadowy survivors huddled near a wall covered in damp papers and old refuge notices. One figure points toward the viewer or toward the unseen booth window, while others whisper with tense body language. Their faces should be partially hidden by shadow, suggesting fear, suspicion and misinformation rather than clear identities. A flickering overhead bulb casts sickly amber light, while cold rain-blue light enters through a small dirty window. Wet coats, metal cups, cracked tiles, stained concrete, old radio cable, and worn wooden crates echo the main booth materials. Composition should feel claustrophobic and paranoid, with central figures and darker empty space along the bottom for text overlay. Gritty painterly realism, muted desaturated warm tones, no text, no logos, no UI labels, 16:9, suitable as a semi-transparent event panel background.

---

### 5. event_distant_scream.png
**Uso en juego:** evento `DistantScream`. Se escucha un grito lejano dentro o fuera del refugio.

**Prompt:**
Dark psychological horror event background connected to the main shelter booth environment. A long abandoned corridor stretching away from the control room into deep darkness, with wet concrete floor, exposed pipes, old ceiling panels and weak emergency lights fading into the distance. At the far end, a barely visible human-like silhouette or open doorway suggests the source of a distant scream, but it must remain ambiguous and almost unreadable. Subtle visual distortion in dust and rain mist implies sound traveling through the corridor without showing literal sound waves. Cold blue rain light leaks from a broken side window, mixed with weak amber light from the booth behind the camera. Strong depth, atmospheric perspective, damp reflections, gritty painterly realism, cinematic low-key lighting, muted blue-gray palette, large dark negative space for text overlay, no text, no logos, no UI, 16:9, suitable as a semi-transparent event panel background.

---

### 6. event_silence_break.png
**Uso en juego:** evento `SilenceBreak`. Un silencio absoluto se rompe por un movimiento seco en los ductos o el techo.

**Prompt:**
Dark psychological horror event background set above the same control booth from the main gameplay panel. Upward-looking view of the shelter ceiling: old metal air ducts, cracked concrete, water stains, hanging dust, loose screws and a faint slit of darkness above the panels. A section of ductwork appears subtly bent or vibrating, with dust particles falling through a cone of dirty amber lamp light. The scene should communicate the exact moment after total silence is broken by a single movement overhead. Include hints of the main booth below, such as the top edge of the window frame, old lamp glow, rain-blue reflections and worn metal supports, but keep the focus on ceiling and ducts. Gritty painterly realism, restrained horror, muted earthy browns and cold blue shadows, cinematic low-key lighting, empty darker side area for text overlay, no text, no logos, no UI, 16:9, suitable as a semi-transparent event panel background.

---

## SPRITES DE PERSONAJES NOCHE 3
Destino: `Assets/Npcs/` y `Assets/Resources/Npcs/`

Formato recomendado:
- PNG.
- Fondo transparente.
- Personaje waist-up o cuerpo completo hasta rodillas.
- Frente al jugador.
- Luz fria de lluvia + leve luz calida del refugio.
- Mismo estilo que Elena, Tomas, Mara, Andres, Julia, Nadie, Bruno y Clara.
- Sin texto.
- Sin UI.
- No recortar cabeza ni manos si son importantes para las pistas.

---

### 7. Diego.png
**Rol sugerido:** humano. Parece sospechoso por panico y heridas, pero sus respuestas deben ser emocionalmente coherentes.

**Prompt:**
Waist-up front-facing character sprite for a dark psychological horror shelter game, transparent background. Diego, a young Latino man in his late 20s, exhausted and soaked by heavy rain, wearing a torn dark jacket over a stained hoodie. His hands are muddy and scraped, one sleeve ripped, knuckles bruised from climbing or crawling, wet hair stuck to his forehead, red tired eyes, shallow breathing, scared but determined expression. He looks directly toward the shelter window with fear and urgency, not aggression. Subtle injuries on cheek and knees visible if composition allows, damp fabric clinging to body, water droplets on shoulders. Lighting must match the main game booth: cold blue rain light from outside, faint warm amber light from inside the refuge hitting one side of his face. Gritty painterly realism, muted colors, horror survival mood, no text, no UI, no background, transparent PNG, consistent with existing NPC sprites.

---

### 8. Renata.png
**Rol sugerido:** imitadora sutil. Casi humana, con limpieza y calma demasiado perfectas.

**Prompt:**
Waist-up front-facing character sprite for a dark psychological horror shelter game, transparent background. Renata, woman in her early 30s, standing unnaturally calm in front of the refuge window. Her coat is suspiciously clean and only lightly damp despite the storm, posture straight and controlled, hands folded too neatly, pale skin, slight unsettling smile, eyes that do not quite focus on the viewer. Her face should look almost ordinary but subtly wrong: too still, too symmetrical, expression delayed, calm where fear should be. Hair tidy but with a few wet strands, clothing dark and practical, no obvious monster features. Lighting consistent with main game booth: cold rain-blue edge light, faint warm amber reflection from the shelter interior. Gritty painterly realism, muted palette, uncanny valley but believable human disguise, no text, no UI, no background, transparent PNG, consistent with existing NPC sprites.

---

### 9. Fabian.png
**Rol sugerido:** humano con panico extremo. Puede parecer peligroso por paranoia y respuestas fragmentadas.

**Prompt:**
Waist-up front-facing character sprite for a dark psychological horror shelter game, transparent background. Fabian, man in his mid 30s, soaked by rain, wearing a heavy dark jacket and scarf, messy wet hair, bloodshot eyes, hands pressed over his ears as if trying to block out voices. His expression is terrified, paranoid and sleep-deprived, looking slightly over his shoulder while still facing the viewer. He should appear unstable but human: trembling jaw, tense shoulders, wet clothing, mud on sleeves, small cuts on face. One hand may show dirt under fingernails, not clean. Lighting should match the main shelter window: cold blue rain light, warm amber rim from inside, deep shadows under eyes. Gritty painterly realism, muted blues and browns, psychological horror, no text, no UI, no background, transparent PNG, consistent with existing NPC sprites.

---

### 10. Sombra.png
**Rol sugerido:** imitador. Se presenta como Lucas, pero la identidad real de diseno es Sombra.

**Prompt:**
Waist-up front-facing character sprite for a dark psychological horror shelter game, transparent background. A figure calling itself Lucas, internally named Sombra, indeterminate age and gender presentation leaning male, wearing completely dry dark clothing despite the rain outside. Perfectly still posture, shoulders level, hands relaxed with unnatural symmetry, wide unblinking eyes, generic forgettable face that becomes more unsettling the longer it is observed. Features are slightly too symmetrical, skin tone muted and lifeless, mouth neutral as if copied from memory. Clothing should look ordinary but oddly untouched by mud or water. No explicit monster traits, no gore, no fantasy elements. Lighting consistent with the main game booth: cold blue exterior rain light and subtle warm amber interior reflection, but the figure absorbs light strangely. Gritty painterly realism, uncanny valley, restrained psychological horror, no text, no UI, no background, transparent PNG, consistent with existing NPC sprites.

---

## ICONO DE MEJORA

### 11. internal_archive.png
Destino: `Assets/Resources/UI/Upgrades/`

**Uso en juego:** mejora `InternalArchive`, archivo interno del refugio.

**Prompt:**
Small upgrade shop icon for a dark psychological horror shelter game, transparent background. An old metal filing cabinet drawer half-open, stained paper folders sticking out, a cracked magnifying glass resting on top, a small paper tag with no readable text, dust and rust on the edges. Warm amber light from the refuge lamp, dark desaturated background shadow, gritty painterly realism with slight game icon readability. Shape must remain clear at 64x64 and 128x128 sizes, strong silhouette, limited details, no readable text, no logos, no UI labels, transparent PNG or very dark transparent-friendly background.

---

## NOTAS DE DIRECCION DE ARTE

- Los eventos deben usar el panel principal como referencia de estilo y mundo.
- No todos los eventos necesitan mostrar exactamente la ventana principal; pueden mostrar radio, pasillos, puertas, ductos o zonas interiores del mismo refugio.
- Evitar escenarios externos amplios salvo que el evento lo justifique. El juego ocurre desde un refugio cerrado: la sensacion debe ser de encierro.
- Mantener continuidad: misma lluvia, misma noche, misma decadencia, mismo tipo de iluminacion.
- Las imagenes de evento deben apoyar texto encima, no competir con el texto.
- Los sprites de personajes deben poder ponerse frente a la ventana sin fondo y sin elementos que parezcan UI.

## CHECKLIST DE ENTREGA

- Generar cada imagen en PNG.
- Eventos: 1920x1080 o similar 16:9.
- NPCs: minimo 512px de alto, fondo transparente.
- Icono upgrade: 128x128 o 256x256, fondo transparente.
- Colocar eventos en `Assets/Resources/UI/Events/`.
- Colocar NPCs en `Assets/Npcs/` y copiar a `Assets/Resources/Npcs/`.
- Colocar icono en `Assets/Resources/UI/Upgrades/`.
- Abrir Unity y dejar que importe automaticamente.
- Revisar que cada imagen conserve legibilidad con overlay oscuro.
