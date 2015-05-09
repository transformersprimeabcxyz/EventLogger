using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Web.Http
{
    /// <summary>
    /// List of file names and content for addding as attachments in a MultipartFormDataContent type of post
    /// </summary>
    public class RestFile:Dictionary<string,byte[]>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public RestFile() { }

        /// <summary>
        /// Adds a single file/attachement to list
        /// </summary>
        /// <param name="fileName">name.ext of file being sent.  ext often used by receiving service and thus should be used</param>
        /// <param name="content">Binary content to send</param>
        public RestFile(string fileName, byte[] content)
        {
            base[fileName] = content;
        }

        /// <summary>
        /// Allows for inline initialization e.g. new RestFile(){{file1,bytes},{file2,bytes}};
        /// </summary>
        /// <param name="fileName">name.ext of file being sent.  ext often used by receiving service and thus should be used.  You can use a  dummy filename but always use a valid extension that maps to file's type</param>
        /// <param name="content">Binary content to send</param>
        public RestFile Add(string fileName, byte[] content)
        {
            base[fileName] = content;
            return this;
        }
    }
}
