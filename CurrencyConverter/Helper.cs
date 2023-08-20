using System.ComponentModel.DataAnnotations;

namespace CurrencyConvertor.CurrencyConverter
{
    public class ConvertHelper
    {
        [Required(ErrorMessage = "From Currency is required.")]
        [StringLength(3, ErrorMessage = "From Currency must be 3 characters.", MinimumLength = 3)]
        public string FromCurrency { get; set; }

        [Required(ErrorMessage = "To Currency is required.")]
        [StringLength(3, ErrorMessage = "To Currency must be 3 characters.", MinimumLength = 3)]
        public string ToCurrency { get; set; }

        [Required(ErrorMessage = "Currency Amount is required.")]
        public double CrAmount { get; set; }
    }
}