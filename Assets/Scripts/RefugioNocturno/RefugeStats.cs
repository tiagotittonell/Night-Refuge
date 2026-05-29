using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RefugeStats : MonoBehaviour
{
    [Header("Recursos iniciales")]
    [SerializeField] private int food = 6;
    [SerializeField] private int security = 5;
    [SerializeField] private int morale = 5;
    [SerializeField] private int population = 2;

    [Header("UI")]
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text securityText;
    [SerializeField] private TMP_Text moraleText;
    [SerializeField] private TMP_Text populationText;

    [Header("UI Legacy Text")]
    [SerializeField] private Text foodLegacyText;
    [SerializeField] private Text securityLegacyText;
    [SerializeField] private Text moraleLegacyText;
    [SerializeField] private Text populationLegacyText;

    public int Food => food;
    public int Security => security;
    public int Morale => morale;
    public int Population => population;
    public bool SecurityCollapsed => security <= 0;
    public bool MoraleCollapsed => morale <= 0;
    public bool FoodDepleted => food <= 0;

    private void Start()
    {
        UpdateUI();
    }

    public void ApplyChanges(int foodChange, int securityChange, int moraleChange, int populationChange)
    {
        food = Mathf.Max(0, food + foodChange);
        security = Mathf.Clamp(security + securityChange, 0, 10);
        morale = Mathf.Clamp(morale + moraleChange, 0, 10);
        population = Mathf.Max(0, population + populationChange);
        UpdateUI();
    }

    public void UpdateUI()
    {
        SetText(foodText, foodLegacyText, $"COMIDA {food}");
        SetText(securityText, securityLegacyText, $"SEGURIDAD {security}");
        SetText(moraleText, moraleLegacyText, $"MORAL {morale}");
        SetText(populationText, populationLegacyText, $"POBLACION {population}");
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
}
