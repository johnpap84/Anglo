using System;
using System.Text;

namespace AngloRota.Shared
{
    public static class StringOperations
    {
        public static string FirstLettersToUpperCase(this string source)
        {
            StringBuilder destination = new StringBuilder();
            bool toUpperCase = true;

            if (!String.IsNullOrWhiteSpace(source))
            {
                foreach (char letter in source)
                {
                    if (toUpperCase)
                    {
                        destination.Append(char.ToUpper(letter));
                    }
                    else
                    {
                        destination.Append(letter);
                    }

                    if (letter.Equals(' '))
                    {
                        toUpperCase = true;
                    }
                    else
                    {
                        toUpperCase = false;
                    }
                }
                return destination.ToString();
            }
            else
            {
                return source;
            }
        }
    }
}
