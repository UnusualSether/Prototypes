using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public void OnEnable()
    {
        PlayerInstance.PlayerGainedCurrency += MoneyHolder.PlayerGainedSoftCurrencyDuringRun;
    }

    public void OnDisable()
    {
        PlayerInstance.PlayerGainedCurrency -= MoneyHolder.PlayerGainedSoftCurrencyDuringRun;
    }
}
