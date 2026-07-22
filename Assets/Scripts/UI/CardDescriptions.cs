using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

public class CardDescriptions : MonoBehaviour
{
    [SerializeField] private Text textBox;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image background;
    [SerializeField] private GraphicRaycaster raycaster1;
    [SerializeField] private GraphicRaycaster raycaster2;
    [SerializeField] private EventSystem eventSystem;

    private string desc;
    private IEnumerator currentRoutine;

    [SerializeField] CardData testCardData;
    private bool testing = true;

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

    void Start()
    {
        DisplayDescription(testCardData);
    }

    void FixedUpdate()
    {
        rectTransform.anchoredPosition = Mouse.current.position.ReadValue();

        if (testing) return;
        if (!IsPointerOverSpecificElement())
            RemoveDescription();
    }

    private bool IsPointerOverSpecificElement()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster1.Raycast(pointerData, results);
        foreach (RaycastResult raycastResult in results)
        {
            if (raycastResult.gameObject.GetComponent<CardVisual>() != null)
            {
                return true;
            }
        }
        raycaster2.Raycast(pointerData, results);
        foreach (RaycastResult raycastResult in results)
        {
            if (raycastResult.gameObject.GetComponent<CardVisual>() != null)
            {
                return true;
            }
        }
        return false;
    }

    private void DisplayDescription(CardData data)
    {
        desc = "";
        if (data.cardType != CardType.Treatment)
        {
            switch (data.cardColor.ToString())
            {
                case "Red":
                    desc = "<b><color=#FF6753>" + data.cardName + "</color></b>\n" + data.description;
                    break;
                case "Green":
                    desc = "<b><color=#00FF00>" + data.cardName + "</color></b>\n" + data.description;
                    break;
                default:
                    desc = "<b><color=" + data.cardColor.ToString() + ">" + data.cardName + "</color></b>\n" + data.description;
                    break;
            }
        }
        else
            desc = "<b><color=cyan>" + data.cardName + "</color></b>\n" + data.description;

        if (data.cardColor == ToothColor.Rainbow)
            desc = "<b><color=magenta>" + data.cardName + "</color></b>\n" + data.description;

        if (textBox.text == desc) return; 
        textBox.text = desc;
        textBox.DOFade(1, 0.15f);
        background.DOFade(0.75f, 0.15f);
        currentRoutine = WriteDescription();
        StartCoroutine(currentRoutine);
    }

    IEnumerator WriteDescription()
    {
        textBox.text = "";
        yield return new WaitForEndOfFrame();
        textBox.text = desc;
        if (testing)
        {
            yield return new WaitForSeconds(0.15f);
            testing = false;
        }
        StopCoroutine(currentRoutine);
    }

    private void RemoveDescription()
    {
        textBox.text = "";
        textBox.DOFade(0, 0.15f);
        background.DOFade(0, 0.15f);
    }
}
