using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics.Config
{
    public class FilterConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (true);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            var x = token.Values();
            var a = token.ToObject<JArray>();

            var retList = new List<List<ConfigProvider>>();
            foreach (var xx in a.Children())
            {
                switch (xx.Type)
                {
                    case JTokenType.String:
                        retList.Add(
                            new List<ConfigProvider>(){
                                xx.ToObject<ConfigProvider>()
                            }
                            );
                        break;
                    case JTokenType.Object:
                        retList.Add(
                            new List<ConfigProvider>() {
                                xx.ToObject<ConfigProvider>()
                            }
                            );
                        break;
                    case JTokenType.Array:
                        var lst = new List<ConfigProvider>();
                        foreach (var configItem in xx.Children())
                        {
                            lst.Add(configItem.ToObject<ConfigProvider>());
                        };
                        retList.Add(lst);
                        break;
                }
                var s = xx.Type;
                var y = xx;
            }
            return retList;

        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            List<List<ConfigProvider>> providersList = (List<List<ConfigProvider>>)value;
            writer.WriteStartArray();
            foreach (var childProviders in providersList)
            {
                if (childProviders.Count == 0) continue;
                if (childProviders.Count == 1 && (childProviders[0].Config == null || childProviders[0].Config.Count == 0))
                {
                    serializer.Serialize(writer, childProviders[0].Name);
                    continue;
                }
                writer.WriteStartArray();
                foreach (var embeddedProvider in childProviders)
                {
                    if (embeddedProvider.Config == null || embeddedProvider.Config.Count == 0)
                    {
                        serializer.Serialize(writer, embeddedProvider.Name);
                        continue;
                    }

                    serializer.Serialize(writer, embeddedProvider);

                }
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
            //serializer.Serialize(writer, providersList);
        }
    }
}
