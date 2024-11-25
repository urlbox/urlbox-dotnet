using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace UrlboxSDK
{
    /// <summary>
    /// A custom naming policy for converting property names from PascalCase to snake_case
    /// when serializing JSON.
    /// 
    /// <remarks>
    /// This JsonNamingPolicy is included by default in .NET 8.0 (JsonNamingPolicy.SnakeCaseLower).
    /// However, a custom implementation has been made here to maintain compatibility with .NET 6.0,
    /// which is still under Long-Term Support (LTS). Keeping the SDK at 6.0 ensures broader accessibility 
    /// for audiences still using this version.
    /// </remarks>
    /// </summary>
    public sealed class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            // Insert underscores when:
            // 1. Lowercase letter followed by an uppercase letter
            // 2. Letter followed by a digit
            // 3. Digit followed by a letter, but NOT when transitioning to "xx" or similar patterns
            return string.Concat(name.Select((character, index) =>
                index > 0 &&
                ((char.IsLower(name[index - 1]) && char.IsUpper(character)) || // Lowercase followed by uppercase
                 (char.IsLetter(name[index - 1]) && char.IsDigit(character)) || // Letter followed by number
                 (char.IsDigit(name[index - 1]) && char.IsLetter(character) && // Number followed by letter
                  !(index + 1 < name.Length && char.IsLower(name[index + 1])))) // Exclude cases like '4xx'
                    ? "_" + character
                    : character.ToString()))
                .ToLower();
        }
    }

}
