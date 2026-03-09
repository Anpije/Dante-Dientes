using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class EnemySystem : MonoBehaviour
{
    [Header("Card Database")]
    public List<CardData> allCards;

    [Header("References")]
    public WorldCardBehaviours[] cardModels;
    public Transform handPositionUI;
    public Transform deckPosition;
    public Transform handPosition;
    public Transform discardPosition;
    public float cardSpacing = 220f;
    CardSystem cardSystem;

    [Header("Tooth Settings")]
    public Transform teethAreaPosition;    
    public int teethPerPlayer = 4;
    public int initialCardCount = 3;
    public float toothSpacing = 250f;  
    public GameObject[] teethModels;

    [Header("State")]
    public List<GameObject> hand = new List<GameObject>();
    public List<GameObject> teethInPlay = new List<GameObject>();


    void Awake()
    {
        cardSystem = FindFirstObjectByType<CardSystem>();
    }

    private void Start()
    {
        Invoke("DrawInitialHand", 0.5f);
        StartCoroutine(DrawAllCards());
    }

    IEnumerator DrawAllCards()
    {
        for (int i = 0; i < cardModels.Length; i++)
        {
            yield return new WaitForSeconds(0.25f);
            cardModels[i].fDrawn(deckPosition);
        }
    }

    private void DrawInitialHand()
    {
        bool toothFound = false;
        for (int i = 0; i < cardSystem.deck.Count; i++)
        {
            if (!toothFound)
            {
                if (cardSystem.deck.ElementAt(i).GetComponent<CardVisual>().cardData.cardType == CardType.HealthyTooth)
                {
                    GameObject card = cardSystem.deck.ElementAt(i);
                    card.SetActive(true);
                    hand.Add(card);
                    List<GameObject> newDeck = new List<GameObject>();
                    foreach(GameObject oldCard in cardSystem.deck)
                        newDeck.Add(oldCard);
                    newDeck.Remove(newDeck[i]);
                    cardSystem.deck.Clear();
                    foreach (GameObject newCard in newDeck)
                        cardSystem.deck.Push(newCard);
                    toothFound = true;
                }
            }
        }

        for (int i = 0; i < initialCardCount;  i++)
        {
            GameObject card = cardSystem.deck.Pop();
            if (card.GetComponent<CardVisual>().cardData.cardType != CardType.HealthyTooth)
            {
                card.SetActive(true);
                hand.Add(card);
            }
            else 
                i--;
        }

        // 5. Actualiza la visual de la mano
        UpdateHandVisual();
    
        Debug.Log($"Mano inicial: {hand.Count} cartas (1 diente + 3 aleatorias)");

        // PlaceInitialTeeth();
    }

    public void PlaceTooth(GameObject card)
    {
        teethInPlay.Add(card);
        hand.Remove(card);
        card.SetActive(false);

        // Activate corresponding tooth
        string toothName = "";
        for (int i = 0; i < 3; i++)
            toothName += card.GetComponent<CardVisual>().cardData.cardName[i];

        switch (toothName)
        {
            case "Can":
                cardModels[0].fGoToPosition(teethModels[0].transform, 0.25f);
                teethModels[0].SetActive(true);
                break;
            case "Inc":
                if (!teethModels[1].activeSelf)
                {
                    cardModels[1].fGoToPosition(teethModels[1].transform, 0.25f);
                    teethModels[1].SetActive(true);
                }
                else
                {
                    cardModels[2].fGoToPosition(teethModels[2].transform, 0.25f);
                    teethModels[2].SetActive(true);
                }
                break;
            case "Mol":
                cardModels[3].fGoToPosition(teethModels[3].transform, 0.25f);
                teethModels[3].SetActive(true);
                break;
        }

        Invoke("DrawCard", 0.5f);
    }

    public GameObject DrawCard()
    {
        if (cardSystem.deck.Count == 0)
        {
            Debug.Log("No quedan cartas en el mazo de " + gameObject.name);
            return null;
        }

        GameObject card = cardSystem.deck.Pop();
        card.SetActive(true);
        hand.Add(card);

        for (int i = 0; i < cardModels.Length; i++)
            if (!cardModels[i].atDefaultTransform) cardModels[i].fDrawn(deckPosition);

        UpdateHandVisual();
        Debug.Log($"Robada: {card.GetComponent<CardVisual>().cardData.cardName}");
        return card;
    }

    public void DiscardCard(GameObject card)
    {
        if (hand.Contains(card))
        {
            hand.Remove(card);
            cardSystem.allCardObjects.Remove(card);
            Destroy(card);
            cardModels[Random.Range(0, cardModels.Length)].fGoToPosition(discardPosition, 1);
            UpdateHandVisual();
            Debug.Log($"Descartada: {card.GetComponent<CardVisual>().cardData.cardName}");
        }
    }

    private void UpdateHandVisual()
    {
        if (hand.Count == 0) return;

        float totalWidth = (hand.Count - 1) * cardSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < hand.Count; i++)
        {
            GameObject card = hand[i];
            Vector3 newPos = handPosition.position + new Vector3(startX + i * cardSpacing, 0, 0);
            card.transform.SetParent(handPositionUI.transform);
            card.transform.position = newPos;
        }
    }

    public List<CardVisual> GetAllCardsInHand()
    {
        List<CardVisual> result = new List<CardVisual>();
        foreach (GameObject card in hand)
        {
            result.Add(card.GetComponent<CardVisual>());
        }
        return result;
    }

    public List<CardVisual> GetTeethInPlay()
    {
        List<CardVisual> result = new List<CardVisual>();
        foreach (GameObject tooth in teethInPlay)
        {
            result.Add(tooth.GetComponent<CardVisual>());
        }
        return result;
    }

    public List<CardVisual> GetCardsInHandOfType(CardType type)
    {
        List<CardVisual> result = new List<CardVisual>();
        foreach (GameObject card in hand)
        {
            CardVisual visual = card.GetComponent<CardVisual>();
            if (visual.cardData.cardType == type)
                result.Add(visual);
        }
        return result;
    }

    public void DebugActiveCards()
    {
        int activeCount = 0;
        Debug.Log("=== CARTAS ACTIVAS ===");

        foreach (GameObject card in cardSystem.allCardObjects)
        {
            if (card.activeSelf)
            {
                activeCount++;
                CardVisual visual = card.GetComponent<CardVisual>();
                Debug.Log($"Activa: {visual.cardData.cardName} - Pos: {card.transform.position}");
            }
        }

        Debug.Log($"Total cartas activas: {activeCount} (Deberían ser: {teethInPlay.Count + hand.Count})");
    }
}
