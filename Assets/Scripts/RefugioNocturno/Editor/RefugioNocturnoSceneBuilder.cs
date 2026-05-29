using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class RefugioNocturnoSceneBuilder
{
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";
    private const string BackgroundPath = "Assets/Art/MainScenario.png";
    private const string FontAssetPath = "Assets/Art/RefugioUIFont.asset";
    private static TMP_FontAsset cachedFontAsset;

    [MenuItem("Refugio Nocturno/Build Main Scenario")]
    public static void BuildMainScenario()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "SampleScene";

        CreateCamera();
        CreateEventSystem();

        GameObject gameManager = new GameObject("GameManager");
        NightManager nightManager = gameManager.AddComponent<NightManager>();
        VisitorManager visitorManager = gameManager.AddComponent<VisitorManager>();
        RefugeStats refugeStats = gameManager.AddComponent<RefugeStats>();
        DecisionController decisionController = gameManager.AddComponent<DecisionController>();

        GameObject canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1536f, 1024f);
        scaler.matchWidthOrHeight = 0.5f;

        Image background = CreateImage(canvasObject.transform, "MainScenarioBackground", Stretch(0, 0, 0, 0), Color.white);
        background.sprite = LoadBackgroundSprite();
        background.preserveAspect = false;

        Image visitorImage = CreateImage(canvasObject.transform, "VisitorImage", Anchored(0, 120, 380, 430), new Color(0.02f, 0.025f, 0.03f, 0.55f));
        visitorImage.raycastTarget = false;
        visitorImage.preserveAspect = true;
        TMP_Text placeholder = CreateText(visitorImage.transform, "VisitorPlaceholder", "VISITANTE", 34, TextAlignmentOptions.Center, new Color(0.65f, 0.62f, 0.55f, 0.45f), Stretch(15, 15, 15, 15));
        placeholder.raycastTarget = false;

        CreateText(canvasObject.transform, "NightText", "NOCHE 1", 24, TextAlignmentOptions.Center, UiGold(), RectFromTopLeft(42, -32, 135, 34));
        CreateText(canvasObject.transform, "ClockText", "01:47 AM", 24, TextAlignmentOptions.Center, UiGold(), RectFromTopLeft(220, -32, 120, 34));

        TMP_Text ruleText = CreateText(canvasObject.transform, "RuleText", string.Empty, 26, TextAlignmentOptions.TopLeft, Color.black, RectFromTopLeft(64, -122, 245, 330));
        CreateText(canvasObject.transform, "MottoText", "OBSERVA\nESCUCHA\nCUESTIONA\nDECIDE", 27, TextAlignmentOptions.Center, Color.black, RectFromTopLeft(110, -570, 135, 155));

        DialogueUI dialogueUI = canvasObject.AddComponent<DialogueUI>();
        TMP_Text visitorNameText = CreateText(canvasObject.transform, "VisitorNameText", "VISITANTE:", 24, TextAlignmentOptions.Left, UiAmber(), RectFromTopLeft(460, -626, 180, 32));
        TMP_Text dialogueText = CreateText(canvasObject.transform, "DialogueText", string.Empty, 23, TextAlignmentOptions.TopLeft, UiText(), RectFromTopLeft(460, -657, 500, 76));
        TMP_Text cluesText = CreateText(canvasObject.transform, "ObservationsText", string.Empty, 23, TextAlignmentOptions.TopLeft, UiText(), RectFromTopLeft(1210, -121, 265, 260));
        CreateText(canvasObject.transform, "NotesText", "NOTAS\n\nObserva contradicciones, heridas y recuerdos antes de decidir.", 22, TextAlignmentOptions.TopLeft, UiTextDim(), RectFromTopLeft(1206, -412, 260, 120));

        QuestionUI questionUI = canvasObject.AddComponent<QuestionUI>();
        CreateText(canvasObject.transform, "QuestionsTitle", "PREGUNTAS", 24, TextAlignmentOptions.Left, UiGold(), RectFromTopLeft(460, -758, 220, 32));
        TMP_Text remainingText = CreateText(canvasObject.transform, "QuestionsRemainingText", "Preguntas: 2", 18, TextAlignmentOptions.Right, UiGold(), RectFromTopLeft(830, -760, 135, 28));

        GameObject questionsContainer = new GameObject("QuestionsContainer", typeof(RectTransform), typeof(VerticalLayoutGroup));
        questionsContainer.transform.SetParent(canvasObject.transform, false);
        ApplyRect(questionsContainer.GetComponent<RectTransform>(), RectFromTopLeft(475, -795, 496, 130));
        VerticalLayoutGroup questionLayout = questionsContainer.GetComponent<VerticalLayoutGroup>();
        questionLayout.spacing = 8;
        questionLayout.childControlHeight = true;
        questionLayout.childControlWidth = true;
        questionLayout.childForceExpandHeight = false;
        questionLayout.childForceExpandWidth = true;

        Button questionButtonPrefab = CreateButton(canvasObject.transform, "QuestionButtonPrefab", "Pregunta", RectFromTopLeft(475, -795, 496, 35), new Color(0.17f, 0.15f, 0.11f, 0.88f), 21);
        questionButtonPrefab.gameObject.SetActive(false);

        Button allowButton = CreateButton(canvasObject.transform, "AllowButton", "PERMITIR", RectFromTopLeft(1082, -680, 104, 31), new Color(0.13f, 0.19f, 0.10f, 0.7f), 18);
        Button rejectButton = CreateButton(canvasObject.transform, "RejectButton", "NO PERMITIR", RectFromTopLeft(1212, -680, 117, 31), new Color(0.20f, 0.11f, 0.09f, 0.7f), 18);
        CreateText(canvasObject.transform, "DecisionWarning", "UNA DECISION PUEDE\nCAMBIARLO TODO", 20, TextAlignmentOptions.Center, UiAmber(), RectFromTopLeft(1110, -815, 220, 55));

        TMP_Text foodText = CreateText(canvasObject.transform, "FoodText", "COMIDA 6", 18, TextAlignmentOptions.Center, UiGold(), RectFromBottomLeft(360, 16, 170, 34));
        TMP_Text securityText = CreateText(canvasObject.transform, "SecurityText", "SEGURIDAD 5", 18, TextAlignmentOptions.Center, UiGold(), RectFromBottomLeft(530, 16, 195, 34));
        TMP_Text moraleText = CreateText(canvasObject.transform, "MoraleText", "MORAL 5", 18, TextAlignmentOptions.Center, UiGold(), RectFromBottomLeft(180, 16, 170, 34));
        TMP_Text populationText = CreateText(canvasObject.transform, "PopulationText", "POBLACION 2", 18, TextAlignmentOptions.Center, UiGold(), RectFromBottomLeft(12, 16, 170, 34));

        GameObject summaryPanel = CreatePanel(canvasObject.transform, "EndNightSummaryPanel", Anchored(0, 0, 660, 560), new Color(0.015f, 0.014f, 0.012f, 0.97f));
        EndNightSummaryUI summaryUI = summaryPanel.AddComponent<EndNightSummaryUI>();
        TMP_Text summaryText = CreateText(summaryPanel.transform, "SummaryText", string.Empty, 26, TextAlignmentOptions.TopLeft, UiText(), Stretch(42, 42, 42, 120));
        Button continueButton = CreateButton(summaryPanel.transform, "ContinueButton", "CONTINUAR", Bottom(35, 250, 58), new Color(0.18f, 0.15f, 0.10f, 0.95f), 22);

        GameObject rulePanel = new GameObject("RulePanel");
        rulePanel.transform.SetParent(canvasObject.transform, false);

        GameObject audio = new GameObject("Audio");
        GameObject rain = new GameObject("RainAmbience", typeof(AudioSource));
        rain.transform.SetParent(audio.transform);
        rain.GetComponent<AudioSource>().loop = true;
        new GameObject("KnockSound", typeof(AudioSource)).transform.SetParent(audio.transform);

        SetField(dialogueUI, "visitorNameText", visitorNameText);
        SetField(dialogueUI, "dialogueText", dialogueText);
        SetField(dialogueUI, "cluesText", cluesText);

        SetField(questionUI, "dialogueUI", dialogueUI);
        SetField(questionUI, "questionButtonPrefab", questionButtonPrefab);
        SetField(questionUI, "questionsContainer", questionsContainer.transform);
        SetField(questionUI, "questionsRemainingText", remainingText);

        SetField(visitorManager, "visitorImage", visitorImage);
        SetField(visitorManager, "dialogueUI", dialogueUI);
        SetField(visitorManager, "questionUI", questionUI);

        SetField(refugeStats, "foodText", foodText);
        SetField(refugeStats, "securityText", securityText);
        SetField(refugeStats, "moraleText", moraleText);
        SetField(refugeStats, "populationText", populationText);

        SetField(decisionController, "visitorManager", visitorManager);
        SetField(decisionController, "refugeStats", refugeStats);
        SetField(decisionController, "nightManager", nightManager);
        SetField(decisionController, "allowButton", allowButton);
        SetField(decisionController, "rejectButton", rejectButton);

        SetField(nightManager, "visitorManager", visitorManager);
        SetField(nightManager, "refugeStats", refugeStats);
        SetField(nightManager, "decisionController", decisionController);
        SetField(nightManager, "endNightSummaryUI", summaryUI);
        SetField(nightManager, "ruleText", ruleText);
        SetField(nightManager, "rulePanel", rulePanel);

        SetField(summaryUI, "panelRoot", summaryPanel);
        SetField(summaryUI, "summaryText", summaryText);
        SetField(summaryUI, "continueButton", continueButton);

        Selection.activeGameObject = gameManager;
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        Debug.Log("Refugio Nocturno main scenario built at " + ScenePath);
    }

    private static Sprite LoadBackgroundSprite()
    {
        TextureImporter importer = AssetImporter.GetAtPath(BackgroundPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(BackgroundPath);
    }

    private static Camera CreateCamera()
    {
        GameObject cameraObject = new GameObject("Main Camera");
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.tag = "MainCamera";
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.black;
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        return camera;
    }

    private static void CreateEventSystem()
    {
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<InputSystemUIInputModule>();
    }

    private static Image CreateImage(Transform parent, string name, RectSpec rect, Color color)
    {
        GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(parent, false);
        ApplyRect(imageObject.GetComponent<RectTransform>(), rect);
        Image image = imageObject.GetComponent<Image>();
        image.color = color;
        return image;
    }

    private static GameObject CreatePanel(Transform parent, string name, RectSpec rect, Color color)
    {
        return CreateImage(parent, name, rect, color).gameObject;
    }

    private static TMP_Text CreateText(Transform parent, string name, string text, float size, TextAlignmentOptions alignment, Color color, RectSpec rect)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);
        ApplyRect(textObject.GetComponent<RectTransform>(), rect);

        TMP_Text tmp = textObject.GetComponent<TMP_Text>();
        tmp.text = text;
        TMP_FontAsset fontAsset = GetOrCreateFontAsset();
        if (fontAsset != null)
        {
            tmp.font = fontAsset;
        }
        tmp.fontSize = size;
        tmp.alignment = alignment;
        tmp.color = color;
        tmp.enableWordWrapping = true;
        tmp.overflowMode = TextOverflowModes.Ellipsis;
        return tmp;
    }

    private static Button CreateButton(Transform parent, string name, string label, RectSpec rect, Color color, float fontSize)
    {
        Image image = CreateImage(parent, name, rect, color);
        Button button = image.gameObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = color * 1.18f;
        colors.pressedColor = color * 0.82f;
        colors.selectedColor = color * 1.08f;
        button.colors = colors;

        TMP_Text text = CreateText(image.transform, "Text", label, fontSize, TextAlignmentOptions.Center, UiGold(), Stretch(6, 4, 6, 4));
        text.fontStyle = FontStyles.Bold;
        return button;
    }

    private static TMP_FontAsset GetOrCreateFontAsset()
    {
        if (cachedFontAsset != null)
        {
            return cachedFontAsset;
        }

        cachedFontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
        if (cachedFontAsset != null)
        {
            return cachedFontAsset;
        }

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
        {
            font = Font.CreateDynamicFontFromOSFont("Arial", 90);
        }

        if (font == null)
        {
            Debug.LogWarning("No se pudo crear RefugioUIFont. Importa TMP Essential Resources si los textos no aparecen.");
            return null;
        }

        cachedFontAsset = TMP_FontAsset.CreateFontAsset(font);
        if (cachedFontAsset == null)
        {
            Debug.LogWarning("No se pudo crear RefugioUIFont. Importa TMP Essential Resources si los textos no aparecen.");
            return null;
        }

        cachedFontAsset.name = "RefugioUIFont";
        AssetDatabase.CreateAsset(cachedFontAsset, FontAssetPath);
        AssetDatabase.SaveAssets();
        return cachedFontAsset;
    }

    private static void ApplyRect(RectTransform rect, RectSpec spec)
    {
        rect.anchorMin = spec.anchorMin;
        rect.anchorMax = spec.anchorMax;
        rect.pivot = spec.pivot;

        if (spec.useOffsets)
        {
            rect.offsetMin = spec.offsetMin;
            rect.offsetMax = spec.offsetMax;
        }
        else
        {
            rect.anchoredPosition = spec.anchoredPosition;
            rect.sizeDelta = spec.sizeDelta;
        }
    }

    private static void SetField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        field.SetValue(target, value);
        EditorUtility.SetDirty((Object)target);
    }

    private static Color UiGold()
    {
        return new Color(0.62f, 0.54f, 0.39f, 1f);
    }

    private static Color UiAmber()
    {
        return new Color(0.72f, 0.50f, 0.18f, 1f);
    }

    private static Color UiText()
    {
        return new Color(0.78f, 0.70f, 0.58f, 1f);
    }

    private static Color UiTextDim()
    {
        return new Color(0.58f, 0.50f, 0.38f, 1f);
    }

    private static RectSpec Stretch(float left, float top, float right, float bottom)
    {
        return new RectSpec
        {
            anchorMin = Vector2.zero,
            anchorMax = Vector2.one,
            pivot = new Vector2(0.5f, 0.5f),
            offsetMin = new Vector2(left, bottom),
            offsetMax = new Vector2(-right, -top),
            useOffsets = true
        };
    }

    private static RectSpec Anchored(float x, float y, float width, float height)
    {
        return new RectSpec
        {
            anchorMin = new Vector2(0.5f, 0.5f),
            anchorMax = new Vector2(0.5f, 0.5f),
            pivot = new Vector2(0.5f, 0.5f),
            anchoredPosition = new Vector2(x, y),
            sizeDelta = new Vector2(width, height)
        };
    }

    private static RectSpec Bottom(float bottom, float width, float height)
    {
        return new RectSpec
        {
            anchorMin = new Vector2(0.5f, 0f),
            anchorMax = new Vector2(0.5f, 0f),
            pivot = new Vector2(0.5f, 0f),
            anchoredPosition = new Vector2(0f, bottom),
            sizeDelta = new Vector2(width, height)
        };
    }

    private static RectSpec RectFromTopLeft(float left, float top, float width, float height)
    {
        return new RectSpec
        {
            anchorMin = new Vector2(0f, 1f),
            anchorMax = new Vector2(0f, 1f),
            pivot = new Vector2(0f, 1f),
            anchoredPosition = new Vector2(left, top),
            sizeDelta = new Vector2(width, height)
        };
    }

    private static RectSpec RectFromBottomLeft(float left, float bottom, float width, float height)
    {
        return new RectSpec
        {
            anchorMin = new Vector2(0f, 0f),
            anchorMax = new Vector2(0f, 0f),
            pivot = new Vector2(0f, 0f),
            anchoredPosition = new Vector2(left, bottom),
            sizeDelta = new Vector2(width, height)
        };
    }

    private struct RectSpec
    {
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 pivot;
        public Vector2 anchoredPosition;
        public Vector2 sizeDelta;
        public Vector2 offsetMin;
        public Vector2 offsetMax;
        public bool useOffsets;
    }
}
