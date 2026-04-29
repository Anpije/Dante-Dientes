using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using System;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private PlayerActions PlAc;
    [SerializeField] private EnemyBehaviour[] _Enemies;
    [SerializeField] private List<EnemyBehaviour> _Players = new List<EnemyBehaviour>();

    [SerializeField] public int currentTurn = 0;

    public static Action PlayerStartTurn;

    void OnEnable()
    {
        EnemyBehaviour.OnEndTurn += fInvokeNextTurn;
        PlayerActions.OnEndTurn += fInvokeNextTurn;
        EnemyBehaviour.OnEnemyWin += EndGame;
        PlayerActions.OnPlayerWin += EndGame;
    }

    void OnDisable()
    {
        EnemyBehaviour.OnEndTurn -= fInvokeNextTurn;
        PlayerActions.OnEndTurn -= fInvokeNextTurn;
        EnemyBehaviour.OnEnemyWin -= EndGame;
        PlayerActions.OnPlayerWin -= EndGame;
    }

    void Start()
    {
        _Players.Add(null);
        for (int i = 0; i < _Enemies.Length; i++)
            if (_Enemies[i].gameObject.activeSelf) _Players.Add(_Enemies[i]);
        PlayerStartTurn?.Invoke();
    }

    void fInvokeNextTurn() { Invoke("fNextTurn", 1); }

    public void fNextTurn()
    {
        StartCoroutine(LoadNextTurn());
    }

    IEnumerator LoadNextTurn()
    {
        CheckAllCardCounts();
        yield return new WaitForEndOfFrame();
        currentTurn++;
        if (currentTurn >= _Players.Count) currentTurn = 0;
        if (_Players[currentTurn] != null)
            _Players[currentTurn].StartTurn();
        else
        {
            Debug.Log("Player's turn");
            if (!PlAc.skipTurn)
                PlayerStartTurn?.Invoke();
            else
            {
                PlAc.skipTurn = false;
                currentTurn++;
                _Players[currentTurn].StartTurn();
            }
        }
        StopCoroutine(LoadNextTurn());
    }

    void EndGame(bool victor)
    {
        if (victor)
            Debug.LogWarning("End of Game. Player Wins");
        else
            Debug.LogWarning("End of Game. Player Lost");

        // Go to whatever screen
    }
    void CheckAllCardCounts()
    {
        Debug.Log("Checking Cards");
        for (int i = 1; i < _Players.Count; i++)
        {

            while (_Players[i]._EnSy.hand.Count > 4)
            {
                GameObject toRemove = _Players[i]._EnSy.hand[_Players[i]._EnSy.hand.Count - 1];
                _Players[i]._EnSy.hand.Remove(toRemove);
                toRemove.transform.SetParent(FindFirstObjectByType<CardSystem>().deckPosition);
                toRemove.SetActive(false);
                Debug.Log("Removing card from " + _Players[i].gameObject.name); 
            }

            while (_Players[i]._EnSy.hand.Count < 4)
            { 
                _Players[i]._EnAc.DrawCard(); 
                Debug.Log("Adding card to " + _Players[i].gameObject.name); 
            }
        }

        while (PlAc.CdSy.hand.Count < 4)
            PlAc.CdSy.DrawCard();

        while (PlAc.CdSy.hand.Count > 4)
        {
            GameObject toRemove = PlAc.CdSy.hand[PlAc.CdSy.hand.Count - 1];
            PlAc.CdSy.hand.Remove(toRemove);
            toRemove.transform.SetParent(FindFirstObjectByType<CardSystem>().deckPosition);
            toRemove.SetActive(false);
        }
    }
}
