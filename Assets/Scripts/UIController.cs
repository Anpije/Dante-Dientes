using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("Botones")]
    public Button buttonRobar;
    public Button buttonMazo;
    public Button buttonDescartar;

    [Header("Texto Informativo")]
    public TextMeshProUGUI infoText;

    private void Start()
    {
        // Verificar que todo está asignado
        if (buttonRobar == null) Debug.LogError("ButtonRobar no asignado");
        if (buttonMazo == null) Debug.LogError("ButtonMazo no asignado");
        if (buttonDescartar == null) Debug.LogError("buttonDescartar no asignado");
        if (infoText == null) Debug.LogError("InfoText no asignado");

        // Asignar funciones a los botones
        buttonRobar.onClick.AddListener(RobarCarta);
        buttonMazo.onClick.AddListener(MostrarInfo);
        buttonDescartar.onClick.AddListener(DescartarPrimera);
    }

    private void RobarCarta()
    {
        if (CardSystem.Instance == null)
        {
            infoText.text = "Error: CardSystem no encontrado";
            return;
        }

        if (CardSystem.Instance.hand.Count >= 4) return;

        GameObject carta = CardSystem.Instance.DrawCard();
        if (carta != null)
        {
            CardVisual visual = carta.GetComponent<CardVisual>();
            infoText.text = $"Robada: {visual.cardData.cardName}";
        }
        else
        {
            infoText.text = "No hay más cartas en el mazo";
        }
    }

    private void MostrarInfo()
    {
        if (CardSystem.Instance == null) return;

        CardSystem.Instance.PrintDeckInfo();

        // Mostrar información en UI
        string info = $"Mazo: {CardSystem.Instance.GetDeckCount()} cartas\n";
        info += $"Mano: {CardSystem.Instance.GetHandCount()} cartas\n\n";

        infoText.text = info;
    }

    private void DescartarPrimera()
    {
        if (CardSystem.Instance == null) return;

        var cartas = CardSystem.Instance.GetAllCardsInHand();
        if (cartas.Count > 0)
        {
            GameObject carta = cartas[0].gameObject;
            string nombre = carta.GetComponent<CardVisual>().cardData.cardName;
            CardSystem.Instance.DiscardCard(carta);
            infoText.text = $"Descartada: {nombre}";
        }
        else
        {
            infoText.text = "No hay cartas en la mano";
        }
    }
}