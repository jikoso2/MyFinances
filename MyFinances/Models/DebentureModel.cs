using MyFinances.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MyFinances.Models
{
    public class DebentureModel
    {
        int amount = DefaultValue.Debenture.Amount;
        [Range(1, 100000, ErrorMessage = "Liczba zakupionych obligacji musi być całkowita,dodatnia i nie większa niż 100 tyś sztuk")]
        public int Amount { get => amount; set => amount = value > 100000 ? 100000 : value; }

        [Required]
        public DebentureType Type { get; set; }

        [Required]
        public double EarlyRedemptionFee { get; set; }

        [Range(0.001, 30, ErrorMessage = "Wysokość oprocentowania nie może być mniejsza od zera lub większa od 30")]
        public double OTSPercentage { get; set; } = DefaultValue.Debenture.OTSPercentage;

        [Range(0.001, 30, ErrorMessage = "Wysokość oprocentowania nie może być mniejsza od zera lub większa od 30")]
        public double DOSPercentage { get; set; } = DefaultValue.Debenture.DOSPercentage;

        [Required]
        public bool BelkaTax { get; set; } = true;

        [DebentureModelValidation.TOZPercentage]
        public List<double> TOZPercentage { get; set; } = DefaultValue.Debenture.TOZPercentage;

        [DebentureModelValidation.EDOPercentage]
        public List<double> EDOPercentage { get; set; } = DefaultValue.Debenture.EDOPercentage;

        [Range(0, 3, ErrorMessage = "Dodatkowe oprocentowanie nie może być mniejsze od zera lub większe od 3")]
        public double EDOAdditionalPercentage { get; set; } = DefaultValue.Debenture.EDOAdditionalPercentage;

        [DebentureModelValidation.COIPercentage]
        public List<double> COIPercentage { get; set; } = DefaultValue.Debenture.COIPercentage;

        [Range(0, 3, ErrorMessage = "Dodatkowe oprocentowanie nie może być mniejsze od zera lub większe od 3")]
        public double COIAdditionalPercentage { get; set; } = DefaultValue.Debenture.COIAdditionalPercentage;

        double rORPercentage = DefaultValue.Debenture.RORPercentage;
        [Range(0, 15)]
        public double RORPercentage { get => rORPercentage; set { rORPercentage = value > 15.0 ? 15.0 : Math.Round(value, 2); rORPercentage = value < 0.0 ? 0.0 : Math.Round(value, 2); } }

        double dORPercentage = DefaultValue.Debenture.DORPercentage;
        [Range(0, 15)]
        public double DORPercentage { get => dORPercentage; set { dORPercentage = value > 15.0 ? 15.0 : Math.Round(value, 2); dORPercentage = value < 0.0 ? 0.0 : Math.Round(value, 2); } }

        double tOSPercentage = DefaultValue.Debenture.TOSPercentage;
        [Range(0, 15)]
        public double TOSPercentage { get => tOSPercentage; set { tOSPercentage = value > 15.0 ? 15.0 : Math.Round(value, 2); tOSPercentage = value < 0.0 ? 0.0 : Math.Round(value, 2); } }

    }

    internal class DebentureModelValidation
    {
        internal class TOZPercentage : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var debentureModel = (DebentureModel)validationContext.ObjectInstance;

                if (debentureModel.TOZPercentage[0] < 0 || debentureModel.TOZPercentage[0] > 30)
                    return new ValidationResult("Oprocentowanie w pierwszym okresie musi być dodatnie i mniejsze od 30%", new[] { validationContext.MemberName });

                for (int i = 1; i < debentureModel.TOZPercentage.Count; i++)
                {
                    if (debentureModel.TOZPercentage[i] <= 0 || debentureModel.TOZPercentage[i] > 30)
                        return new ValidationResult("Wskaźnik WIBOR 6M musi być dodatni i mniejszy od 30%", new[] { validationContext.MemberName });
                }

                return null;
            }
        }

        internal class EDOPercentage : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var debentureModel = (DebentureModel)validationContext.ObjectInstance;

                if (debentureModel.EDOPercentage[0] < 0 || debentureModel.EDOPercentage[0] > 30)
                    return new ValidationResult("Oprocentowanie w pierwszym okresie musi być dodatnie i mniejsze od 30%", new[] { validationContext.MemberName });

                foreach (var percentage in debentureModel.EDOPercentage)
                {
                    if (percentage > 30 || percentage < -30)
                        return new ValidationResult("Odczyt inflacji nie może być większy od 30% i mniejszy od -30%", new[] { validationContext.MemberName });
                }

                return null;
            }
        }

        internal class COIPercentage : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var debentureModel = (DebentureModel)validationContext.ObjectInstance;

                if (debentureModel.COIPercentage[0] < 0)
                    return new ValidationResult("Oprocentowanie w pierwszym okresie musi być dodatnie", new[] { validationContext.MemberName });

                foreach (var percentage in debentureModel.COIPercentage)
                {
                    if (percentage > 30 || percentage < -30)
                        return new ValidationResult("Odczyt inflacji nie może być większy od 30% i mniejszy od -30%", new[] { validationContext.MemberName });
                }

                return null;
            }
        }
    }

    public enum DebentureType
    {
        OTS,
        DOS,
        TOZ,
        COI,
        EDO,
        ROR,
        DOR,
        TOS
    }
}
