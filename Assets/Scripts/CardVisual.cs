using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class CardVisual : MonoBehaviour, IPointerClickHandler
{
    [Header("Card Data")]
    public CardData cardData;
    
    [Header("UI References")]
    public Image backgroundImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI typeText;
    public Image colorIndicator;
    public TextMeshProUGUI descriptionText;

    // Colores para las cartas (Más adelante se puede quitar para el arte)

    // Beige
    private readonly Color toothColor = new Color(1f, 0.9f, 0.7f);

    // Verde más brillante
    private readonly Color protectiveColor = new Color(0.5f, 1f, 0.5f);

    // Rojo más brillante
    private readonly Color harmfulColor = new Color(1f, 0.5f, 0.5f);

    // Azul más brillante
    private readonly Color treatmentColor = new Color(0.5f, 0.8f, 1f); 
    
    public void Initialize(CardData data)
    {
        cardData = data;
        
        // Para el nobre las cartas (Reemplable también por el arte)
        if (nameText != null)
            nameText.text = data.cardName;
        
        if (typeText != null)
            typeText.text = GetTypeName(data.cardType);
        
        if (descriptionText != null)
            descriptionText.text = data.description;

        // esto solo será para el color del fondo esto lo quitaremos cuando ya los de arte tenga los diseños de las cartas
        if (backgroundImage != null)
        {
            switch (data.cardType)
            {
                case CardType.HealthyTooth:
                    backgroundImage.color = toothColor;
                    break;
                case CardType.Protective:
                    backgroundImage.color = protectiveColor;
                    break;
                case CardType.Harmful:
                    backgroundImage.color = harmfulColor;
                    break;
                case CardType.Treatment:
                    backgroundImage.color = treatmentColor;
                    break;
            }
            
            // la imagen de fondo sea visible de background del Panel
            backgroundImage.raycastTarget = true;
        }
        
        // indicador de color de cada carta, también reemplazable por el arte
        if (colorIndicator != null)
        {
            if (data.cardType == CardType.Treatment)
            {
                colorIndicator.gameObject.SetActive(false);
            }
            else
            {
                colorIndicator.gameObject.SetActive(true);
                colorIndicator.color = data.GetColorRGB();
            }
        }

        // renderizado de las carta tengas
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1;
        }
    }
    
    //En escena tenemos la carta por su nombre y que tipo es, aquí definó que son 
    private string GetTypeName(CardType type)
    {
        switch (type)
        {
            case CardType.HealthyTooth: return "Diente Sano";
            case CardType.Protective: return "Protectora";
            case CardType.Harmful: return "Dañina";
            case CardType.Treatment: return "Tratamiento";
            default: return type.ToString();
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Carta: {cardData.cardName} | Tipo: {cardData.cardType} | Color: {cardData.cardColor}");
    }
}