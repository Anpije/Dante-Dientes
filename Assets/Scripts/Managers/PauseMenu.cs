using UnityEngine;
using DG.Tweening;

public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup group;
    [SerializeField] private RectTransform panel;

    private bool IsPaused = false;
    public bool isPaused
    {
        get { return IsPaused; }
        set 
        { 
            IsPaused = value; 
            if (value)
                OpenPauseMenu();
            else
                ClosePauseMenu();
        }
    }

    private void OpenPauseMenu()
    {
        group.blocksRaycasts = true;
        group.interactable = true;
        group.DOFade(1, 0.15f);
        panel.DOAnchorPosX(0, 0.45f);
        Invoke("StopTime", 1);
    }

    private void StopTime()
    {
        Time.timeScale = 0;
    }

    private void ClosePauseMenu()
    {
        StartTime();
        panel.DOAnchorPosX(-658, 0.15f);
        group.DOFade(0, 0.45f);
        group.blocksRaycasts = false;
        group.interactable = false;
    }

    private void StartTime()
    {
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        ClosePauseMenu();
        FindFirstObjectByType<TurnManager>().ExitGame();
    }
}
