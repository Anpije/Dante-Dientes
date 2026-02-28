using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    // Script para crear las probabilidades de que la IA realiza cierto acción

    private enum Difficulties { EASY, NORMAL, HARD }
    [SerializeField] private Difficulties Difficulty = Difficulties.NORMAL;


}