using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HealthCode : MonoBehaviour
{
    private float HP;
    public float MaxHP, SizeX, SizeY, Heal;
    [SerializeField]
    private RectTransform healthBar;
    [SerializeField]
    private RectTransform nohealthBar;

    [SerializeField]private UIToggleManager UIToggleManager;

    private void OnEnable()
    {
        GameHandler.PlayerTookDamage += OnDemegedTest;
        GameHandler.PlayerTookDamage += TakeDamage;
    }
    private void OnDisable()
    {
        GameHandler.PlayerTookDamage -= OnDemegedTest;
        GameHandler.PlayerTookDamage -= TakeDamage;
    }
    void Start()
    {
        //Debug.Log("Helth start");

        Debug.Log("HP, MaxHP, SizeX, SizeY, Damage, Heal: " + HP + ", " + MaxHP + ", " + SizeX + ", " + SizeY + ", " + Heal);
        HP = MaxHP;
    }

    void Update()
    {
        if (HP <= 0)
        {
            UIToggleManager.ToggleUI();
        }
        //CurrentHealth(MaxHP);
    }

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

    /*
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
    */
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

    public void OnDemegedTest(float d)
    {
        Debug.Log("HealthCode dameged Called");
    }
    public void TakeDamage(float Demage)
    {
        if(HP - Demage <= 0)
        {
            HP = 0;
            HealthDroppedToZero?.Invoke();
        }
        else
        {
            HP -= Demage;
        }
        CurrentHealth(MaxHP);
    }


    //Botões para curar e receber dano
    
}
