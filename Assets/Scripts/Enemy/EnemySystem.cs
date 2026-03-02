using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemySystem : MonoBehaviour
{
    [Header("Card Database")]
    public List<CardData> allCards;

    [Header("References")]
    public GameObject cardPrefab;
    public Transform deckPosition;
    public Transform handPosition;
    public float cardSpacing = 220f;
    CardSystem cardSystem;

    [Header("Tooth Settings")]
    // La posición para los dientes en escena
    public Transform teethAreaPosition;    
       // El número de dientes por jugador 
    public int teethPerPlayer = 4;
    // Número de cartas del mano inicial
    public int initialCardCount = 3;
    // Espaciado para los dientes en escena
    public float toothSpacing = 250f;  

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
            card.SetActive(true);
            hand.Add(card);
        }

        // 5. Actualiza la visual de la mano
        UpdateHandVisual();
    
        Debug.Log($"Mano inicial: {hand.Count} cartas (1 diente + 3 aleatorias)");

        PlaceInitialTeeth();
    }
    private void PlaceInitialTeeth()
    {
        List<GameObject> selectedTeeth = new List<GameObject>();
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.HealthyTooth)
            {
                selectedTeeth.Add(hand[i]);
                hand.Remove(hand[i]);
            }
        }

        teethInPlay.Clear();

        // Posicionamos los dientes seleccionados
        float totalWidth = (selectedTeeth.Count - 1) * toothSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < selectedTeeth.Count; i++)
        {
            GameObject tooth = selectedTeeth[i];

            tooth.SetActive(true);
            teethInPlay.Add(tooth);

            // Vector3 para la UI
            Vector3 newPos = teethAreaPosition.position + new Vector3(startX + i * toothSpacing, 0, 0);
            tooth.transform.position = newPos;
            tooth.transform.rotation = Quaternion.identity;

            Debug.Log($"Diente en juego: {tooth.GetComponent<CardVisual>().cardData.cardName} en {newPos} para " + gameObject.name);
        }

        Debug.Log($"{teethInPlay.Count} dientes colocados para " + gameObject.name);
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

        UpdateHandVisual();
        Debug.Log($"Robada: {card.GetComponent<CardVisual>().cardData.cardName}");
        return card;
    }

    public void DiscardCard(GameObject card)
    {
        if (hand.Contains(card))
        {
            hand.Remove(card);
            card.SetActive(false);
            card.transform.position = deckPosition.position;

            cardSystem.deck.Push(card);

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
            card.transform.SetParent(deckPosition.transform);
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
