using System;
using System.ComponentModel;
using Mits.Tools;
using Newtonsoft.Json.Linq;

namespace Mits.Utilities
{
	public static class HelpRenderer
	{
		public static void RenderHelp(IReadOnlyList<ITool> tools)
		{
			Console.WriteLine("Mits | Maui Image Tools");
            Console.WriteLine();


            Console.WriteLine("Options:");

            var options = typeof(Options).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).Where(f => f.FieldType == typeof(string)).ToList();

			foreach (var option in options)
			{
				var value = option.GetValue(null);

				var description = (option.GetCustomAttributes(typeof(DescriptionAttribute), inherit: true).FirstOrDefault() as DescriptionAttribute)?.Description ?? "No documentation available";

				Console.WriteLine(value);
                Console.WriteLine(description);
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine(Constants.LineBreak);
            Console.WriteLine("Tools:");

            foreach (var tool in tools)
            {
                Console.WriteLine(tool.Name);
                Console.WriteLine(tool.Help);
                Console.WriteLine();
            }
		}
	}
}

