using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
//using Unity.Android.Gradle.Manifest;

public class CardVisual : MonoBehaviour, IPointerClickHandler
{
    [Header("Card Data")]
    public CardData cardData;

    [Header("UI References")]
    private Image _Image;

    public static System.Action<CardVisual> OnCursorOverCard;
    public static System.Action OnCursorExitCard;

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
        _Image = GetComponent<Image>();
        cardData = data;

        if (data.cardSprite != null)
            _Image.sprite = data.cardSprite;
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
    
    public void ToggleText(bool visible)
    {
        Transform parentCanvas = GetComponentInParent<Canvas>().gameObject.transform;
        if (visible)
            OnCursorOverCard?.Invoke(this);
        else
            OnCursorExitCard?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Carta: {cardData.cardName} | Tipo: {cardData.cardType} | Color: {cardData.cardColor}");
    }
}