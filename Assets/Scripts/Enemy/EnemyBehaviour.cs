using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    // Script para crear las probabilidades de que la IA realiza cierto acción
    public enum Difficulties { EASY, NORMAL, HARD }
    public Difficulties Difficulty = Difficulties.NORMAL;

    [ContextMenu("StartTurn")]
    public void StartTurn()
    {
        for (int i = 0; i < GetComponent<EnemySystem>().hand.Count; i++)
        {
            if (GetComponent<EnemySystem>().hand[i].GetComponent<CardVisual>().cardData.cardType == CardType.HealthyTooth)
            {
                GetComponent<EnemySystem>().PlaceTooth(GetComponent<EnemySystem>().hand[i]);
                EndTurn();
                return;
            }
        }
    }

    private void EndTurn()
    {
        Debug.Log(gameObject.name + "'s turn has ended");
    }
}