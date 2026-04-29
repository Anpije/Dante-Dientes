using UnityEngine;

public class EnemyDifficulty : MonoBehaviour
{
    public static EnemyDifficulty Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public enum Difficulties { EASY, NORMAL, HARD }
    public Difficulties Difficulty = Difficulties.NORMAL;
}
