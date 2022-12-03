using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace MyFinances.Data.DataBase
{
	public class Configuration
	{
		[Key]
		public int iid { get; set; }

		[DataMember]
		public string name { get; set; }

		[DataMember]
		public string value { get; set; }
	}
}
