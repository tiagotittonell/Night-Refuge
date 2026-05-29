using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private Button questionButtonPrefab;
    [SerializeField] private Transform questionsContainer;
    [SerializeField] private TMP_Text questionsRemainingText;
    [SerializeField] private Text questionsRemainingLegacyText;

    [Header("Reglas")]
    [SerializeField] private int maxQuestionsPerVisitor = 2;

    public event Action<QuestionAnswer> QuestionAnswered;

    private readonly List<Button> spawnedButtons = new List<Button>();
    private int questionsRemaining;

    private void Awake()
    {
        EnsureRuntimeBindings();
    }

    public void SetupQuestions(VisitorData visitor)
    {
        EnsureRuntimeBindings();
        ClearButtons();
        questionsRemaining = maxQuestionsPerVisitor;
        UpdateRemainingText();

        if (visitor == null || questionsContainer == null)
        {
            return;
        }

        foreach (QuestionAnswer questionAnswer in visitor.answers)
        {
            Button button = questionButtonPrefab != null
                ? Instantiate(questionButtonPrefab, questionsContainer)
                : CreateFallbackQuestionButton(questionsContainer);
            button.gameObject.SetActive(true);
            ApplyQuestionButtonRect(button);

            TMP_Text label = button.GetComponentInChildren<TMP_Text>();
            Text legacyLabel = button.GetComponentInChildren<Text>();

            if (label != null)
            {
                label.text = questionAnswer.question;
            }

            if (legacyLabel != null)
            {
                legacyLabel.text = questionAnswer.question;
            }

            QuestionAnswer capturedAnswer = questionAnswer;
            button.onClick.AddListener(() => AskQuestion(button, capturedAnswer));
            spawnedButtons.Add(button);
        }
    }

    public void SetInteractable(bool interactable)
    {
        foreach (Button button in spawnedButtons)
        {
            if (button != null)
            {
                button.interactable = interactable && questionsRemaining > 0;
            }
        }
    }

    private void AskQuestion(Button button, QuestionAnswer questionAnswer)
    {
        if (questionsRemaining <= 0)
        {
            return;
        }

        questionsRemaining--;
        button.interactable = false;

        if (dialogueUI != null)
        {
            dialogueUI.ShowAnswer(questionAnswer.answer);
        }

        // Consume clock time for asking a question
        NightClock clock = UnityEngine.Object.FindFirstObjectByType<NightClock>();
        if (clock != null)
        {
            clock.ConsumeQuestion();
        }

        QuestionAnswered?.Invoke(questionAnswer);

        if (questionsRemaining <= 0)
        {
            SetInteractable(false);
        }

        UpdateRemainingText();
    }

    private void UpdateRemainingText()
    {
        EnsureRuntimeBindings();

        if (questionsRemainingText != null)
        {
            questionsRemainingText.text = $"Preguntas: {questionsRemaining}";
        }

        if (questionsRemainingLegacyText != null)
        {
            questionsRemainingLegacyText.text = $"Preguntas: {questionsRemaining}";
        }
    }

    private void ClearButtons()
    {
        foreach (Button button in spawnedButtons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }

        spawnedButtons.Clear();
    }

    private void EnsureRuntimeBindings()
    {
        GameObject dynamicLayer = GameObject.Find("DynamicGameplayLayer");
        if (dynamicLayer != null)
        {
            dynamicLayer.transform.SetAsLastSibling();
        }

        if (questionsContainer == null)
        {
            GameObject container = GameObject.Find("RuntimeQuestionsContainer");
            if (container != null)
            {
                questionsContainer = container.transform;
            }
        }

        if (questionsRemainingLegacyText == null)
        {
            GameObject counter = GameObject.Find("RuntimeQuestionCounterText");
            if (counter != null)
            {
                questionsRemainingLegacyText = counter.GetComponent<Text>();
            }
        }

        if (questionButtonPrefab == null)
        {
            GameObject prefab = GameObject.Find("RuntimeQuestionButtonPrefab");
            if (prefab != null)
            {
                questionButtonPrefab = prefab.GetComponent<Button>();
            }
        }
    }

    private Button CreateFallbackQuestionButton(Transform parent)
    {
        GameObject buttonObject = new GameObject("RuntimeQuestionButton", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(400f, 30f);

        LayoutElement layoutElement = buttonObject.AddComponent<LayoutElement>();
        layoutElement.minHeight = 30f;
        layoutElement.preferredHeight = 30f;
        layoutElement.flexibleHeight = 0f;

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.16f, 0.13f, 0.09f, 0.75f);

        GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(buttonObject.transform, false);
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(12f, 2f);
        textRect.offsetMax = new Vector2(-12f, -2f);

        Text text = textObject.GetComponent<Text>();
        text.font = Font.CreateDynamicFontFromOSFont(new[] { "Consolas", "Courier New", "Arial" }, 18);
        text.fontSize = 17;
        text.color = new Color(0.78f, 0.70f, 0.58f, 1f);
        text.alignment = TextAnchor.MiddleLeft;
        text.raycastTarget = false;

        return buttonObject.GetComponent<Button>();
    }

    private void ApplyQuestionButtonRect(Button button)
    {
        RectTransform rect = button.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(0f, 30f);
            rect.localScale = Vector3.one;
        }

        LayoutElement layoutElement = button.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = button.gameObject.AddComponent<LayoutElement>();
        }

        layoutElement.minHeight = 30f;
        layoutElement.preferredHeight = 30f;
        layoutElement.flexibleHeight = 0f;
    }
}
