using UnityEngine.Events;
using UnityEngine;
using System;

public class EnemyBehaviour : MonoBehaviour
{
    // Script para crear las probabilidades de que la IA realiza cierto acción
    public enum Difficulties { EASY, NORMAL, HARD }
    public Difficulties Difficulty = Difficulties.NORMAL;

    float[] targetPlayerChance = { 25, 50, 75 };

    public static Action OnEndTurn;

    [ContextMenu("StartTurn")]
    public void StartTurn()
    {
        // Si tiene una carta de diente a mano
        for (int i = 0; i < GetComponent<EnemySystem>().hand.Count; i++)
        {
            if (GetComponent<EnemySystem>().hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.HealthyTooth)
            {
                GetComponent<EnemySystem>().PlaceTooth(GetComponent<EnemySystem>().hand[i]);
                EndTurn();
                return;
            }
        }

        if (GetComponent<EnemySystem>().teethInPlay.Count > 0)
        {
            // Si tiene dientes jugados y una carta protectora a mano
            for (int i = 0; i < GetComponent<EnemySystem>().hand.Count; i++)
            {
                if (GetComponent<EnemySystem>().hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Protective)
                {
                
                    return;
                }
            }
        }

        for (int i = 0; i < GetComponent<EnemySystem>().hand.Count; i++)
        {
            if (GetComponent<EnemySystem>().hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Harmful)
            {
                
                return;
            }
        }

        for (int i = 0; i < GetComponent<EnemySystem>().hand.Count; i++)
        {
            if (GetComponent<EnemySystem>().hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.Treatment)
            {
                
                return;
            }
        }
    }

    private void EndTurn()
    {
        OnEndTurn?.Invoke();
        Debug.Log(gameObject.name + "'s turn has ended");
    }
}