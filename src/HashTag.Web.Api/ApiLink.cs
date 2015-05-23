#pragma warning disable 1591
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace HashTag.Web.Api
{
    public class ApiLink
    {
        /// <summary>
        /// '_self' - following location reference will return the current resource
        /// </summary>
        public const string REL_SELF = "_self";

        /// <summary>
        /// 'parent' - in tree based models folowing location returns parent of current node
        /// </summary>
        public const string REL_PARENT = "parent";

        /// <summary>
        /// 'firstchild' - in tree based models folowing location returns first of current node
        /// </summary>
        public const string REL_FIRSTCHILD = "firstchild";

        /// <summary>
        /// 'children'  - in tree based models folowing location returns list of all children for 
        /// </summary>
        public const string REL_CHILD = "children";

        /// <summary>
        /// 'root'  - in tree based models folowing location returns root node of current node
        /// </summary>
        public const string REL_ROOT = "root";

        /// <summary>
        /// 'siblings' - in tree based models folowing location returns all children of current node
        /// </summary>
        public const string REL_SIBLING = "siblings";

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        /// <summary>
        /// Relative or absolute URI to resource location.  Start with '/' inject root path at runtime, {:id} to inject Id parameter
        /// </summary>
        [JsonProperty("href", NullValueHandling = NullValueHandling.Ignore)]
        public string Location { get; set; }

        [JsonProperty("rel", NullValueHandling = NullValueHandling.Ignore)]
        public string Relation { get; set; }

        /// <summary>
        /// Any id value that identifies this resource.  Helpful for storing id database or other external references
        /// </summary>
        [JsonProperty("id",NullValueHandling=NullValueHandling.Ignore)]
        public string Id { get; set; }

        public ApiLink Clone()
        {
            return new ApiLink()
            {
                Title = this.Title,
                Location = this.Location,
                Relation = this.Relation,
                Id = this.Id
            };
        }

        /// <summary>
        /// Performs parameter substitution on Location property and optionally create absolute url from relative one
        /// </summary>
        /// <param name="baseUrl">Uri to resource, automatically appends trailing '/' if ncessary</param>
        public void Resolve(string baseUrl=null)
        {
            if (string.IsNullOrWhiteSpace(Location))
            {
                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    return;
                }
            }
            else
            {
                 Location = Location.Replace("{:id}", Id);               
            }

            if (string.IsNullOrWhiteSpace(baseUrl) == false)
            {
                baseUrl = baseUrl.TrimEnd('/');
                var location = Location.TrimStart('/'); //remove first '/' since we'll be adding it back again with the baseUrl                
                Location = string.Format("{0}/{1}", baseUrl, location);
            }
            
        }

        public static ApiLinkBuilder NewLink()
        {
            return new ApiLinkBuilder();
        }

        public static ApiLinkBuilder NewLink(string location, string relation=null)
        {
            return ApiLinkBuilder.NewLink().Location(location).Relation(relation);
        }
    }
}
