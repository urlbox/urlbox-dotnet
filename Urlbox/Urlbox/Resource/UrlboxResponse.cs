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
    /// Interface for Urlbox response types.
    /// Allows one response type for makeUrlboxPostRequest which can then
    /// be cast to the specific /sync or /async response
    /// Implementations represent either synchronous or asynchronous responses.
    /// </summary>
    public interface IUrlboxResponse
    {
    }

    /// <summary>
    /// Represents a synchronous Urlbox response.
    /// </summary>
    public class SyncUrlboxResponse : IUrlboxResponse
    {
        const string EXTENSION_HTML = ".html";
        const string EXTENSION_MHTML = ".mhtml";
        const string EXTENSION_MARKDOWN = ".md";
        const string EXTENSION_METADATA = ".json";
        public string RenderUrl { get; } // The location of the screenshot
        public int Size { get; }

        public string HtmlUrl { get; } // The location of the html screenshot if save_html
        public string MhtmlUrl { get; } // The location of the mhtml screenshot if save_mhtml
        public string MetadataUrl { get; } // The location of the metadata screenshot if save_metadata
        public string MarkdownUrl { get; } // The location of the markdown screenshot if save_markdown
        public UrlboxMetadata Metadata { get; } // The markdown of the render if save_metadata or metadata=true

        public SyncUrlboxResponse(
            string renderUrl,
            int size,
            string htmlUrl = null,
            string mhtmlUrl = null,
            string metadataUrl = null,
            string markdownUrl = null,
            UrlboxMetadata metadata = null
        )
        {
            this.RenderUrl = renderUrl;
            this.Size = size;
            if (!String.IsNullOrEmpty(htmlUrl)) this.HtmlUrl = checkExtension(htmlUrl, EXTENSION_HTML);
            if (!String.IsNullOrEmpty(mhtmlUrl)) this.MhtmlUrl = checkExtension(mhtmlUrl, EXTENSION_MHTML);
            if (!String.IsNullOrEmpty(metadataUrl)) this.MetadataUrl = checkExtension(metadataUrl, EXTENSION_METADATA);
            if (!String.IsNullOrEmpty(markdownUrl)) this.MarkdownUrl = checkExtension(markdownUrl, EXTENSION_MARKDOWN);
            if (metadata != null) this.Metadata = metadata;
        }

        /// <summary>
        /// Checks that a given url has its relevant file extension
        /// </summary>
        /// <param name="url"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private string checkExtension(string url, string extension)
        {
            if (!url.Contains(extension))
            {
                throw new ArgumentException($"The URL {url} does not contain extension {extension}");
            }
            return url;
        }
    }

    /// <summary>
    /// Represents an asynchronous Urlbox response.
    /// </summary>
    public class AsyncUrlboxResponse : IUrlboxResponse
    {
        public string Status { get; } // EG 'succeeded'
        public string RenderId { get; } // A UUID for the request
        public string StatusUrl { get; } // A url which you can poll to check the render's status

        public AsyncUrlboxResponse(string status, string renderId, string statusUrl)
        {
            this.Status = status;
            this.RenderId = renderId;
            this.StatusUrl = statusUrl;
        }
    }
}