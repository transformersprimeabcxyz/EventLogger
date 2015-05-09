using HashTag.Diagnostics;
using System;
namespace HashTag.Web.Http
{
    public interface IRestSession<TBodyType>
    {
        TBodyType Body { get; }
        void Dispose();
        void Dispose(bool isDisposing);
        TimeSpan ElapsedTime { get; set; }
        LogException ClientException { get; set; }
        bool IsCallOk { get; }
        bool IsOk { get; }
        RestRequest Request { get; set; }
        RestResponse<TBodyType> Response { get; set; }
        System.Net.HttpStatusCode StatusCode { get; }
        string ToString();
    }
}
