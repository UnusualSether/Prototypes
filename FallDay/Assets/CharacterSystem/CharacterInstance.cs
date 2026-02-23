using JetBrains.Annotations;
using System;
using UnityEngine;

[Serializable]
public class CharacterInstance
{
    public CharacterData data;

    public string charName;


    public Sprite InGamesprite;

    public Sprite playerCardSprite;

    public int health;

    public int attack;

    public int speed;

    public int defense;

    
    public CharacterInstance(CharacterData data)
    {
        this.data = data;

        charName = data.CharacterName;

        InGamesprite = data.characterSprite;

        playerCardSprite = data.characterCard;

        //Set stats

        health = data.health;

        attack = data.attack;

        speed = data.speed;

        defense = data.defense;

    }
}


