using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ApiDataRetriever.Entidades.Managers
{
    public static class RequestHelper
    {
        public static WebRequest CreateRequest(string url) 
        {
            WebRequest request = WebRequest.Create(url);
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-us");
            request.Headers.Add(HttpRequestHeader.AcceptCharset, "ISO-8859-1,utf-8");
            request.Headers.Add("Origin", "https://developer.riotgames.com");
            
            return request;
        } 
    }
}
