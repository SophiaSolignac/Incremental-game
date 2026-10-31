using UnityEngine;

public class GameManager : MonoBehaviour
{
    private CurrencyHandler currency;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("GameManager started.");
        currency = new CurrencyHandler(5.0f, 2);
        Debug.Log($"Initial currency value: {currency.Currency.Mantissa} * 10^{currency.Currency.Exponent}");
        currency.OnCurrencyChanged.AddListener(() =>
        {
            Debug.Log($"Currency changed: {currency.Currency.Mantissa} * 10^{currency.Currency.Exponent}");
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Currency addedValue = new Currency(Random.Range(0.01f,10), (int)Random.Range(-2,20));
            Debug.Log($"Space key pressed. Adding {addedValue.Mantissa} * 10^{addedValue.Exponent} to currency.");
            currency.AddSplitValue(addedValue.Mantissa, addedValue.Exponent);
        }
    }
}
