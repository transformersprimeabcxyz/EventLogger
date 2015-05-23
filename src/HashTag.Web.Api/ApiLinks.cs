#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace HashTag.Web.Api
{
    /// <summary>
    /// List of reference links to return to caller
    /// </summary>
    public class ApiLinks : List<ApiLink>
    {
        /// <summary>
        /// Add a new link emtpy to collection.  Supply builder arguments to hydrate link.
        /// </summary>
        /// <returns></returns>
        public ApiLinkBuilder Add()
        {
            var newLink = new ApiLink();
            base.Add(newLink);

            return new ApiLinkBuilder(newLink);
        }

        /// <summary>
        /// Add a new link to collection
        /// </summary>
        /// <param name="location">Absolute or relative path to resource of link</param>
        /// <param name="relation">Relationship of current resource to linked resource (optional)</param>
        /// <returns></returns>
        public ApiLinkBuilder Add(string location, string relation=null)
        {
            return Add().Location(location).Relation(relation);
        }

        /// <summary>
        /// Add a new link to colltion
        /// </summary>
        /// <param name="location">Absolute or relative path to resource of link</param>
        /// <param name="relation">Relationship of current resource to linked resource (optional)</param>
        /// <param name="id">Identifier of this resource.  Makes consuming link easier than parsing id from Uri</param>
        /// <returns></returns>
        public ApiLinkBuilder Add(string location, string relation,string id)
        {
            return Add().Location(location).Relation(relation).Id(id);
           
        }

        /// <summary>
        /// Inject fully qualified path into list of links and replace any embedded tokens
        /// </summary>
        /// <param name="serviceRootUrl">Fully quaified path to root of link path</param>
        /// <returns></returns>
        public ApiLinks Resolve(string serviceRootUrl)
        {
            
            for (int x = 0; x < this.Count; x++)
            {
                this[x].Resolve(serviceRootUrl);
            }
            return this;
        }

    }
}
