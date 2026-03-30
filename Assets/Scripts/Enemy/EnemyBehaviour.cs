using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System;

public class EnemyBehaviour : MonoBehaviour
{
    // Script para crear las probabilidades de que la IA realiza cierto acción
    public enum Difficulties { EASY, NORMAL, HARD }
    public Difficulties Difficulty = Difficulties.NORMAL;

    [Range(3, 4)]
    public int nPlayers = 3;

    Vector3[] targetPlayerChance = 
        { new Vector3(25, 50, 75),       // Probabilidades según la dificultad si hay 3 jugadores en total
          new Vector3 (16.5f, 33, 66) }; // Probabilidades según la dificultad si hay 4 jugadores en total

    public static Action OnEndTurn;

    EnemySystem _EnSy;
    EnemyActions _EnAc;
    CardSystem _P1Sy;
    List<EnemySystem> _EnemySystems = new List<EnemySystem>();

    void Awake()
    {
        _EnSy = GetComponent<EnemySystem>();
        _EnAc = GetComponent<EnemyActions>();
        _P1Sy = FindFirstObjectByType<CardSystem>();

        EnemySystem[] _EnSys = FindObjectsByType<EnemySystem>(FindObjectsSortMode.None);
        for (int i = 0; i < _EnSys.Length; i++)
            if (_EnSys[i] != this) _EnemySystems.Add(_EnSys[i]);
        nPlayers = _EnemySystems.Count + 1;
    }

    [ContextMenu("StartTurn")]
    public void StartTurn()
    {
        // Si tiene una carta de diente a mano
        for (int i = 0; i < _EnSy.hand.Count; i++)
        {
            if (_EnSy.hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.HealthyTooth)
            {
                _EnSy.PlaceTooth(_EnSy.hand[i]);
                EndTurn();
                return;
            }
        }

        if (_EnSy.teethInPlay.Count > 0)
        {
            // Si tiene dientes jugados y una carta protectora a mano
            for (int i = 0; i < _EnSy.hand.Count; i++)
            {
                if (_EnSy.hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Protective)
                {
                    var A = AffectTooth(_EnSy.teethInPlay, _EnSy.hand[i].GetComponent<CardVisual>(), 1);
                    if (A) { EndTurn(); return; }
                    else {
                        _EnAc.Discard(_EnSy.hand[i]);
                        EndTurn();
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < _EnSy.hand.Count; i++)
        {
            if (_EnSy.hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Harmful)
            {
                if (UnityEngine.Random.Range(0f, 100f) < targetPlayerChance[nPlayers - 3][(int)Difficulty])
                {
                    var B = AffectTooth(_P1Sy.teethInPlay, _EnSy.hand[i].GetComponent<CardVisual>(), - 1);
                    if (B) { EndTurn(); return; }
                }

                int en = UnityEngine.Random.Range(0, nPlayers - 3);
                var A = AffectTooth(_EnemySystems[en].teethInPlay, _EnSy.hand[i].GetComponent<CardVisual>(), -1);
                if (A) { EndTurn(); return; }
                
                for (int j = 0; j < _EnSy.hand.Count; j++)
                {
                    var B = AffectTooth(_EnemySystems[j].teethInPlay, _EnSy.hand[i].GetComponent<CardVisual>(), -1);
                    if (B) { EndTurn(); return; }
                }

                _EnAc.Discard(_EnSy.hand[i]);
                EndTurn();
                return;
            }
        }

        for (int i = 0; i < _EnSy.hand.Count; i++)
        {
            if (_EnSy.hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Treatment)
            {
                
                return;
            }
        }
    }

    bool AffectTooth(List<GameObject> teeth, CardVisual effectCard, int effectToApply)
    {
        if (teeth.Count > 0)
        {
            for (int a = 0; a < teeth.Count; a++)
            {
                if (teeth[a].GetComponent<CardFunctionByHolder>().toothProtection == 0 && teeth[a].GetComponent<CardVisual>().cardData.cardColor == effectCard.cardData.cardColor)
                {
                    teeth[a].GetComponent<CardFunctionByHolder>().toothProtection += effectToApply;
                    Debug.Log(gameObject.name + "'s card has found and affected player's card");
                    return true;
                }
            }

            int damagedTooth = 100;
            int savedTooth = 0;
            for (int a = 0; a < teeth.Count; a++)
            {
                if (teeth[a].GetComponent<CardFunctionByHolder>().toothProtection < damagedTooth && teeth[a].GetComponent<CardVisual>().cardData.cardColor == effectCard.cardData.cardColor)
                {
                    damagedTooth = teeth[a].GetComponent<CardFunctionByHolder>().toothProtection;
                    savedTooth = a;
                }
            }

            if (damagedTooth != 100)
            {
                teeth[savedTooth].GetComponent<CardFunctionByHolder>().toothProtection += effectToApply;
                Debug.Log(gameObject.name + "'s card has found and affected player's card");
                return true;
            }
            else
            {
                Debug.Log(gameObject.name + "'s card can't be used");
                return false;
            }
        }
        else
        {
            Debug.Log(gameObject.name + "'s card has failed to find other player's card");
            return false;
        }
    }

    private void EndTurn()
    {
        OnEndTurn?.Invoke();
        Debug.Log(gameObject.name + "'s turn has ended");
    }
}