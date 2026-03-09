using UnityEngine.UI;
using UnityEngine;

public class EnemyActions : MonoBehaviour
{
    EnemySystem _EnSystem;
    EnemyBehaviour _EnBehave;

    void Awake()
    {
        _EnSystem = GetComponent<EnemySystem>();
        _EnBehave = GetComponent<EnemyBehaviour>();
    }

    public void PlayCard(GameObject card)
    {
        // Play card
        _EnSystem.DiscardCard(card);
    }

    [ContextMenu("Draw")]
    public void DrawCard()
    {
        GameObject newCard = _EnSystem.DrawCard();
        if (newCard != null)
        {
            CardVisual visual = newCard.GetComponent<CardVisual>();
        }
    }

    [ContextMenu("RevealHand")]
    public void RevealHand()
    {
        _EnSystem.handPositionUI.gameObject.SetActive(true);
    }

    public void HideHand()
    {
        _EnSystem.handPositionUI.gameObject.SetActive(false);
    }
    
    public void StealCard()
    {
        
    }

    [ContextMenu("Discard")]
    public void Discard()
    {
        var cards = _EnSystem.GetAllCardsInHand();
        if (cards.Count > 0)
        {
            GameObject card = cards[0].gameObject;
            string name = card.GetComponent<CardVisual>().cardData.cardName;
            _EnSystem.DiscardCard(card);
            Invoke("DrawCard", 1);
        }
    }
}
