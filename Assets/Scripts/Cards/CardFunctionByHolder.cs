using UnityEngine.UI;
using UnityEngine;

public class CardFunctionByHolder : MonoBehaviour
{
    public enum Holders { Player, Enemy }
    public Holders currentHolder;

    public void OnCardSelected()
    {
        if (currentHolder != Holders.Player)
        {
            FindFirstObjectByType<EnemyHandEventManager>().selectedCards.Add(gameObject);
            FindFirstObjectByType<EnemyHandEventManager>().stealTheCard(gameObject);
        }
    }
}
