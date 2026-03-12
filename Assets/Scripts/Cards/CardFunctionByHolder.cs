using UnityEngine.UI;
using UnityEngine;

public class CardFunctionByHolder : MonoBehaviour
{
    public enum Holders { Player, Enemy }
    public Holders currentHolder;

    public void OnCardSelected()
    {
        if (currentHolder != Holders.Player || !FindFirstObjectByType<EnemyHandEventManager>().takeCards)
        {
            if (FindFirstObjectByType<EnemyHandEventManager>().storedIDs.Count != 0)
            {
                FindFirstObjectByType<EnemyHandEventManager>().selectedCards.Add(gameObject);
                FindFirstObjectByType<EnemyHandEventManager>().stealTheCard(gameObject);
            }
        }
    }
}
