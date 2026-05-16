using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup group;
    [SerializeField] private RectTransform panel;
    [SerializeField] private Button[] buttons;

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
        usable = true;
        Invoke("SetButtons", 0.45f);
        Invoke("StopTime", 0.45f);
    }

    private void StopTime()
    {
        Time.timeScale = 0;
    }

    private void ClosePauseMenu()
    {
        Time.timeScale = 1;
        panel.DOAnchorPosX(-658, 0.15f);
        group.DOFade(0, 0.45f);
        group.blocksRaycasts = false;
        group.interactable = false;
        usable = false;
        Invoke("SetButtons", 0.45f);
    }

    bool usable = false;
    private void SetButtons()
    {
        buttons[0].interactable = usable;
        buttons[1].interactable = usable;
    }

    public void ExitGame()
    {
        Time.timeScale = 1;
        FindFirstObjectByType<TurnManager>().ExitGame();
    }
}
