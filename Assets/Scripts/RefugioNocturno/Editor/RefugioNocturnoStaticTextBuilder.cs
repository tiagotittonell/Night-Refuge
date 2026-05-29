using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class RefugioNocturnoStaticTextBuilder
{
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";
    private const string LayerName = "StaticTextLayer";
    private const string LayoutPath = "Assets/Scripts/RefugioNocturno/Editor/StaticTextLayout.json";

    [MenuItem("Refugio Nocturno/Add Static Texts")]
    public static void AddStaticTexts()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        Canvas canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No se encontro Canvas en SampleScene.");
            return;
        }

        Transform existingLayer = canvas.transform.Find(LayerName);
        if (existingLayer != null)
        {
            UnityEngine.Object.DestroyImmediate(existingLayer.gameObject);
        }

        GameObject layerObject = new GameObject(LayerName, typeof(RectTransform));
        layerObject.transform.SetParent(canvas.transform, false);
        RectTransform layerRect = layerObject.GetComponent<RectTransform>();
        layerRect.anchorMin = Vector2.zero;
        layerRect.anchorMax = Vector2.one;
        layerRect.offsetMin = Vector2.zero;
        layerRect.offsetMax = Vector2.zero;

        Font font = GetUiFont();

        CreateText(layerObject.transform, "RulesTitle", "REGLAS DEL REFUGIO", RectFromTopLeft(67, -123, 260, 38), font, 28, FontStyle.Bold, new Color(0.05f, 0.035f, 0.02f, 1f), TextAnchor.MiddleLeft);
        CreateText(layerObject.transform, "RulesBody",
            "- No dejes entrar a\n  imitadores.\n\n" +
            "- No dejes entrar a nadie\n  si tienes dudas.\n\n" +
            "- Recursos limitados.\n  Prioriza tu supervivencia.\n\n" +
            "- Confia en tu instinto,\n  pero verifica.",
            RectFromTopLeft(63, -184, 265, 290), font, 22, FontStyle.Normal, new Color(0.06f, 0.045f, 0.03f, 1f), TextAnchor.UpperLeft);

        CreateText(layerObject.transform, "HelpCard", "OBSERVA\nESCUCHA\nCUESTIONA\nDECIDE", RectFromTopLeft(114, -570, 145, 150), font, 28, FontStyle.Bold, new Color(0.06f, 0.045f, 0.03f, 1f), TextAnchor.MiddleCenter);

        CreateText(layerObject.transform, "ObservationsTitle", "OBSERVACIONES", RectFromTopLeft(1212, -72, 250, 36), font, 24, FontStyle.Bold, UiGold(), TextAnchor.MiddleLeft);
        CreateText(layerObject.transform, "ObservationLabels",
            "Ropa mojada\n\nTemblor\n\nMirada evasiva\n\nHeridas visibles\n\nComportamiento\n\nRespuestas",
            RectFromTopLeft(1212, -126, 190, 250), font, 20, FontStyle.Normal, UiTextDim(), TextAnchor.UpperLeft);
        CreateText(layerObject.transform, "ObservationValues",
            "SI\n\nNO\n\nSI\n\nNO\n\nNERVIOSO\n\nINCONSISTENTES",
            RectFromTopLeft(1405, -126, 100, 250), font, 20, FontStyle.Bold, UiAmber(), TextAnchor.UpperRight);

        CreateText(layerObject.transform, "NotesTitle", "NOTAS", RectFromTopLeft(1208, -415, 120, 35), font, 22, FontStyle.Bold, UiGold(), TextAnchor.MiddleLeft);
        CreateText(layerObject.transform, "NotesBody", "La tercera persona\nmenciona la misma\nhistoria...", RectFromTopLeft(1208, -456, 260, 95), font, 19, FontStyle.Normal, UiTextDim(), TextAnchor.UpperLeft);

        CreateText(layerObject.transform, "QuestionsTitleStatic", "PREGUNTAS", RectFromTopLeft(460, -758, 220, 32), font, 23, FontStyle.Bold, UiGold(), TextAnchor.MiddleLeft);
        CreateText(layerObject.transform, "DialogueTitleStatic", "VISITANTE:", RectFromTopLeft(460, -626, 180, 32), font, 23, FontStyle.Bold, UiAmber(), TextAnchor.MiddleLeft);

        CreateText(layerObject.transform, "AllowButtonLabel", "PERMITIR", RectFromTopLeft(1083, -681, 104, 31), font, 17, FontStyle.Bold, UiGold(), TextAnchor.MiddleCenter);
        CreateText(layerObject.transform, "RejectButtonLabel", "NO PERMITIR", RectFromTopLeft(1213, -681, 117, 31), font, 16, FontStyle.Bold, UiGold(), TextAnchor.MiddleCenter);
        CreateText(layerObject.transform, "DecisionWarningStatic", "UNA DECISION PUEDE\nCAMBIARLO TODO", RectFromTopLeft(1110, -815, 220, 55), font, 19, FontStyle.Bold, UiAmber(), TextAnchor.MiddleCenter);

        CreateText(layerObject.transform, "NightStatic", "NOCHE 1", RectFromTopLeft(42, -32, 135, 34), font, 22, FontStyle.Bold, UiGold(), TextAnchor.MiddleCenter);
        CreateText(layerObject.transform, "ClockStatic", "01:47 AM", RectFromTopLeft(220, -32, 120, 34), font, 22, FontStyle.Bold, UiGold(), TextAnchor.MiddleCenter);

        ApplyCapturedLayout(layerObject.transform);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Textos estaticos agregados a " + ScenePath);
    }

    [MenuItem("Refugio Nocturno/Capture Static Text Layout")]
    public static void CaptureStaticTextLayout()
    {
        Transform layer = FindStaticTextLayer();
        if (layer == null)
        {
            Debug.LogError("No se encontro StaticTextLayer. Crea o selecciona la escena con los textos estaticos primero.");
            return;
        }

        StaticTextLayout layout = new StaticTextLayout();
        foreach (Text text in layer.GetComponentsInChildren<Text>(true))
        {
            RectTransform rect = text.GetComponent<RectTransform>();
            layout.items.Add(new StaticTextLayoutItem
            {
                name = text.gameObject.name,
                text = text.text,
                anchorMin = rect.anchorMin,
                anchorMax = rect.anchorMax,
                pivot = rect.pivot,
                anchoredPosition = rect.anchoredPosition,
                sizeDelta = rect.sizeDelta,
                localScale = rect.localScale,
                localRotation = rect.localEulerAngles,
                fontSize = text.fontSize,
                fontStyle = text.fontStyle,
                color = text.color,
                alignment = text.alignment,
                horizontalOverflow = text.horizontalOverflow,
                verticalOverflow = text.verticalOverflow
            });
        }

        string json = JsonUtility.ToJson(layout, true);
        File.WriteAllText(LayoutPath, json);
        AssetDatabase.ImportAsset(LayoutPath);
        Debug.Log($"Layout de textos estaticos capturado: {layout.items.Count} elementos en {LayoutPath}");
    }

    [MenuItem("Refugio Nocturno/Apply Captured Static Text Layout")]
    public static void ApplyCapturedStaticTextLayout()
    {
        Transform layer = FindStaticTextLayer();
        if (layer == null)
        {
            Debug.LogError("No se encontro StaticTextLayer para aplicar layout.");
            return;
        }

        ApplyCapturedLayout(layer);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Layout capturado reaplicado sobre StaticTextLayer.");
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

    private static Transform FindStaticTextLayer()
    {
        Canvas canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            return null;
        }

        return canvas.transform.Find(LayerName);
    }

    private static void ApplyCapturedLayout(Transform layer)
    {
        if (!File.Exists(LayoutPath))
        {
            return;
        }

        StaticTextLayout layout = JsonUtility.FromJson<StaticTextLayout>(File.ReadAllText(LayoutPath));
        if (layout == null || layout.items == null)
        {
            return;
        }

        foreach (StaticTextLayoutItem item in layout.items)
        {
            Transform child = layer.Find(item.name);
            if (child == null)
            {
                continue;
            }

            RectTransform rect = child.GetComponent<RectTransform>();
            Text text = child.GetComponent<Text>();
            if (rect == null || text == null)
            {
                continue;
            }

            rect.anchorMin = item.anchorMin;
            rect.anchorMax = item.anchorMax;
            rect.pivot = item.pivot;
            rect.anchoredPosition = item.anchoredPosition;
            rect.sizeDelta = item.sizeDelta;
            rect.localScale = item.localScale;
            rect.localEulerAngles = item.localRotation;

            text.text = item.text;
            text.fontSize = item.fontSize;
            text.fontStyle = item.fontStyle;
            text.color = item.color;
            text.alignment = item.alignment;
            text.horizontalOverflow = item.horizontalOverflow;
            text.verticalOverflow = item.verticalOverflow;
        }
    }

    private static Font GetUiFont()
    {
        string[] preferredFonts = { "Consolas", "Courier New", "Lucida Console", "Arial" };
        Font font = Font.CreateDynamicFontFromOSFont(preferredFonts, 28);
        if (font != null)
        {
            return font;
        }

        return Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    private static Color UiGold()
    {
        return new Color(0.62f, 0.54f, 0.39f, 1f);
    }

    private static Color UiAmber()
    {
        return new Color(0.72f, 0.50f, 0.18f, 1f);
    }

    private static Color UiTextDim()
    {
        return new Color(0.58f, 0.50f, 0.38f, 1f);
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

    private static void ApplyRect(RectTransform rect, RectSpec spec)
    {
        rect.anchorMin = spec.anchorMin;
        rect.anchorMax = spec.anchorMax;
        rect.pivot = spec.pivot;
        rect.anchoredPosition = spec.anchoredPosition;
        rect.sizeDelta = spec.sizeDelta;
    }

    private struct RectSpec
    {
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 pivot;
        public Vector2 anchoredPosition;
        public Vector2 sizeDelta;
    }

    [Serializable]
    private class StaticTextLayout
    {
        public List<StaticTextLayoutItem> items = new List<StaticTextLayoutItem>();
    }

    [Serializable]
    private class StaticTextLayoutItem
    {
        public string name;
        public string text;
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 pivot;
        public Vector2 anchoredPosition;
        public Vector2 sizeDelta;
        public Vector3 localScale;
        public Vector3 localRotation;
        public int fontSize;
        public FontStyle fontStyle;
        public Color color;
        public TextAnchor alignment;
        public HorizontalWrapMode horizontalOverflow;
        public VerticalWrapMode verticalOverflow;
    }
}
