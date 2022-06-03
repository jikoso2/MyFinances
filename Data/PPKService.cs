using MyFinances.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFinances.Helpers;
using System.Collections;

namespace MyFinances.Data
{
	public class PPKService
	{
		PPKModel PPKModel;

		public Task<PPK> GetPPKCalculatedAsync(PPKModel ppkModel)
		{
			PPKModel = ppkModel;
			var ppkResult = CalculatePPKResult();
			return Task.FromResult(ppkResult);
		}

		private PPK CalculatePPKResult()
		{
			var ppkResult = new PPK();

			return ppkResult;
		}
	}
	public class PPK
	{
		public PPK()
		{
			this.PPKData = new PPKData();
		}

		public PPKData PPKData { get; set; }
		public List<Tuple<string, string>> PPKInfo { get; set; }
	}

	public class PPKData
	{
		public string[] Head { get; set; }
	}
}
