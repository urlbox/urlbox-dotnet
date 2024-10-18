using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Screenshots
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
    public class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            // Convert PascalCase to snake_case
            return string.Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }
    }

}