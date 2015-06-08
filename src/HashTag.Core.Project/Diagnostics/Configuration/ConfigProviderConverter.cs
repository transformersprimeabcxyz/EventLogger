using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics.Config
{
    public class ProvderConfigConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);

            switch (token.Type)
            {
                case JTokenType.String: return new ConfigProvider()
                {
                    Name = token.Value<string>()
                };
                case JTokenType.Object:

                    var retObject = new ConfigProvider();
                    foreach (JProperty p in ((JObject)token).Properties())
                    {
                        var value = p.Value;

                        switch (p.Name.ToUpperInvariant())
                        {
                            case "NAME": retObject.Name = p.Value.ToString();
                                break;
                            case "CONFIG":
                                if (value == null) break;
                                var o = (JObject)value;
                                foreach (JProperty configProp in o.Properties())
                                {
                                    retObject.Config[configProp.Name] = configProp.Value.ToString();
                                };
                                break;
                            case "TYPE":
                                retObject.Type = p.Value.ToString();
                                break;
                        }
                    }
                    return retObject;                    
            }
            return new ConfigProvider();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var provider = (ConfigProvider)value;
            // serializer.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            writer.WriteStartObject();
            writer.WritePropertyName("Name");
            serializer.Serialize(writer, provider.Name);
            if (provider.Config != null && provider.Config.Count > 0)
            {
                writer.WritePropertyName("Config");
                serializer.Serialize(writer, provider.Config);
            }
            writer.WriteEndObject();

            //var g = JToken.FromObject(value, serializer);

            //   writer.WritePropertyName("Name");


        }
    }
}
