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
        /// <param name="Extension">Current string.</param>
        /// <returns></returns>
        internal static string ExtWithSpaceBefore(this string Extension)
        {
            return " " + Extension;
        }

        /// <summary>
        /// Concatenates the current string with space to the end.
        /// </summary>
        /// <param name="Extension">Current string.</param>
        /// <returns></returns>
        internal static string ExtWithSpaceAfter(this string Extension)
        {
            return Extension + " ";
        }

        /// <summary>
        /// Concatenates the current string with a bracket in front and to the end.
        /// </summary>
        /// <param name="Extension">Current string.</param>
        /// <returns></returns>
        internal static string ExtWithBrackets(this string Extension)
        {
            return ExtWith(Extension, '{', '}');
        }

        /// <summary>
        /// Concatenates the current string with a parenthesis in front and to the end.
        /// </summary>
        /// <param name="Extension">Current string.</param>
        /// <returns></returns>
        internal static string ExtWithParenthesis(this string Extension)
        {
            return ExtWith(Extension, '(', ')');
        }

        /// <summary>
        /// Concatenates the current string with a character in front and another character to the end.
        /// </summary>
        /// <param name="Extension">Current string.</param>
        /// <param name="Left">Front character.</param>
        /// <param name="Right">End character.</param>
        /// <returns></returns>
        internal static string ExtWith(this string Extension, char Left, char Right)
        {
            return Left + Extension + Right;
        }

        /// <summary>
        /// Remove all spaces from the current string.
        /// </summary>
        /// <param name="Extension">Current string.</param>
        /// <returns></returns>
        internal static string ExtWithoutSpaces(this string Extension)
        {
            return Extension.ExtWithout(" ");
        }

        /// <summary>
        /// Remove all specified characters from the current string.
        /// </summary>
        /// <param name="Extension">Current string.</param>
        /// <param name="Chars">Characters to remove.</param>
        /// <returns></returns>
        internal static string ExtWithout(this string Extension, string Chars)
        {
            foreach (var Char in Chars)
            {
                Extension = Extension.Replace(Char.ToString(), "");
            }
            return Extension;
        }

        /// <summary>
        /// Converts the control's tag to text and splits it.
        /// </summary>
        /// <param name="Extension">Current string.</param>
        /// <param name="Index">Split index.</param>
        /// <param name="Seperator">Split seperators.</param>
        /// <returns></returns>
        internal static string ExtSplitFromTag(this System.Windows.Forms.Control Extension, int Index = 0, char Seperator = ';')
        {
            return Extension.Tag.ToString().ExtSplit(Index, Seperator);
        }

        /// <summary>
        /// Splits the string and returns specified substring.
        /// </summary>
        /// <param name="Extension">Current string.</param>
        /// <param name="Index">Split index.</param>
        /// <param name="Seperator">Split seperators.</param>
        /// <returns></returns>
        internal static string ExtSplit(this string Extension, int Index, char Seperator = ';')
        {
            if (Extension != string.Empty)
            {
                try
                {
                    var Text = Extension;
                    if (Text.Contains(Seperator.ToString()))
                    {
                        return Text.Split(Seperator)[Index];
                    }
                    return Text;
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
        /// <param name="Extension">Current string.</param>
        /// <param name="InvalidChars">Invalid characters.</param>
        /// <returns>Validity of the string's characters for Base32 format.</returns>
        internal static bool ExtIsBase32(this string Extension, out string InvalidChars)
        {
            InvalidChars = null;
            try
            {
                foreach (var CurrentChar in Extension)
                {
                    var CurrentCharValue = Char.GetNumericValue(CurrentChar);
                    if (Char.IsLetter(CurrentChar))
                    {
                        continue;
                    }
                    if (Char.IsDigit(CurrentChar))
                    {
                        if ((CurrentCharValue > 1) && (CurrentCharValue < 8))
                        {
                            continue;
                        }
                    }
                    InvalidChars = (InvalidChars + CurrentCharValue.ToString().ExtWithSpaceBefore()).Trim();
                }
            }
            catch (Exception)
            {
                InvalidChars = Localization.Strings.Error;
            }
            return InvalidChars == null;
        }
    }
}
