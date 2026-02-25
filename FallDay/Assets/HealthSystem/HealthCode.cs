using UnityEngine;
using UnityEngine.UI;

public class HealthCode : MonoBehaviour
{

    public float HP, MaxHP, SizeX, SizeY, Damage, Heal;

    [SerializeField]
    private RectTransform healthBar;

    //Quantidade de Vida
    public void CurrentHealth(float MaxHealth)
    {
        float CurrentHP = (HP / MaxHP) * SizeX;

        healthBar.sizeDelta = new Vector2(CurrentHP, SizeY);
    }

    //
    public void ManualHP(float HPvalor)
    {
        HP += HPvalor;
        HP = Mathf.Clamp(HP, 0, MaxHP);

        CurrentHealth(MaxHP);
    }

    void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            ManualHP(-Damage);
        }
        if (Input.GetKeyDown("d"))
        {
            ManualHP(Heal);
        }
    }

}
