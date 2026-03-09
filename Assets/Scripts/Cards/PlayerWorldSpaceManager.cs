using System.Collections;
using UnityEngine;

public class PlayerWorldSpaceManager : MonoBehaviour
{
    [SerializeField] private WorldCardBehaviours[] cards;
    [SerializeField] private GameObject[] teeth;
    [SerializeField] Transform deck;
    [SerializeField] Transform discardPile;

    void Start()
    {
        StartCoroutine(DrawAllCards());
    }

    public void DrawCard(int cardID)
    {
        cards[cardID].fDrawn(deck);
    }

    IEnumerator DrawAllCards()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            yield return new WaitForSeconds(0.25f);
            cards[i].fDrawn(deck);
        }
    }

    public void DiscardCard(int cardID)
    {
        cards[cardID].fGoToPosition(discardPile, 1);
    }

    public void CardToTooth(int cardID, int toothID)
    {
        cards[cardID].fGoToPosition(teeth[toothID].transform, 0.25f);
    }

    public void ToggleTooth(int toothID, bool visible)
    {
        teeth[toothID].SetActive(visible);
    }
}
