using System;
using Unity.Mathematics;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Events;

public struct Currency
{
    public float Mantissa;
    public int Exponent;
    public Currency(float mantissa, int exponent)
    {
        Mantissa = mantissa;
        Exponent = exponent;
    }
}
public class CurrencyHandler
{
    // CONSTANTS
    private const int MAX_EXPO_DISTANCE = 12;

    // VARIABLES
    Currency _currency;

    // PROPERTIES
    public Currency Currency { get => _currency; }

    //EVENTS
    public UnityEvent OnCurrencyChanged = new UnityEvent();

    public CurrencyHandler(float mantissa, int exponent)
    {
        _currency.Mantissa = mantissa;
        _currency.Exponent = exponent;
    }

    public bool AddValue(float value)
    {
        return AddSplitValue(value, 0);
    }

    public bool AddSplitValue(Currency value)
    {
        return AddSplitValue(value.Mantissa, value.Exponent);
    }

    public bool AddSplitValue(float mantissa, int exponent)
    {
        (float,int) res = NormalizeValue(mantissa, exponent);
        mantissa = res.Item1;
        exponent = res.Item2;

        //Don't allow adding values that are too low in exponent
        if (_currency.Exponent - exponent > MAX_EXPO_DISTANCE) return true;

        //Replace the current value if the new value is too high in exponent
        if (exponent - _currency.Exponent > MAX_EXPO_DISTANCE)
        {
            if(mantissa < 0f)
            {
                return false;
            }
            _currency.Mantissa = mantissa;
            _currency.Exponent = exponent;
            return true;
        }
        float newCurrencyAmount = _currency.Mantissa;
        int newExponent = _currency.Exponent;
        if (_currency.Exponent > exponent)
        {
            mantissa *= Mathf.Pow(10, exponent - _currency.Exponent );
        }
        else if (_currency.Exponent < exponent)
        {
            newCurrencyAmount = _currency.Mantissa * Mathf.Pow(10, _currency.Exponent - exponent);
            newExponent = exponent;
        }
        newCurrencyAmount = newCurrencyAmount + mantissa;
        if(newCurrencyAmount < 0f)
        {
            return false;
        }
        _currency.Mantissa = newCurrencyAmount;
        _currency.Exponent = newExponent;
        NormalizeValue(ref _currency);
        UpdateValue();
        return true;
    }
    private void NormalizeValue(ref Currency currency)
    {
        (float,int) val = NormalizeValue(currency.Mantissa, currency.Exponent);
        currency.Mantissa = val.Item1;
        currency.Exponent = val.Item2;
    }
    private (float,int) NormalizeValue(float mantissa, int exponent)
    {
        if(mantissa == 0f)
        {
            return (0f, 0);
        }

        int divider = Mathf.FloorToInt(Mathf.Log10(Mathf.Abs(mantissa)));

        if(divider == 0)
        {
            return (mantissa, exponent);
        }
        mantissa *= Mathf.Pow(0.1f, divider);
        exponent += divider;
        return (mantissa, exponent);
    }

    private void UpdateValue()
    {
        OnCurrencyChanged.Invoke();
    }
}
