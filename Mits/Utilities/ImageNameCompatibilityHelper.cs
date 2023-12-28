using System;
using Mits.Models;

namespace FileRenamer
{
	public static class ImageNameCompatibilityHelper
	{
        public static ImageNumericBehaviour SuffixBehaviour = ImageNumericBehaviour.Ammend;
        public static ImageNumericBehaviour PrefixBehaviour = ImageNumericBehaviour.Ammend;

        public const string invalidCharacterReplacement = "_";

        public static string ConvertToCompatibleName(string imageName, out bool didConvert)
        {
            didConvert = false;

            if (!IsIncompatibleImageName(imageName, out _))
            {
                return imageName;
            }

            var nameWithoutExtension = Path.GetFileNameWithoutExtension(imageName);
            if (string.IsNullOrWhiteSpace(nameWithoutExtension))
            {
                return imageName;
            }

            var newName = "";
            var index = 0;
            foreach (var c in nameWithoutExtension)
            {
                if (char.IsUpper(c))
                {
                    if (index > 0 && nameWithoutExtension[index - 1] != '_')
                    {
                        newName += "_";
                    }

                    newName += c.ToString().ToLower();
                }
                else
                {
                    newName += c;
                }
                index++;
            }

            if (newName.StartsWith("_"))
            {
                newName = newName.Remove(0, 1);
            }

            newName = RepairNumberNameStart(newName, PrefixBehaviour);
            newName = RepairNumberNameEnd(newName, SuffixBehaviour);

            newName = RemoveIncompatibleCharacters(newName);

            newName = RepairUnderscoreNameStart(newName);
            newName = RepairUnderscoreNameEnd(newName);

            didConvert = newName != imageName;

            return newName;
        }

        private static string RemoveIncompatibleCharacters(string newName)
        {
            var newValue = "";
            foreach (var c in newName)
            {
                if (IsCompatibleCharacter(c))
                {
                    newValue += c;
                }
                else
                {
                    newValue += '_';
                }
            }

            return newValue;
        }

        private static string RepairUnderscoreNameStart(string newName)
        {
            while (!string.IsNullOrEmpty(newName) && newName.First() == '_')
            {
                newName = newName.Substring(1, newName.Length - 1);
            }

            return newName;
        }

        private static string RepairUnderscoreNameEnd(string newName)
        {
            while (!string.IsNullOrEmpty(newName) && newName.Last() == '_')
            {
                newName = newName.Substring(0, newName.Length - 1);
            }
            return newName;
        }

        private static string RepairNumberNameStart(string newName, ImageNumericBehaviour prefixBehaviour)
        {
            var hasNumberStart = false;
            var number = "";
            while (char.IsNumber(newName.First()))
            {
                hasNumberStart = true;
                number = $"{newName.First()}{number}";
                newName = newName.Substring(1, newName.Length - 1);
            }

            if (hasNumberStart)
            {
                if (int.TryParse(number, out var numberValue))
                {
                    if (prefixBehaviour == ImageNumericBehaviour.Ammend)
                    {
                        newName = "n_" + numberValue + "_" + newName;
                    }
                    else
                    {
                        newName = NumberToWords(numberValue) + "_" + newName;
                    }
                }
            }

            return newName;
        }

        private static string RepairNumberNameEnd(string newName, ImageNumericBehaviour numericBehavoiur)
        {
            var hasNumberEnd = false;
            var number = "";
            while (char.IsNumber(newName.Last()))
            {
                hasNumberEnd = true;
                number = $"{newName.Last()}{number}";
                newName = newName.Substring(0, newName.Length - 1);
            }

            if (hasNumberEnd)
            {
                if (int.TryParse(number, out var numberValue))
                {
                    if (numericBehavoiur == ImageNumericBehaviour.Ammend)
                    {
                        newName += numberValue + "_n";
                    }
                    else
                    {
                        newName += "_" + NumberToWords(numberValue);
                    }
                }
            }

            return newName;
        }


        public static bool IsIncompatibleImageName(string imageName, out string validationError)
		{
			validationError = "";
            bool isCompatible = true;

            if (string.IsNullOrWhiteSpace(imageName))
			{
				return false;
			}

			var name = Path.GetFileNameWithoutExtension(imageName);
			if (string.IsNullOrWhiteSpace(name))
			{
                return false;
			}

			var includesUpperCaseCharacters = name.Any(c => char.IsUpper(c));
			if (includesUpperCaseCharacters)
            {
                isCompatible = false;
                validationError += $"\nThe image name '{imageName}' contains upper case letters";
			}

            if (char.IsNumber(name.First()))
            {
                isCompatible = false;
                validationError += $"\nThe image name '{imageName}' starts with a number. Image names must start with a lowercase letter.";
            }

            if (char.IsNumber(name.Last()))
            {
                isCompatible = false;
                validationError += $"\nThe image name '{imageName}' ends with a number. Image names must end with a lowercase letter,";
            }

            if (name.First() == '_')
            {
                isCompatible = false;
                validationError += $"\nThe image name '{imageName}' starts with the '_' character.  Image names must start with a lowercase letter.";
            }

            if (name.Last() == '_')
            {
                isCompatible = false;
                validationError = $"\nThe image name '{imageName}' ends with the '_' character.  Image names must end with a lowercase letter.";
            }

			var includesInvalidCharacters = name.Any(IsInvalidCharacter);
			if (includesInvalidCharacters)
            {
                isCompatible = false;
                validationError += $"\nThe image name '{imageName}' contains invalid characters. Names must contain only lower case letters, numbers and the '_' character.";
			}

			return isCompatible == false;
        }

        private static bool IsCompatibleCharacter(char @char)
        {
            var isUpper = char.IsUpper(@char);
            if (isUpper)
            {
                return false;
            }

            var isCompatible = char.IsLetterOrDigit(@char) || @char == '_';

			return isCompatible;
        }

        private static bool IsInvalidCharacter(char @char)
        {
            return !IsCompatibleCharacter(@char);
        }

        // Credit: https://stackoverflow.com/questions/2729752/converting-numbers-in-to-words-c-sharp
        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }
    }

}

