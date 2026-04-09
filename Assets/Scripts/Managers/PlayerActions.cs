using System.Collections;
using UnityEngine;
using System;

public class PlayerActions : MonoBehaviour
{
    CardSystem CdSy;

    [SerializeField] Transform playerHandPos;
    [SerializeField] Transform playerTeethPos;
    [SerializeField] Transform deckPosition;
    [SerializeField] Transform discardPosition;

    public WorldCardBehaviours[] cardModels;
    public ToothObject[] teethModels;

    public static Action OnEndTurn;

    public GameObject playedCard;

    void Awake()
    { 
        CdSy = FindFirstObjectByType<CardSystem>();
    }

    void Start()
    {
        StartCoroutine("DrawAllCards");
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
        Debug.Log("Player's turn has ended");
        OnEndTurn?.Invoke();
    }
}
