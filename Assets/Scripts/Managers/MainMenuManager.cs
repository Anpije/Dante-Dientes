using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class MainMenuManager : MonoBehaviour
{
    public int playerCount = 0;
    
    [SerializeField] Image blackScreen;


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
        blackScreen.DOFade(1, 1.5f);
        
        EnemyDifficulty.Instance.Difficulty = (EnemyDifficulty.Difficulties)difficulty;

        yield return new WaitForSeconds(2);

        SceneManager.LoadScene(playerCount);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
