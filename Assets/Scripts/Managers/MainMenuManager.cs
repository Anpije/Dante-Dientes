using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public int playerCount = 0;

    public void SetPlayerCount(int newCount)
    {
        playerCount = newCount;
    }

    public void LoadGame(int difficulty)
    {
        StartCoroutine(LoadGameWithSettings(difficulty));
    }

    IEnumerator LoadGameWithSettings(int difficulty)
    {
        EnemyDifficulty.Instance.Difficulty = (EnemyDifficulty.Difficulties)difficulty;

        yield return new WaitForEndOfFrame();

        SceneManager.LoadScene(playerCount);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
