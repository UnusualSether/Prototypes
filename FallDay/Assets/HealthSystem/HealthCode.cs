using UnityEngine;
using UnityEngine.UI;

public class HealthCode : MonoBehaviour
{

    public float HP, MaxHP, SizeX, SizeY, Damage, Heal;

    [SerializeField]
    private RectTransform healthBar;

    [SerializeField]
    private RectTransform nohealthBar;

    //Quantidade de Vida e Ttamanho da Barra
    public void CurrentHealth(float MaxHealth)
    {
        float CurrentHP = (HP / MaxHP) * SizeX;

        healthBar.sizeDelta = new Vector2(CurrentHP, SizeY);
        nohealthBar.sizeDelta = new Vector2(SizeX, SizeY);

    }

    //Calculo do HP
    public void ManualHP(float HPvalor)
    {
        HP += HPvalor;
        HP = Mathf.Clamp(HP, 0, MaxHP);

        CurrentHealth(MaxHP);
    }

    //Botões para curar e receber dano
    void Update()
    {
        CurrentHealth(MaxHP);
 
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
