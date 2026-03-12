using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class EnemyHandEventManager : MonoBehaviour
{
    [SerializeField] GameObject _Panel;
    [SerializeField] Button _PlayerHandButton;
    [SerializeField] Button[] _EnemyButtons;
    [SerializeField] GameObject _HandPanel;
    [SerializeField] GameObject[] handPanels;
    [SerializeField] GameObject _TeethPanel;
    [SerializeField] GameObject[] teethPanels;

    [SerializeField] private List<int> storedIDs = new List<int>();
    int idsToHold = 0;
    public List<GameObject> selectedCards = new List<GameObject>();
    int cardsToHold = 0;
    public bool takeCards = true;
    private string _Purpose;

    [SerializeField] EnemySystem[] _EnemySystems;

    [ContextMenu("Try")]
    public void LetsTry()
    {
        OpenPanelAs("stealOrSwapCards");
    }

    public void OpenPanelAs(string purpose)
    {
        idsToHold = 0;
        storedIDs.Clear();
        cardsToHold = 0;
        selectedCards.Clear();
        _HandPanel.SetActive(false);
        _TeethPanel.SetActive(false);
        _PlayerHandButton.gameObject.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            _EnemyButtons[i].gameObject.SetActive(true);
            _EnemyButtons[i].interactable = true;
            handPanels[i].SetActive(false);
        }
        for (int i = 0; i < 4; i++)
            teethPanels[i].SetActive(false);
        _Panel.SetActive(true);

        _Purpose = purpose;

        switch (purpose)
        {
            case "selectEnemy":
                fSelectEnemy();
                break;
            case "stealOrSwapCards":
                fStealSwap();
                break;
            case "swapTeeth":
                fSwapTeeth();
                break;
        }
    }

    void fSelectEnemy()
    {
        idsToHold = 1;
    }

    void fStealSwap()
    {
        idsToHold = 2;
        _HandPanel.SetActive(true);
    }

    void fSwapTeeth()
    {
        idsToHold = 2;
    }

    IEnumerator fExchangeCards()
    {
        _PlayerHandButton.gameObject.SetActive(false);
        for (int i = 0; i < 3; i++)
            _EnemyButtons[i].gameObject.SetActive(false);
        
        if (storedIDs[0] == 3 || storedIDs[1] == 3)
        {
            int other = 0;
            for (int i = 0; i < 2; i++)
                if (storedIDs[i] != 3) other = storedIDs[i];
            cardsToHold = 1;
            handPanels[other].SetActive(true);
        }
        else
        {
            int firstCard = Random.Range(0, 4);
            Transform newTrans1 = _EnemySystems[storedIDs[0]].cardModels[firstCard].gameObject.transform;
            GameObject card1 = _EnemySystems[storedIDs[0]].hand[firstCard];
            _EnemySystems[storedIDs[0]].hand.Remove(card1);
            int secondCard = Random.Range(0, 4);
            Transform newTrans2 = _EnemySystems[storedIDs[1]].cardModels[secondCard].gameObject.transform;
            GameObject card2 = _EnemySystems[storedIDs[1]].hand[secondCard];
            _EnemySystems[storedIDs[1]].hand.Remove(card2);
            _EnemySystems[storedIDs[1]].hand.Add(card1);
            _EnemySystems[storedIDs[0]].hand.Add(card2);
            card1.transform.SetParent(_EnemySystems[storedIDs[1]].handPositionUI);
            card2.transform.SetParent(_EnemySystems[storedIDs[0]].handPositionUI);

            _Panel.SetActive(false);

            _EnemySystems[storedIDs[0]].cardModels[firstCard].fReturnFromPosition(newTrans2, 3);
            yield return new WaitForSeconds(0.2f);
            _EnemySystems[storedIDs[1]].cardModels[secondCard].fReturnFromPosition(newTrans1, 3);
        }
        StopCoroutine(fExchangeCards());
    }

    void fExchangeTeeth()
    {
        _TeethPanel.SetActive(true);
        for (int i = 0; i < 2; i++)
            teethPanels[storedIDs[i]].SetActive(true);
        cardsToHold = 2;
    }

    public void stealTheCard(GameObject card)
    {
        if (selectedCards.Count == cardsToHold)
        {
            if (cardsToHold == 1) // Estamos cambiando/robando cartas
            {
                int enSyst = 0;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < _EnemySystems[i].hand.Count; j++)
                    {
                        if (_EnemySystems[i].hand[j] == card) enSyst = i;
                    }
                }
                int randomNum = Random.Range(0, 4);
                Transform newTrans = _EnemySystems[enSyst].cardModels[randomNum].gameObject.transform;
                _EnemySystems[enSyst].cardModels[randomNum].fToggleCard(false);
                _EnemySystems[enSyst].hand.Remove(card);
                FindFirstObjectByType<CardSystem>().hand.Add(card);
                selectedCards[0].transform.SetParent(FindFirstObjectByType<CardSystem>().deckPosition);
                selectedCards[0].GetComponent<CardFunctionByHolder>().currentHolder = CardFunctionByHolder.Holders.Player;
                FindFirstObjectByType<CardSystem>().UpdateHandVisual();
                int cardID = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (!FindFirstObjectByType<PlayerWorldSpaceManager>().cards[i].atDefaultTransform)
                        cardID = i;
                }
                _EnemySystems[enSyst].GetComponent<EnemyActions>().DrawCard();
                _Panel.SetActive(false);
                FindFirstObjectByType<PlayerWorldSpaceManager>().ReturnFromPosition(cardID, newTrans, 3);
            }
            if (cardsToHold == 2) // Estamos cambiando dientes
            {
                GameObject playersOldTooth = null;
                int enemysToothID = 0;
                int enemySystID = 0;
                for (int i = 0; i < 2; i++)
                {
                    if (selectedCards[i].GetComponent<CardFunctionByHolder>().currentHolder == CardFunctionByHolder.Holders.Player)
                    {
                        playersOldTooth = selectedCards[i];
                        if (i == 0) enemysToothID = 1; else enemysToothID = 0;
                    }

                    if (storedIDs[i] != 3) enemySystID = storedIDs[i];
                }

                if (playersOldTooth != null)
                {
                    FindFirstObjectByType<CardSystem>().teethInPlay.Remove(playersOldTooth);
                    playersOldTooth.GetComponent<CardFunctionByHolder>().currentHolder = CardFunctionByHolder.Holders.Enemy;
                    playersOldTooth.transform.SetParent(teethPanels[enemySystID].transform);
                    _EnemySystems[enemySystID].teethInPlay.Add(playersOldTooth);
                    _EnemySystems[enemySystID].teethInPlay.Remove(selectedCards[enemysToothID]);
                    selectedCards[enemysToothID].GetComponent<CardFunctionByHolder>().currentHolder = CardFunctionByHolder.Holders.Player;
                    selectedCards[enemysToothID].transform.SetParent(teethPanels[3].transform);
                    FindFirstObjectByType<CardSystem>().teethInPlay.Add(selectedCards[enemysToothID]);
                    _Panel.SetActive(false);
                }
                else
                {
                    Debug.Log("Whoopsie doodle");
                }

            }
        }
    }

    public void fEnemyButtonFunction(int enemyID)
    {
        switch (_Purpose)
        {
            case "selectEnemy":
                storedIDs.Add(enemyID);
                break;
            case "stealOrSwapCards":
                storedIDs.Add(enemyID);
                _EnemyButtons[enemyID].interactable = false;
                if (storedIDs.Count == idsToHold) 
                    if (takeCards) StartCoroutine(fExchangeCards());
                    else fExchangeTeeth();
                break;
            case "swapTeeth":
                fSwapTeeth();
                break;
        }
    }

    public void fPlayerButton()
    {
        storedIDs.Add(3);
        if (storedIDs.Count == idsToHold) 
            if (takeCards) StartCoroutine(fExchangeCards());
            else fExchangeTeeth();
    }

    void fEchangeAllCards()
    {

    }
}
