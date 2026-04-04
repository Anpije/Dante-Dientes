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

    List<GameObject> _UselessCards = new List<GameObject>();

    CardVisual tempCard;
    public bool skipTurn = false;

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
        if (skipTurn) { skipTurn = false; return; }

        _UselessCards.Clear();

        // Si tiene una carta de diente a mano
        for (int i = 0; i < _EnSy.hand.Count; i++)
        {
            if (_EnSy.hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.HealthyTooth)
            {
                if (_EnSy.teethInPlay.Count < 4)
                {
                    _EnSy.PlaceTooth(_EnSy.hand[i]);
                    EndTurn();
                    return;
                }
                _UselessCards.Add(_EnSy.hand[i]);
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
                    else
                        _UselessCards.Add(_EnSy.hand[i]);
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

                _UselessCards.Add(_EnSy.hand[i]);
            }
        }

        for (int i = 0; i < _EnSy.hand.Count; i++)
        {
            if (_EnSy.hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Treatment)
            {
                var checkFunction = ActionByCardDescription(_EnSy.hand[i].GetComponent<CardVisual>());
                if (checkFunction) { EndTurn(); return; }

                _UselessCards.Add(_EnSy.hand[i]);
            }
        }

        // Si no ha podido jugar una carta se eligirá uno para descartar
    }

    bool ActionByCardDescription(CardVisual card)
    {
        switch (card.cardData.description)
        {
            case "Immunización Total":
                var checkTeeth = AffectTooth(_EnSy.teethInPlay, card, 1);
                if (checkTeeth)
                {
                    tempCard = card;
                    return true;
                }
                else { return false; }
            case "Cambio de Turno":
                if (UnityEngine.Random.Range(0f, 100f) < targetPlayerChance[nPlayers - 3][(int)Difficulty])
                {
                    // Skip players turn
                }
                else
                {
                    int en = UnityEngine.Random.Range(0, nPlayers - 3);
                    _EnemySystems[en].enBv.skipTurn = true;
                }
                return true;
            case "Emergencia Dental":
                var checkForTeeth = AffectTooth(_EnSy.teethInPlay, card, 1);
                if (checkForTeeth) { return true; } else return false;
            case "Revisión Sorpresa":
                if (UnityEngine.Random.Range(0f, 100f) < targetPlayerChance[nPlayers - 3][(int)Difficulty])
                {
                    _P1Sy.DiscardMostValuable();
                    return true;
                }
                else
                {
                    int en = UnityEngine.Random.Range(0, nPlayers - 3);
                    _EnemySystems[en].enAc.DiscardMostValuable();
                    return true;
                }
            case "Refuerzo de Esmalte":
                if (_EnSy.teethInPlay.Count > 0)
                {
                    for (int i = 0; i < _EnSy.teethInPlay.Count; i++)
                    {
                        if (_EnSy.teethInPlay[i].GetComponent<CardFunctionByHolder>().toothProtection > 0)
                        {
                            _EnSy.teethInPlay[i].GetComponent<CardFunctionByHolder>().toothProtection++;
                            return true;
                        }
                    }
                }
                return false;
            case "Intercambio Carta":
                if (UnityEngine.Random.Range(0f, 100f) < targetPlayerChance[nPlayers - 3][(int)Difficulty])
                {
                    // Steal Player's card
                    return true;
                }
                else
                {
                    if (nPlayers == 3)
                    {
                        _EnAc.SwapCard(_EnemySystems[0].enAc.FindMostValuable(), _EnemySystems[0]);
                        return true;
                    }
                    else
                    {
                        _EnemySystems[0].enAc.SwapCard(_EnemySystems[1].enAc.FindMostValuable(), _EnemySystems[1]);
                        return true;
                    }
                }
            case "Intercambio Diente":
                int mostTeeth = 0;
                int playerWithTeeth = 0;
                for (int i = 0; i < _EnemySystems.Count; i++)
                {
                    if (_EnemySystems[i].teethInPlay.Count > mostTeeth)
                    {
                        mostTeeth = _EnemySystems[i].teethInPlay.Count;
                        playerWithTeeth = i;
                    }
                }
                if (_P1Sy.teethInPlay.Count >= mostTeeth)
                {
                    if (UnityEngine.Random.Range(1, 100f) < targetPlayerChance[nPlayers - 3][(int)Difficulty])
                    {
                        if (mostTeeth != 0)
                        {
                            // Steal Player's teeth
                            return true;
                        }
                        else
                            return false;
                    }
                }
                else
                {
                    if (mostTeeth == 0)
                        return false;
                    else
                    {
                        _EnemySystems[playerWithTeeth].enAc.SwapTeeth(_EnAc.FindToothByDamage(false, null), _EnSy);
                        return true;
                    }
                }
                return false;
            /*case "Recuperación Rápida":

                // if () return true; else return false;
                break;
            case "Bloqueo de Azúcar":

                // if () return true; else return false;
                break;
            case "Tratamiento Intensivo":

                // if () return true; else return false;
                break;
            case "Tiempo Extra":

                // if () return true; else return false;
                break;
            case "Confusión Clínica":

                // if () return true; else return false;
                break;*/
            default:
                Debug.LogError(card.name + "'s Description is not a valid treatment");
                return false;
        }
    }

    bool AffectTooth(List<GameObject> teeth, CardVisual effectCard, int effectToApply)
    {
        if (teeth.Count > 0)
        {
            for (int a = 0; a < teeth.Count; a++)
            {
                if (teeth[a].GetComponent<CardFunctionByHolder>().toothProtection == 0 && (teeth[a].GetComponent<CardVisual>().cardData.cardColor == effectCard.cardData.cardColor || effectCard.cardData.cardType == CardType.Treatment))
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
                if (teeth[a].GetComponent<CardFunctionByHolder>().toothProtection < damagedTooth && (teeth[a].GetComponent<CardVisual>().cardData.cardColor == effectCard.cardData.cardColor || effectCard.cardData.cardType == CardType.Treatment))
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