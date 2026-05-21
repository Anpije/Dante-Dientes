using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

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
        if (debugMode == "confusionClinica")
            fEchangeAllCards(null, null);
        else
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
        _PlayerHandButton.interactable = true;
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
        player.DiscardCard(plAc.playedCard);
        plAc.playedCard = null;
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
                    selectedCards[enemysToothID].transform.SetSiblingIndex(playersToothIndex);
                    playersOldTooth.transform.SetSiblingIndex(enemysToothIndex);
                    player.teethInPlay.Add(selectedCards[enemysToothID]);
                    _Panel.DOFade(0, 0.15f);
                    _Panel.blocksRaycasts = false;
                    // Cambiar los dientes en el espacio del mundo
                    _EnemySystems[enemySystID].teethModels[enemysToothIndex].fModifyTooth((int)playersOldTooth.GetComponent<CardVisual>().cardData.cardColor);
                    playersOldTooth.GetComponent<CardFunctionByHolder>().connectedTooth = _EnemySystems[enemySystID].teethModels[enemysToothIndex];
                    playersOldTooth.GetComponent<CardFunctionByHolder>().currentHolder = CardFunctionByHolder.Holders.Enemy;
                    _EnemySystems[enemySystID].teethModels[enemysToothIndex].effect = playersOldTooth.GetComponent<CardFunctionByHolder>().toothProtection;
                    plAc.teethModels[playersToothIndex].fModifyTooth((int)selectedCards[enemysToothID].GetComponent<CardVisual>().cardData.cardColor);
                    selectedCards[enemysToothID].GetComponent<CardFunctionByHolder>().connectedTooth = plAc.teethModels[playersToothIndex];
                    selectedCards[enemysToothID].GetComponent<CardFunctionByHolder>().currentHolder = CardFunctionByHolder.Holders.Player;
                    plAc.teethModels[playersToothIndex].effect = selectedCards[enemysToothID].GetComponent<CardFunctionByHolder>().toothProtection;
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
                        selectedCards[1].GetComponent<CardFunctionByHolder>().connectedTooth = _EnemySystems[storedIDs[0]].teethModels[en1index];
                        _EnemySystems[storedIDs[0]].teethModels[en1index].effect = selectedCards[1].GetComponent<CardFunctionByHolder>().toothProtection;
                        _EnemySystems[storedIDs[1]].teethModels[en2index].fModifyTooth((int)selectedCards[0].GetComponent<CardVisual>().cardData.cardColor);
                        _EnemySystems[storedIDs[1]].teethModels[en2index].effect = selectedCards[0].GetComponent<CardFunctionByHolder>().toothProtection;
                        selectedCards[0].GetComponent<CardFunctionByHolder>().connectedTooth = _EnemySystems[storedIDs[1]].teethModels[en2index];
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
                        selectedCards[0].GetComponent<CardFunctionByHolder>().connectedTooth = _EnemySystems[storedIDs[0]].teethModels[en2index];
                        _EnemySystems[storedIDs[0]].teethModels[en2index].effect = selectedCards[0].GetComponent<CardFunctionByHolder>().toothProtection;
                        _EnemySystems[storedIDs[1]].teethModels[en1index].fModifyTooth((int)selectedCards[1].GetComponent<CardVisual>().cardData.cardColor);
                        _EnemySystems[storedIDs[1]].teethModels[en1index].effect = selectedCards[1].GetComponent<CardFunctionByHolder>().toothProtection;
                        selectedCards[1].GetComponent<CardFunctionByHolder>().connectedTooth = _EnemySystems[storedIDs[1]].teethModels[en1index];
                    }
                }

            }
            _Purpose = "";
            if (plAc.playedCard != null)
            {
                player.DiscardCard(plAc.playedCard); 
                plAc.playedCard = null;
            }
            plAc.EndTurn();
        }
        else
        {
            if (cardsToHold == 2)
            {
                if (selectedCards.Count == 1)
                {
                    if (storedIDs[0] == 3)
                    {
                        if (plAc.CdSy.teethInPlay.Contains(selectedCards[0]))
                        {
                            for (int i = 0; i < plAc.CdSy.teethInPlay.Count; i++)
                                plAc.CdSy.teethInPlay[i].GetComponent<CardFunctionByHolder>().DisableButton();
                        }

                        if (_EnemySystems[storedIDs[1]].teethInPlay.Contains(selectedCards[0]))
                        {
                            for (int i = 0; i < _EnemySystems[storedIDs[1]].teethInPlay.Count; i++)
                            {
                                _EnemySystems[storedIDs[1]].teethInPlay[i].GetComponent<CardFunctionByHolder>().DisableButton();
                            }
                        }
                    }
                    
                    if (storedIDs[1] == 3)
                    {
                        if (plAc.CdSy.teethInPlay.Contains(selectedCards[0]))
                        {
                            for (int i = 0; i < plAc.CdSy.teethInPlay.Count; i++)
                                plAc.CdSy.teethInPlay[i].GetComponent<CardFunctionByHolder>().DisableButton();
                        }

                        if (_EnemySystems[storedIDs[0]].teethInPlay.Contains(selectedCards[0]))
                        {
                            for (int i = 0; i < _EnemySystems[storedIDs[0]].teethInPlay.Count; i++)
                            {
                                _EnemySystems[storedIDs[0]].teethInPlay[i].GetComponent<CardFunctionByHolder>().DisableButton();
                            }
                        }
                    }
                }

                if (selectedCards.Count == 2)
                {
                    if (storedIDs[0] == 3)
                    {
                        if (plAc.CdSy.teethInPlay.Contains(selectedCards[1]))
                        {
                            for (int i = 0; i < plAc.CdSy.teethInPlay.Count; i++)
                                plAc.CdSy.teethInPlay[i].GetComponent<CardFunctionByHolder>().DisableButton();
                        }

                        if (_EnemySystems[storedIDs[1]].teethInPlay.Contains(selectedCards[1]))
                        {
                            for (int i = 0; i < _EnemySystems[storedIDs[1]].teethInPlay.Count; i++)
                            {
                                _EnemySystems[storedIDs[1]].teethInPlay[i].GetComponent<CardFunctionByHolder>().DisableButton();
                            }
                        }
                    }
                    if (storedIDs[1] == 3)
                    {
                        if (plAc.CdSy.teethInPlay.Contains(selectedCards[1]))
                        {
                            for (int i = 0; i < plAc.CdSy.teethInPlay.Count; i++)
                                plAc.CdSy.teethInPlay[i].GetComponent<CardFunctionByHolder>().DisableButton();
                        }

                        if (_EnemySystems[storedIDs[0]].teethInPlay.Contains(selectedCards[1]))
                        {
                            for (int i = 0; i < _EnemySystems[storedIDs[0]].teethInPlay.Count; i++)
                            {
                                _EnemySystems[storedIDs[0]].teethInPlay[i].GetComponent<CardFunctionByHolder>().DisableButton();
                            }
                        }
                    }
                }
            }
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
        _EnemySystems[storedIDs[0]].enAc.Discard(selectedCards[0], null);
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
        newTooth.GetComponent<CardFunctionByHolder>().connectedTooth = plAc.teethModels[toothIndex];
        plAc.teethModels[toothIndex].effect = newTooth.GetComponent<CardFunctionByHolder>().toothProtection;

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
        if (player.PlAc.blockSugarCard == card)
            player.PlAc.blockSugarCard = null;
        if (player.PlAc.extraTimeCard == card)
            player.PlAc.extraTimeCard = null;

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
                {
                    for (int i = 0; i < _EnemyButtons.Length; i++)
                    {
                        if (_EnemyButtons[i].gameObject.activeSelf)
                            _EnemyButtons[i].interactable = false;
                    }
                    _PlayerHandButton.interactable = false;
                    SkipEnemy();
                }
                break;
            case "swapCards":
                storedIDs.Add(enemyID);
                _EnemyButtons[enemyID].interactable = false;
                if (storedIDs.Count == idsToHold)
                {
                    for (int i = 0; i < _EnemyButtons.Length; i++)
                    {
                        if (_EnemyButtons[i].gameObject.activeSelf)
                            _EnemyButtons[i].interactable = false;
                    }
                    _PlayerHandButton.interactable = false;
                    StartCoroutine(fExchangeCards());
                }
                break;
            case "stealCard":
                storedIDs.Add(enemyID);
                _EnemyButtons[enemyID].interactable = false;
                if (storedIDs.Count == idsToHold)
                {
                    for (int i = 0; i < _EnemyButtons.Length; i++)
                    {
                        if (_EnemyButtons[i].gameObject.activeSelf)
                            _EnemyButtons[i].interactable = false;
                    }
                    _PlayerHandButton.interactable = false;
                    StartCoroutine(fExchangeCards());
                }
                break;
            case "swapTeeth":
                storedIDs.Add(enemyID);
                _EnemyButtons[enemyID].interactable = false;
                if (storedIDs.Count == idsToHold)
                {
                    for (int i = 0; i < _EnemyButtons.Length; i++)
                    {
                        if (_EnemyButtons[i].gameObject.activeSelf)
                            _EnemyButtons[i].interactable = false;
                    }
                    _PlayerHandButton.interactable = false;
                    fExchangeTeeth();
                }
                break;
            case "affectEnemyTooth":
                storedIDs.Add(enemyID);
                for (int i = 0; i < _EnemyButtons.Length; i++)
                {
                    if (_EnemyButtons[i].gameObject.activeSelf)
                        _EnemyButtons[i].interactable = false;
                }
                _PlayerHandButton.interactable = false;
                fExchangeTeeth();
                break;
            case "forceDiscard":
                storedIDs.Add(enemyID);
                if (storedIDs.Count == idsToHold)
                {
                    for (int i = 0; i < _EnemyButtons.Length; i++)
                    {
                        if (_EnemyButtons[i].gameObject.activeSelf)
                            _EnemyButtons[i].interactable = false;
                    }
                    _PlayerHandButton.interactable = false;
                    SelectDiscard();
                }
                break;
        }
    }

    public void fApplyEffectToTooth(CardVisual tooth)
    {
        // apply effect to tooth card
        bool success = true;
        if (tooth == _EnemySystems[storedIDs[0]].enBv.totalImnunityCard)
        {
            success = false;
            FindFirstObjectByType<OnScreenAnnouncement>().SplashText("¡La carta fue imune!", Color.yellow);
        }
        else
        {
            if (effectToApply == 0)
            {
                tooth.GetComponent<CardFunctionByHolder>().toothProtection = 0;
            }
            else
            {
                if (tooth.cardData.cardColor == plAc.playedCard.GetComponent<CardVisual>().cardData.cardColor || tooth.cardData.cardColor == ToothColor.Rainbow ||
                    plAc.playedCard.GetComponent<CardVisual>().cardData.cardColor == ToothColor.Rainbow || plAc.playedCard.GetComponent<CardVisual>().cardData.cardType == CardType.Treatment)
                {
                    if (_EnemySystems[storedIDs[0]].enBv.blockSugarCard == null)
                    {
                        tooth.GetComponent<CardFunctionByHolder>().toothProtection += effectToApply;
                    }
                    else
                    {
                        _EnemySystems[storedIDs[0]].DiscardCard(_EnemySystems[storedIDs[0]].enBv.blockSugarCard, null);
                        _EnemySystems[storedIDs[0]].enBv.blockSugarCard = null;
                        FindFirstObjectByType<OnScreenAnnouncement>().SplashText("¡La carta fue protegida!", Color.yellow);
                    }
                }
                else
                    success = false;
            }
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

    public void fApplyEffectToPlayer(CardVisual tooth)
    {
        bool success = true;
        if (effectToApply == 0)
        {
            tooth.GetComponent<CardFunctionByHolder>().toothProtection = 0;
        }
        else
        {
            if (tooth.cardData.cardColor == plAc.playedCard.GetComponent<CardVisual>().cardData.cardColor || tooth.cardData.cardColor == ToothColor.Rainbow || 
                plAc.playedCard.GetComponent<CardVisual>().cardData.cardColor == ToothColor.Rainbow || plAc.playedCard.GetComponent<CardVisual>().cardData.cardType == CardType.Treatment)
            {
                tooth.GetComponent<CardFunctionByHolder>().toothProtection += effectToApply;
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
        if (storedIDs.Count != 0)
        {
            if (storedIDs[0] == 3)
                return;
        }
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
        StartCoroutine(SwapTheCards(NME, card));
    }

    IEnumerator SwapTheCards(EnemyActions NME, GameObject card)
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
            NME.Discard(card, null);

        FindFirstObjectByType<TurnManager>().CheckAllCardCounts();
        yield return new WaitForEndOfFrame();

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
                yield return new WaitForEndOfFrame();
                cardIndexes.Add(_EnemySystems[i].hand[cardToAdd].transform.GetSiblingIndex());
            }
        }

        cards.Add(player.hand[Random.Range(0, 4)]);
        cardIndexes.Add(cards[cards.Count - 1].transform.GetSiblingIndex());

        FindFirstObjectByType<OnScreenAnnouncement>().SlideText("Confusión Clínica", Color.blue);
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
            plAc.cardModels[cardIndexes[3]].fReturnFromPosition(_EnemySystems[0].cardModels[cardIndexes[0]].transform, 0.25f);

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

            _EnemySystems[1].cardModels[cardIndexes[0]].fReturnFromPosition(_EnemySystems[2].cardModels[cardIndexes[1]].transform, 0.25f);
            _EnemySystems[2].cardModels[cardIndexes[1]].fReturnFromPosition(plAc.cardModels[cardIndexes[2]].transform, 0.25f);
            plAc.cardModels[cardIndexes[2]].fReturnFromPosition(_EnemySystems[1].cardModels[cardIndexes[0]].transform, 0.25f);

            cards[0].GetComponent<CardFunctionByHolder>().currentHolder = CardFunctionByHolder.Holders.Player;
            cards[2].GetComponent<CardFunctionByHolder>().currentHolder = CardFunctionByHolder.Holders.Enemy;
        }

        if (FindFirstObjectByType<TurnManager>().currentTurn == 0)
            plAc.EndTurn();

        StopCoroutine("SwapTheCards");
    }

    public void ClosePanel()
    {
        panelBackground.DOFade(0, 0.15f);
        _Panel.DOFade(0, 0.15f);
        _Purpose = "";
        _Panel.blocksRaycasts = false;
    }
}
