using System.Collections.Generic;
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
    
    public void SwapTeeth(GameObject newTooth, EnemySystem otherEn)
    {
        GameObject toothToSwap = FindToothByDamage(true, newTooth.GetComponent<CardVisual>());
        _EnSystem.teethInPlay.Remove(toothToSwap);
        otherEn.teethInPlay.Remove(newTooth);
        _EnSystem.teethInPlay.Add(newTooth);
        otherEn.teethInPlay.Add(toothToSwap);
        newTooth.transform.SetParent(_EnSystem.teethPositionUI);
        toothToSwap.transform.SetParent(otherEn.teethPositionUI);
    }

    public void SwapCard(GameObject newCard, EnemySystem otherEn)
    {
        GameObject cardToSwap = FindMostValuable();
        _EnSystem.hand.Remove(cardToSwap);
        otherEn.hand.Remove(newCard);
        _EnSystem.hand.Add(newCard);
        otherEn.hand.Add(cardToSwap);
        newCard.transform.SetParent(otherEn.handPositionUI);
        cardToSwap.transform.SetParent(_EnSystem.handPositionUI);
    }

    public GameObject FindToothByDamage(bool inverse, CardVisual otherTooth)
    {
        GameObject potentialTooth = null;
        int damage = 0;
        if (inverse)
        {
            damage = -100;
            for (int i = 0; i < _EnSystem.teethInPlay.Count; i++)
            {
                if (_EnSystem.teethInPlay[i].GetComponent<CardFunctionByHolder>().toothProtection > damage)
                {
                    damage = _EnSystem.teethInPlay[i].GetComponent<CardFunctionByHolder>().toothProtection;
                    potentialTooth = _EnSystem.teethInPlay[i];
                }
            }
        }
        else
        {
            damage = 100;
            for (int i = 0; i < _EnSystem.teethInPlay.Count; i++)
            {
                if (_EnSystem.teethInPlay[i].GetComponent<CardFunctionByHolder>().toothProtection < damage)
                {
                    damage = _EnSystem.teethInPlay[i].GetComponent<CardFunctionByHolder>().toothProtection;
                    potentialTooth = _EnSystem.teethInPlay[i];
                }
            }
        }

        if (damage == 0)
        {
            if (!inverse)
                return FindToothByColor(true, null);
            else
                return FindToothByColor(false, otherTooth);
        }
        else
            return potentialTooth;
    }

    public GameObject FindToothByColor(bool match, CardVisual otherTooth)
    {
        GameObject potentialTooth = null;
        if (match)
        {
            for (int i = 0; i < _EnSystem.teethInPlay.Count; i++)
            {
                if (potentialTooth == null)
                    potentialTooth = _EnSystem.teethInPlay[i];
                else
                {
                    if (_EnSystem.teethInPlay[i].GetComponent<CardVisual>().cardData.cardColor == potentialTooth.GetComponent<CardVisual>().cardData.cardColor)
                        potentialTooth = _EnSystem.teethInPlay[i];
                }
            }
        }
        else
        {
            if (otherTooth == null)
            {
                for (int i = 0; i < _EnSystem.teethInPlay.Count; i++)
                {
                    if (potentialTooth == null)
                        potentialTooth = _EnSystem.teethInPlay[i];
                    else
                    {
                        if (_EnSystem.teethInPlay[i].GetComponent<CardVisual>().cardData.cardColor != potentialTooth.GetComponent<CardVisual>().cardData.cardColor)
                            potentialTooth = _EnSystem.teethInPlay[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < _EnSystem.teethInPlay.Count; i++)
                {
                    if (_EnSystem.teethInPlay[i].GetComponent<CardVisual>().cardData.cardColor != otherTooth.cardData.cardColor)
                        potentialTooth = _EnSystem.teethInPlay[i];
                }
            }
        }
        return potentialTooth;
    }

    public GameObject FindMostValuable()
    {
        for (int i = 0; i < _EnSystem.hand.Count; i++)
        {
            if (_EnSystem.hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.HealthyTooth)
            {
                return _EnSystem.hand[i];
            }
        }

        for (int i = 0; i < _EnSystem.hand.Count; i++)
        {
            if (_EnSystem.hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Protective)
            {
                return _EnSystem.hand[i];
            }
        }

        for (int i = 0; i < _EnSystem.hand.Count; i++)
        {
            if (_EnSystem.hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Harmful)
            {
                return _EnSystem.hand[i];
            }
        }

        for (int i = 0; i < _EnSystem.hand.Count; i++)
        {
            if (_EnSystem.hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Treatment)
            {
                return _EnSystem.hand[i];
            }
        }

        Debug.LogError("Couldn't find a valuable card in " + gameObject.name + "'s hand");
        return null;
    }

    public void DiscardMostValuable()
    {
        Discard(FindMostValuable());
    }

    [ContextMenu("Discard")]
    public void Discard(GameObject card)
    {
        string name = card.GetComponent<CardVisual>().cardData.cardName;
        _EnSystem.DiscardCard(card);
        Invoke("DrawCard", 1);
    }
}
