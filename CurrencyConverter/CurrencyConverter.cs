using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConvertor.CurrencyConverter
{
    public class CurrencyConverter : ICurrencyConverter
    {
        /// <summary>
        /// The reason for using ConcurrentDictionary<String, Dictionary<String, Double>> conversionRates 
        /// in the provided CurrencyConverter class is to store and manage the currency conversion rates 
        /// efficiently and in a thread-safe manner. 
        /// </summary>
        private readonly ConcurrentDictionary<string, Dictionary<string, double>> conversionRates
        = new ConcurrentDictionary<string, Dictionary<string, double>>
        {
            //Path 1 From USD To EUR
            ["USD"] = new Dictionary<string, double> { ["CAD"] = 1.34 },
            ["CAD"] = new Dictionary<string, double> { ["GBP"] = 0.58 },
            ["GBP"] = new Dictionary<string, double> { ["EUR"] = 0.43 },
            ["EUR"] = new Dictionary<string, double> { ["USD"] = 1.16 },

            ////Path 2 From USD To EUR
            //["USD"] = new Dictionary<string, double> { ["JGD"] = 0.7 },
            //["JGD"] = new Dictionary<string, double> { ["EUR"] = 0.3 },

            //Path 3 From USD To EUR
            //["USD"] = new Dictionary<string, double> { ["PLK"] = 7.2 },
            //["PLK"] = new Dictionary<string, double> { ["CVF"] = 33.98 },
            //["CVF"] = new Dictionary<string, double> { ["BDE"] = 89.32 },
            //["BDE"] = new Dictionary<string, double> { ["TVS"] = 12.11 },
            //["TVS"] = new Dictionary<string, double> { ["QTA"] = 9.85 },
            //["QTA"] = new Dictionary<string, double> { ["EUR"] = 7.11 },
        };

        //private readonly ConcurrentDictionary<string, Dictionary<string, double>> conversionRates
        //= new ConcurrentDictionary<string, Dictionary<string, double>>
        //{
        //    ["USD"] = new Dictionary<string, double> { ["CAD"] = 1.34, ["EUR"] = 0.86 },
        //    ["CAD"] = new Dictionary<string, double> { ["USD"] = 0.75, ["GBP"] = 0.58 },
        //    ["EUR"] = new Dictionary<string, double> { ["USD"] = 1.16 }
        //};

        /// <summary>
        /// In the provided code, the "Singleton" design pattern is used to implement the CurrencyConverter 
        /// class. Singleton is a design pattern that ensures a class has only one instance of itself and 
        /// allows access to that instance from any point in the code.
        /// In your code, Lazy Initialization is used to delay the creation of the CurrencyConverter instance 
        /// until it's actually needed, and it's created only once when requested.This approach is facilitated 
        /// using the Lazy<T> construct defined as follows:
        /// With this approach, you might reduce the overhead of instance creation and manage system resources 
        /// more efficiently since the instance is created only when it's required. Additionally, 
        /// by keeping the instance private, you can prevent unauthorized access to this instance and ensure 
        /// that only one instance of the CurrencyConverter class exists.
        /// </summary>
        private static readonly Lazy<CurrencyConverter> instance = new Lazy<CurrencyConverter>(() => new CurrencyConverter());

        /// <summary>
        /// The line public static CurrencyConverter Instance => instance.Value; is used to provide a convenient and 
        /// consistent way to access the single instance of the CurrencyConverter class created using the Singleton pattern.
        /// By defining this line, you create a public static property called Instance, which allows other parts of your 
        /// code to easily access the single instance of the CurrencyConverter class without needing to directly instantiate 
        /// it.This property utilizes the Value property of the Lazy<T> instance instance, ensuring that the instance is 
        /// created if it hasn't been already and then returning it.
        /// Using the Instance property is a common practice when implementing the Singleton pattern, as it abstracts away 
        /// the details of instance creation and ensures that everyone who wants to use the CurrencyConverter class can access 
        /// the same single instance.This provides a consistent and straightforward way to interact with the Singleton instance 
        /// throughout your codebase.
        /// </summary>
        public static CurrencyConverter Instance => instance.Value;

        /// <summary>
        /// this.conversionRates = new ConcurrentDictionary<String, Dictionary<String, Double>>();: This line 
        /// initializes the conversionRates dictionary with a new instance of ConcurrentDictionary<string, Dictionary<String, Double>>. 
        /// This dictionary will be used to store the currency conversion rates.
        /// The main purpose of this constructor is to ensure that whenever an instance of the CurrencyConverter 
        /// class is created(which, in this case, will be via the singleton pattern), the conversionRates dictionary 
        /// is initialized and ready to store currency conversion rates in a thread-safe manner.
        /// </summary>
        public CurrencyConverter()
        {
            //this.conversionRates = new ConcurrentDictionary<String, Dictionary<String, Double>>();
        }


        public void ClearConfiguration()
        {
            this.conversionRates.Clear();
        }

        public double Convert(string fromCurrency, string toCurrency, double amount)
        {
            if (String.Equals(fromCurrency, toCurrency, StringComparison.OrdinalIgnoreCase))
            {
                return amount;
            }

            //if (!conversionRates.ContainsKey(fromCurrency) || !conversionRates.ContainsKey(toCurrency))
            //{
            //    throw new InvalidOperationException("Currency conversion rates are not available.");
            //}

            var visitedCurrencies = new HashSet<string>();
            var queue = new Queue<string>();
            var conversionAmounts = new Dictionary<string, double>();

            queue.Enqueue(fromCurrency);
            conversionAmounts[fromCurrency] = amount;

            while (queue.Count > 0)
            {
                var currentCurrency = queue.Dequeue();

                if (visitedCurrencies.Contains(currentCurrency))
                {
                    continue;
                }

                visitedCurrencies.Add(currentCurrency);

                if (currentCurrency == toCurrency)
                {
                    return conversionAmounts[currentCurrency];
                }

                foreach (var nextCurrency in conversionRates[currentCurrency].Keys)
                {
                    var conversionRate = conversionRates[currentCurrency][nextCurrency];
                    queue.Enqueue(nextCurrency);
                    conversionAmounts[nextCurrency] = conversionAmounts[currentCurrency] * conversionRate;
                }
            }

            throw new InvalidOperationException("Cannot find a conversion path.");
        }

              

        /// <summary>
        /// In this line of code, we are using the AddOrUpdate function of the conversionRates dictionary to safely update or add currency conversion rates. This function has the capability to either add a new value to the dictionary with a specified key or update an existing value based on the provided key.
        /// fromCurrency: This parameter represents the source currency for the conversion rate.
        /// new Dictionary<String, Double> { { toCurrency, rateValue }}: This part creates a new dictionary that contains the conversion rate from the source currency to the target currency with the value rateValue.
        /// (_, existingRates) => { existingRates[toCurrency] = rateValue; return existingRates; }: This part is a Lambda function that updates the existing conversion rate if the fromCurrency key exists in the dictionary. Specifically:
        /// _: This is a placeholder for the fromCurrency key, which is not used in this case and acts as a placeholder for an irrelevant key.
        /// existingRates: This parameter represents the target dictionary for the fromCurrency key, which is located within the conversionRates dictionary.
        /// existingRates[toCurrency] = rateValue;: This part updates the conversion rate of the target currency to the new rateValue.
        /// return existingRates;: This part returns the updated dictionary as the result of the function.
        /// In general, this line of code, if the conversion rate for the source currency fromCurrency exists in the dictionary, updates the conversion rate of the target currency toCurrency in the target dictionary. If the source currency rate does not exist, a new entry with the source key and target rates is created.
        /// </summary>
        /// <param name="conversionRates"></param>
        public void UpdateConfiguration(IEnumerable<Tuple<String, String, Double>> conversionRates)
        {
            foreach (var rt in conversionRates)
            {
                var fromCurrency = rt.Item1;
                var toCurrency = rt.Item2;
                var rateValue = rt.Item3;

                #region Description
                /*
                The reason for using this.conversionRates.AddOrUpdate is to safely update or add 
                currency conversion rates. This method ensures safe and synchronized addition or 
                modification of values within the dictionary, especially in a multi-threaded context. 
                In general, AddOrUpdate is used to add or update a value in a dictionary based on a 
                specified key. If the key already exists, the new value replaces the previous one; 
                otherwise, a new entry with the specified key and value is added.
                In the UpdateConfiguration method, which updates the conversion rates, 
                AddOrUpdate is used to ensure that if a rate for the source currency exists, 
                the target rate is updated. If the source rate doesn't exist, a new entry with 
                these rates is created. This approach ensures that updated conversion rates are 
                stored correctly in the conversionRates dictionary and maintains safety in 
                concurrent scenarios.
                Using AddOrUpdate is crucial from a synchronization and safety standpoint to 
                handle concurrent access and modification of data in multi-threaded environments, 
                preventing thread-related issues and data conflicts.
                */
                #endregion
                this.conversionRates.AddOrUpdate(
                    fromCurrency,
                    new Dictionary<String, Double> { { toCurrency, rateValue } },
                    (_, existingRates) =>
                    {
                        existingRates[toCurrency] = rateValue;
                        return existingRates;
                    });
            }
        }
    }
}
