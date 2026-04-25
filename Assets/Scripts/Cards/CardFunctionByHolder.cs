using UnityEngine.UI;
using UnityEngine;

public class CardFunctionByHolder : MonoBehaviour
{
    public enum Holders { Player, Enemy }
    public Holders currentHolder;

    [SerializeField] Text protectionDisplay;
    [SerializeField] private int ToothProtection;
    public int toothProtection
    {
        get { return ToothProtection; }
        set
        {
            ToothProtection = value;
            OnProtectionChanged();
        }
    }

    void OnProtectionChanged()
    {
        if (ToothProtection == 0)
            protectionDisplay.text = "";
        else
            protectionDisplay.text = ToothProtection.ToString();
    }

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
                case "stealCard":
                    FindFirstObjectByType<EnemyHandEventManager>().selectedCards.Add(gameObject);
                    FindFirstObjectByType<EnemyHandEventManager>().stealTheCard(gameObject);
                    return;
                case "swapCards":
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
                case "forceDiscard":
                    FindFirstObjectByType<EnemyHandEventManager>().selectedCards.Add(gameObject);
                    FindFirstObjectByType<EnemyHandEventManager>().ForceDiscard();
                    return;
                case "immunizePlayerTooth":
                    FindFirstObjectByType<EnemyHandEventManager>().selectedCards.Add(gameObject);
                    FindFirstObjectByType<EnemyHandEventManager>().fImmunizeThisCard();
                    return;
                case "replaceTooth":
                    FindFirstObjectByType<EnemyHandEventManager>().selectedCards.Add(gameObject);
                    FindFirstObjectByType<EnemyHandEventManager>().fReplacePlayerTooth(FindFirstObjectByType<PlayerActions>().playedCard);
                    return;
                case "discardPlayer":
                    FindFirstObjectByType<EnemyHandEventManager>().DiscardSelected(gameObject);
                    return;
            }
        }

        CardVisual card = GetComponent<CardVisual>();
        switch (card.cardData.cardType)
        {
            case CardType.HealthyTooth:
                if (FindFirstObjectByType<CardSystem>().teethInPlay.Count != 4)
                    FindFirstObjectByType<PlayerActions>().PlaceTooth(gameObject);
                else
                {
                    FindFirstObjectByType<PlayerActions>().playedCard = gameObject;
                    FindFirstObjectByType<EnemyHandEventManager>().OpenPanelAs("replaceTooth");
                }
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
                FindFirstObjectByType<PlayerActions>().playedCard = gameObject;
                FuntionByDescription(card.cardData.cardName);
                return;
        }
    }

    void FuntionByDescription(string description)
    {
        switch (description)
        {
            case "Immunización Total":
                FindFirstObjectByType<EnemyHandEventManager>().OpenPanelAs("immunizePlayerTooth");
                break;
            case "Cambio de Turno":
                FindFirstObjectByType<EnemyHandEventManager>().OpenPanelAs("selectEnemy");
                break;
            case "Emergencia Dental":
                FindFirstObjectByType<EnemyHandEventManager>().effectToApply = 1;
                FindFirstObjectByType<EnemyHandEventManager>().OpenPanelAs("affectPlayerTooth");
                break;
            case "Revisión Sorpresa":
                FindFirstObjectByType<EnemyHandEventManager>().OpenPanelAs("forceDiscard");
                break;
            case "Refuerzo de Esmalte":
                FindFirstObjectByType<EnemyHandEventManager>().effectToApply = 1;
                FindFirstObjectByType<EnemyHandEventManager>().OpenPanelAs("affectPlayerTooth");
                break;
            case "Intercambiar Cartas":
                FindFirstObjectByType<EnemyHandEventManager>().OpenPanelAs("swapCards");
                break;
            case "Robar Carta":
                FindFirstObjectByType<EnemyHandEventManager>().OpenPanelAs("stealCard");
                break;
            case "Intercambiar Dientes":
                FindFirstObjectByType<EnemyHandEventManager>().OpenPanelAs("swapTeeth");
                break;
            case "Tratamiento Intensivo":
                FindFirstObjectByType<EnemyHandEventManager>().effectToApply = 0;
                FindFirstObjectByType<EnemyHandEventManager>().OpenPanelAs("affectPlayerTooth");
                break;
            case "Confusión Clínica":
                FindFirstObjectByType<EnemyHandEventManager>().fEchangeAllCards(null, null);
                break;
            default:
                Debug.LogError(gameObject.name + "'s Description is not a valid treatment");
                break;
        }
    }
}
