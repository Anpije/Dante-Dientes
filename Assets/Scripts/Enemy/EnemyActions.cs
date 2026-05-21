using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyActions : MonoBehaviour
{
    EnemySystem _EnSystem;
    EnemyBehaviour _EnBehave;

    void Awake()
    {
        _EnSystem = GetComponent<EnemySystem>();
        _EnBehave = GetComponent<EnemyBehaviour>();
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
        int oldIndex = toothToSwap.transform.GetSiblingIndex();
        int newIndex = newTooth.transform.GetSiblingIndex();
        _EnSystem.teethInPlay.Remove(toothToSwap);
        otherEn.teethInPlay.Remove(newTooth);
        _EnSystem.teethInPlay.Add(newTooth);
        otherEn.teethInPlay.Add(toothToSwap);
        newTooth.transform.SetParent(_EnSystem.teethPositionUI);
        newTooth.transform.SetSiblingIndex(oldIndex);
        toothToSwap.transform.SetParent(otherEn.teethPositionUI);
        toothToSwap.transform.SetSiblingIndex(newIndex);
        _EnSystem.teethModels[oldIndex].fModifyTooth((int)newTooth.GetComponent<CardVisual>().cardData.cardColor);
        newTooth.GetComponent<CardFunctionByHolder>().connectedTooth = _EnSystem.teethModels[oldIndex];
        _EnSystem.teethModels[oldIndex].effect = newTooth.GetComponent<CardFunctionByHolder>().toothProtection;
        otherEn.teethModels[newIndex].fModifyTooth((int)toothToSwap.GetComponent<CardVisual>().cardData.cardColor);
        toothToSwap.GetComponent<CardFunctionByHolder>().connectedTooth = otherEn.teethModels[newIndex];
        otherEn.teethModels[newIndex].effect = toothToSwap.GetComponent<CardFunctionByHolder>().toothProtection;
    }

    public void SwapCard(GameObject newCard, EnemySystem otherEn)
    {
        GameObject cardToSwap = _EnSystem.hand[Random.Range(0, 4)];
        int oldIndex = cardToSwap.transform.GetSiblingIndex();
        int newIndex = newCard.transform.GetSiblingIndex();
        _EnSystem.hand.Remove(cardToSwap);
        otherEn.hand.Remove(newCard);
        _EnSystem.hand.Add(newCard);
        otherEn.hand.Add(cardToSwap);
        cardToSwap.transform.SetParent(otherEn.handPositionUI);
        cardToSwap.transform.SetSiblingIndex(newIndex);
        newCard.transform.SetParent(_EnSystem.handPositionUI);
        newCard.transform.SetSiblingIndex(oldIndex);
        otherEn.cardModels[newIndex].fReturnFromPosition(_EnSystem.cardModels[oldIndex].transform, 0.15f);
        _EnSystem.cardModels[oldIndex].fReturnFromPosition(otherEn.cardModels[newIndex].transform, 0.15f);
    }

    public GameObject ReplaceTooth(GameObject newTooth)
    {
        GameObject toothToSwap = null;

        for (int i = 0; i < _EnSystem.teethInPlay.Count; i++)
        {
            for (int j = 0; j < _EnSystem.teethInPlay.Count; j++)
            {
                if (i != j && _EnSystem.teethInPlay[i].GetComponent<CardVisual>().cardData.cardColor == _EnSystem.teethInPlay[j].GetComponent<CardVisual>().cardData.cardColor)
                {
                    if (_EnSystem.teethInPlay[i].GetComponent<CardVisual>().cardData.cardColor != newTooth.GetComponent<CardVisual>().cardData.cardColor)
                        toothToSwap = _EnSystem.teethInPlay[i];
                }
            }
        }

        if (toothToSwap != null)
        {
            int toothIndex = toothToSwap.transform.GetSiblingIndex();

            _EnSystem.teethInPlay.Remove(toothToSwap);
            _EnSystem.teethInPlay.Add(newTooth);
            newTooth.transform.SetParent(_EnSystem.teethPositionUI);
            toothToSwap.transform.SetAsLastSibling();
            newTooth.transform.SetSiblingIndex(toothIndex);
            FindFirstObjectByType<CardSystem>().allCardObjects.Remove(toothToSwap);
            Destroy(toothToSwap);

            _EnSystem.teethModels[toothIndex].fModifyTooth((int)newTooth.GetComponent<CardVisual>().cardData.cardColor);
            newTooth.GetComponent<CardFunctionByHolder>().connectedTooth = _EnSystem.teethModels[toothIndex];
            _EnSystem.teethModels[toothIndex].effect = newTooth.GetComponent<CardFunctionByHolder>().toothProtection;
            return null;
        }
        else
        {
            for (int i = 0; i < _EnSystem.teethInPlay.Count; i++)
            {
                if (_EnSystem.teethInPlay[i].GetComponent<CardVisual>().cardData.cardColor == newTooth.GetComponent<CardVisual>().cardData.cardColor)
                    toothToSwap = _EnSystem.teethInPlay[i];
            }

            if (toothToSwap == null)
                return newTooth;

            if (toothToSwap.GetComponent<CardFunctionByHolder>().toothProtection >= 0)
                return newTooth;

            int toothIndex = toothToSwap.transform.GetSiblingIndex();

            _EnSystem.teethInPlay.Remove(toothToSwap);
            _EnSystem.teethInPlay.Add(newTooth);
            newTooth.transform.SetParent(_EnSystem.teethPositionUI);
            toothToSwap.transform.SetAsLastSibling();
            newTooth.transform.SetSiblingIndex(toothIndex);
            FindFirstObjectByType<CardSystem>().allCardObjects.Remove(toothToSwap);
            Destroy(toothToSwap);

            _EnSystem.teethModels[toothIndex].fModifyTooth((int)newTooth.GetComponent<CardVisual>().cardData.cardColor);
            newTooth.GetComponent<CardFunctionByHolder>().connectedTooth = _EnSystem.teethModels[toothIndex];
            _EnSystem.teethModels[toothIndex].effect = newTooth.GetComponent<CardFunctionByHolder>().toothProtection;

            return null;
        }
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
        if (potentialTooth == null)
            return _EnSystem.teethInPlay[Random.Range(0, _EnSystem.teethInPlay.Count)];
        else
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

    public GameObject FindLeastValuable()
    {
        for (int i = 0; i < _EnSystem.hand.Count; i++)
        {
            if (_EnSystem.hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Treatment)
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
            if (_EnSystem.hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Protective)
            {
                return _EnSystem.hand[i];
            }
        }

        for (int i = 0; i < _EnSystem.hand.Count; i++)
        {
            if (_EnSystem.hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.HealthyTooth)
            {
                return _EnSystem.hand[i];
            }
        }

        Debug.LogError("Couldn't find a valuable card in " + gameObject.name + "'s hand");
        return null;
    }

    public bool CheckWinCondition()
    {
        CardVisual[] teethColors = new CardVisual[4];
        int rainbowTeeth = 0;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if ((int)_EnSystem.teethInPlay[i].GetComponent<CardVisual>().cardData.cardColor == j)
                {
                    if (_EnSystem.teethInPlay[i].GetComponent<CardFunctionByHolder>().toothProtection >= 0)
                        teethColors[j] = _EnSystem.teethInPlay[i].GetComponent<CardVisual>();
                }
            }

            if ((int)_EnSystem.teethInPlay[i].GetComponent<CardVisual>().cardData.cardColor == 4 && _EnSystem.teethInPlay[i].GetComponent<CardFunctionByHolder>().toothProtection >= 0)
                rainbowTeeth++;
        }

        int toWin = 0;

        for (int i = 0; i < 4; i++)
            if (teethColors[i] != null) toWin++;

        if (toWin + rainbowTeeth == 4)
            return true;
        else
            return false;
    }

    public void DiscardMostValuable()
    {
        Discard(FindMostValuable(), null);
    }

    [ContextMenu("Discard")]
    public void Discard(GameObject card, Transform newPos)
    {
        string name = card.GetComponent<CardVisual>().cardData.cardName;
        _EnSystem.DiscardCard(card, newPos);
        Invoke("DrawCard", 1);
    }
}
