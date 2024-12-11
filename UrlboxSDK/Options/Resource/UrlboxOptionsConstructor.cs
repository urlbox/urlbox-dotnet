namespace UrlboxSDK.Options.Resource
{
    using System;
    /// <summary>
    /// Constructor for UrlboxOptions. Allows QT to autogen type with no construct
    /// </summary>
    public partial class UrlboxOptions
    {
        // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        /* 
            All options get serialized as nullable due to the automated options properties having 
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] above them.

            The warning CS8618 is suppressed because the compiler doesn't know that these fields 
            are intentionally left uninitialized in the constructor. The properties may be set later, 
            and `JsonIgnoreCondition.WhenWritingNull` ensures they won't cause issues if left as `null`. 

            This behavior is acceptable because `UrlboxOptions` is designed to work with optional 
            properties that default to `null`. Since properties are optional and assigned later, 
            the warning can be safely ignored.
        */
#pragma warning disable CS8618
        public UrlboxOptions(string? url = null, string? html = null)
#pragma warning restore CS8618
        {
            if (
                String.IsNullOrEmpty(url) && !String.IsNullOrEmpty(html)
            )
            {
                Html = html;
            }
            else if (
                !String.IsNullOrEmpty(url) && String.IsNullOrEmpty(html)
            )
            {
                Url = url;
            }
            else
            {
                throw new ArgumentException("Either but not both options 'url' or 'html' must be provided.");
            }
        }
    }
}