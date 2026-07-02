using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class LastCardPlayed : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Text text;
    [SerializeField] private RectTransform rectTransform;

    private float scrnWdth;
    private float scrnHght;

    void OnEnable()
    {
        EnemyBehaviour.OnPlayCard += DisplayCard;
    }

    void OnDisable()
    {
        EnemyBehaviour.OnPlayCard -= DisplayCard;
    }

    void DisplayCard(CardData card)
    {
        DOTween.Kill(this);

        rectTransform.anchoredPosition = new Vector3(scrnWdth / 2, scrnHght + (scrnHght / 3), 0);
        rectTransform.rotation = Quaternion.identity;
        image.sprite = card.cardSprite;
        text.text = card.cardName;

        rectTransform.DOAnchorPos(new Vector2(scrnWdth / 2, scrnHght/ 2), 0.15f);
        rectTransform.DOLocalRotate(new Vector3(0, 0, -22.5f), 0.15f);

        Invoke("EndDisplayCard", 1.5f);
    }

    void EndDisplayCard()
    {
        rectTransform.DOAnchorPos(new Vector2(scrnWdth / 2, scrnHght / -3), 0.15f);
        rectTransform.DOLocalRotate(new Vector3(0, 0, -90), 0.15f);
    }

    void FixedUpdate()
    {
        scrnHght = Screen.height;
        scrnWdth = Screen.width;
    }
}
