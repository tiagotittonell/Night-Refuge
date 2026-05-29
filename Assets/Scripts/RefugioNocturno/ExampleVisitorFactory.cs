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
                            QA("De donde venis?", "Del hospital viejo."),
                            QA("Que paso afuera?", "No pude verlos bien, solo escuche pasos."),
                            QA("Conoces a alguien dentro?", "Mi hermano deberia estar aca.")
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
                            QA("De donde venis?", "De afuera."),
                            QA("Que paso afuera?", "Nada. Solo necesito entrar."),
                            QA("Conoces a alguien dentro?", "Si. A todos.")
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
                            QA("De donde venis?", "De la estacion."),
                            QA("Que paso afuera?", "El camino esta bloqueado."),
                            QA("Por que necesitas entrar?", "Tengo informacion sobre los imitadores.")
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
                            QA("De donde venis?", "Del refugio norte."),
                            QA("Que paso afuera?", "Nada importante."),
                            QA("Conoces a alguien dentro?", "Conozco a Elena.")
                        },
                        -1, -2, 0, 0,
                        0, 1, 0, 0,
                        "Andres sonrie al oir el cerrojo. Nadie del refugio norte contesta la radio.\nComida -1 | Seguridad -2",
                        "Andres espera demasiado quieto. Luego desaparece entre las luces.\nSeguridad +1")
                }
            },
            new NightData
            {
                nightLabel = "NOCHE 2",
                clockTime = "03:12 AM",
                rule = "Esta noche los imitadores evitan nombres propios. No todos los heridos son peligrosos.",
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
                            QA("De donde venis?", "Del puesto de guardia de San Mateo."),
                            QA("Que recordas antes de llegar?", "La sirena, despues la lluvia y una puerta golpeando."),
                            QA("Conoces a alguien dentro?", "A Ruben, el enfermero.")
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
                            QA("De donde venis?", "De donde vienen todos."),
                            QA("Que paso afuera?", "La noche paso."),
                            QA("Conoces a alguien dentro?", "Conozco sus voces.")
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
                            QA("De donde venis?", "De las casas bajas, cerca del puente."),
                            QA("Que paso afuera?", "Alguien imitaba la voz de mi madre."),
                            QA("Por que necesitas entrar?", "No puedo seguir corriendo.")
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
                            QA("De donde venis?", "Clara viene de Clara."),
                            QA("Que recordas antes de llegar?", "Entrar. Entrar. Entrar."),
                            QA("Conoces a alguien dentro?", "A Elena. A Tomas. A todos.")
                        },
                        0, -4, -1, 0,
                        0, 1, 1, 0,
                        "Clara entra repitiendo su nombre hasta que otros empiezan a repetirlo tambien.\nSeguridad -4 | Moral -1",
                        "Clara golpea el vidrio con el mismo ritmo exacto hasta alejarse.\nSeguridad +1 | Moral +1")
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
        VisitorData visitor = ScriptableObject.CreateInstance<VisitorData>();
        visitor.visitorName = visitorName;
        visitor.isImitator = isImitator;
        visitor.visitorSprite = LoadVisitorSprite(visitorName);
        visitor.introDialogue = introDialogue;
        visitor.visualClues.AddRange(visualClues);
        visitor.observationProfile = observationProfile;
        visitor.answers.AddRange(answers);
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

    private static QuestionAnswer QA(string question, string answer)
    {
        return new QuestionAnswer
        {
            question = question,
            answer = answer
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
