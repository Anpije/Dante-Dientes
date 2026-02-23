using UnityEngine;

[CreateAssetMenu(fileName = "NuevaCarta", menuName = "Cartas/Carta")]
public class Cards : ScriptableObject
{
    [Header(" Info")]
    public string cardName;
    public string description;
    public Sprite cardFront; 
    public Sprite cardBack;
    
    
    [Header(" Propiedades")]
    public CardType cardType;
    public CardColor cardColor;
    public int value;
    
    [Header(" Efectos ")]
    public bool canTargetPlayer = true;
    public bool canTargetOrgan = false;
    public CardEffect[] effects;
    
    public enum CardType
    {
        Tooth,
        Virus,
        Medicine,
        Treatment
    }
    
    public enum CardColor
    {
        Red,
        Blue,
        Green,
        Yellow,
        Multicolor
    }
    
    [System.Serializable]
    public class CardEffect
    {
        public EffectType type;
        public int amount;
        public CardColor targetColor;
        
        public enum EffectType
        {
            Heal,
            Damage,
            Protect,
            Steal,
            Draw
        }
    }
}