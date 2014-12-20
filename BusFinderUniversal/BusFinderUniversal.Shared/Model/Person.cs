using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusFinderUniversal.Model
{
	public class Person : ObservableObject
	{
		public string _name;
		public int _age;

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				Set("Name", ref _name, value);
			}
		}

		public int Age
		{
			get
			{
				return _age;
			}
			set
			{
				Set("Age", ref _age, value);
			}
		}

	}
}
