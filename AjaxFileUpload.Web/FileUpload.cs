using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;

namespace AjaxFileUpload.Web
{
    public static class FileUpload
    {
        private static IDictionary<Guid, int> _Requests = new ConcurrentDictionary<Guid, int>();

        public static void RegisterHandler(HttpRequest request, String path)
        {   
            Guid id = BuildGuid(request);
            if (id == Guid.Empty)
                return; // can't report progress without id; exit.

            if (_Requests.ContainsKey(id))
                return; // a file upload operation with the Guid value already exists; exit.

            _Requests.Add(new KeyValuePair<Guid, int>(id, 0));

            //using (Stream stream = request.GetBufferlessInputStream())
            using (Stream stream = request.GetBufferedInputStream())
            {
                long size;
                if (!long.TryParse(request.Headers["Content-Length"], out size))
                    return;
                FileHandler handler = new FileHandler(path, id);
                handler.SaveFile(size, stream, i => _Requests[id] = i);
            }
 
        }

        /// <summary>
        /// Return the progress of a file saving operation represented by a Guid
        /// </summary>
        /// <param name="id">Guid representing a file saving operation</param>
        /// <returns>an integer denoting the progress; -1 if <paramref name="id"/> does not return a valid operation</returns>
        public static int CheckProgress(Guid id)
        {
            if (_Requests.ContainsKey(id))
                return _Requests[id];

            return -1;
        }

        private static Guid BuildGuid(HttpRequest request)
        {
            NameValueCollection queryParameters = GetQueryParameters(request);
            String idVal = queryParameters.Get("id");
            if (string.IsNullOrEmpty(idVal))
                return Guid.Empty;

            return Guid.Parse(idVal);
        }

        private static NameValueCollection GetQueryParameters(HttpRequest request)
        {
            return HttpUtility.ParseQueryString(request.Url.Query);
        }

        
    }
}