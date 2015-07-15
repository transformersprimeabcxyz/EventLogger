using System;
using Newtonsoft.Json;

namespace HashTag.Diagnostics.Models
{
    /// <summary>
    /// Defines a group-name-value entry
    /// </summary>
    public class LogEventProperty
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public LogEventProperty()
        {
            UUID = Guid.NewGuid();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="group">Group this pair belongs to</param>
        /// <param name="name">Name to add to this pair</param>
        /// <param name="value">Value to add to this pair</param>
        public LogEventProperty(string group, string name, string value):this()
        {
            Group = group;
            Value = value;
            Name = name;
        }

        /// <summary>
        /// Constructor (leaving Group=null)
        /// </summary>
        /// <param name="name">Name to add to this pair</param>
        /// <param name="value">Value to add to this pair</param>
        public LogEventProperty(string name, string value)
            : this(null,name,value)
        {
            
        }

        /// <summary>
        /// Uniqueue identifier for this 3-tuplie
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid UUID { get; set; }

        /// <summary>
        /// Event this property belongs to.
        /// </summary>
        [JsonProperty(PropertyName = "eventId")]
        public Guid EventUUID { get; set; }

        /// <summary>
        /// Group the key-value belongs to
        /// </summary>
        [JsonProperty(PropertyName = "group", NullValueHandling = NullValueHandling.Ignore)]
        public string Group { get; set; }


        /// <summary>
        /// Name (or 'key') of this value
        /// </summary>
        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        

        /// <summary>
        /// Value associated with the group+name pair
        /// </summary>
        [JsonProperty(PropertyName = "value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }


        /// <summary>
        /// Returns well formatted textual representation of this 3-tuple
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[{0}::{1}] = {2}", Group, Name, Value);
        }
    }

   
}
