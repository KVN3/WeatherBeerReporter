using System;
using System.Collections.Generic;
using System.Text;

namespace BeerReporter.AzureLibrary.Helpers
{
    public interface ITemperatureConverter
    {
        float ToCelsius(float fahrenheit);
        float ToFahrenheit(float celsius);
    }

    public class TemperatureConverter : ITemperatureConverter
    {
        public float ToCelsius(float fahrenheit)
        {
            // C = (F – 32) * 5 / 9
            float celsius = (fahrenheit - 32) * 5 / 9;
            return celsius;
        }

        public float ToFahrenheit(float celsius)
        {
            // F = (C * 9) / 5 + 32;
            float fahrenheit = (celsius * 9) / 5 + 32;
            return fahrenheit;
        }
    }
}
