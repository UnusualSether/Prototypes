using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HealthDisplay : MonoBehaviour
{

    public GameHandler handler;

    public PlayerInstance player_instance => handler.player;


    
    private float HP => player_instance.current_hp;
    private float MaxHP => player_instance.stats.max_hp;


    public float SizeX, SizeY, Heal;
    [SerializeField]
    private RectTransform healthBar;
    [SerializeField]
    private RectTransform nohealthBar;
    public bool debugisOn = false;

    [SerializeField]private UIToggleManager UIToggleManager;

    void Start()
    {
        //Debug.Log("Helth start");

        if(debugisOn) Debug.Log("HP, MaxHP, SizeX, SizeY, Damage, Heal: " + HP + ", " + MaxHP + ", " + SizeX + ", " + SizeY + ", " + Heal);

        GameHandler.PlayerTookDamage += CurrentHealth;
    }

    void Update()
    {
        CurrentHealth(0);
    }

    #region Events

    public event Action HealthDroppedToZero;

    #endregion

    //Quantidade de Vida e Ttamanho da Barra
    public void CurrentHealth(float damage_taken)
    {
        float CurrentHP_to_bar_length = (HP / MaxHP) * SizeX;

        healthBar.sizeDelta = new Vector2(CurrentHP_to_bar_length, SizeY);
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
    


    //Botões para curar e receber dano
    
}
