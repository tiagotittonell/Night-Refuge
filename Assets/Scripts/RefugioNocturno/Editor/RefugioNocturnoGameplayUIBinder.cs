using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class RefugioNocturnoGameplayUIBinder
{
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";
    private const string LayerName = "DynamicGameplayLayer";
    private const string DynamicLayoutPath = "Assets/Scripts/RefugioNocturno/Editor/DynamicGameplayLayout.json";

    private static readonly string[] DuplicateRootObjectNames =
    {
        "NightText",
        "ClockText",
        "MottoText",
        "VisitorNameText",
        "DialogueText",
        "ObservationsText",
        "NotesText",
        "QuestionsTitle",
        "QuestionsRemainingText",
        "QuestionsContainer",
        "QuestionButtonPrefab",
        "FoodText",
        "SecurityText",
        "MoraleText",
        "PopulationText",
        "DecisionWarning"
    };

    [MenuItem("Refugio Nocturno/Setup Dynamic Gameplay UI")]
    public static void SetupDynamicGameplayUI()
    {
        SyncNpcSpritesToResourcesInternal();
        ConfigureNpcSprites();

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No se encontro Canvas en SampleScene.");
            return;
        }

        ApplyVisitorImageDefaults(canvas.transform);

        Transform existingLayer = canvas.transform.Find(LayerName);
        if (existingLayer != null)
        {
            Object.DestroyImmediate(existingLayer.gameObject);
        }

        Font font = GetUiFont();
        GameObject layerObject = new GameObject(LayerName, typeof(RectTransform));
        layerObject.transform.SetParent(canvas.transform, false);
        layerObject.transform.SetAsLastSibling();
        RectTransform layerRect = layerObject.GetComponent<RectTransform>();
        layerRect.anchorMin = Vector2.zero;
        layerRect.anchorMax = Vector2.one;
        layerRect.offsetMin = Vector2.zero;
        layerRect.offsetMax = Vector2.zero;

        Text visitorNameText = CreateText(layerObject.transform, "CurrentVisitorNameText", "", RectFromTopLeft(675, -382, 180, 30), font, 20, FontStyle.Bold, UiAmber(), TextAnchor.MiddleLeft);
        Text dialogueText = CreateText(layerObject.transform, "RuntimeDialogueText", "", RectFromTopLeft(675, -405, 410, 70), font, 19, FontStyle.Normal, UiText(), TextAnchor.UpperLeft);
        Text cluesText = CreateText(layerObject.transform, "RuntimeCluesText", "", RectFromTopLeft(1190, -125, 230, 190), font, 16, FontStyle.Normal, UiTextDim(), TextAnchor.UpperLeft);

        Text questionCounterText = CreateText(layerObject.transform, "RuntimeQuestionCounterText", "", RectFromTopLeft(960, -447, 110, 24), font, 14, FontStyle.Bold, UiGold(), TextAnchor.MiddleRight);

        GameObject questionContainer = new GameObject("RuntimeQuestionsContainer", typeof(RectTransform), typeof(VerticalLayoutGroup));
        questionContainer.transform.SetParent(layerObject.transform, false);
        ApplyRect(questionContainer.GetComponent<RectTransform>(), RectFromTopLeft(675, -477, 400, 110));
        VerticalLayoutGroup layout = questionContainer.GetComponent<VerticalLayoutGroup>();
        ApplyRuntimeQuestionContainerDefaults(questionContainer.transform);

        Button questionPrefab = CreateButton(layerObject.transform, "RuntimeQuestionButtonPrefab", "Pregunta", RectFromTopLeft(675, -477, 400, 30), font, 17);
        ApplyRuntimeQuestionButtonDefaults(questionPrefab.transform);
        questionPrefab.gameObject.SetActive(false);

        Text foodText = CreateText(layerObject.transform, "RuntimeFoodText", "COMIDA 6", RectFromTopLeft(360, -990, 130, 24), font, 15, FontStyle.Bold, UiGold(), TextAnchor.MiddleCenter);
        Text securityText = CreateText(layerObject.transform, "RuntimeSecurityText", "SEGURIDAD 5", RectFromTopLeft(500, -990, 145, 24), font, 15, FontStyle.Bold, UiGold(), TextAnchor.MiddleCenter);
        Text moraleText = CreateText(layerObject.transform, "RuntimeMoraleText", "MORAL 5", RectFromTopLeft(190, -990, 130, 24), font, 15, FontStyle.Bold, UiGold(), TextAnchor.MiddleCenter);
        Text populationText = CreateText(layerObject.transform, "RuntimePopulationText", "POBLACION 2", RectFromTopLeft(20, -990, 145, 24), font, 15, FontStyle.Bold, UiGold(), TextAnchor.MiddleCenter);

        DialogueUI dialogueUI = Object.FindFirstObjectByType<DialogueUI>();
        QuestionUI questionUI = Object.FindFirstObjectByType<QuestionUI>();
        RefugeStats stats = Object.FindFirstObjectByType<RefugeStats>();

        if (dialogueUI != null)
        {
            SerializedObject serialized = new SerializedObject(dialogueUI);
            serialized.FindProperty("visitorNameLegacyText").objectReferenceValue = visitorNameText;
            serialized.FindProperty("dialogueLegacyText").objectReferenceValue = dialogueText;
            serialized.FindProperty("cluesLegacyText").objectReferenceValue = cluesText;
            serialized.ApplyModifiedProperties();
        }

        if (questionUI != null)
        {
            SerializedObject serialized = new SerializedObject(questionUI);
            serialized.FindProperty("questionButtonPrefab").objectReferenceValue = questionPrefab;
            serialized.FindProperty("questionsContainer").objectReferenceValue = questionContainer.transform;
            serialized.FindProperty("questionsRemainingLegacyText").objectReferenceValue = questionCounterText;
            serialized.ApplyModifiedProperties();
        }

        if (stats != null)
        {
            SerializedObject serialized = new SerializedObject(stats);
            serialized.FindProperty("foodLegacyText").objectReferenceValue = foodText;
            serialized.FindProperty("securityLegacyText").objectReferenceValue = securityText;
            serialized.FindProperty("moraleLegacyText").objectReferenceValue = moraleText;
            serialized.FindProperty("populationLegacyText").objectReferenceValue = populationText;
            serialized.ApplyModifiedProperties();
        }

        ApplyCapturedDynamicLayout(layerObject.transform);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("UI dinamica conectada en " + ScenePath);
    }

    [MenuItem("Refugio Nocturno/Cleanup Duplicate Base UI")]
    public static void CleanupDuplicateBaseUI()
    {
        Scene scene = EnsureSceneOpen();
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No se encontro Canvas para limpiar duplicados.");
            return;
        }

        int disabledCount = 0;
        foreach (string objectName in DuplicateRootObjectNames)
        {
            Transform child = canvas.transform.Find(objectName);
            if (child == null || child.parent != canvas.transform)
            {
                continue;
            }

            Undo.RecordObject(child.gameObject, "Disable duplicate Refugio UI object");
            child.gameObject.SetActive(false);
            disabledCount++;
        }

        ApplyVisitorImageDefaults(canvas.transform);

        Transform staticLayer = canvas.transform.Find("StaticTextLayer");
        if (staticLayer != null)
        {
            staticLayer.gameObject.SetActive(true);
        }

        Transform dynamicLayer = canvas.transform.Find(LayerName);
        if (dynamicLayer != null)
        {
            dynamicLayer.gameObject.SetActive(true);
            dynamicLayer.SetAsLastSibling();
            ApplyCapturedDynamicLayout(dynamicLayer);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log($"Limpieza lista: {disabledCount} objetos duplicados del Canvas raiz fueron desactivados.");
    }

    [MenuItem("Refugio Nocturno/Capture Dynamic Gameplay Layout")]
    public static void CaptureDynamicGameplayLayout()
    {
        Transform layer = FindDynamicLayer();
        if (layer == null)
        {
            Debug.LogError("No se encontro DynamicGameplayLayer. Ejecuta Setup Dynamic Gameplay UI primero.");
            return;
        }

        DynamicLayout layout = new DynamicLayout();
        foreach (RectTransform rect in layer.GetComponentsInChildren<RectTransform>(true))
        {
            if (rect.transform == layer)
            {
                continue;
            }

            Text text = rect.GetComponent<Text>();
            LayoutElement layoutElement = rect.GetComponent<LayoutElement>();

            DynamicLayoutItem item = new DynamicLayoutItem
            {
                path = GetPath(layer, rect.transform),
                active = rect.gameObject.activeSelf,
                anchorMin = rect.anchorMin,
                anchorMax = rect.anchorMax,
                pivot = rect.pivot,
                anchoredPosition = rect.anchoredPosition,
                sizeDelta = rect.sizeDelta,
                offsetMin = rect.offsetMin,
                offsetMax = rect.offsetMax,
                localScale = rect.localScale,
                localRotation = rect.localEulerAngles,
                hasLayoutElement = layoutElement != null,
                minHeight = layoutElement != null ? layoutElement.minHeight : 0f,
                preferredHeight = layoutElement != null ? layoutElement.preferredHeight : 0f,
                flexibleHeight = layoutElement != null ? layoutElement.flexibleHeight : 0f,
                hasText = text != null
            };

            if (text != null)
            {
                item.text = text.text;
                item.fontSize = text.fontSize;
                item.fontStyle = text.fontStyle;
                item.color = text.color;
                item.alignment = text.alignment;
                item.horizontalOverflow = text.horizontalOverflow;
                item.verticalOverflow = text.verticalOverflow;
            }

            layout.items.Add(item);
        }

        File.WriteAllText(DynamicLayoutPath, JsonUtility.ToJson(layout, true));
        AssetDatabase.ImportAsset(DynamicLayoutPath);
        Debug.Log($"Layout dinamico capturado: {layout.items.Count} elementos en {DynamicLayoutPath}");
    }

    [MenuItem("Refugio Nocturno/Apply Captured Dynamic Gameplay Layout")]
    public static void ApplyCapturedDynamicGameplayLayout()
    {
        Transform layer = FindDynamicLayer();
        if (layer == null)
        {
            Debug.LogError("No se encontro DynamicGameplayLayer para aplicar layout.");
            return;
        }

        ApplyCapturedDynamicLayout(layer);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Layout dinamico capturado reaplicado.");
    }

    [MenuItem("Refugio Nocturno/Apply Runtime Question Defaults")]
    public static void ApplyRuntimeQuestionDefaultsMenu()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No se encontro Canvas en la escena abierta.");
            return;
        }

        Transform dynamicLayer = canvas.transform.Find(LayerName);
        if (dynamicLayer == null)
        {
            Debug.LogError("No se encontro DynamicGameplayLayer. Ejecuta Setup Dynamic Gameplay UI primero.");
            return;
        }

        Transform counter = dynamicLayer.Find("RuntimeQuestionCounterText");
        ApplyRectIfFound(counter, RectFromTopLeft(960, -447, 110, 24));

        Transform container = dynamicLayer.Find("RuntimeQuestionsContainer");
        ApplyRectIfFound(container, RectFromTopLeft(675, -477, 400, 110));
        ApplyRuntimeQuestionContainerDefaults(container);

        Transform prefab = dynamicLayer.Find("RuntimeQuestionButtonPrefab");
        ApplyRectIfFound(prefab, RectFromTopLeft(675, -477, 400, 30));
        ApplyRuntimeQuestionButtonDefaults(prefab);

        if (container != null)
        {
            foreach (Transform child in container)
            {
                ApplyRuntimeQuestionButtonDefaults(child);
            }
        }

        dynamicLayer.SetAsLastSibling();
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Defaults de RuntimeQuestion aplicados.");
    }

    [MenuItem("Refugio Nocturno/Apply Visitor Image Defaults")]
    public static void ApplyVisitorImageDefaultsMenu()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No se encontro Canvas en la escena abierta.");
            return;
        }

        ApplyVisitorImageDefaults(canvas.transform);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Valores por defecto aplicados a VisitorImage.");
    }

    [MenuItem("Refugio Nocturno/Preview Elena In Editor")]
    public static void PreviewElenaInEditor()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No se encontro Canvas en SampleScene.");
            return;
        }

        Transform dynamicLayer = canvas.transform.Find(LayerName);
        if (dynamicLayer == null)
        {
            Debug.LogWarning("No existe DynamicGameplayLayer. Ejecutando Setup Dynamic Gameplay UI primero.");
            SetupDynamicGameplayUI();
            canvas = Object.FindFirstObjectByType<Canvas>();
            dynamicLayer = canvas != null ? canvas.transform.Find(LayerName) : null;
        }

        if (dynamicLayer == null)
        {
            Debug.LogError("No se pudo crear DynamicGameplayLayer.");
            return;
        }

        dynamicLayer.SetAsLastSibling();
        SetText(dynamicLayer, "CurrentVisitorNameText", "Elena");
        SetText(dynamicLayer, "RuntimeDialogueText", "Por favor, abrime. Hay algo siguiendome.");
        SetText(dynamicLayer, "RuntimeCluesText", "Pistas observables:\n- ropa mojada\n- temblor\n- herida visible");
        SetText(dynamicLayer, "RuntimeQuestionCounterText", "Preguntas: 2");
        SetText(dynamicLayer, "RuntimeFoodText", "COMIDA 6");
        SetText(dynamicLayer, "RuntimeSecurityText", "SEGURIDAD 5");
        SetText(dynamicLayer, "RuntimeMoraleText", "MORAL 5");
        SetText(dynamicLayer, "RuntimePopulationText", "POBLACION 2");

        Transform questionContainer = dynamicLayer.Find("RuntimeQuestionsContainer");
        if (questionContainer != null)
        {
            for (int i = questionContainer.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(questionContainer.GetChild(i).gameObject);
            }

            Font font = GetUiFont();
            ApplyRuntimeQuestionContainerDefaults(questionContainer);
            ApplyRuntimeQuestionButtonDefaults(CreateButton(questionContainer, "PreviewQuestion1", "1. De donde venis?", Stretch(0, 0, 0, 0), font, 17).transform);
            ApplyRuntimeQuestionButtonDefaults(CreateButton(questionContainer, "PreviewQuestion2", "2. Que paso afuera?", Stretch(0, 0, 0, 0), font, 17).transform);
            ApplyRuntimeQuestionButtonDefaults(CreateButton(questionContainer, "PreviewQuestion3", "3. Conoces a alguien dentro?", Stretch(0, 0, 0, 0), font, 17).transform);
        }

        Transform visitorImageTransform = canvas.transform.Find("VisitorImage");
        if (visitorImageTransform != null)
        {
            Image visitorImage = visitorImageTransform.GetComponent<Image>();
            if (visitorImage != null)
            {
                ApplyVisitorImageDefaults(canvas.transform);
                visitorImage.sprite = LoadSpriteFromPath("Assets/Resources/Npcs/Elena.png");
                visitorImage.enabled = true;
                visitorImage.preserveAspect = true;
                visitorImage.color = Color.white;
                visitorImageTransform.SetAsLastSibling();
                dynamicLayer.SetAsLastSibling();
            }
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Preview de Elena aplicado en la escena.");
    }

    [MenuItem("Refugio Nocturno/Configure NPC Sprites")]
    public static void ConfigureNpcSprites()
    {
        string[] spritePaths = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Resources/Npcs", "Assets/Npcs" });
        foreach (string guid in spritePaths)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                continue;
            }

            bool changed = false;
            if (importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                changed = true;
            }

            if (importer.spriteImportMode != SpriteImportMode.Single)
            {
                importer.spriteImportMode = SpriteImportMode.Single;
                changed = true;
            }

            if (changed)
            {
                importer.SaveAndReimport();
            }
        }
    }

    [MenuItem("Refugio Nocturno/Sync NPC Sprites To Resources")]
    public static void SyncNpcSpritesToResources()
    {
        int copiedCount = SyncNpcSpritesToResourcesInternal();
        ConfigureNpcSprites();
        AssetDatabase.Refresh();
        Debug.Log($"Sprites de NPC sincronizados: {copiedCount} archivos copiados a Assets/Resources/Npcs.");
    }

    private static int SyncNpcSpritesToResourcesInternal()
    {
        const string sourceFolder = "Assets/Npcs";
        const string targetFolder = "Assets/Resources/Npcs";

        if (!Directory.Exists(sourceFolder))
        {
            Debug.LogWarning("No existe Assets/Npcs. No hay sprites para sincronizar.");
            return 0;
        }

        Directory.CreateDirectory(targetFolder);

        int copiedCount = 0;
        string[] spritePaths = Directory.GetFiles(sourceFolder, "*.png", SearchOption.TopDirectoryOnly);
        foreach (string sourcePath in spritePaths)
        {
            string fileName = Path.GetFileName(sourcePath);
            string targetPath = Path.Combine(targetFolder, fileName).Replace("\\", "/");
            File.Copy(sourcePath, targetPath, true);
            AssetDatabase.ImportAsset(targetPath);
            copiedCount++;
        }

        return copiedCount;
    }

    private static Text CreateText(Transform parent, string name, string value, RectSpec rectSpec, Font font, int size, FontStyle style, Color color, TextAnchor alignment)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(parent, false);
        ApplyRect(textObject.GetComponent<RectTransform>(), rectSpec);

        Text text = textObject.GetComponent<Text>();
        text.text = value;
        text.font = font;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.raycastTarget = false;
        return text;
    }

    private static void SetText(Transform root, string childName, string value)
    {
        Transform child = root.Find(childName);
        if (child == null)
        {
            return;
        }

        Text text = child.GetComponent<Text>();
        if (text != null)
        {
            text.text = value;
        }
    }

    private static Sprite LoadSpriteFromPath(string path)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite != null)
        {
            return sprite;
        }

        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
        foreach (Object asset in assets)
        {
            if (asset is Sprite foundSprite)
            {
                return foundSprite;
            }
        }

        return null;
    }

    private static void ApplyRuntimeQuestionContainerDefaults(Transform container)
    {
        if (container == null)
        {
            return;
        }

        VerticalLayoutGroup layout = container.GetComponent<VerticalLayoutGroup>();
        if (layout == null)
        {
            layout = container.gameObject.AddComponent<VerticalLayoutGroup>();
        }

        layout.padding = new RectOffset(0, 0, 0, 0);
        layout.spacing = 5f;
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
    }

    private static void ApplyRuntimeQuestionButtonDefaults(Transform buttonTransform)
    {
        if (buttonTransform == null)
        {
            return;
        }

        RectTransform rect = buttonTransform.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(0f, 30f);
            rect.localScale = Vector3.one;
        }

        LayoutElement layoutElement = buttonTransform.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = buttonTransform.gameObject.AddComponent<LayoutElement>();
        }

        layoutElement.minHeight = 30f;
        layoutElement.preferredHeight = 30f;
        layoutElement.flexibleHeight = 0f;

        Image image = buttonTransform.GetComponent<Image>();
        if (image != null)
        {
            image.color = new Color(0.16f, 0.13f, 0.09f, 0.72f);
            image.raycastTarget = true;
        }

        Text label = buttonTransform.GetComponentInChildren<Text>(true);
        if (label != null)
        {
            RectTransform labelRect = label.GetComponent<RectTransform>();
            if (labelRect != null)
            {
                labelRect.anchorMin = Vector2.zero;
                labelRect.anchorMax = Vector2.one;
                labelRect.offsetMin = new Vector2(12f, 2f);
                labelRect.offsetMax = new Vector2(-12f, -2f);
                labelRect.localScale = Vector3.one;
            }

            label.font = GetUiFont();
            label.fontSize = 17;
            label.color = UiText();
            label.alignment = TextAnchor.MiddleLeft;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.raycastTarget = false;
        }
    }

    private static void ApplyRectIfFound(Transform target, RectSpec spec)
    {
        if (target == null)
        {
            return;
        }

        RectTransform rect = target.GetComponent<RectTransform>();
        if (rect != null)
        {
            ApplyRect(rect, spec);
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
        }
    }

    private static void ApplyVisitorImageDefaults(Transform canvasTransform)
    {
        Transform visitorImageTransform = canvasTransform.Find("VisitorImage");
        if (visitorImageTransform == null)
        {
            Debug.LogWarning("No se encontro VisitorImage dentro del Canvas.");
            return;
        }

        RectTransform rect = visitorImageTransform.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, 120f);
            rect.sizeDelta = new Vector2(380f, 430f);
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            rect.SetAsLastSibling();
        }

        Image image = visitorImageTransform.GetComponent<Image>();
        if (image != null)
        {
            image.enabled = true;
            image.preserveAspect = true;
            image.raycastTarget = false;
        }
    }

    private static Scene EnsureSceneOpen()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (activeScene.IsValid() && activeScene.path == ScenePath)
        {
            return activeScene;
        }

        return EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
    }

    private static Transform FindDynamicLayer()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            return null;
        }

        return canvas.transform.Find(LayerName);
    }

    private static void ApplyCapturedDynamicLayout(Transform layer)
    {
        if (layer == null || !File.Exists(DynamicLayoutPath))
        {
            return;
        }

        DynamicLayout layout = JsonUtility.FromJson<DynamicLayout>(File.ReadAllText(DynamicLayoutPath));
        if (layout == null || layout.items == null)
        {
            return;
        }

        foreach (DynamicLayoutItem item in layout.items)
        {
            Transform target = layer.Find(item.path);
            if (target == null)
            {
                continue;
            }

            RectTransform rect = target.GetComponent<RectTransform>();
            if (rect == null)
            {
                continue;
            }

            target.gameObject.SetActive(item.active);
            rect.anchorMin = item.anchorMin;
            rect.anchorMax = item.anchorMax;
            rect.pivot = item.pivot;
            rect.localScale = item.localScale;
            rect.localEulerAngles = item.localRotation;

            bool isStretching = item.anchorMin != item.anchorMax;
            if (isStretching)
            {
                rect.offsetMin = item.offsetMin;
                rect.offsetMax = item.offsetMax;
            }
            else
            {
                rect.anchoredPosition = item.anchoredPosition;
                rect.sizeDelta = item.sizeDelta;
            }

            LayoutElement layoutElement = target.GetComponent<LayoutElement>();
            if (item.hasLayoutElement)
            {
                if (layoutElement == null)
                {
                    layoutElement = target.gameObject.AddComponent<LayoutElement>();
                }

                layoutElement.minHeight = item.minHeight;
                layoutElement.preferredHeight = item.preferredHeight;
                layoutElement.flexibleHeight = item.flexibleHeight;
            }

            Text text = target.GetComponent<Text>();
            if (item.hasText && text != null)
            {
                text.text = item.text;
                text.fontSize = item.fontSize;
                text.fontStyle = item.fontStyle;
                text.color = item.color;
                text.alignment = item.alignment;
                text.horizontalOverflow = item.horizontalOverflow;
                text.verticalOverflow = item.verticalOverflow;
            }
        }
    }

    private static string GetPath(Transform root, Transform target)
    {
        List<string> names = new List<string>();
        Transform current = target;
        while (current != null && current != root)
        {
            names.Add(current.name);
            current = current.parent;
        }

        names.Reverse();
        return string.Join("/", names);
    }

    private static Button CreateButton(Transform parent, string name, string value, RectSpec rectSpec, Font font, int size)
    {
        GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);
        ApplyRect(buttonObject.GetComponent<RectTransform>(), rectSpec);

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.16f, 0.13f, 0.09f, 0.65f);

        Button button = buttonObject.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = image.color;
        colors.highlightedColor = new Color(0.28f, 0.24f, 0.16f, 0.8f);
        colors.pressedColor = new Color(0.10f, 0.08f, 0.06f, 0.9f);
        button.colors = colors;

        Text label = CreateText(buttonObject.transform, "Text", value, Stretch(12, 2, 12, 2), font, size, FontStyle.Normal, UiText(), TextAnchor.MiddleLeft);
        label.raycastTarget = false;
        return button;
    }

    private static Font GetUiFont()
    {
        string[] preferredFonts = { "Consolas", "Courier New", "Lucida Console", "Arial" };
        Font font = Font.CreateDynamicFontFromOSFont(preferredFonts, 24);
        if (font != null)
        {
            return font;
        }

        return Resources.GetBuiltinResource<Font>("Arial.ttf");
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

    [System.Serializable]
    private class DynamicLayout
    {
        public List<DynamicLayoutItem> items = new List<DynamicLayoutItem>();
    }

    [System.Serializable]
    private class DynamicLayoutItem
    {
        public string path;
        public bool active;
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 pivot;
        public Vector2 anchoredPosition;
        public Vector2 sizeDelta;
        public Vector2 offsetMin;
        public Vector2 offsetMax;
        public Vector3 localScale;
        public Vector3 localRotation;
        public bool hasLayoutElement;
        public float minHeight;
        public float preferredHeight;
        public float flexibleHeight;
        public bool hasText;
        public string text;
        public int fontSize;
        public FontStyle fontStyle;
        public Color color;
        public TextAnchor alignment;
        public HorizontalWrapMode horizontalOverflow;
        public VerticalWrapMode verticalOverflow;
    }
}
