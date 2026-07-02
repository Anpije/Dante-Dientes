using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class Announcement : MonoBehaviour
{
    [SerializeField] private Text announcement;
    [SerializeField] private RectTransform rectTransform;

    Vector3 defaultPos = new Vector3(Screen.width / 2, Screen.height / 2, 0);

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

        Invoke("VanishText", 1.5f);
        StopCoroutine("SplashTheText");
    }

    private void VanishText()
    {
        announcement.DOFade(0, 1);
        Invoke("DeleteThis", 2);
    }

    public void SlideText(string newText, Color newColor)
    {
        rectTransform.localPosition = new Vector3(Screen.width, Screen.height, 0);
        announcement.color = newColor;
        announcement.text = newText;

        rectTransform.DOMove(defaultPos, 0.25f);

        Invoke("SlideOutText", 1.5f);
    }

    private void SlideOutText()
    {
        rectTransform.DOMove(new Vector3((Screen.width / 2) * -1, Screen.height, 0), 0.25f);
        Invoke("DeleteThis", 1);
    }

    private void DeleteThis()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        float scnWdth = Screen.width;
        float scnHeight = Screen.height;
        defaultPos = new Vector3(scnWdth / 2, scnHeight / 2, 0);
        Debug.LogWarning("Screen Width from announcement = " + scnWdth);
    }
}
