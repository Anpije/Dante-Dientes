using UnityEngine.UI;
using UnityEngine;

public class CardFunctionByHolder : MonoBehaviour
{
    public enum Holders { Player, Enemy }
    public Holders currentHolder;

    public int toothProtection = 0;

    public void OnCardSelected()
    {
        EnemyHandEventManager enMan = FindFirstObjectByType<EnemyHandEventManager>();

        if (enMan.storedIDs.Count == 0) return;
        switch (enMan._Purpose)
        {
            case "selectEnemy":
                
                break;
            case "stealOrSwapCards":
                FindFirstObjectByType<EnemyHandEventManager>().selectedCards.Add(gameObject);
                FindFirstObjectByType<EnemyHandEventManager>().stealTheCard(gameObject);
                break;
            case "swapTeeth":
                FindFirstObjectByType<EnemyHandEventManager>().selectedCards.Add(gameObject);
                FindFirstObjectByType<EnemyHandEventManager>().stealTheCard(gameObject);
                break;
            case "affectTooth":
                FindFirstObjectByType<EnemyHandEventManager>().fApplyEffectToTooth(gameObject.GetComponent<CardVisual>(), transform.GetSiblingIndex());
                break;
        }
    }
}
