using System.Collections.Generic;

namespace PdfHandler
{
    /// <summary>
    /// Represents a list of command line arguments in a dictionary format.
    /// ToString method represents keys and values as "key=value". A key with a blank string value will be rendered as "key".
    /// </summary>
    public class CommandLineArgs : Dictionary<string, string>
    {
        public override string ToString()
        {
            string returnString = "";
            foreach (string key in Keys)
            {
                string argString;
                TryGetValue(key, out string? value);
                value = value ?? string.Empty;
                if (value == string.Empty)
                {
                    argString = key + " ";
                }
                else
                {
                    argString = $"{key}={value} ";
                }

                returnString += argString;
            }

            return returnString.Trim();
        }
    }
}