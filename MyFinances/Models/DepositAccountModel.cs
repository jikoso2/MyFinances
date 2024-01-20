using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MyFinances.Models
{
    public class DepositAccountModel
    {
        [Required]
        [Range(0.0, 1000000, ErrorMessage = "Zgromadzony kapitał musi być dodatni")]
        public double StartAmount { get; set; } = Helpers.DefaultValue.DepositAccount.StartAmount;

        [Required]
        [Range(0.0, 1000000, ErrorMessage = "Zgromadzony kapitał od Państwa musi być dodatni")]
        public double MonthlyPayment { get; set; } = Helpers.DefaultValue.DepositAccount.MonthlyPayment;

        [Required]
        [Range(1, 360, ErrorMessage = "Długość oszczędzania musi być dłuższa od miesiąca i krótsza od 30 lat")]
        public int Lenght { get; set; } = Helpers.DefaultValue.DepositAccount.Length;

        [Required]
        [Range(0, 15, ErrorMessage = "Wartość oprocentowania musi być większa od 0 i mniejsza od 15")]
        public double Percentage { get; set; } = Helpers.DefaultValue.DepositAccount.Percentage;

        public bool BelkaTax { get; set; } = true;

        public DepositAccountType DepositAccountType { get; set; } = DepositAccountType.Prosty;
    }

    public enum DepositAccountType
    {
        Prosty,
        Rozbudowany
    }
}
