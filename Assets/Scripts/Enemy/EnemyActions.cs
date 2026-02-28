using UnityEngine.UI;
using UnityEngine;

public class EnemyActions : MonoBehaviour
{
    EnemySystem _EnSystem;

    [Header("Display Info")]
    [SerializeField] private Text infoText;

    void Awake()
    {
        _EnSystem = GetComponent<EnemySystem>();
    }

    public void PlayCard()
    {

    }

    public void DrawCard()
    {
        GameObject newCard = _EnSystem.DrawCard();
        if (newCard != null)
        {
            CardVisual visual = newCard.GetComponent<CardVisual>();
            infoText.text = $"Drawn: {visual.cardData.cardName} for {gameObject.name}";
        }
        else
            infoText.text = "No hay más cartas en el mazo";
    }

    public void StealCard()
    {
        
    }

    public void Discard()
    {
        if (CardSystem.Instance == null) return;

        var cards = _EnSystem.GetAllCardsInHand();
        if (cards.Count > 0)
        {
            GameObject card = cards[0].gameObject;
            string name = card.GetComponent<CardVisual>().cardData.cardName;
            CardSystem.Instance.DiscardCard(card);
            infoText.text = $"Descartada: {name}";
        }
        else
            infoText.text = "No hay cartas en la mano";
    }
}
