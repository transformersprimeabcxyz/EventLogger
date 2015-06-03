using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Diagnostics.Config
{
    public class ConfigValueConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (true);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);

            switch (token.Type)
            {
                case JTokenType.String: return new ConfigValue() { Default = token.ToString() }; //token.Values<string>().ToString() };
                case JTokenType.Integer: return new ConfigValue() { Default = token.Value<int>().ToString() };
                case JTokenType.Object:
                    var retObject = new ConfigValue();
                    foreach (JProperty p in ((JObject)token).Properties())
                    {
                        var value = p.Value;

                        switch (p.Name.ToUpperInvariant())
                        {
                            case "DEFAULT": retObject.Default = p.Value.ToString();
                                break;
                            case "CONFIGKEY":
                                retObject.ConfigKey = p.Value.ToString();
                                break;
                        }
                    }
                    return retObject;
                default:
                    return new ConfigValue()
                    {
                        Default = token.Value<object>().ToString()
                    };

            }

        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var ci = (ConfigValue)value;
            if (!string.IsNullOrWhiteSpace(ci.Default) && string.IsNullOrWhiteSpace(ci.ConfigKey))
            {
                serializer.Serialize(writer, ci.Default);
            }
            else
            {
                writer.WriteStartObject();
                writer.WritePropertyName("Default");
                serializer.Serialize(writer, ci.Default);
                writer.WritePropertyName("ConfigKey");
                serializer.Serialize(writer, ci.ConfigKey);
                writer.WriteEndObject();
            }
        }
    }

}
