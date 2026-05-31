using System.Collections.Generic;
using UnityEngine;

public static class ExampleVisitorFactory
{
    public static List<NightData> CreateExampleNights()
    {
        return new List<NightData>
        {
            new NightData
            {
                nightLabel = "NOCHE 1",
                clockTime = "01:47 AM",
                rule = "Los imitadores suelen contradecirse, pero algunos humanos tambien parecen nerviosos.",
                suspicionRule = new NightRuleData
                {
                    description = "Los imitadores suelen contradecirse, pero algunos humanos tambien parecen nerviosos.",
                    ruleType = NightRuleType.ImitatorsAvoidSpecificDetails,
                    suspicionOnViolation = 2,
                    reliefOnCompliance = 1
                },
                visitors = new List<VisitorData>
                {
                    CreateVisitor(
                        "Elena",
                        false,
                        "Por favor, abrime. Hay algo siguiendome.",
                        new[] { "ropa mojada", "temblor", "herida visible" },
                        Obs("SI", "SI", "NO", "SI", "ASUSTADA", "COHERENTES"),
                        new[]
                        {
                            QA("De donde venis?", "Del hospital viejo.", ResponseTag.Coherent),
                            QA("Que paso afuera?", "No pude verlos bien, solo escuche pasos.", ResponseTag.Coherent),
                            QA("Conoces a alguien dentro?", "Mi hermano deberia estar aca.", ResponseTag.Coherent)
                        },
                        -1, 0, 1, 1,
                        0, 0, -2, 0,
                        "Elena entra temblando. Alguien en el refugio la reconoce.\nComida -1 | Moral +1 | Poblacion +1",
                        "Elena golpea el vidrio una ultima vez antes de perderse en la lluvia.\nMoral -2"),
                    CreateVisitor(
                        "Tomas",
                        true,
                        "Necesito entrar. Soy humano. Tengo frio.",
                        new[] { "mirada fija", "ropa seca pese a la lluvia", "respuestas repetitivas" },
                        Obs("NO", "NO", "NO", "NO", "QUIETO", "REPETITIVAS"),
                        new[]
                        {
                            QA("De donde venis?", "De afuera.", ResponseTag.Evasive),
                            QA("Que paso afuera?", "Nada. Solo necesito entrar.", ResponseTag.Evasive),
                            QA("Conoces a alguien dentro?", "Si. A todos.", ResponseTag.Contradictory)
                        },
                        0, -3, -1, 0,
                        0, 1, 0, 0,
                        "Tomas cruza el umbral sin mirar a nadie. La cerradura queda fria.\nSeguridad -3 | Moral -1",
                        "Tomas deja de fingir frio y se queda inmovil bajo la lluvia.\nSeguridad +1"),
                    CreateVisitor(
                        "Mara",
                        false,
                        "No tengo mucho tiempo. Dejame pasar.",
                        new[] { "nerviosa", "cansada", "manos manchadas" },
                        Obs("SI", "NO", "SI", "NO", "NERVIOSA", "COHERENTES"),
                        new[]
                        {
                            QA("De donde venis?", "De la estacion.", ResponseTag.Coherent),
                            QA("Que paso afuera?", "El camino esta bloqueado.", ResponseTag.Coherent),
                            QA("Por que necesitas entrar?", "Tengo informacion sobre los imitadores.", ResponseTag.Coherent)
                        },
                        -1, 1, 0, 1,
                        0, -1, -1, 0,
                        "Mara entrega un mapa mojado antes de sentarse contra la pared.\nComida -1 | Seguridad +1 | Poblacion +1",
                        "Mara se va con la informacion que necesitabas.\nSeguridad -1 | Moral -1"),
                    CreateVisitor(
                        "Andres",
                        true,
                        "Me mandaron desde el refugio norte.",
                        new[] { "no parpadea", "voz calma", "sin heridas" },
                        Obs("NO", "NO", "NO", "NO", "CALMO", "INCONSISTENTES"),
                        new[]
                        {
                            QA("De donde venis?", "Del refugio norte.", ResponseTag.Coherent),
                            QA("Que paso afuera?", "Nada importante.", ResponseTag.Evasive),
                            QA("Conoces a alguien dentro?", "Conozco a Elena.", ResponseTag.Contradictory)
                        },
                        -1, -2, 0, 0,
                        0, 1, 0, 0,
                        "Andres sonrie al oir el cerrojo. Nadie del refugio norte contesta la radio.\nComida -1 | Seguridad -2",
                        "Andres espera demasiado quieto. Luego desaparece entre las luces.\nSeguridad +1")
                },
                interEvents = new List<InterVisitorEvent>
                {
                    new InterVisitorEvent
                    {
                        eventType = InterEventType.NoiseInDucts,
                        narrativeText = "Un ruido metalico recorre los ductos. Alguien contiene la respiracion.",
                        securityChange = 0,
                        moraleChange = -1,
                        probability = 0.7f
                    },
                    new InterVisitorEvent
                    {
                        eventType = InterEventType.IntermittentRadio,
                        narrativeText = "La radio emite estatica. Por un momento, parece que alguien dice un nombre.",
                        securityChange = 0,
                        moraleChange = 0,
                        probability = 0.5f
                    }
                }
            },
            new NightData
            {
                nightLabel = "NOCHE 2",
                clockTime = "03:12 AM",
                rule = "Esta noche los imitadores evitan nombres propios. No todos los heridos son peligrosos.",
                suspicionRule = new NightRuleData
                {
                    description = "Los imitadores evitan nombres propios. No todos los heridos son peligrosos.",
                    ruleType = NightRuleType.ImitatorsAvoidProperNames,
                    suspicionOnViolation = 3,
                    reliefOnCompliance = 1
                },
                visitors = new List<VisitorData>
                {
                    CreateVisitor(
                        "Julia",
                        false,
                        "Traigo vendas. Si me dejan entrar puedo ayudar.",
                        new[] { "abrigo rasgado", "pulso agitado", "mochila medica" },
                        Obs("SI", "NO", "NO", "SI", "AGITADA", "COHERENTES"),
                        new[]
                        {
                            QA("De donde venis?", "Del puesto de guardia de San Mateo.", ResponseTag.Coherent),
                            QA("Que recordas antes de llegar?", "La sirena, despues la lluvia y una puerta golpeando.", ResponseTag.Coherent),
                            QA("Conoces a alguien dentro?", "A Ruben, el enfermero.", ResponseTag.Coherent)
                        },
                        -1, 1, 1, 1,
                        0, 0, -2, 0,
                        "Julia reparte vendas y baja la voz para calmar al grupo.\nComida -1 | Seguridad +1 | Moral +1 | Poblacion +1",
                        "Julia deja la mochila en la puerta antes de irse. Nadie se anima a abrirla.\nMoral -2"),
                    CreateVisitor(
                        "Nadie",
                        true,
                        "Mi nombre no importa. Abran.",
                        new[] { "sonrisa inmovil", "voz sin aire", "manos limpias" },
                        Obs("NO", "NO", "NO", "NO", "EXTRANIO", "EVASIVAS"),
                        new[]
                        {
                            QA("De donde venis?", "De donde vienen todos.", ResponseTag.Evasive),
                            QA("Que paso afuera?", "La noche paso.", ResponseTag.Evasive),
                            QA("Conoces a alguien dentro?", "Conozco sus voces.", ResponseTag.Dangerous)
                        },
                        -1, -3, -2, 0,
                        0, 1, 0, 0,
                        "La figura entra sin decir su nombre. Las voces del refugio empiezan a sonar ajenas.\nComida -1 | Seguridad -3 | Moral -2",
                        "La figura se retira cuando le pedis un nombre. La puerta aguanta.\nSeguridad +1"),
                    CreateVisitor(
                        "Bruno",
                        false,
                        "Me perdi cuando apagaron las luces del barrio.",
                        new[] { "barro en las botas", "mirada evasiva", "tiembla al hablar" },
                        Obs("SI", "SI", "SI", "NO", "ASUSTADO", "COHERENTES"),
                        new[]
                        {
                            QA("De donde venis?", "De las casas bajas, cerca del puente.", ResponseTag.Coherent),
                            QA("Que paso afuera?", "Alguien imitaba la voz de mi madre.", ResponseTag.Coherent),
                            QA("Por que necesitas entrar?", "No puedo seguir corriendo.", ResponseTag.Coherent)
                        },
                        -1, 0, 1, 1,
                        0, 0, -1, 0,
                        "Bruno cae de rodillas apenas entra. Sigue repitiendo que no era su madre.\nComida -1 | Moral +1 | Poblacion +1",
                        "Bruno corre de nuevo hacia el barrio apagado.\nMoral -1"),
                    CreateVisitor(
                        "Clara",
                        true,
                        "Soy Clara. Soy Clara. Soy Clara.",
                        new[] { "repite su nombre", "ropa empapada sin gotear", "mirada demasiado quieta" },
                        Obs("SI", "NO", "NO", "NO", "REPETITIVA", "INCONSISTENTES"),
                        new[]
                        {
                            QA("De donde venis?", "Clara viene de Clara.", ResponseTag.Contradictory),
                            QA("Que recordas antes de llegar?", "Entrar. Entrar. Entrar.", ResponseTag.Dangerous),
                            QA("Conoces a alguien dentro?", "A Elena. A Tomas. A todos.", ResponseTag.Contradictory)
                        },
                        0, -4, -1, 0,
                        0, 1, 1, 0,
                        "Clara entra repitiendo su nombre hasta que otros empiezan a repetirlo tambien.\nSeguridad -4 | Moral -1",
                        "Clara golpea el vidrio con el mismo ritmo exacto hasta alejarse.\nSeguridad +1 | Moral +1")
                },
                interEvents = new List<InterVisitorEvent>
                {
                    new InterVisitorEvent
                    {
                        eventType = InterEventType.ShelterProtest,
                        narrativeText = "Alguien dentro del refugio grita: 'No dejen entrar a nadie mas!'",
                        moraleChange = -1,
                        probability = 0.6f
                    },
                    new InterVisitorEvent
                    {
                        eventType = InterEventType.PartialBlackout,
                        narrativeText = "Las luces parpadean y se apagan un instante. Cuando vuelven, algo parece distinto.",
                        securityChange = -1,
                        probability = 0.5f,
                        minSecurityToTrigger = 3
                    },
                    new InterVisitorEvent
                    {
                        eventType = InterEventType.AcceptedPersonInfo,
                        narrativeText = "Alguien que dejaste entrar antes se acerca: 'El ultimo... no era como nosotros.'",
                        securityChange = 0,
                        moraleChange = 0,
                        probability = 0.8f,
                        requiresPreviousAccepted = true
                    }
                }
            },
            // === NOCHE 3 ===
            new NightData
            {
                nightLabel = "NOCHE 3",
                clockTime = "04:33 AM",
                rule = "Los imitadores aprendieron a usar nombres propios, pero fallan al recordar lugares concretos.",
                suspicionRule = new NightRuleData
                {
                    description = "Los imitadores usan nombres propios, pero fallan al recordar lugares.",
                    ruleType = NightRuleType.ImitatorsUseNamesButFailPlaces,
                    suspicionOnViolation = 3,
                    reliefOnCompliance = 1
                },
                visitors = new List<VisitorData>
                {
                    CreateVisitor(
                        "Diego",
                        false,
                        "Por favor... vengo del puente de la Avenida San Martin. Esta destruido.",
                        new[] { "rodillas raspadas", "respira con dificultad", "manos con barro" },
                        Obs("SI", "SI", "NO", "SI", "AGITADO", "COHERENTES",
                            "Tiembla al hablar", "Agitada", "NORMAL"),
                        new[]
                        {
                            QA("De donde venis?", "Del puente sobre la Avenida San Martin, cerca del molino viejo.", ResponseTag.Coherent),
                            QA("Que paso afuera?", "El puente cayo. Vi a tres personas caer al rio. Una era Lucia, la maestra.", ResponseTag.Coherent),
                            QA("Conoces a alguien dentro?", "A Julia. Trabajamos juntos en el puesto de San Mateo.", ResponseTag.Coherent)
                        },
                        new[]
                        {
                            QA("Que escuchaste en la radio?", "Una voz repitiendo coordenadas. Creo que era del refugio norte.", ResponseTag.Coherent)
                        },
                        -1, 0, 1, 1,
                        0, 0, -1, 0,
                        "Diego entra arrastrando la pierna. Julia lo reconoce de inmediato.\nComida -1 | Moral +1 | Poblacion +1",
                        "Diego intenta caminar pero la pierna no le responde. Se sienta junto a la puerta bajo la lluvia.\nMoral -1"),
                    CreateVisitor(
                        "Renata",
                        true,
                        "Soy Renata. Vengo de parte de Marcos, del refugio norte.",
                        new[] { "sonrisa leve", "ropa limpia pese al barro afuera", "voz demasiado calma" },
                        Obs("NO", "NO", "NO", "NO", "CALMA", "INCONSISTENTES",
                            "Voz demasiado calma", "Mecanica", "FRIA"),
                        new[]
                        {
                            QA("De donde venis?", "Del refugio norte. Marcos me mando.", ResponseTag.Coherent),
                            QA("Donde queda el refugio norte?", "Cerca de... las casas. Las casas grandes.", ResponseTag.Evasive),
                            QA("Conoces a alguien dentro?", "A Elena. Y a Bruno. Los conozco bien.", ResponseTag.Contradictory)
                        },
                        new[]
                        {
                            QA("Que dijo la radio sobre el refugio norte?", "No se de que radio hablas.", ResponseTag.Evasive)
                        },
                        0, -3, -1, 0,
                        0, 1, 0, 0,
                        "Renata entra con paso firme. Nadie del refugio norte ha respondido la radio en dos noches.\nSeguridad -3 | Moral -1",
                        "Renata deja de sonreir cuando le pedis que describa el camino. Se retira sin insistir.\nSeguridad +1"),
                    CreateVisitor(
                        "Fabian",
                        false,
                        "No aguanto mas afuera. Escucho cosas que no deberian estar ahi.",
                        new[] { "ojos rojos", "manos tapando orejas", "campera empapada" },
                        Obs("SI", "SI", "SI", "NO", "PARANOICO", "COHERENTES",
                            "Tiembla al hablar", "Agitada", "NORMAL"),
                        new[]
                        {
                            QA("De donde venis?", "De la terminal de colectivos de la calle Rivadavia.", ResponseTag.Coherent),
                            QA("Que paso afuera?", "Algo imita voces. Escuche a mi hermano llamarme, pero el murio hace un año.", ResponseTag.Coherent),
                            QA("Por que necesitas entrar?", "Si sigo escuchando esas voces voy a abrir la puerta yo mismo.", ResponseTag.Coherent)
                        },
                        new[]
                        {
                            QA("Que escuchaste en la radio?", "Interferencia. Y despues una voz que decia mi nombre.", ResponseTag.Coherent)
                        },
                        -1, 0, 0, 1,
                        0, -1, -1, 0,
                        "Fabian se sienta en un rincon tapandose los oidos. No habla con nadie.\nComida -1 | Poblacion +1",
                        "Fabian corre hacia la oscuridad gritando que ya no le importa.\nSeguridad -1 | Moral -1"),
                    CreateVisitor(
                        "Sombra",
                        true,
                        "Me llamo Lucas. Vengo del hospital de la zona sur.",
                        new[] { "no parpadea", "nombra lugares pero no describe caminos", "sin marcas de lluvia" },
                        Obs("NO", "NO", "NO", "NO", "INMOVIL", "EVASIVAS",
                            "Repite cadencia", "Ausente", "IRREGULAR"),
                        new[]
                        {
                            QA("De donde venis?", "Del hospital. El de la zona sur. Con Daniela y Roberto.", ResponseTag.Coherent),
                            QA("Como llegaste hasta aca?", "Camine. Por las calles. Las calles de aca.", ResponseTag.Evasive),
                            QA("Que habia en el camino?", "Cosas. Cosas normales. Nada fuera de lugar.", ResponseTag.Contradictory)
                        },
                        new[]
                        {
                            QA("La radio menciono sobrevivientes del hospital?", "Si. Exacto. Eso. Sobrevivientes.", ResponseTag.Evasive)
                        },
                        -1, -4, -2, 0,
                        0, 2, 0, 0,
                        "Lucas entra nombrando gente que nadie conoce. Las luces del pasillo parpadean una vez.\nComida -1 | Seguridad -4 | Moral -2",
                        "Lucas se queda mirando la puerta cerrada. Despues de un minuto, desaparece de la ventana.\nSeguridad +2")
                },
                interEvents = new List<InterVisitorEvent>
                {
                    new InterVisitorEvent
                    {
                        eventType = InterEventType.InteriorDoorKnock,
                        narrativeText = "Alguien golpea una puerta interior que deberia estar cerrada con llave.",
                        securityChange = -1,
                        moraleChange = -1,
                        probability = 0.7f
                    },
                    new InterVisitorEvent
                    {
                        eventType = InterEventType.FalseRumor,
                        narrativeText = "Corre un rumor entre los residentes: 'Dicen que el operador dejo entrar a uno de ellos.'",
                        moraleChange = -2,
                        probability = 0.6f,
                        requiresPreviousAccepted = true
                    },
                    new InterVisitorEvent
                    {
                        eventType = InterEventType.DistantScream,
                        narrativeText = "Un grito lejano atraviesa las paredes del refugio. Nadie sabe si viene de adentro o de afuera.",
                        moraleChange = -1,
                        securityChange = -1,
                        probability = 0.5f
                    },
                    new InterVisitorEvent
                    {
                        eventType = InterEventType.SilenceBreak,
                        narrativeText = "El silencio se quiebra de golpe. Un ruido como de algo arrastrándose en el techo.",
                        securityChange = 0,
                        moraleChange = -1,
                        probability = 0.8f
                    }
                }
            }
        };
    }

    private static VisitorData CreateVisitor(
        string visitorName,
        bool isImitator,
        string introDialogue,
        string[] visualClues,
        ObservationProfile observationProfile,
        QuestionAnswer[] answers,
        int foodAccept,
        int securityAccept,
        int moraleAccept,
        int populationAccept,
        int foodReject,
        int securityReject,
        int moraleReject,
        int populationReject,
        string feedbackOnAccept,
        string feedbackOnReject)
    {
        return CreateVisitor(visitorName, isImitator, introDialogue, visualClues,
            observationProfile, answers, null,
            foodAccept, securityAccept, moraleAccept, populationAccept,
            foodReject, securityReject, moraleReject, populationReject,
            feedbackOnAccept, feedbackOnReject);
    }

    private static VisitorData CreateVisitor(
        string visitorName,
        bool isImitator,
        string introDialogue,
        string[] visualClues,
        ObservationProfile observationProfile,
        QuestionAnswer[] answers,
        QuestionAnswer[] radioQuestions,
        int foodAccept,
        int securityAccept,
        int moraleAccept,
        int populationAccept,
        int foodReject,
        int securityReject,
        int moraleReject,
        int populationReject,
        string feedbackOnAccept,
        string feedbackOnReject)
    {
        VisitorData visitor = ScriptableObject.CreateInstance<VisitorData>();
        visitor.visitorName = visitorName;
        visitor.isImitator = isImitator;
        visitor.visitorSprite = LoadVisitorSprite(visitorName);
        visitor.introDialogue = introDialogue;
        visitor.visualClues.AddRange(visualClues);
        visitor.observationProfile = observationProfile;
        visitor.answers.AddRange(answers);
        if (radioQuestions != null)
        {
            visitor.radioQuestions.AddRange(radioQuestions);
        }
        visitor.foodChangeOnAccept = foodAccept;
        visitor.securityChangeOnAccept = securityAccept;
        visitor.moraleChangeOnAccept = moraleAccept;
        visitor.populationChangeOnAccept = populationAccept;
        visitor.foodChangeOnReject = foodReject;
        visitor.securityChangeOnReject = securityReject;
        visitor.moraleChangeOnReject = moraleReject;
        visitor.populationChangeOnReject = populationReject;
        visitor.feedbackOnAccept = feedbackOnAccept;
        visitor.feedbackOnReject = feedbackOnReject;
        return visitor;
    }

    private static ObservationProfile Obs(string wetClothes, string tremor, string evasiveLook, string visibleWounds, string behavior, string answers)
    {
        return new ObservationProfile
        {
            wetClothes = wetClothes,
            tremor = tremor,
            evasiveLook = evasiveLook,
            visibleWounds = visibleWounds,
            behavior = behavior,
            answers = answers
        };
    }

    private static ObservationProfile Obs(string wetClothes, string tremor, string evasiveLook, string visibleWounds, string behavior, string answers, string voiceTone, string breathingPattern, string bodyTemperature)
    {
        return new ObservationProfile
        {
            wetClothes = wetClothes,
            tremor = tremor,
            evasiveLook = evasiveLook,
            visibleWounds = visibleWounds,
            behavior = behavior,
            answers = answers,
            voiceTone = voiceTone,
            breathingPattern = breathingPattern,
            bodyTemperature = bodyTemperature
        };
    }

    private static QuestionAnswer QA(string question, string answer, ResponseTag tag = ResponseTag.Unknown)
    {
        return new QuestionAnswer
        {
            question = question,
            answer = answer,
            responseTag = tag
        };
    }

    private static Sprite LoadVisitorSprite(string visitorName)
    {
        Sprite sprite = Resources.Load<Sprite>($"Npcs/{visitorName}");
        if (sprite != null)
        {
            return sprite;
        }

        Sprite[] sprites = Resources.LoadAll<Sprite>($"Npcs/{visitorName}");
        if (sprites != null && sprites.Length > 0)
        {
            return sprites[0];
        }

        Texture2D texture = Resources.Load<Texture2D>($"Npcs/{visitorName}");
        if (texture != null)
        {
            return Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f);
        }

        return null;
    }
}
