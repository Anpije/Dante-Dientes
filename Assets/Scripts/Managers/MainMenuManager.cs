using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class MainMenuManager : MonoBehaviour
{
    public int playerCount = 0;
    
    [SerializeField] Image blackScreen;


    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sfxButton;
    [SerializeField] private AudioClip sfxStartGame;

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
        audioSource.clip = sfxStartGame;
        audioSource.Play();
        blackScreen.raycastTarget = true;
        blackScreen.DOFade(1, 1.5f);
        
        EnemyDifficulty.Instance.Difficulty = (EnemyDifficulty.Difficulties)difficulty;

        yield return new WaitForSeconds(2);

        SceneManager.LoadScene(playerCount);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayButtonSound()
    {
        audioSource.clip = sfxButton;
        audioSource.Play();
    }

    public void OpenLink(string link)
    {
        Application.OpenURL(link);
    }
}
