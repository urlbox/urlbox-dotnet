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
    /// Initializes a new instance of the UrlboxOptions. These are used as part of any Urlbox method which requires render options.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the Url OR Html option isn't passed in on init.</exception>
    public class UrlboxOptions
    {

        public UrlboxOptions(string url = null, string html = null)
        {
            if (string.IsNullOrEmpty(url) && string.IsNullOrEmpty(html))
            {
                throw new ArgumentException("Either of options 'url' or 'html' must be provided.");
            }
            Url = url;
            Html = html;
        }

        public string Url { get; set; }
        public string Html { get; set; }
        public string Format { get; set; } // png jpeg webp avif svg pdf html mp4 webm md
        public int Width { get; set; }
        public int Height { get; set; }
        public bool FullPage { get; set; }
        public string Selector { get; set; }
        public string Clip { get; set; } // x,y,width,height EG "0,0,400,400"
        public bool Gpu { get; set; }
        public string ResponseType { get; set; } // one of json or binary
        public bool BlockAds { get; set; }
        public bool HideCookieBanners { get; set; }
        public bool ClickAccept { get; set; }
        public string[] BlockUrls { get; set; }
        public bool BlockImages { get; set; }
        public bool BlockFonts { get; set; }
        public bool BlockMedias { get; set; }
        public bool BlockStyles { get; set; }
        public bool BlockScripts { get; set; }
        public bool BlockFrames { get; set; }
        public bool BlockFetch { get; set; }
        public bool BlockXhr { get; set; }
        public bool BlockSockets { get; set; }
        public string HideSelector { get; set; }
        public string Js { get; set; }
        public string Css { get; set; }
        public bool DarkMode { get; set; }
        public bool ReducedMotion { get; set; }
        public bool Retina { get; set; }
        public int ThumbWidth { get; set; }
        public int ThumbHeight { get; set; }
        public string ImgFit { get; set; } // cover contain fill inside outside
        public string ImgPosition { get; set; } // if img_fit is cover or contain then possible values for this are: north northeast east southeast south southwest west northwest center centre

        public string ImgBg { get; set; } // red #ccc rgb() rgba() or hsl()
        public string ImgPad { get; set; } // either 10 or 10,10,10,10
        public int Quality { get; set; } // between 0 to 100
        public bool Transparent { get; set; }
        public int MaxHeight { get; set; }
        public string Download { get; set; }
        public string PdfPageSize { get; set; } // A0 A1 A2 A3 A4 A5 A6 Legal Letter Ledger Tabloid
        public string PdfPageRange { get; set; }
        public int PdfPageWidth { get; set; }
        public int PdfPageHeight { get; set; }
        public string PdfMargin { get; set; } //none default minimum
        public int PdfMarginTop { get; set; }
        public int PdfMarginRight { get; set; }
        public int PdfMarginBottom { get; set; }
        public int PdfMarginLeft { get; set; }
        public bool PdfAutoCrop { get; set; }
        public double PdfScale { get; set; } // 0.1 up to 2
        public string PdfOrientation { get; set; } // portrait landscape
        public bool PdfBackground { get; set; }
        public bool DisableLigatures { get; set; }
        public string Media { get; set; } // print or screen
        public bool PdfShowHeader { get; set; }
        public string PdfHeader { get; set; }
        public bool PdfShowFooter { get; set; }
        public string PdfFooter { get; set; }
        public bool Readable { get; set; }
        public bool Force { get; set; }
        public string Unique { get; set; }
        public int Ttl { get; set; }
        public string Proxy { get; set; }

        private object _header;
        private object _cookie;

        public object Header
        {
            get { return _header; }
            set { _header = ValidateStringOrArray(value, nameof(Header)); }
        }

        public object Cookie
        {
            get { return _cookie; }
            set { _cookie = ValidateStringOrArray(value, nameof(Cookie)); }
        }

        /// <summary>
        /// Tightens a type to string or string[] for Urlbox options which allow singles+multiples
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private object ValidateStringOrArray(object value, string propertyName)
        {
            if (value is string || value is string[])
            {
                return value;
            }
            else
            {
                throw new ArgumentException($"{propertyName} must be either a string or a string array.");
            }
        }

        public string UserAgent { get; set; }
        public string Platform { get; set; } // MacIntel | Linux x86_64 | Linux armv81 | Win32
        public string AcceptLang { get; set; }
        public string Authorization { get; set; }
        public string Tz { get; set; }
        public string EngineVersion { get; set; }
        public int Delay { get; set; }
        public int Timeout { get; set; }
        public string WaitUntil { get; set; } // domloaded mostrequestsfinished requestsfinished loaded
        public string WaitFor { get; set; }
        public string WaitToLeave { get; set; }
        public int WaitTimeout { get; set; }
        public bool FailIfSelectorMissing { get; set; }
        public bool FailIfSelectorPresent { get; set; }
        public bool FailOn4xx { get; set; }
        public bool FailOn5xx { get; set; }
        public string ScrollTo { get; set; }
        public string Click { get; set; }
        public string ClickAll { get; set; }
        public string Hover { get; set; }
        public string BgColor { get; set; }
        public bool DisableJs { get; set; }
        public string FullPageMode { get; set; } // stitch native
        public bool FullWidth { get; set; }
        public bool AllowInfinite { get; set; }
        public bool SkipScroll { get; set; }
        public bool DetectFullHeight { get; set; }
        public int MaxSectionHeight { get; set; }
        public int ScrollIncrement { get; set; }
        public int ScrollDelay { get; set; }
        public string Highlight { get; set; }
        public string HighlightFg { get; set; }
        public string HighlightBg { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Accuracy { get; set; }
        public bool UseS3 { get; set; }
        public string S3Path { get; set; }
        public string S3Bucket { get; set; }
        public string S3Endpoint { get; set; }
        public string S3Region { get; set; }
        public string CdnHost { get; set; }
        public string S3StorageClass { get; set; }
    }

}