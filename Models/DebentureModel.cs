﻿using MyFinances.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MyFinances.Models
{
	public class DebentureModel
	{
		[Required]
		[Range(1, 10000, ErrorMessage = "Liczba zakupionych obligacji musi być całkowita i dodatnia")]
		public int Amount { get; set; }

		[Required]
		public DebentureType Type { get; set; }

		[Range(0.001, 30, ErrorMessage = "Wysokość oprocentowania nie może być mniejsza od zera lub większa od 30")]
		public double OTSPercentage { get; set; } = DefaultValue.OTSPercentage;

		[Range(0.001, 30, ErrorMessage = "Wysokość oprocentowania nie może być mniejsza od zera lub większa od 30")]
		public double DOSPercentage { get; set; } = DefaultValue.DOSPercentage;

		[Required]
		public bool BelkaTax { get; set; }

		[DebentureModelValidation.TOZPercentage]
		public List<double> TOZPercentage { get; set; } = DefaultValue.TOZPercentage;
	}

	internal class DebentureModelValidation
	{
		internal class TOZPercentage : ValidationAttribute
		{
			protected override ValidationResult IsValid(object value, ValidationContext validationContext)
			{
				var debentureModel = (DebentureModel)validationContext.ObjectInstance;

				if(debentureModel.TOZPercentage[0] < 0 )
					return new ValidationResult("Oprocentowanie w pierwszym okresie musi być dodatnie", new[] { validationContext.MemberName });

				for (int i = 1; i < debentureModel.TOZPercentage.Count(); i++)
				{
					if (debentureModel.TOZPercentage[i] <= 0)
						return new ValidationResult("Wskaźnik WIBOR 6M musi być dodatni", new[] { validationContext.MemberName });
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
		EDO
	}
}
