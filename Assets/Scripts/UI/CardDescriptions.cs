using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.InputSystem;

public class CardDescriptions : MonoBehaviour
{
    [SerializeField] private Text[] textBoxes;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image background;

    void OnEnable()
    {
        CardVisual.OnCursorOverCard += DisplayDescription;
        CardVisual.OnCursorExitCard += RemoveDescription;
    }

    void OnDisable()
    {
        CardVisual.OnCursorOverCard -= DisplayDescription;
        CardVisual.OnCursorExitCard -= RemoveDescription;
    }

    void Update()
    {
        rectTransform.anchoredPosition = Mouse.current.position.ReadValue();
    }

    private void DisplayDescription(CardVisual data)
    {
        string desc = "";
        if (data.cardData.cardType != CardType.Treatment)
            desc = "<color=" + data.cardData.cardColor.ToString() + ">" + data.cardData.cardName + "</color>\n" + data.cardData.description;
        else
            desc = "<color=cyan>" + data.cardData.cardName + "</color>\n" + data.cardData.description;

        if (data.cardData.cardColor == ToothColor.Rainbow)
            desc = "<color=magenta>" + data.cardData.cardName + "</color>\n" + data.cardData.description;

        textBoxes[0].text = desc;
        textBoxes[1].text = desc;
        textBoxes[1].DOFade(1, 0.15f);
        background.DOFade(0.75f, 0.15f);
    }

    private void RemoveDescription()
    {
        textBoxes[0].text = "";
        textBoxes[1].text = "";
        textBoxes[1].DOFade(0, 0.15f);
        background.DOFade(0, 0.15f);
    }
}
