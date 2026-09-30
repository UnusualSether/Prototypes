using UnityEngine;

public static class MoneyHolder
{

    public static SoftCurrency total_soft_currency;


    public static void GainSoftCurrency(int amount)
    {


        total_soft_currency.total += amount;
    }




}
