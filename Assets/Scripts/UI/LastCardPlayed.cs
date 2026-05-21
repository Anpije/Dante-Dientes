using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class LastCardPlayed : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private RectTransform rectTransform;

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

        rectTransform.anchoredPosition = new Vector3(960, 1285, 0);
        rectTransform.rotation = Quaternion.identity;
        image.sprite = card.cardSprite;

        rectTransform.DOAnchorPos(new Vector2(960, 540), 0.15f);
        rectTransform.DOLocalRotate(new Vector3(0, 0, -22.5f), 0.15f);

        Invoke("EndDisplayCard", 1.5f);
    }

    void EndDisplayCard()
    {
        rectTransform.DOAnchorPos(new Vector2(960, -192), 0.15f);
        rectTransform.DOLocalRotate(new Vector3(0, 0, -180), 0.15f);
    }
}
