using UnityEngine;


[CreateAssetMenu(fileName = "NewExplorersTrinket", menuName = "Trinket/Explorer's Trinket")]
public class ExplorersTrinket : RoomClearTrinket
{

    public int soft_currency_gained;

    public override void EventTrigger(TrinketEventType called_event_type, PlayerInstance instance_to_affect)
    {
        if (called_event_type == TrinketEventType.OnRoomComplete)
        {
            instance_to_affect.GainSoftCurrency(soft_currency_gained);
        }
    }


}
