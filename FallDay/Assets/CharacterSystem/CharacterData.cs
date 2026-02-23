using UnityEngine;

[CreateAssetMenu(menuName = "Characters/CharacterData")]
public class CharacterData : ScriptableObject
{
    //Visual Elements

    public string CharacterName;

    public string CharacterDescription;

    public Sprite characterSprite;

    public Sprite characterCard;

    //internal values

    public int health;

    public int attack;

    public int speed;

    public int defense;

}
