using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "DientesSanos/Card Data")]
public class CardData : ScriptableObject
{
    public string cardName;
    public CardType cardType;
    public ToothColor cardColor;

    public int toothProtection = 0;

    [TextArea(2, 4)]
    public string description;

    // debugging 
    public Color GetColorRGB()
    {
        switch (cardColor)
        {
            case ToothColor.Blue: return Color.blue;
            case ToothColor.Red: return Color.red;
            case ToothColor.Green: return Color.green;
            case ToothColor.Yellow: return Color.yellow;
            default: return Color.white;
        }
    }
}