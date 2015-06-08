using HashTag.Configuration;
using HashTag.Diagnostics.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Diagnostics.TraceListeners
{
    public class EventServiceListener : TraceListener
    {

        public override void Write(string message)
        {
            throw new NotImplementedException();
        }
        private string _connectionStringName { get; set; }
        private string _connectionString { get; set; }
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if (data == null) return;

            if (string.IsNullOrWhiteSpace(_connectionStringName))
            {
                foreach (DictionaryEntry entry in Attributes)
                {
                    switch (entry.Key.ToString().ToUpper())
                    {
                        case "CONNECTIONSTRINGNAME": _connectionStringName = entry.Value.ToString(); break;
                    }
                }
                if (string.IsNullOrWhiteSpace(_connectionStringName))
                {
                    throw new ConfigurationErrorsException("Required 'connectionStringName' attribute on listener is missing or has an empty value");
                }
                if (ConfigurationManager.ConnectionStrings[_connectionStringName] == null)
                {
                    throw new ConfigurationErrorsException("Required connection string '{0}' is missing or has an empty value");
                }
                _connectionString = ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString;
                if (string.IsNullOrWhiteSpace(_connectionString))
                {
                    throw new ConfigurationErrorsException("Required connection string '{0}' is missing or has an empty value");
                }
            }

            if (!(data is List<LogEvent>))
            {
                throw new ArgumentException("'data' parameter is expected to be List<LogEvent>.  Received " + data.GetType().FullName);
            }

            sendDataToEventService(data as List<LogEvent>).Wait();

        }

        private async Task sendDataToEventService(List<LogEvent> eventList)
        {

            using (var client = new HttpClient())
            {
                var msg = new HttpRequestMessage(HttpMethod.Post, _connectionString);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsJsonAsync(msg.RequestUri, eventList);
                response.EnsureSuccessStatusCode();
            }
        }


        public override void WriteLine(string message)
        {
            throw new NotImplementedException();

        }
        protected override string[] GetSupportedAttributes()
        {
            return new string[] {
                "connectionStringName"
            };
        }
    }
}
