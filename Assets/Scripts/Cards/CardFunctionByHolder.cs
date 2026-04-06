using UnityEngine.UI;
using UnityEngine;

public class CardFunctionByHolder : MonoBehaviour
{
    public enum Holders { Player, Enemy }
    public Holders currentHolder;

    public int toothProtection = 0;

    void OnEnable()
    {
        TurnManager.PlayerStartTurn += EnablePlayerCards;
        PlayerActions.OnEndTurn += DisablePlayerCards;
    }

    void OnDisable()
    {
        TurnManager.PlayerStartTurn -= EnablePlayerCards;
        PlayerActions.OnEndTurn -= DisablePlayerCards;
    }

    void EnablePlayerCards()
    {
        if (currentHolder == Holders.Player)
        {
            GetComponentInChildren<Button>().interactable = true;
        }
    }

    void DisablePlayerCards()
    {
        if (currentHolder == Holders.Player)
        {
            GetComponentInChildren<Button>().interactable = false;
        }
    }

    public void OnCardSelected()
    {
        EnemyHandEventManager enMan = FindFirstObjectByType<EnemyHandEventManager>();

        if (enMan.storedIDs.Count != 0)
        {
            switch (enMan._Purpose)
            {
                case "selectEnemy":
                
                    return;
                case "stealOrSwapCards":
                    FindFirstObjectByType<EnemyHandEventManager>().selectedCards.Add(gameObject);
                    FindFirstObjectByType<EnemyHandEventManager>().stealTheCard(gameObject);
                    return;
                case "swapTeeth":
                    FindFirstObjectByType<EnemyHandEventManager>().selectedCards.Add(gameObject);
                    FindFirstObjectByType<EnemyHandEventManager>().stealTheCard(gameObject);
                    return;
                case "affectEnemyTooth":
                    FindFirstObjectByType<EnemyHandEventManager>().fApplyEffectToTooth(gameObject.GetComponent<CardVisual>(), transform.GetSiblingIndex());
                    return;
                case "affectPlayerTooth":
                    FindFirstObjectByType<EnemyHandEventManager>().fApplyEffectToPlayer(gameObject.GetComponent<CardVisual>(), transform.GetSiblingIndex());
                    return;
            }
        }

        CardVisual card = GetComponent<CardVisual>();
        switch (card.cardData.cardType)
        {
            case CardType.HealthyTooth:
                FindFirstObjectByType<PlayerActions>().PlaceTooth(gameObject);
                return;
            case CardType.Protective:
                FindFirstObjectByType<EnemyHandEventManager>().effectToApply = 1;
                FindFirstObjectByType<EnemyHandEventManager>().OpenPanelAs("affectPlayerTooth");
                FindFirstObjectByType<PlayerActions>().playedCard = gameObject;
                return;
            case CardType.Harmful:
                FindFirstObjectByType<EnemyHandEventManager>().effectToApply = -1;
                FindFirstObjectByType<EnemyHandEventManager>().OpenPanelAs("affectEnemyTooth");
                FindFirstObjectByType<PlayerActions>().playedCard = gameObject;
                return;
            case CardType.Treatment:
                FuntionByDescription(card.cardData.description);
                FindFirstObjectByType<PlayerActions>().playedCard = gameObject;
                return;
        }
    }

    void FuntionByDescription(string description)
    {
        switch (description)
        {
            case "Immunización Total":

                break;
            case "Cambio de Turno":

                break;
            case "Emergencia Dental":

                break;
            case "Revisión Sorpresa":
                
                break;
            case "Refuerzo de Esmalte":
                break;
            case "Intercambio Carta":
                break;
            case "Intercambio Diente":
                
                break;
            case "Tratamiento Intensivo":

                break;
            case "Confusión Clínica":
                FindFirstObjectByType<EnemyHandEventManager>().fEchangeAllCards();
                break;
            default:
                Debug.LogError(gameObject.name + "'s Description is not a valid treatment");
                break;
        }
    }
}
