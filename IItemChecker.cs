using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AntiItemCheating
{
	internal interface IItemChecker
	{
		public bool Obsolete { get; }
		public void Add(int id);
		//public void Add(params int[] ids)
		//{
		//	foreach (var id in ids)
		//	{
		//		Add(id);
		//	}
		//}
		public bool Contains(int id);
	}
	internal static class Ext
	{
		public static void Add(this IItemChecker checker, params int[] ids)
		{
			foreach (var id in ids)
			{
				checker.Add(id);
			}
		}
	}
}
