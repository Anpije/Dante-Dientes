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

    GameObject tempCard;
    GameObject blockSugarCard;
    GameObject extraTimeCard;
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

        if (tempCard != null) { tempCard.GetComponent<CardFunctionByHolder>().toothProtection--; tempCard = null; }

        for (int i = 0; i < _EnSy.hand.Count; i++)
        {
            if (_EnSy.hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Treatment)
            {
                if (_EnSy.hand[i].GetComponent<CardVisual>().cardData.description == "Bloqueo de Azúcar")
                    blockSugarCard = _EnSy.hand[i];
                if (_EnSy.hand[i].GetComponent<CardVisual>().cardData.description == "Tiempo Extra")
                    extraTimeCard = _EnSy.hand[i];
            }
        }
        
        _UselessCards.Clear();

        // Si tiene una carta de diente a mano
        for (int i = 0; i < _EnSy.hand.Count; i++)
        {
            if (_EnSy.hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.HealthyTooth)
            {
                if (_EnSy.teethInPlay.Count < 4)
                {
                    _EnSy.PlaceTooth(_EnSy.hand[i]); Debug.Log(gameObject.name + "placed a tooth");
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
                    var A = AffectTooth(_EnSy, _EnSy.hand[i].GetComponent<CardVisual>(), 1); Debug.Log(gameObject.name + "protected a tooth");
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
                    var B = AffectPlayerTooth(_P1Sy, _EnSy.hand[i].GetComponent<CardVisual>(), - 1); 
                    if (B) { EndTurn(); Debug.Log(gameObject.name + "damaged one of your teeth"); return; }
                }

                int en = UnityEngine.Random.Range(0, nPlayers - 3);
                var A = AffectTooth(_EnemySystems[en], _EnSy.hand[i].GetComponent<CardVisual>(), -1);
                if (A) { EndTurn(); Debug.Log(gameObject.name + "damaged an oponent's tooth"); return; }
                
                for (int j = 0; j < _EnemySystems.Count; j++)
                {
                    var B = AffectTooth(_EnemySystems[j], _EnSy.hand[i].GetComponent<CardVisual>(), -1);
                    if (B) { EndTurn(); Debug.Log(gameObject.name + "damaged an oponent's tooth"); return; }
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
        _EnAc.Discard(_EnAc.FindLeastValuable());
        EndTurn();
    }

    bool ActionByCardDescription(CardVisual card)
    {
        switch (card.cardData.description)
        {
            case "Immunización Total":
                var checkTeeth = AffectTooth(_EnSy, card, 1);
                if (checkTeeth)
                {
                    tempCard = card.gameObject;
                    return true;
                }
                else { return false; }
            case "Cambio de Turno":
                if (UnityEngine.Random.Range(0f, 100f) < targetPlayerChance[nPlayers - 3][(int)Difficulty])
                    _P1Sy.skipTurn = true;
                else
                {
                    int en = UnityEngine.Random.Range(0, nPlayers - 3);
                    _EnemySystems[en].enBv.skipTurn = true;
                }
                return true;
            case "Emergencia Dental":
                var checkForTeeth = AffectTooth(_EnSy, card, 1);
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
            case "Tratamiento Intensivo":
                var toothToTreat = AffectTooth(_EnSy, card, 0);
                if (toothToTreat) return true; else return false;
            case "Confusión Clínica":
                FindFirstObjectByType<EnemyHandEventManager>().fEchangeAllCards();
                return true;
            case "Bloqueo de Azúcar":
                return false;
            case "Tiempo Extra":
                return false;
            default:
                Debug.LogError(card.name + "'s Description is not a valid treatment");
                return false;
        }
    }

    bool AffectTooth(EnemySystem NME, CardVisual effectCard, int effectToApply)
    {
        if (NME.teethInPlay.Count > 0)
        {
            if (NME.enBv.blockSugarCard != null)
            {
                Debug.Log("Tooth was protected by a sugar barrier");
                NME.enAc.Discard(blockSugarCard);
                NME.enBv.blockSugarCard = null;
                return true;
            }

            if (effectToApply == 1 || effectToApply == -1)
            {
                for (int a = 0; a < NME.teethInPlay.Count; a++)
                {
                    if (NME.teethInPlay[a].GetComponent<CardFunctionByHolder>().toothProtection == 0 && (NME.teethInPlay[a].GetComponent<CardVisual>().cardData.cardColor == effectCard.cardData.cardColor || effectCard.cardData.cardType == CardType.Treatment))
                    {
                        NME.teethInPlay[a].GetComponent<CardFunctionByHolder>().toothProtection += effectToApply;
                        Debug.Log(gameObject.name + "'s card has found and affected player's card");
                        return true;
                    }
                }
            }

            int damagedTooth = 100;
            int savedTooth = 0;
            for (int a = 0; a < NME.teethInPlay.Count; a++)
            {
                if (NME.teethInPlay[a].GetComponent<CardFunctionByHolder>().toothProtection < damagedTooth && (NME.teethInPlay[a].GetComponent<CardVisual>().cardData.cardColor == effectCard.cardData.cardColor || effectCard.cardData.cardType == CardType.Treatment))
                {
                    damagedTooth = NME.teethInPlay[a].GetComponent<CardFunctionByHolder>().toothProtection;
                    savedTooth = a;
                }
            }

            if (damagedTooth != 100)
            {
                if (effectToApply == 1 || effectToApply == -1)
                    NME.teethInPlay[savedTooth].GetComponent<CardFunctionByHolder>().toothProtection += effectToApply;
                else
                    NME.teethInPlay[savedTooth].GetComponent<CardFunctionByHolder>().toothProtection *= effectToApply;
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

    bool AffectPlayerTooth(CardSystem NME, CardVisual effectCard, int effectToApply)
    {
        if (NME.teethInPlay.Count > 0)
        {
            if (NME.blockSugarCard != null)
            {
                Debug.Log("Tooth was protected by a sugar barrier");
                NME.blockSugarCard = null;
                NME.DiscardCard(blockSugarCard);
                return true;
            }

            if (effectToApply == 1 || effectToApply == -1)
            {
                for (int a = 0; a < NME.teethInPlay.Count; a++)
                {
                    if (NME.teethInPlay[a].GetComponent<CardFunctionByHolder>().toothProtection == 0 && (NME.teethInPlay[a].GetComponent<CardVisual>().cardData.cardColor == effectCard.cardData.cardColor || effectCard.cardData.cardType == CardType.Treatment))
                    {
                        NME.teethInPlay[a].GetComponent<CardFunctionByHolder>().toothProtection += effectToApply;
                        Debug.Log(gameObject.name + "'s card has found and affected player's card");
                        return true;
                    }
                }
            }

            int damagedTooth = 100;
            int savedTooth = 0;
            for (int a = 0; a < NME.teethInPlay.Count; a++)
            {
                if (NME.teethInPlay[a].GetComponent<CardFunctionByHolder>().toothProtection < damagedTooth && (NME.teethInPlay[a].GetComponent<CardVisual>().cardData.cardColor == effectCard.cardData.cardColor || effectCard.cardData.cardType == CardType.Treatment))
                {
                    damagedTooth = NME.teethInPlay[a].GetComponent<CardFunctionByHolder>().toothProtection;
                    savedTooth = a;
                }
            }

            if (damagedTooth != 100)
            {
                if (effectToApply == 1 || effectToApply == -1)
                    NME.teethInPlay[savedTooth].GetComponent<CardFunctionByHolder>().toothProtection += effectToApply;
                else
                    NME.teethInPlay[savedTooth].GetComponent<CardFunctionByHolder>().toothProtection *= effectToApply;
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
        if (extraTimeCard == null)
        {
            OnEndTurn?.Invoke();
            Debug.Log(gameObject.name + "'s turn has ended");
        }
        else
        {
            Debug.Log(gameObject.name + " played Extra Time, and can go again");
            _EnAc.Discard(extraTimeCard);
            extraTimeCard = null;
            StartTurn();
        }
    }
}