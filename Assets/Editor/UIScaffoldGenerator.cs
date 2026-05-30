using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// Herramienta de editor que genera los GameObjects de UI dinámicos en la escena
/// para que puedan ser posicionados y redimensionados manualmente fuera de Play Mode.
/// Los scripts en runtime los encuentran por nombre y no crean fallbacks.
/// </summary>
public class UIScaffoldGenerator : EditorWindow
{
    [MenuItem("Refugio Nocturno/Generar UI en Escena")]
    public static void ShowWindow()
    {
        GetWindow<UIScaffoldGenerator>("UI Scaffold");
    }

    private void OnGUI()
    {
        GUILayout.Label("Genera los objetos de UI para ajustarlos en Editor", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Generar TODO (Sospecha + Log + Eventos + Tienda)"))
        {
            GenerateAll();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Solo: Indicador de Sospecha"))
        {
            GenerateSuspicionUI();
        }

        if (GUILayout.Button("Solo: Panel de Registro (Visitor Log)"))
        {
            GenerateVisitorLogPanel();
        }

        if (GUILayout.Button("Solo: Panel de Eventos"))
        {
            GenerateInterEventPanel();
        }

        if (GUILayout.Button("Solo: Panel de Tienda (Upgrade Shop)"))
        {
            GenerateUpgradeShopPanel();
        }
    }

    private static void GenerateAll()
    {
        GenerateSuspicionUI();
        GenerateVisitorLogPanel();
        GenerateInterEventPanel();
        GenerateUpgradeShopPanel();
        Debug.Log("[UIScaffold] Todos los paneles generados. Ajusta posiciones en el Editor.");
    }

    private static Transform GetOrCreateParent(string parentName)
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[UIScaffold] No se encontró un Canvas en la escena.");
            return null;
        }

        // Try to find DynamicGameplayLayer first
        GameObject dynamicLayer = GameObject.Find("DynamicGameplayLayer");
        if (dynamicLayer != null)
        {
            return dynamicLayer.transform;
        }

        return canvas.transform;
    }

    private static void GenerateSuspicionUI()
    {
        if (GameObject.Find("SuspicionContainer") != null)
        {
            Debug.LogWarning("[UIScaffold] SuspicionContainer ya existe.");
            return;
        }

        Transform parent = GetOrCreateParent("DynamicGameplayLayer");
        if (parent == null) return;

        // Container
        GameObject container = new GameObject("SuspicionContainer", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        container.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(container, "Create SuspicionContainer");

        RectTransform containerRect = container.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0f, 1f);
        containerRect.anchorMax = new Vector2(0.4f, 1f);
        containerRect.pivot = new Vector2(0f, 1f);
        containerRect.anchoredPosition = new Vector2(15f, -10f);
        containerRect.sizeDelta = new Vector2(0f, 40f);

        HorizontalLayoutGroup hlg = container.GetComponent<HorizontalLayoutGroup>();
        hlg.spacing = 6f;
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childControlWidth = false;
        hlg.childControlHeight = false;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;

        // Icon
        GameObject iconObj = new GameObject("SuspicionIcon", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
        iconObj.transform.SetParent(container.transform, false);

        Image iconImage = iconObj.GetComponent<Image>();
        iconImage.preserveAspect = true;
        iconImage.raycastTarget = false;
        iconImage.color = new Color(1f, 1f, 1f, 0.5f);

        Sprite iconSprite = Resources.Load<Sprite>("UI/Suspicion/suspicion_medium");
        if (iconSprite != null) iconImage.sprite = iconSprite;

        LayoutElement iconLayout = iconObj.GetComponent<LayoutElement>();
        iconLayout.preferredWidth = 32f;
        iconLayout.preferredHeight = 32f;

        // Text
        GameObject textObj = new GameObject("SuspicionText", typeof(RectTransform), typeof(Text), typeof(LayoutElement));
        textObj.transform.SetParent(container.transform, false);

        LayoutElement textLayout = textObj.GetComponent<LayoutElement>();
        textLayout.preferredWidth = 250f;
        textLayout.preferredHeight = 35f;

        Text text = textObj.GetComponent<Text>();
        text.font = Font.CreateDynamicFontFromOSFont(new[] { "Consolas", "Courier New", "Arial" }, 18);
        text.fontSize = 18;
        text.color = new Color(0.85f, 0.7f, 0.2f, 1f);
        text.alignment = TextAnchor.MiddleLeft;
        text.raycastTarget = false;
        text.text = "SOSPECHA: MEDIA";

        Debug.Log("[UIScaffold] SuspicionContainer creado. Ajusta tamaño del icono via LayoutElement.");
    }

    private static void GenerateVisitorLogPanel()
    {
        if (GameObject.Find("VisitorLogPanel") != null)
        {
            Debug.LogWarning("[UIScaffold] VisitorLogPanel ya existe.");
            return;
        }

        Transform parent = GetOrCreateParent("DynamicGameplayLayer");
        if (parent == null) return;

        GameObject panel = new GameObject("VisitorLogPanel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(panel, "Create VisitorLogPanel");

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.15f, 0.1f);
        panelRect.anchorMax = new Vector2(0.85f, 0.9f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = Vector2.zero;

        Image panelImage = panel.GetComponent<Image>();
        Sprite bgSprite = Resources.Load<Sprite>("UI/Panels/log_background");
        if (bgSprite != null)
        {
            panelImage.sprite = bgSprite;
            panelImage.type = Image.Type.Sliced;
            panelImage.color = Color.white;
        }
        else
        {
            panelImage.color = new Color(0.04f, 0.035f, 0.03f, 0.95f);
        }

        // Log text
        GameObject textObj = new GameObject("LogText", typeof(RectTransform), typeof(Text));
        textObj.transform.SetParent(panel.transform, false);

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0f, 0.08f);
        textRect.anchorMax = new Vector2(1f, 1f);
        textRect.offsetMin = new Vector2(24f, 0f);
        textRect.offsetMax = new Vector2(-24f, -16f);

        Text logText = textObj.GetComponent<Text>();
        logText.font = Font.CreateDynamicFontFromOSFont(new[] { "Consolas", "Courier New", "Arial" }, 15);
        logText.fontSize = 15;
        logText.color = new Color(0.70f, 0.63f, 0.52f, 1f);
        logText.alignment = TextAnchor.UpperLeft;
        logText.raycastTarget = false;
        logText.text = "REGISTRO DE VISITANTES\n═══════════════════\n\n▪ Visitante ejemplo [22:30]\n  Decisión: PERMITIDO\n  Sospecha: Baja";

        panel.SetActive(false);

        Debug.Log("[UIScaffold] VisitorLogPanel creado (inactivo). Actívalo para ajustar layout.");
    }

    private static void GenerateInterEventPanel()
    {
        if (GameObject.Find("InterEventText") != null)
        {
            Debug.LogWarning("[UIScaffold] InterEventText ya existe.");
            return;
        }

        Transform parent = GetOrCreateParent("DynamicGameplayLayer");
        if (parent == null) return;

        GameObject panel = new GameObject("InterEventText", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(panel, "Create InterEventText");

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.15f, 0.35f);
        panelRect.anchorMax = new Vector2(0.85f, 0.65f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = Vector2.zero;

        Image panelImage = panel.GetComponent<Image>();
        Sprite eventBg = Resources.Load<Sprite>("UI/Events/event_blackout");
        if (eventBg != null)
        {
            panelImage.sprite = eventBg;
            panelImage.color = new Color(1f, 1f, 1f, 0.92f);
        }
        else
        {
            panelImage.color = new Color(0.05f, 0.04f, 0.03f, 0.92f);
        }
        panelImage.raycastTarget = false;

        // Event text
        GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(Text));
        textObj.transform.SetParent(panel.transform, false);

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(24f, 16f);
        textRect.offsetMax = new Vector2(-24f, -16f);

        Text text = textObj.GetComponent<Text>();
        text.font = Font.CreateDynamicFontFromOSFont(new[] { "Consolas", "Courier New", "Arial" }, 20);
        text.fontSize = 20;
        text.fontStyle = FontStyle.Italic;
        text.color = new Color(0.6f, 0.55f, 0.45f, 1f);
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;
        text.text = "[Evento de ejemplo: Se escuchan ruidos en los ductos...]";

        panel.SetActive(false);

        Debug.Log("[UIScaffold] InterEventText creado (inactivo). Actívalo para ver tamaño.");
    }

    private static void GenerateUpgradeShopPanel()
    {
        if (GameObject.Find("UpgradeShopPanel") != null)
        {
            Debug.LogWarning("[UIScaffold] UpgradeShopPanel ya existe.");
            return;
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[UIScaffold] No se encontró Canvas.");
            return;
        }

        GameObject panel = new GameObject("UpgradeShopPanel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvas.transform, false);
        Undo.RegisterCreatedObjectUndo(panel, "Create UpgradeShopPanel");

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.1f, 0.05f);
        panelRect.anchorMax = new Vector2(0.9f, 0.95f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = Vector2.zero;

        Image panelImage = panel.GetComponent<Image>();
        Sprite shopBg = Resources.Load<Sprite>("UI/Shop/shop_background");
        if (shopBg != null)
        {
            panelImage.sprite = shopBg;
            panelImage.type = Image.Type.Sliced;
            panelImage.color = Color.white;
        }
        else
        {
            panelImage.color = new Color(0.04f, 0.035f, 0.03f, 0.97f);
        }

        // Buttons container placeholder
        GameObject containerObj = new GameObject("UpgradeButtonsContainer", typeof(RectTransform), typeof(VerticalLayoutGroup));
        containerObj.transform.SetParent(panel.transform, false);

        RectTransform containerRect = containerObj.GetComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.57f, 0.15f);
        containerRect.anchorMax = new Vector2(0.97f, 0.88f);
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;

        VerticalLayoutGroup vlg = containerObj.GetComponent<VerticalLayoutGroup>();
        vlg.spacing = 6f;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        // Continue button
        GameObject btnObj = new GameObject("UpgradeShopContinueButton", typeof(RectTransform), typeof(Image), typeof(Button));
        btnObj.transform.SetParent(panel.transform, false);

        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0f);
        btnRect.anchorMax = new Vector2(0.5f, 0f);
        btnRect.pivot = new Vector2(0.5f, 0f);
        btnRect.anchoredPosition = new Vector2(0f, 15f);
        btnRect.sizeDelta = new Vector2(250f, 50f);

        Image btnImage = btnObj.GetComponent<Image>();
        Sprite btnSprite = Resources.Load<Sprite>("UI/Buttons/button_normal");
        if (btnSprite != null)
        {
            btnImage.sprite = btnSprite;
            btnImage.type = Image.Type.Sliced;
            btnImage.color = Color.white;
        }
        else
        {
            btnImage.color = new Color(0.18f, 0.15f, 0.10f, 0.95f);
        }

        GameObject btnLabel = new GameObject("Label", typeof(RectTransform), typeof(Text));
        btnLabel.transform.SetParent(btnObj.transform, false);

        RectTransform btnLabelRect = btnLabel.GetComponent<RectTransform>();
        btnLabelRect.anchorMin = Vector2.zero;
        btnLabelRect.anchorMax = Vector2.one;
        btnLabelRect.offsetMin = Vector2.zero;
        btnLabelRect.offsetMax = Vector2.zero;

        Text btnText = btnLabel.GetComponent<Text>();
        btnText.font = Font.CreateDynamicFontFromOSFont(new[] { "Consolas", "Courier New", "Arial" }, 20);
        btnText.fontSize = 20;
        btnText.color = new Color(0.78f, 0.70f, 0.58f, 1f);
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.raycastTarget = false;
        btnText.text = "CONTINUAR";

        panel.SetActive(false);

        Debug.Log("[UIScaffold] UpgradeShopPanel creado (inactivo). Actívalo para ajustar.");
    }

    private static T FindFirstObjectByType<T>() where T : Object
    {
        return Object.FindFirstObjectByType<T>();
    }
}
