using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;

public class PlayerActions : MonoBehaviour
{
    public CardSystem CdSy;

    [Header("World Space References")]
    [SerializeField] Transform playerHandPos;
    [SerializeField] Transform playerTeethPos;
    [SerializeField] Transform deckPosition;
    [SerializeField] Transform discardPosition;

    public WorldCardBehaviours[] cardModels;
    public ToothObject[] teethModels;

    public static Action OnEndTurn;
    public static Action<bool> OnPlayerWin;

    [Header("Effects")]
    public GameObject playedCard;
    public GameObject totalImunityCard;
    public GameObject blockSugarCard;
    public GameObject extraTimeCard;
    public bool skipTurn = false;

    [Header("Component References")]
    [SerializeField] private GameObject discardButton;
    [SerializeField] private GameObject drawButton;
    [SerializeField] private GameObject returnButton;

    void Awake()
    { 
        CdSy = FindFirstObjectByType<CardSystem>();
    }

    void Start()
    {
        StartCoroutine("DrawAllCards");
    }

    private void OnEnable()
    {
        TurnManager.PlayerStartTurn += StartPlayersTurn;
    }

    private void OnDisable()
    {
        TurnManager.PlayerStartTurn -= StartPlayersTurn;
    }

    void StartPlayersTurn()
    {
        discardButton.SetActive(true);
        discardButton.GetComponent<Button>().interactable = true;
        drawButton.SetActive(true);

        /*while (CdSy.hand.Count < 4)
            CdSy.DrawCard();*/

        if (totalImunityCard != null) totalImunityCard = null; 

        for (int i = 0; i < CdSy.hand.Count; i++)
        {
            if (CdSy.hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Treatment)
            {
                if (CdSy.hand[i].GetComponent<CardVisual>().cardData.cardName == "Bloqueo de Azúcar")
                    blockSugarCard = CdSy.hand[i];
                if (CdSy.hand[i].GetComponent<CardVisual>().cardData.cardName == "Tiempo Extra")
                    extraTimeCard = CdSy.hand[i];
            }
        }
    }

    IEnumerator DrawAllCards()
    {
        for (int i = 0; i < cardModels.Length; i++)
        {
            yield return new WaitForSeconds(0.25f);
            cardModels[i].fDrawn(deckPosition);
        }
    }

    public void PlaceTooth(GameObject card)
    {
        int index = card.transform.GetSiblingIndex();
        CdSy.teethInPlay.Add(card);
        CdSy.hand.Remove(card);

        card.transform.SetParent(playerTeethPos);

        // Activate corresponding tooth
        switch (card.GetComponent<CardVisual>().cardData.cardColor)
        {
            case ToothColor.Blue:
                cardModels[index].fGoToPosition(teethModels[CdSy.teethInPlay.Count - 1].target, 0.25f);
                teethModels[CdSy.teethInPlay.Count - 1].fAddTooth(0);
                break;
            case ToothColor.Red:
                cardModels[index].fGoToPosition(teethModels[CdSy.teethInPlay.Count - 1].target, 0.25f);
                teethModels[CdSy.teethInPlay.Count - 1].fAddTooth(1);
                break;
            case ToothColor.Green:
                cardModels[index].fGoToPosition(teethModels[CdSy.teethInPlay.Count - 1].target, 0.25f);
                teethModels[CdSy.teethInPlay.Count - 1].fAddTooth(2);
                break;
            case ToothColor.Yellow:
                cardModels[index].fGoToPosition(teethModels[CdSy.teethInPlay.Count - 1].target, 0.25f);
                teethModels[CdSy.teethInPlay.Count - 1].fAddTooth(3);
                break;
            case ToothColor.Rainbow:
                cardModels[index].fGoToPosition(teethModels[CdSy.teethInPlay.Count - 1].target, 0.25f);
                teethModels[CdSy.teethInPlay.Count - 1].fAddTooth(4);
                break;
        }

        Invoke("DrawCard", 0.25f);
    }

    void DrawCard()
    {
        CdSy.DrawCard();

        for (int i = 0; i < cardModels.Length; i++)
            if (!cardModels[i].atDefaultTransform) cardModels[i].fDrawn(deckPosition);

        Invoke("EndTurn", 0.25f);
    }

    public void SwapCards(EnemySystem otherEn)
    {
        GameObject cardToSwap = CdSy.hand[UnityEngine.Random.Range(0, 4)];
        GameObject newCard = otherEn.hand[UnityEngine.Random.Range(0, 4)];
        CdSy.hand.Remove(cardToSwap);
        otherEn.hand.Remove(newCard);
        CdSy.hand.Add(newCard);
        otherEn.hand.Add(cardToSwap);
        newCard.transform.SetParent(otherEn.handPositionUI);
        cardToSwap.transform.SetParent(CdSy.handPosition);
    }

    public GameObject FindToothByDamage(bool inverse, CardVisual otherTooth)
    {
        GameObject potentialTooth = null;
        int damage = 0;
        if (inverse)
        {
            damage = -100;
            for (int i = 0; i < CdSy.teethInPlay.Count; i++)
            {
                if (CdSy.teethInPlay[i].GetComponent<CardFunctionByHolder>().toothProtection > damage)
                {
                    damage = CdSy.teethInPlay[i].GetComponent<CardFunctionByHolder>().toothProtection;
                    potentialTooth = CdSy.teethInPlay[i];
                }
            }
        }
        else
        {
            damage = 100;
            for (int i = 0; i < CdSy.teethInPlay.Count; i++)
            {
                if (CdSy.teethInPlay[i].GetComponent<CardFunctionByHolder>().toothProtection < damage)
                {
                    damage = CdSy.teethInPlay[i].GetComponent<CardFunctionByHolder>().toothProtection;
                    potentialTooth = CdSy.teethInPlay[i];
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
            for (int i = 0; i < CdSy.teethInPlay.Count; i++)
            {
                if (potentialTooth == null)
                    potentialTooth = CdSy.teethInPlay[i];
                else
                {
                    if (CdSy.teethInPlay[i].GetComponent<CardVisual>().cardData.cardColor == potentialTooth.GetComponent<CardVisual>().cardData.cardColor)
                        potentialTooth = CdSy.teethInPlay[i];
                }
            }
        }
        else
        {
            if (otherTooth == null)
            {
                for (int i = 0; i < CdSy.teethInPlay.Count; i++)
                {
                    if (potentialTooth == null)
                        potentialTooth = CdSy.teethInPlay[i];
                    else
                    {
                        if (CdSy.teethInPlay[i].GetComponent<CardVisual>().cardData.cardColor != potentialTooth.GetComponent<CardVisual>().cardData.cardColor)
                            potentialTooth = CdSy.teethInPlay[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < CdSy.teethInPlay.Count; i++)
                {
                    if (CdSy.teethInPlay[i].GetComponent<CardVisual>().cardData.cardColor != otherTooth.cardData.cardColor)
                        potentialTooth = CdSy.teethInPlay[i];
                }
            }
        }
        return potentialTooth;
    }

    public void EndTurn()
    {
        if (CdSy.teethInPlay.Count == 4)
        {
            var won = CheckWinCondition();

            if (won)
            {
                for (int i = 0; i < CdSy.hand.Count; i++)
                    CdSy.hand[i].GetComponentInChildren<Button>().interactable = false;
                discardButton.SetActive(false);
                drawButton.SetActive(false);
                Debug.Log("YOU WON!");
                OnPlayerWin?.Invoke(true);
                return;
            }
        }

        if (extraTimeCard == null)
        {
            Debug.Log("Player's turn has ended");
            if (CdSy.hand.Count < 4) CdSy.DrawCard();
            discardButton.SetActive(false);
            drawButton.SetActive(false);
            OnEndTurn?.Invoke();
        }
        else
        {
            Debug.Log("You played Extra Time, and can go again");
            FindFirstObjectByType<OnScreenAnnouncement>().SlideText("Tiempo Extra \n ¡Juega otra carta!", new Color(0.7294118f, 0.3333333f, 0.8679245f));
            CdSy.DiscardCard(extraTimeCard);
            CdSy.Invoke("DrawCard", 0.15f);
            discardButton.SetActive(true);
            drawButton.SetActive(true);
            extraTimeCard = null;
        }
    }

    bool CheckWinCondition()
    {
        CardVisual[] teethColors = new CardVisual[4];
        int rainbowTeeth = 0;

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if ((int)CdSy.teethInPlay[i].GetComponent<CardVisual>().cardData.cardColor == j)
                {
                    if (CdSy.teethInPlay[i].GetComponent<CardFunctionByHolder>().toothProtection >= 0)
                        teethColors[j] = CdSy.teethInPlay[i].GetComponent<CardVisual>();
                }
            }
            if ((int)CdSy.teethInPlay[i].GetComponent<CardVisual>().cardData.cardColor == 4 && CdSy.teethInPlay[i].GetComponent<CardFunctionByHolder>().toothProtection >= 0)
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
}
