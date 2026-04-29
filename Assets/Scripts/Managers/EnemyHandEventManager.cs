using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class EnemyHandEventManager : MonoBehaviour
{
    [SerializeField] CanvasGroup _Panel;
    [SerializeField] Image panelBackground;
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
    [SerializeField] CardSystem player;
    [SerializeField] PlayerActions plAc;

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
        
        if (purpose != "discardPlayer") 
        {
            _Panel.DOFade(1, 0.15f);
            _Panel.blocksRaycasts = true;
        }

        _Purpose = purpose;

        switch (purpose)
        {
            case "selectEnemy":
                fSelectEnemy();
                break;
            case "stealCard":
                fStealCard();
                break;
            case "swapCards":
                SwapCards();
                break;
            case "swapTeeth":
                fSwapTeeth();
                break;
            case "affectEnemyTooth":
                faffectTooth();
                break;
            case "affectPlayerTooth":
                faffectPlayerTooth();
                break;
            case "forceDiscard":
                fForceDiscard();
                break;
            case "immunizePlayerTooth":
                fImmunizePlayerTooth();
                break;
            case "replaceTooth":
                fReplaceTooth();
                break;
            case "discardPlayer":
                DiscardPlayerCard();
                break;
        }
    }

    void fSelectEnemy()
    {
        idsToHold = 1;
        _PlayerHandButton.gameObject.SetActive(false);
    }

    void fStealCard()
    {
        idsToHold = 2;
        _PlayerHandButton.gameObject.SetActive(false);
        storedIDs.Add(3);
        _HandPanel.SetActive(true);
    }

    void SwapCards()
    {
        idsToHold = 2;
        _HandPanel.SetActive(true);
        _PlayerHandButton.gameObject.SetActive(false);
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
        _PlayerHandButton.gameObject.SetActive(false);
    }

    void faffectPlayerTooth()
    {
        for (int i = 0; i < 3; i++)
            _EnemyButtons[i].gameObject.SetActive(false);
        storedIDs.Add(0);
        teethPanels[3].SetActive(true);
        cardsToHold = 1;
    }

    void fForceDiscard()
    {
        idsToHold = 1;
        _HandPanel.SetActive(true);
        _PlayerHandButton.gameObject.SetActive(false);
    }

    void fImmunizePlayerTooth()
    {
        for (int i = 0; i < 3; i++)
            _EnemyButtons[i].gameObject.SetActive(false);
        storedIDs.Add(0);
        teethPanels[3].SetActive(true);
        cardsToHold = 1;
    }

    void fReplaceTooth()
    {
        for (int i = 0; i < 3; i++)
            _EnemyButtons[i].gameObject.SetActive(false);
        storedIDs.Add(0);
        teethPanels[3].SetActive(true);
        cardsToHold = 1;
    }

    void DiscardPlayerCard()
    {
        storedIDs.Add(0);
        panelBackground.DOFade(0.5f, 0.15f);
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

            _Panel.DOFade(0, 0.15f);
            _Panel.blocksRaycasts = false;

            _EnemySystems[storedIDs[0]].cardModels[firstCard].fReturnFromPosition(newTrans2, 3);
            yield return new WaitForSeconds(0.2f);
            _EnemySystems[storedIDs[1]].cardModels[secondCard].fReturnFromPosition(newTrans1, 3);
            _Purpose = "";
            player.DiscardCard(plAc.playedCard);
            plAc.playedCard = null;
            plAc.EndTurn();
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
                player.hand.Add(card);
                selectedCards[0].transform.SetParent(player.deckPosition);
                selectedCards[0].GetComponent<CardFunctionByHolder>().currentHolder = CardFunctionByHolder.Holders.Player;
                player.UpdateHandVisual();
                int cardID = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (!plAc.cardModels[i].atDefaultTransform)
                        cardID = i;
                }
                _EnemySystems[enSyst].GetComponent<EnemyActions>().DrawCard();
                _Panel.DOFade(0, 0.15f);
                _Panel.blocksRaycasts = false;
                plAc.cardModels[cardID].fReturnFromPosition(newTrans, 3);
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
                    player.teethInPlay.Remove(playersOldTooth);
                    playersOldTooth.GetComponent<CardFunctionByHolder>().currentHolder = CardFunctionByHolder.Holders.Enemy;
                    playersOldTooth.transform.SetParent(teethPanels[enemySystID].transform);
                    _EnemySystems[enemySystID].teethInPlay.Add(playersOldTooth);
                    _EnemySystems[enemySystID].teethInPlay.Remove(selectedCards[enemysToothID]);
                    selectedCards[enemysToothID].GetComponent<CardFunctionByHolder>().currentHolder = CardFunctionByHolder.Holders.Player;
                    selectedCards[enemysToothID].transform.SetParent(teethPanels[3].transform);
                    player.teethInPlay.Add(selectedCards[enemysToothID]);
                    _Panel.DOFade(0, 0.15f);
                    _Panel.blocksRaycasts = false;
                    // Cambiar los dientes en el espacio del mundo
                    _EnemySystems[enemySystID].teethModels[enemysToothIndex].fModifyTooth((int)playersOldTooth.GetComponent<CardVisual>().cardData.cardColor);
                    plAc.teethModels[playersToothIndex].fModifyTooth((int)selectedCards[enemysToothID].GetComponent<CardVisual>().cardData.cardColor);
                    plAc.teethModels[playersToothIndex].fModifyTooth((int)selectedCards[enemysToothID].GetComponent<CardVisual>().cardData.cardColor);
                }
                else
                {
                    _Panel.DOFade(0, 0.15f);
                    _Panel.blocksRaycasts = false;
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
            player.DiscardCard(plAc.playedCard); 
            plAc.playedCard = null;
            plAc.EndTurn();
        }
    }

    void SelectDiscard()
    {
        handPanels[storedIDs[0]].SetActive(true);
        cardsToHold = 1;
    }

    public void ForceDiscard()
    {
        _Panel.DOFade(0, 0.15f);
        _Panel.blocksRaycasts = false;
        _EnemySystems[storedIDs[0]].enAc.Discard(selectedCards[0]);
        _Purpose = "";
        player.DiscardCard(plAc.playedCard);
        plAc.playedCard = null;
        plAc.EndTurn();
    }

    public void fReplacePlayerTooth(GameObject newTooth)
    {
        _Purpose = "";
        _Panel.DOFade(0, 0.15f);
        _Panel.blocksRaycasts = false;

        int toothIndex = selectedCards[0].transform.GetSiblingIndex();

        player.teethInPlay.Remove(selectedCards[0]);
        player.allCardObjects.Remove(selectedCards[0]);
        Destroy(selectedCards[0]);

        plAc.teethModels[toothIndex].fModifyTooth((int)newTooth.GetComponent<CardVisual>().cardData.cardColor);

        player.hand.Remove(newTooth);
        player.teethInPlay.Add(newTooth);
        newTooth.transform.SetParent(player.teethAreaPosition);
        newTooth.transform.SetSiblingIndex(toothIndex);

        plAc.playedCard = null;

        plAc.EndTurn();
    }

    public void DiscardSelected(GameObject card)
    {
        _Purpose = "";
        player.DiscardCard(card);
        player.DrawCard();

        panelBackground.DOFade(0, 0.15f);

        plAc.EndTurn();
    }

    public void fEnemyButtonFunction(int enemyID)
    {
        switch (_Purpose)
        {
            case "selectEnemy":
                storedIDs.Add(enemyID);
                if (storedIDs.Count == idsToHold)
                    SkipEnemy();
                break;
            case "swapCards":
                storedIDs.Add(enemyID);
                _EnemyButtons[enemyID].interactable = false;
                if (storedIDs.Count == idsToHold) 
                    StartCoroutine(fExchangeCards());
                break;
            case "stealCard":
                storedIDs.Add(enemyID);
                _EnemyButtons[enemyID].interactable = false;
                if (storedIDs.Count == idsToHold)
                    StartCoroutine(fExchangeCards());
                break;
            case "swapTeeth":
                storedIDs.Add(enemyID);
                _EnemyButtons[enemyID].interactable = false;
                if (storedIDs.Count == idsToHold) 
                    fExchangeTeeth();
                break;
            case "affectEnemyTooth":
                storedIDs.Add(enemyID);
                fExchangeTeeth();
                break;
            case "forceDiscard":
                storedIDs.Add(enemyID);
                if (storedIDs.Count == idsToHold)
                    SelectDiscard();
                break;

        }
    }

    public void fApplyEffectToTooth(CardVisual tooth, int toothID)
    {
        // apply effect to tooth card
        bool success = true;
        if (effectToApply == 0)
        {
            _EnemySystems[storedIDs[0]].teethModels[toothID].effect = 0;
            _EnemySystems[storedIDs[0]].teethInPlay[toothID].GetComponent<CardFunctionByHolder>().toothProtection = 0;
        }
        else
        {
            if (tooth.cardData.cardColor == plAc.playedCard.GetComponent<CardVisual>().cardData.cardColor || tooth.cardData.cardColor == ToothColor.Rainbow || 
                plAc.playedCard.GetComponent<CardVisual>().cardData.cardColor == ToothColor.Rainbow || plAc.playedCard.GetComponent<CardVisual>().cardData.cardType == CardType.Treatment)
            {
                _EnemySystems[storedIDs[0]].teethModels[toothID].effect += effectToApply;
                _EnemySystems[storedIDs[0]].teethInPlay[toothID].GetComponent<CardFunctionByHolder>().toothProtection += effectToApply;
            }
            else
                success = false;
        }
        _Purpose = "";
        _Panel.DOFade(0, 0.15f);
        _Panel.blocksRaycasts = false;
        if (success)
        {
            player.DiscardCard(plAc.playedCard); 
            plAc.EndTurn();
        }
        plAc.playedCard = null;
    }

    public void fApplyEffectToPlayer(CardVisual tooth, int toothID)
    {
        bool success = true;
        if (effectToApply == 0)
        {
            plAc.teethModels[toothID].effect = 0;
            player.teethInPlay[toothID].GetComponent<CardFunctionByHolder>().toothProtection = 0;
        }
        else
        {
            if (tooth.cardData.cardColor == plAc.playedCard.GetComponent<CardVisual>().cardData.cardColor || tooth.cardData.cardColor == ToothColor.Rainbow || 
                plAc.playedCard.GetComponent<CardVisual>().cardData.cardColor == ToothColor.Rainbow || plAc.playedCard.GetComponent<CardVisual>().cardData.cardType == CardType.Treatment)
            {
                plAc.teethModels[toothID].effect += effectToApply;
                player.teethInPlay[toothID].GetComponent<CardFunctionByHolder>().toothProtection += effectToApply;
            }
            else
                success = false;
        }
        _Purpose = "";
        _Panel.DOFade(0, 0.15f);
        _Panel.blocksRaycasts = false;
        if (success)
        {
            player.DiscardCard(plAc.playedCard); 
            plAc.EndTurn();
        }
            plAc.playedCard = null;
    }

    public void fImmunizeThisCard()
    {
        player.PlAc.totalImunityCard = selectedCards[0];
        _Purpose = "";
        _Panel.DOFade(0, 0.15f);
        _Panel.blocksRaycasts = false;
        player.DiscardCard(plAc.playedCard);
        plAc.playedCard = null;
        plAc.EndTurn();
    }

    public void fPlayerButton()
    {
        storedIDs.Add(3);
        if (storedIDs.Count == idsToHold) 
            if (_Purpose == "stealOrSwapCards") StartCoroutine(fExchangeCards());
            else if (_Purpose == "swapTeeth") fExchangeTeeth();
    }

    void SkipEnemy()
    {
        _EnemySystems[storedIDs[0]].enBv.skipTurn = true;
        _Purpose = "";
        _Panel.DOFade(0, 0.15f);
        _Panel.blocksRaycasts = false;
        player.DiscardCard(plAc.playedCard);
        plAc.playedCard = null;
        plAc.EndTurn();
    }


    public void fEchangeAllCards(EnemyActions NME, GameObject card)
    {
        List<GameObject> cards = new List<GameObject>();
        List<int> cardIndexes = new List<int>();

        if (FindFirstObjectByType<TurnManager>().currentTurn == 0)
        {
            player.DiscardCard(plAc.playedCard); 
            plAc.playedCard = null;
            player.DrawCard();
        }
        else
            NME.Discard(card);

        for (int i = 0; i < 3; i++)
        {
            if (_EnemySystems[i].gameObject.activeSelf)
            {
                while (_EnemySystems[i].hand.Count < 4)
                    _EnemySystems[i].DrawCard();
            }
        }

        while (player.hand.Count < 4)
            player.DrawCard();

        for (int i = 0; i < 3; i++)
        {
            Debug.Log("Out " + i);
            if (_EnemySystems[i].gameObject.activeSelf)
            {
                int cardToAdd = Random.Range(0, 4);
                Debug.Log("Doing " + i);
                Debug.Log(cardToAdd);
                Debug.Log(_EnemySystems[i]);
                Debug.Log(_EnemySystems[i].hand[cardToAdd]);
                cards.Add(_EnemySystems[i].hand[cardToAdd]);
                cardIndexes.Add(cards[i].transform.GetSiblingIndex());
            }
        }

        cards.Add(player.hand[Random.Range(0, 4)]);
        cardIndexes.Add(cards[cards.Count-1].transform.GetSiblingIndex());

        if (_EnemySystems[0].gameObject.activeSelf)
        {
            _EnemySystems[0].hand.Remove(cards[0]);
            _EnemySystems[1].hand.Remove(cards[1]);
            _EnemySystems[2].hand.Remove(cards[2]);
            player.hand.Remove(cards[3]);

            _EnemySystems[0].hand.Add(cards[1]);
            _EnemySystems[1].hand.Add(cards[2]);
            _EnemySystems[2].hand.Add(cards[3]);
            player.hand.Add(cards[0]);

            cards[0].transform.SetParent(player.deckPosition);
            player.UpdateHandVisual();
            cards[1].transform.SetParent(_EnemySystems[0].handPosition);
            cards[2].transform.SetParent(_EnemySystems[1].handPosition);
            cards[3].transform.SetParent(_EnemySystems[2].handPosition);

            _EnemySystems[0].cardModels[cardIndexes[0]].fReturnFromPosition(_EnemySystems[1].cardModels[cardIndexes[1]].transform, 0.25f);
            _EnemySystems[1].cardModels[cardIndexes[1]].fReturnFromPosition(_EnemySystems[2].cardModels[cardIndexes[2]].transform, 0.25f);
            _EnemySystems[2].cardModels[cardIndexes[2]].fReturnFromPosition(plAc.cardModels[cardIndexes[3]].transform, 0.25f);
            plAc.cardModels[cardIndexes[3]].fReturnFromPosition(_EnemySystems[0].cardModels[cardIndexes[0]].transform, 0.15f);

            cards[0].GetComponent<CardFunctionByHolder>().currentHolder = CardFunctionByHolder.Holders.Player;
            cards[3].GetComponent<CardFunctionByHolder>().currentHolder = CardFunctionByHolder.Holders.Enemy;
        }
        else
        {
            _EnemySystems[1].hand.Remove(cards[0]);
            _EnemySystems[2].hand.Remove(cards[1]);
            player.hand.Remove(cards[2]);

            _EnemySystems[1].hand.Add(cards[1]);
            _EnemySystems[2].hand.Add(cards[2]);
            player.hand.Add(cards[0]);

            cards[0].transform.SetParent(player.deckPosition);
            player.UpdateHandVisual();
            cards[1].transform.SetParent(_EnemySystems[1].handPosition);
            cards[2].transform.SetParent(_EnemySystems[2].handPosition);

            _EnemySystems[0].cardModels[cardIndexes[0]].fReturnFromPosition(_EnemySystems[2].cardModels[cardIndexes[1]].transform, 0.25f);
            _EnemySystems[1].cardModels[cardIndexes[1]].fReturnFromPosition(plAc.cardModels[cardIndexes[2]].transform, 0.25f);
            plAc.cardModels[cardIndexes[2]].fReturnFromPosition(_EnemySystems[0].cardModels[cardIndexes[0]].transform, 0.15f);

            cards[0].GetComponent<CardFunctionByHolder>().currentHolder = CardFunctionByHolder.Holders.Player;
            cards[2].GetComponent<CardFunctionByHolder>().currentHolder = CardFunctionByHolder.Holders.Enemy;
        }

        if (FindFirstObjectByType<TurnManager>().currentTurn == 0)
            plAc.EndTurn(); 
    }

}
