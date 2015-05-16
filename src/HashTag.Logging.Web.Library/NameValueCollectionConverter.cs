using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTag.Logging.Web.Library
{
    internal class NameValuePair
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class NameValueCollectionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is NameValueCollection))
            {
                return;
            }

            var collection = (NameValueCollection)value;
            var container = collection.AllKeys.Select(key => new NameValuePair
            {
                Name = key,
                Value = collection[key]
            }).ToList();

            var serialized = JsonConvert.SerializeObject(container);

            writer.WriteRawValue(serialized);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object originalValue, JsonSerializer serializer)
        {
            return reader.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            var t = (IsNullableType(objectType))
                        ? Nullable.GetUnderlyingType(objectType)
                        : objectType;

            return typeof(NameValueCollection).IsAssignableFrom(t);
        }

        public static bool IsNullable(Type type)
        {
            return type != null && (!type.IsValueType || IsNullableType(type));
        }

        public static bool IsNullableType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            return (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}
