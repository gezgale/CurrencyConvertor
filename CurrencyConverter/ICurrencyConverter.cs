using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConvertor.CurrencyConverter
{
    public interface ICurrencyConverter
    {
        /// <summary>
        /// Clears any prior configuration.
        /// </summary>
        void ClearConfiguration();
        /// <summary>
        /// Updates the configuration. Rates are inserted or replaced internally.
        /// </summary>
        /// <param name="conversionRates"></param>
        void UpdateConfiguration(IEnumerable<Tuple<string, string, double>> conversionRates);
        /// <summary>
        /// Converts the specified amount to the desired currency.
        /// </summary>
        /// <param name="fromCurrency"></param>
        /// <param name="toCurrency"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        double Convert(string fromCurrency, string toCurrency, double amount);
    }
}
