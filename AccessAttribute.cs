using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Piranha
{
	public class AccessAttribute : Attribute
	{
		public string Function { get ; set ; }
	}
}
