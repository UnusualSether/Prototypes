using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthCode : MonoBehaviour
{

    public float HP, MaxHP, SizeX, SizeY, Damage, Heal;

    [SerializeField]
    private RectTransform healthBar;
    [SerializeField]
    private RectTransform nohealthBar;

    #region Events

    public event Action HealthDroppedToZero;

    #endregion

    //Quantidade de Vida e Ttamanho da Barra
    public void CurrentHealth(float MaxHealth)
    {
        float CurrentHP = (HP / MaxHP) * SizeX;

        healthBar.sizeDelta = new Vector2(CurrentHP, SizeY);
        nohealthBar.sizeDelta = new Vector2(SizeX, SizeY);

    }

    //Calculo do HP
    public void ManualHP(float newlyRecievedValue)
    {
        HP += newlyRecievedValue;
        HP = Mathf.Clamp(HP, 0, MaxHP);

        if (DeathCheck(HP))
        {
            Debug.Log("[HealthSystem] The player died!!! But I'm not going to do anything myself, I'm just gonna spread the word is all.");
            HealthDroppedToZero?.Invoke();
        }

        CurrentHealth(MaxHP);
    }

    private bool DeathCheck(float currentHP)
    {
        if (currentHP <= 0)
        {
            return true;
        }

        else
        {
            return false;
        }
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
