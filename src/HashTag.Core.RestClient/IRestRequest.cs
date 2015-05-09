using System;
namespace HashTag.Web.Http
{
    public interface IRestRequest
    {
        string Address { get; set; }
        string Body { get; set; }
        long ContentLength { get; set; }
        string ContentType { get; set; }
        void Dispose();
        void Dispose(bool isDisposing);
        long HeaderLength { get; set; }
        HashTag.Collections.PropertyBag Headers { get; set; }
        string Method { get; set; }
        byte[] Payload { get; set; }
        DateTime SentDateTime { get; set; }
        string ToString();
        System.Net.Http.HttpRequestMessage Web { get; set; }
    }
}
