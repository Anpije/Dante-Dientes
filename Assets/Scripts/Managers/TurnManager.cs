using System.Collections.Generic;
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
    }

    void OnDisable()
    {
        EnemyBehaviour.OnEndTurn -= fInvokeNextTurn;
        PlayerActions.OnEndTurn -= fInvokeNextTurn;
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
    }
}
