using UnityEngine;

public class OnScreenAnnouncement : MonoBehaviour
{
    [SerializeField] private GameObject announcement;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    public void SplashText(string newText, Color newColor)
    {
        if (!audioSource.isPlaying) audioSource.Play();
        GameObject news = Instantiate(announcement);
        news.transform.SetParent(transform);
        news.GetComponent<Announcement>().SplashText(newText, newColor);
    }

    public void SlideText(string newText, Color newColor)
    {
        if (!audioSource.isPlaying) audioSource.Play();
        GameObject news = Instantiate(announcement);
        news.transform.SetParent(transform);
        news.GetComponent<Announcement>().SlideText(newText, newColor);
    }
}
