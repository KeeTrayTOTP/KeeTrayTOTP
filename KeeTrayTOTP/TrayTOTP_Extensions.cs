using System;

namespace KeeTrayTOTP
{
    /// <summary>
    /// Class to support custom extensions.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Concatenates a space in front of the current string.
        /// </summary>
        /// <param name="extension">Current string.</param>
        /// <returns></returns>
        internal static string ExtWithSpaceBefore(this string extension)
        {
            return " " + extension;
        }

        /// <summary>
        /// Concatenates the current string with space to the end.
        /// </summary>
        /// <param name="extension">Current string.</param>
        /// <returns></returns>
        internal static string ExtWithSpaceAfter(this string extension)
        {
            return extension + " ";
        }

        /// <summary>
        /// Concatenates the current string with a bracket in front and to the end.
        /// </summary>
        /// <param name="extension">Current string.</param>
        /// <returns></returns>
        internal static string ExtWithBrackets(this string extension)
        {
            return ExtWith(extension, '{', '}');
        }

        /// <summary>
        /// Concatenates the current string with a parenthesis in front and to the end.
        /// </summary>
        /// <param name="extension">Current string.</param>
        /// <returns></returns>
        internal static string ExtWithParenthesis(this string extension)
        {
            return ExtWith(extension, '(', ')');
        }

        /// <summary>
        /// Concatenates the current string with a character in front and another character to the end.
        /// </summary>
        /// <param name="extension">Current string.</param>
        /// <param name="left">Front character.</param>
        /// <param name="right">End character.</param>
        /// <returns></returns>
        internal static string ExtWith(this string extension, char left, char right)
        {
            return left + extension + right;
        }

        /// <summary>
        /// Remove all spaces from the current string.
        /// </summary>
        /// <param name="extension">Current string.</param>
        /// <returns></returns>
        internal static string ExtWithoutSpaces(this string extension)
        {
            return extension.ExtWithout(" ");
        }

        /// <summary>
        /// Remove all specified characters from the current string.
        /// </summary>
        /// <param name="extension">Current string.</param>
        /// <param name="chars">Characters to remove.</param>
        /// <returns></returns>
        internal static string ExtWithout(this string extension, string chars)
        {
            foreach (var @char in chars)
            {
                extension = extension.Replace(@char.ToString(), "");
            }
            return extension;
        }

        /// <summary>
        /// Converts the control's tag to text and splits it.
        /// </summary>
        /// <param name="extension">Current string.</param>
        /// <param name="index">Split index.</param>
        /// <param name="seperator">Split seperators.</param>
        /// <returns></returns>
        internal static string ExtSplitFromTag(this System.Windows.Forms.Control extension, int index = 0, char seperator = ';')
        {
            return extension.Tag.ToString().ExtSplit(index, seperator);
        }

        /// <summary>
        /// Splits the string and returns specified substring.
        /// </summary>
        /// <param name="extension">Current string.</param>
        /// <param name="index">Split index.</param>
        /// <param name="seperator">Split seperators.</param>
        /// <returns></returns>
        internal static string ExtSplit(this string extension, int index, char seperator = ';')
        {
            if (extension != string.Empty)
            {
                try
                {
                    var text = extension;
                    if (text.Contains(seperator.ToString()))
                    {
                        return text.Split(seperator)[index];
                    }
                    return text;
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Makes sure the string provided as a Seed is Base32. Invalid characters are available as out string.
        /// </summary>
        /// <param name="extension">Current string.</param>
        /// <param name="invalidChars">Invalid characters.</param>
        /// <returns>Validity of the string's characters for Base32 format.</returns>
        internal static bool ExtIsBase32(this string extension, out string invalidChars)
        {
            invalidChars = null;
            try
            {
                foreach (var currentChar in extension)
                {
                    var currentCharValue = char.GetNumericValue(currentChar);
                    if (char.IsLetter(currentChar))
                    {
                        continue;
                    }
                    if (char.IsDigit(currentChar))
                    {
                        if ((currentCharValue > 1) && (currentCharValue < 8))
                        {
                            continue;
                        }
                    }
                    invalidChars = (invalidChars + currentCharValue.ToString().ExtWithSpaceBefore()).Trim();
                }
            }
            catch (Exception)
            {
                invalidChars = Localization.Strings.Error;
            }
            return invalidChars == null;
        }
    }
}
