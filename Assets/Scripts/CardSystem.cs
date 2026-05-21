using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardSystem : MonoBehaviour
{
    public static CardSystem Instance { get; private set; }

    [Header("Card Database")]
    public List<CardData> allCards;
    public int duplicateCards = 5;

    [Header("References")]
    public PlayerActions PlAc;
    public GameObject cardPrefab;
    public Transform deckPosition;
    public Transform handPosition;
    public Transform discardPosition;
    public float cardSpacing = 220f;

    [Header("Tooth Settings")]
    // La posición para los dientes en escena
    public Transform teethAreaPosition;
    // El número de dientes por jugador 
    public int teethPerPlayer = 4;
    // Espaciado para los dientes en escena
    public float toothSpacing = 250f;

    [Header("State")]
    public Stack<GameObject> deck = new Stack<GameObject>();
    public List<GameObject> hand = new List<GameObject>();
    public List<GameObject> teethInPlay = new List<GameObject>();
    public List<GameObject> allCardObjects = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Crea todas las cartas
        CreateAllCards();
        // Coloca los dientes en juego
        // PlaceInitialTeeth();
        // Construye el mazo con las cartas restantes
        BuildDeck();
        // Roba la mano inicial (1 diente + 3 aleatorias)
        DrawInitialHand();
    }

    private void CreateAllCards()
    {
        allCardObjects.Clear();

        for (int i = 0; i < duplicateCards; i++)
        {
            foreach (CardData cardData in allCards)
            {
                GameObject cardObj = Instantiate(cardPrefab, deckPosition);
                CardVisual visual = cardObj.GetComponent<CardVisual>();
                visual.Initialize(cardData);
                // Todas empiezan desactivadas
                cardObj.SetActive(false);
                allCardObjects.Add(cardObj);
            }
        }

        Debug.Log($"Creadas {allCardObjects.Count} cartas");
    }

    private void PlaceInitialTeeth()
    {
        // Separar las cartas de dientes del resto
        // ya que tenemos las cartas por nombre podemos separar los dientes de los demás cartas
        List<GameObject> toothCards = new List<GameObject>();

        foreach (GameObject card in allCardObjects)
        {
            CardVisual visual = card.GetComponent<CardVisual>();
            if (visual.cardData.cardType == CardType.HealthyTooth)
            {
                toothCards.Add(card);
            }
        }

        Debug.Log($"Encontradas {toothCards.Count} cartas de dientes en total");

        // Seleccionamos los dientes de manera aleatoria
        System.Random rng = new System.Random();
        var selectedTeeth = toothCards.OrderBy(x => rng.Next()).Take(teethPerPlayer).ToList();

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
            tooth.transform.SetParent(teethAreaPosition);
            Vector3 newPos = teethAreaPosition.position + new Vector3(startX + i * toothSpacing, 0, 0);
            tooth.transform.position = newPos;
            tooth.transform.rotation = Quaternion.identity;
            FindFirstObjectByType<PlayerActions>().teethModels[i].fAddTooth((int)tooth.GetComponent<CardVisual>().cardData.cardColor);

            Debug.Log($"Diente en juego: {tooth.GetComponent<CardVisual>().cardData.cardName} en {newPos}");
        }

        Debug.Log($"{teethInPlay.Count} dientes colocados");
    }

    private void BuildDeck()
    {
        // El mazo se construye con todas las cartas con escepción los dientes que ya están en escena
        List<GameObject> deckCards = new List<GameObject>();

        foreach (GameObject card in allCardObjects)
        {
            // si la carta no está en teethInPlay, va al mazo
            if (!teethInPlay.Contains(card))
            {
                deckCards.Add(card);
                // aseguramos de que este desactivada
                card.SetActive(false);
                card.transform.position = deckPosition.position;
            }
        }

        // barajamos el mazo con un systema aleatoria
        System.Random rng = new System.Random();
        var shuffledDeck = deckCards.OrderBy(x => rng.Next()).ToList();

        deck.Clear();
        foreach (GameObject card in shuffledDeck)
        {
            deck.Push(card);
        }

        Debug.Log($"Mazo construido con {deck.Count} cartas");
    }

    private void DrawInitialHand()
    {
        // 4 condicones es que podemos hacer que el jugaador empiece con un número de cantidad 
        // cartas de dientes esto nos puede servir para agregar el nivel de dificultad 

        // 1. Busca un diente que no esté en escena para la mano
        List<GameObject> availableTeeth = new List<GameObject>();

        foreach (GameObject card in deck)
        {
            CardVisual visual = card.GetComponent<CardVisual>();
            if (visual.cardData.cardType == CardType.HealthyTooth)
            {
                availableTeeth.Add(card);
            }
        }

        if (availableTeeth.Count == 0)
        {
            Debug.LogError("No hay dientes disponibles en el mazo para la mano inicial");
            return;
        }

        // 2. Selecciona un diente aleatorio para la mano
        System.Random rng = new System.Random();
        GameObject toothForHand = availableTeeth[rng.Next(availableTeeth.Count)];

        // 3. Roba ese diente lo quita del mazo y lo pone en la mano
        List<GameObject> tempList = deck.ToList();
        tempList.Remove(toothForHand);
        deck = new Stack<GameObject>(tempList);

        toothForHand.SetActive(true);
        hand.Add(toothForHand);

        Debug.Log($"Diente para mano: {toothForHand.GetComponent<CardVisual>().cardData.cardName}");

        // 4. Roba 3 cartas aleatorias más del mazo para copletar la mano
        // esto si es que vamos a empezar con 4 cartas en el inicio de la partida aún no he tenido claro está regla de cuantas cartas empezarems el juego
        for (int i = 0; i < 3; i++)
        {
            if (deck.Count == 0)
            {
                Debug.Log("No hay suficientes cartas en el mazo");
                break;
            }

            GameObject card = deck.Pop();
            card.SetActive(true);
            hand.Add(card);

            Debug.Log($"Carta aleatoria: {card.GetComponent<CardVisual>().cardData.cardName}");
        }

        // 5. Actualiza la visual de la mano
        UpdateHandVisual();

        Debug.Log($"Mano inicial: {hand.Count} cartas (1 diente + 3 aleatorias)");
    }

    [ContextMenu("Draw New Card")]
    void DrawNewCard()
    {
        DiscardCard(hand[0]);
        DrawCard();
    }

    public GameObject DrawCard()
    {
        if (deck.Count == 0)
        {
            Debug.Log("No quedan cartas en el mazo");
            return null;
        }

        GameObject card = deck.Pop();
        card.SetActive(true);
        hand.Add(card);

        int cardIndex = 0;

        for (int i = 0; i < PlAc.cardModels.Length; i++)
            if (!PlAc.cardModels[i].atDefaultTransform) cardIndex = i;

        PlAc.cardModels[cardIndex].fDrawn(deckPosition);
        card.transform.SetParent(handPosition);
        card.transform.SetSiblingIndex(cardIndex);


        //UpdateHandVisual();
        Debug.Log($"Robada: {card.GetComponent<CardVisual>().cardData.cardName}");
        return card;
    }

    public void DiscardCard(GameObject card)
    {
        if (hand.Contains(card))
        {
            int index = card.transform.GetSiblingIndex();

            hand.Remove(card);
            allCardObjects.Remove(card);
            Destroy(card);

            PlAc.cardModels[index].fGoToPosition(discardPosition, 0.25f);

            UpdateHandVisual();
            Debug.Log($"Descartada: {card.GetComponent<CardVisual>().cardData.cardName}");
        }
    }

    public void UpdateHandVisual()
    {
        if (hand.Count == 0) return;

        float totalWidth = (hand.Count - 1) * cardSpacing;
        float startX = -totalWidth / 2f;

        for (int i = 0; i < hand.Count; i++)
        {
            hand[i].transform.SetParent(handPosition);
            /*GameObject card = hand[i];
            Vector3 newPos = handPosition.position + new Vector3(startX + i * cardSpacing, 0, 0);
            card.transform.position = newPos;*/

        }
    }

    public int GetDeckCount()
    {
        return deck.Count;
    }

    public int GetHandCount()
    {
        return hand.Count;
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

    public void PrintDeckInfo()
    {
        Debug.Log($"=== ESTADO ACTUAL ===");
        Debug.Log($"Dientes en juego: {teethInPlay.Count}");
        Debug.Log($"Mazo: {deck.Count} cartas");
        Debug.Log($"Mano: {hand.Count} cartas");

        Debug.Log("--- Dientes en juego ---");
        for (int i = 0; i < teethInPlay.Count; i++)
        {
            CardVisual visual = teethInPlay[i].GetComponent<CardVisual>();
            Debug.Log($"   {i + 1}. {visual.cardData.cardName}");
        }

        Debug.Log("--- Cartas en mano ---");
        for (int i = 0; i < hand.Count; i++)
        {
            CardVisual visual = hand[i].GetComponent<CardVisual>();
            Debug.Log($"   {i + 1}. {visual.cardData.cardName} ({visual.cardData.cardType})");
        }
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

        foreach (GameObject card in allCardObjects)
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

    public void DiscardMostValuable()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.HealthyTooth)
            {
                DiscardCard(hand[i]);
                return;
            }
        }

        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Protective)
            {
                DiscardCard(hand[i]);
                return;
            }
        }

        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Harmful)
            {
                DiscardCard(hand[i]);
                return;
            }
        }

        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Treatment)
            {
                DiscardCard(hand[i]);
                return;
            }
        }
    }

    public GameObject FindMostValuable()
    {
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.HealthyTooth)
            {
                return hand[i];
            }
        }

        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Protective)
            {
                return hand[i];
            }
        }

        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Harmful)
            {
                return hand[i];
            }
        }

        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Treatment)
            {
                return hand[i];
            }
        }

        return null;
    }
}