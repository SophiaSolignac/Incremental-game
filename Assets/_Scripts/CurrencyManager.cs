using System.Collections;
using UnityEditor;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    private CurrencyHandler currency;
    private bool addCoroutineOn = true;
    private float timeBetweenAdding = 0.5f;

    public delegate bool TryBuyDelegate(Currency cost);
    public static event TryBuyDelegate OnTryBuy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("GameManager started.");
        currency = new CurrencyHandler(5.0f, 2);
        Debug.Log($"Initial currency value: {currency.Currency.Mantissa} * 10^{currency.Currency.Exponent}");

        OnTryBuy += TryBuy;
        //StartCoroutine(PeriodicCurrencyAddition());
        currency.AddValue(100);
        currency.AddValue(-1000);
        currency.AddValue(0.01f);
        currency.AddValue(-0.01f);
    }
    private bool TryBuy(Currency cost)
    {
        return currency.AddSplitValue(-cost.Mantissa, cost.Exponent);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Currency cost = new Currency(Random.Range(0.01f, 10), (int)Random.Range(1, 5));
            bool success = OnTryBuy?.Invoke(cost) ?? false;
            if(success)
            {
                Debug.Log($"Purchase successful! Cost: {cost.Mantissa} * 10^{cost.Exponent}");
            }
            else
            {
                Debug.Log($"Purchase failed! Cost: {cost.Mantissa} * 10^{cost.Exponent}");
            }
            EditorApplication.isPaused = false; // Stop the game after one purchase attempt for testing purposes
        }
    }

    private IEnumerator PeriodicCurrencyAddition()
    {
        while (addCoroutineOn)
        {
            yield return new WaitForSeconds(timeBetweenAdding);
            currency.AddSplitValue(new Currency(Random.Range(-1000, 1000), (int)Random.Range(0, 0)));
            Debug.Log($"Added currency. New value: {currency.Currency.Mantissa} * 10^{currency.Currency.Exponent}");
        }
    }
}
