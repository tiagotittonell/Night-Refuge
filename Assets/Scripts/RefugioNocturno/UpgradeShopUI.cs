using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Panel de tienda de mejoras que aparece entre noches.
/// Muestra mejoras disponibles y permite comprarlas con suministros.
/// </summary>
public class UpgradeShopUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text suppliesText;
    [SerializeField] private Text suppliesLegacyText;
    [SerializeField] private TMP_Text upgradesListText;
    [SerializeField] private Text upgradesListLegacyText;
    [SerializeField] private Transform buttonsContainer;
    [SerializeField] private Button continueButton;

    private UpgradeManager upgradeManager;
    private NightManager nightManager;
    private readonly List<Button> spawnedButtons = new List<Button>();

    public event System.Action ShopClosed;

    private void Awake()
    {
        EnsureBindings();
        Hide();
    }

    public void Initialize(NightManager manager)
    {
        nightManager = manager;
    }

    public void Show()
    {
        EnsureBindings();

        if (panelRoot == null)
        {
            CreateFallbackPanel();
        }

        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
            panelRoot.transform.SetAsLastSibling();
        }

        RefreshDisplay();

        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(OnContinue);
            continueButton.onClick.AddListener(OnContinue);
        }
    }

    public void Hide()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }

        ClearButtons();
    }

    private void OnContinue()
    {
        Hide();
        ShopClosed?.Invoke();
    }

    private void RefreshDisplay()
    {
        if (upgradeManager == null)
        {
            return;
        }

        string suppliesValue = $"SUMINISTROS: {upgradeManager.Supplies}";
        SetText(suppliesText, suppliesLegacyText, suppliesValue);

        ClearButtons();

        List<UpgradeData> purchasable = upgradeManager.GetPurchasableUpgrades();
        StringBuilder listBuilder = new StringBuilder();
        listBuilder.AppendLine("MEJORAS DISPONIBLES");
        listBuilder.AppendLine("═══════════════════════════");

        foreach (UpgradeData upgrade in purchasable)
        {
            bool canAfford = upgradeManager.CanAfford(upgrade);
            string affordMarker = canAfford ? "●" : "○";
            listBuilder.AppendLine();
            listBuilder.AppendLine($"{affordMarker} {upgrade.upgradeName} [{upgrade.cost} suministros]");
            listBuilder.AppendLine($"  {upgrade.description}");

            if (!string.IsNullOrWhiteSpace(upgrade.drawback))
            {
                listBuilder.AppendLine($"  ⚠ {upgrade.drawback}");
            }

            // Create purchase button
            if (buttonsContainer != null)
            {
                CreateUpgradeButton(upgrade, canAfford);
            }
        }

        if (purchasable.Count == 0)
        {
            listBuilder.AppendLine();
            listBuilder.AppendLine("[ No hay mejoras disponibles ]");
        }

        SetText(upgradesListText, upgradesListLegacyText, listBuilder.ToString());
    }

    private void CreateUpgradeButton(UpgradeData upgrade, bool canAfford)
    {
        GameObject buttonObject = new GameObject($"UpgradeBtn_{upgrade.id}", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(buttonsContainer, false);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(0f, 50f);

        LayoutElement layout = buttonObject.AddComponent<LayoutElement>();
        layout.minHeight = 50f;
        layout.preferredHeight = 50f;

        Image image = buttonObject.GetComponent<Image>();
        Sprite btnSprite = canAfford ? UISpriteLoader.ButtonNormal : UISpriteLoader.ButtonDisabled;
        if (btnSprite != null)
        {
            image.sprite = btnSprite;
            image.type = Image.Type.Sliced;
            image.color = Color.white;
        }
        else
        {
            image.color = canAfford
                ? new Color(0.15f, 0.2f, 0.12f, 0.85f)
                : new Color(0.12f, 0.1f, 0.08f, 0.6f);
        }

        Button button = buttonObject.GetComponent<Button>();
        button.interactable = canAfford;

        // Apply sprite transitions for hover/disabled
        Sprite hoverSprite = UISpriteLoader.ButtonHover;
        Sprite disabledSprite = UISpriteLoader.ButtonDisabled;
        if (btnSprite != null && hoverSprite != null)
        {
            button.transition = Selectable.Transition.SpriteSwap;
            SpriteState spriteState = new SpriteState
            {
                highlightedSprite = hoverSprite,
                pressedSprite = hoverSprite,
                disabledSprite = disabledSprite
            };
            button.spriteState = spriteState;
        }

        // Upgrade icon
        Sprite upgradeIcon = UISpriteLoader.GetUpgradeIcon(upgrade.effect);
        if (upgradeIcon != null)
        {
            GameObject iconObj = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            iconObj.transform.SetParent(buttonObject.transform, false);

            RectTransform iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0f, 0.5f);
            iconRect.anchorMax = new Vector2(0f, 0.5f);
            iconRect.pivot = new Vector2(0f, 0.5f);
            iconRect.anchoredPosition = new Vector2(6f, 0f);
            iconRect.sizeDelta = new Vector2(36f, 36f);

            Image iconImage = iconObj.GetComponent<Image>();
            iconImage.sprite = upgradeIcon;
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;
        }

        // Label (offset to the right if icon present)
        float labelLeft = upgradeIcon != null ? 48f : 12f;
        GameObject textObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(buttonObject.transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(labelLeft, 2f);
        textRect.offsetMax = new Vector2(-12f, -2f);

        Text label = textObject.GetComponent<Text>();
        label.font = Font.CreateDynamicFontFromOSFont(new[] { "Consolas", "Courier New", "Arial" }, 15);
        label.fontSize = 15;
        label.color = canAfford
            ? new Color(0.78f, 0.70f, 0.58f, 1f)
            : new Color(0.5f, 0.45f, 0.4f, 0.7f);
        label.alignment = TextAnchor.MiddleLeft;
        label.raycastTarget = false;
        label.text = $"{upgrade.upgradeName} - {upgrade.cost} suministros";

        UpgradeData capturedUpgrade = upgrade;
        button.onClick.AddListener(() => PurchaseUpgrade(capturedUpgrade));

        spawnedButtons.Add(button);
    }

    private void PurchaseUpgrade(UpgradeData upgrade)
    {
        if (upgradeManager == null)
        {
            return;
        }

        if (upgradeManager.TryPurchase(upgrade))
        {
            RefreshDisplay();
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

    private void SetText(TMP_Text tmpText, Text legacyText, string value)
    {
        if (tmpText != null)
        {
            tmpText.text = value;
        }

        if (legacyText != null)
        {
            legacyText.text = value;
        }
    }

    private void EnsureBindings()
    {
        if (upgradeManager == null)
        {
            upgradeManager = Object.FindFirstObjectByType<UpgradeManager>();
        }

        if (panelRoot == null)
        {
            GameObject existing = GameObject.Find("UpgradeShopPanel");
            if (existing != null)
            {
                panelRoot = existing;
            }
        }

        if (continueButton == null)
        {
            GameObject btn = GameObject.Find("UpgradeShopContinueButton");
            if (btn != null)
            {
                continueButton = btn.GetComponent<Button>();
            }
        }

        if (buttonsContainer == null)
        {
            GameObject container = GameObject.Find("UpgradeButtonsContainer");
            if (container != null)
            {
                buttonsContainer = container.transform;
            }
        }
    }

    private void CreateFallbackPanel()
    {
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        Transform parent = canvas != null ? canvas.transform : transform;

        // Panel root
        GameObject panel = new GameObject("UpgradeShopPanel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(parent, false);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.1f, 0.05f);
        panelRect.anchorMax = new Vector2(0.9f, 0.95f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = Vector2.zero;

        Image panelImage = panel.GetComponent<Image>();
        Sprite shopBg = UISpriteLoader.ShopBackground;
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

        panelRoot = panel;

        // Supplies text with coin icon
        GameObject suppliesObj = new GameObject("SuppliesText", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        suppliesObj.transform.SetParent(panel.transform, false);

        RectTransform suppliesRect = suppliesObj.GetComponent<RectTransform>();
        suppliesRect.anchorMin = new Vector2(0f, 0.9f);
        suppliesRect.anchorMax = new Vector2(1f, 1f);
        suppliesRect.offsetMin = new Vector2(30f, 0f);
        suppliesRect.offsetMax = new Vector2(-30f, -10f);

        HorizontalLayoutGroup hlg = suppliesObj.GetComponent<HorizontalLayoutGroup>();
        hlg.spacing = 8f;
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childControlWidth = false;
        hlg.childControlHeight = false;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;

        // Coin icon
        Sprite coinSprite = UISpriteLoader.SuppliesCoin;
        if (coinSprite != null)
        {
            GameObject coinObj = new GameObject("CoinIcon", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            coinObj.transform.SetParent(suppliesObj.transform, false);

            Image coinImage = coinObj.GetComponent<Image>();
            coinImage.sprite = coinSprite;
            coinImage.preserveAspect = true;
            coinImage.raycastTarget = false;

            LayoutElement coinLayout = coinObj.GetComponent<LayoutElement>();
            coinLayout.preferredWidth = 28f;
            coinLayout.preferredHeight = 28f;
        }

        // Supplies label
        GameObject suppliesTextObj = new GameObject("Label", typeof(RectTransform), typeof(Text), typeof(LayoutElement));
        suppliesTextObj.transform.SetParent(suppliesObj.transform, false);

        LayoutElement suppliesTextLayout = suppliesTextObj.GetComponent<LayoutElement>();
        suppliesTextLayout.preferredWidth = 350f;
        suppliesTextLayout.preferredHeight = 30f;

        Text suppliesLabel = suppliesTextObj.GetComponent<Text>();
        suppliesLabel.font = Font.CreateDynamicFontFromOSFont(new[] { "Consolas", "Courier New", "Arial" }, 22);
        suppliesLabel.fontSize = 22;
        suppliesLabel.color = new Color(0.85f, 0.75f, 0.4f, 1f);
        suppliesLabel.alignment = TextAnchor.MiddleLeft;
        suppliesLabel.raycastTarget = false;

        suppliesLegacyText = suppliesLabel;

        // Upgrades list text
        GameObject listObj = new GameObject("UpgradesListText", typeof(RectTransform), typeof(Text));
        listObj.transform.SetParent(panel.transform, false);

        RectTransform listRect = listObj.GetComponent<RectTransform>();
        listRect.anchorMin = new Vector2(0f, 0.15f);
        listRect.anchorMax = new Vector2(0.55f, 0.88f);
        listRect.offsetMin = new Vector2(30f, 0f);
        listRect.offsetMax = new Vector2(0f, 0f);

        Text listLabel = listObj.GetComponent<Text>();
        listLabel.font = Font.CreateDynamicFontFromOSFont(new[] { "Consolas", "Courier New", "Arial" }, 16);
        listLabel.fontSize = 16;
        listLabel.color = new Color(0.70f, 0.63f, 0.52f, 1f);
        listLabel.alignment = TextAnchor.UpperLeft;
        listLabel.raycastTarget = false;

        upgradesListLegacyText = listLabel;

        // Buttons container
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

        buttonsContainer = containerObj.transform;

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
        Sprite continueBtnSprite = UISpriteLoader.ButtonNormal;
        if (continueBtnSprite != null)
        {
            btnImage.sprite = continueBtnSprite;
            btnImage.type = Image.Type.Sliced;
            btnImage.color = Color.white;
        }
        else
        {
            btnImage.color = new Color(0.18f, 0.15f, 0.10f, 0.95f);
        }

        continueButton = btnObj.GetComponent<Button>();

        // Apply sprite transitions for continue button
        Sprite continueHover = UISpriteLoader.ButtonHover;
        if (continueBtnSprite != null && continueHover != null)
        {
            continueButton.transition = Selectable.Transition.SpriteSwap;
            SpriteState continueSpriteState = new SpriteState
            {
                highlightedSprite = continueHover,
                pressedSprite = continueHover
            };
            continueButton.spriteState = continueSpriteState;
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
    }
}
