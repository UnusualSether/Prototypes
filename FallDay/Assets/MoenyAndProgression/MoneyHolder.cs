using UnityEngine;

public static class MoneyHolder
{

    public static SoftCurrency total_soft_currency = new SoftCurrency();



    private static void GainSoftCurrency(int amount)
    {


        total_soft_currency.total += amount;
    }

    private static void LoseSoftCurrency(int amount)
    {
        total_soft_currency.total -= amount;
    }

    public static bool Purchase(BuyBill request)
    {
        if (request.charged_soft_currency.total > total_soft_currency.total)
        {
            return false;
        }

        else
        {
            ProcessPayment(request);

            return true;
        }


    }
    public static void PlayerGainedSoftCurrencyDuringRun(int amount)
    {

        GainSoftCurrency(amount);

    }

    private static void ProcessPayment(BuyBill request)
    {
        LoseSoftCurrency(request.charged_soft_currency.total);


    }

}

public class BuyBill
{


    public SoftCurrency charged_soft_currency = new SoftCurrency();

}
