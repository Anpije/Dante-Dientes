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

    [SerializeField] public List<int> storedIDs = new List<int>();
    int idsToHold = 0;
    public List<GameObject> selectedCards = new List<GameObject>();
    int cardsToHold = 0;
    public int effectToApply;
    public string _Purpose;

    [SerializeField] EnemySystem[] _EnemySystems;

    [SerializeField] string debugMode = "";
    [ContextMenu("Try")]
    public void LetsTry()
    {
        OpenPanelAs(debugMode);
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
            if (_EnemySystems[i].gameObject.activeSelf)
            {
                _EnemyButtons[i].gameObject.SetActive(true);
                _EnemyButtons[i].interactable = true;
            }
            else
                _EnemyButtons[i].gameObject.SetActive(false);
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
            case "affectTooth":
                faffectTooth();
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
        _TeethPanel.SetActive(true);
    }

    void faffectTooth()
    {
        idsToHold = 1;
        _TeethPanel.SetActive(true);
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
            _Purpose = "";
        }
        StopCoroutine(fExchangeCards());
    }

    void fExchangeTeeth()
    {
        for (int i = 0; i < storedIDs.Count; i++)
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
                    int playersToothIndex = playersOldTooth.transform.GetSiblingIndex();
                    int enemysToothIndex = selectedCards[enemysToothID].transform.GetSiblingIndex();
                    FindFirstObjectByType<CardSystem>().teethInPlay.Remove(playersOldTooth);
                    playersOldTooth.GetComponent<CardFunctionByHolder>().currentHolder = CardFunctionByHolder.Holders.Enemy;
                    playersOldTooth.transform.SetParent(teethPanels[enemySystID].transform);
                    _EnemySystems[enemySystID].teethInPlay.Add(playersOldTooth);
                    _EnemySystems[enemySystID].teethInPlay.Remove(selectedCards[enemysToothID]);
                    selectedCards[enemysToothID].GetComponent<CardFunctionByHolder>().currentHolder = CardFunctionByHolder.Holders.Player;
                    selectedCards[enemysToothID].transform.SetParent(teethPanels[3].transform);
                    FindFirstObjectByType<CardSystem>().teethInPlay.Add(selectedCards[enemysToothID]);
                    _Panel.SetActive(false);
                    // Cambiar los dientes en el espacio del mundo
                    _EnemySystems[enemySystID].teethModels[enemysToothIndex].fModifyTooth((int)playersOldTooth.GetComponent<CardVisual>().cardData.cardColor);
                    FindFirstObjectByType<PlayerWorldSpaceManager>().ChangeTooth(playersToothIndex, (int)selectedCards[enemysToothID].GetComponent<CardVisual>().cardData.cardColor);
                }
                else
                {
                    _Panel.SetActive(false);
                    if (_EnemySystems[storedIDs[0]].teethInPlay.Contains(selectedCards[0]))
                    {
                        int en1index = selectedCards[0].transform.GetSiblingIndex();
                        int en2index = selectedCards[1].transform.GetSiblingIndex();
                        _EnemySystems[storedIDs[0]].teethInPlay.Remove(selectedCards[0]);
                        selectedCards[0].transform.SetParent(teethPanels[storedIDs[1]].transform);
                        _EnemySystems[storedIDs[1]].teethInPlay.Add(selectedCards[0]);
                        _EnemySystems[storedIDs[1]].teethInPlay.Remove(selectedCards[1]);
                        selectedCards[1].transform.SetParent(teethPanels[storedIDs[0]].transform);
                        _EnemySystems[storedIDs[0]].teethInPlay.Add(selectedCards[1]);
                        selectedCards[0].transform.SetSiblingIndex(en2index);
                        selectedCards[1].transform.SetSiblingIndex(en1index);
                        // Cambiar los dientes en el espacio del mundo
                        _EnemySystems[storedIDs[0]].teethModels[en1index].fModifyTooth((int)selectedCards[1].GetComponent<CardVisual>().cardData.cardColor);
                        _EnemySystems[storedIDs[1]].teethModels[en2index].fModifyTooth((int)selectedCards[0].GetComponent<CardVisual>().cardData.cardColor);
                    }
                    else
                    {
                        int en1index = selectedCards[1].transform.GetSiblingIndex();
                        int en2index = selectedCards[0].transform.GetSiblingIndex();
                        _EnemySystems[storedIDs[0]].teethInPlay.Remove(selectedCards[1]);
                        selectedCards[0].transform.SetParent(teethPanels[storedIDs[0]].transform);
                        _EnemySystems[storedIDs[1]].teethInPlay.Add(selectedCards[1]);
                        _EnemySystems[storedIDs[1]].teethInPlay.Remove(selectedCards[0]);
                        selectedCards[1].transform.SetParent(teethPanels[storedIDs[1]].transform);
                        _EnemySystems[storedIDs[0]].teethInPlay.Add(selectedCards[0]);
                        selectedCards[1].transform.SetSiblingIndex(en2index);
                        selectedCards[0].transform.SetSiblingIndex(en1index);
                        // Cambiar los dientes en el espacio del mundo
                        _EnemySystems[storedIDs[0]].teethModels[en2index].fModifyTooth((int)selectedCards[0].GetComponent<CardVisual>().cardData.cardColor);
                        _EnemySystems[storedIDs[1]].teethModels[en1index].fModifyTooth((int)selectedCards[1].GetComponent<CardVisual>().cardData.cardColor);
                    }
                }

            }
            _Purpose = "";
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
                    StartCoroutine(fExchangeCards());
                    else fExchangeTeeth();
                break;
            case "swapTeeth":
                storedIDs.Add(enemyID);
                _EnemyButtons[enemyID].interactable = false;
                if (storedIDs.Count == idsToHold) 
                    fExchangeTeeth();
                break;
            case "affectTooth":
                storedIDs.Add(enemyID);
                fExchangeTeeth();
                break;
        }
    }

    public void fApplyEffectToTooth(CardVisual tooth, int toothID)
    {
        // apply effect to tooth card
        _EnemySystems[storedIDs[0]].teethModels[toothID].fAlterEffect(effectToApply);
        _Purpose = "";
        _Panel.SetActive(false);
    }

    public void fPlayerButton()
    {
        storedIDs.Add(3);
        if (storedIDs.Count == idsToHold) 
            if (_Purpose == "stealOrSwapCards") StartCoroutine(fExchangeCards());
            else if (_Purpose == "swapTeeth") fExchangeTeeth();
    }

    void fEchangeAllCards()
    {

    }
}
