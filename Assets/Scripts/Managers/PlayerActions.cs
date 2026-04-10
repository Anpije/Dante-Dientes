using System.Collections;
using UnityEngine;
using System;

public class PlayerActions : MonoBehaviour
{
    CardSystem CdSy;

    [Header("World Space References")]
    [SerializeField] Transform playerHandPos;
    [SerializeField] Transform playerTeethPos;
    [SerializeField] Transform deckPosition;
    [SerializeField] Transform discardPosition;

    public WorldCardBehaviours[] cardModels;
    public ToothObject[] teethModels;

    public static Action OnEndTurn;

    [Header("Effects")]
    public GameObject playedCard;
    public GameObject totalImunityCard;
    public GameObject blockSugarCard;
    public GameObject extraTimeCard;
    public bool skipTurn = false;

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
        if (totalImunityCard != null) totalImunityCard = null; 

        for (int i = 0; i < CdSy.hand.Count; i++)
        {
            if (CdSy.hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Treatment)
            {
                if (CdSy.hand[i].GetComponent<CardVisual>().cardData.description == "Bloqueo de Azúcar")
                    blockSugarCard = CdSy.hand[i];
                if (CdSy.hand[i].GetComponent<CardVisual>().cardData.description == "Tiempo Extra")
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

    public void EndTurn()
    {
        if (extraTimeCard == null)
        {
            Debug.Log("Player's turn has ended");
            OnEndTurn?.Invoke();
        }
        else
        {
            Debug.Log("You played Extra Time, and can go again");
            CdSy.DiscardCard(extraTimeCard);
            extraTimeCard = null;
        }
    }
}
