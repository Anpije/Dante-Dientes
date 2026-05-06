using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class OnScreenAnnouncement : MonoBehaviour
{
    [SerializeField] private Text announcement;
    [SerializeField] private RectTransform rectTransform;

    Vector3 defaultPos = new Vector3(458.7331f, 446.8618f, 0);

    [ContextMenu("Try Splash")]
    public void DoSplash()
    {
        SplashText("Test", Color.gold);
    }

    public void SplashText(string newText, Color newColor)
    {
        rectTransform.position = defaultPos;
        announcement.color = new Color(newColor.r, newColor.g, newColor.b, 0);
        announcement.text = newText;

        StartCoroutine("SplashTheText");
    }

    IEnumerator SplashTheText()
    {
        yield return new WaitForSeconds(0.5f);
        announcement.DOFade(1, 0.25f);

        Invoke("VanishText", 2);
        StopCoroutine("SplashTheText");
    }

    private void VanishText()
    {
        announcement.DOFade(0, 1);
    }

    public void SlideText(string newText, Color newColor)
    {
        rectTransform.localPosition = new Vector3(1920, 1080, 0);
        announcement.color = newColor;
        announcement.text = newText;

        rectTransform.DOMove(defaultPos, 0.25f);

        Invoke("SlideOutText", 1f);
    }

    private void SlideOutText()
    {
        rectTransform.DOMove(new Vector3(-458.7331f, 1080, 0), 0.25f);
    }
}
