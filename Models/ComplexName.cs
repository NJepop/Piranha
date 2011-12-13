using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Piranha.Models
{
	/// <summary>
	/// Class for defining a name in both singular and plural.
	/// </summary>
	public class ComplexName {
		/// <summary>
		/// Gets/sets the name in its singular form.
		/// </summary>
		[Display(Name="Namn i singular")]
		[Required(ErrorMessage="Du måste ange ett namn.")]
		public string Singular { get ; set ; }

		/// <summary>
		/// Gets/sets the name in its plural form.
		/// </summary>
		[Display(Name="Namn i plural")]
		[Required(ErrorMessage="Du måste ange ett namn.")]
		public string Plural { get ; set ; }
	}
}
