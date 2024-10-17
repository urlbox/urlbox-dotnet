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
        public string RenderUrl { get; set; }
        public int Size { get; set; }
    }

    /// <summary>
    /// Represents an asynchronous Urlbox response.
    /// </summary>
    public class AsyncUrlboxResponse : IUrlboxResponse
    {
        public string Status { get; set; }
        public string RenderId { get; set; }
        public string StatusUrl { get; set; }
    }
}