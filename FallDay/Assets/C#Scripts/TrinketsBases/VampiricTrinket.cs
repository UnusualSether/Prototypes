using UnityEngine;


[CreateAssetMenu(fileName = "NewVampiricTrinket", menuName = "Trinket/Vampiric Trinket")]
public class VampiricTrinket : OnKillTrinket
{

    public int health_gain;

    public override void EventTrigger(TrinketEventType called_event_type, PlayerInstance instance_to_affect)
    {
        if (called_event_type == TrinketEventType.OnKill)
        {
            instance_to_affect.GainHealth(health_gain);
        }
    }


}
