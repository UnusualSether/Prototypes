using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStalkerTrinket", menuName = "Trinket/StalkerTrinket")]
[Serializable]
public class StalkerTrinket : Trinket, IDamageFilterTrinket, IVisualClassApplyingTrinket
{

    public List<Zombie> stalked_list = new List<Zombie>();


    public string VisualClass() => visual_class;

    public int stalked_damage_bonus;

    public Texture status_icon;

    private string visual_class = "stalked";

    public void ApplyVisual(GameDisplay.ZombieDisplay target)
    {
        Debug.Log($"{trinket_name} : Added {visual_class} to a zombie!");

        VisualElementManipulation.AddStatusIcon(target.displayElement, status_icon, "stalked");
    }

    public void RemoveVisual()
    {

    }

    public int ModifiedDamage(int damage, Zombie target)
    {
        if (target.phase == Zombie.ZombiePhase.Far)
        {

            if (!stalked_list.Contains(target))
            {
                stalked_list.Add(target);

                ApplyVisual(GameDisplay.ZombieToDisplay(target));
            }

            return 0;


        }

        else
        {
            if (stalked_list.Contains(target) && target.phase != Zombie.ZombiePhase.Far)
            {
                return damage + stalked_damage_bonus;
            }

            else
            {
                return damage;
            }
        }




    }


}
