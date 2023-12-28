using System;


namespace Mits.Models
{
	public enum ProjectKind
	{
		/// <summary>
		/// A dotnet MAUI project
		/// </summary>
		Maui,

		/// <summary>
		/// A legacy Xamarin.iOS project
		/// </summary>
		XamariniOS,

		/// <summary>
		/// A legacy Xamarin.Android project
		/// </summary>
		XamarinAndroid,

		/// <summary>
		/// Any other dotnet project kind. The tool is not interested in these project types.
		/// </summary>
		Other,
	}
}

