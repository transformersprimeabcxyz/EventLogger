#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashTag.Web.Api
{
    public class ApiLinkBuilder
    {
        ApiLink _link;

        public ApiLinkBuilder(ApiLink link)
        {
            _link = link;
        }

        public ApiLinkBuilder()
        {
            _link = new ApiLink();
        }

        public static ApiLinkBuilder NewLink()
        {
            return new ApiLinkBuilder();
        }

        public ApiLinkBuilder Title(string title, params object[] args)
        {
            if (title == null)
            {
                _link.Title = null;
            }
            else
            {
                _link.Title = string.Format(title, args);
            }
            return this;
        }

        public ApiLinkBuilder Location(string location, params object[] args)
        {
            if (location == null)
            {
                _link.Location = null;
            }
            else
            {
                _link.Location = string.Format(location, args);
            }
            return this;
        }

        public ApiLinkBuilder Relation(string relation, params object[] args)
        {
            if (relation == null)
            {
                _link.Relation = null;
            }
            else
            {
                _link.Relation = string.Format(relation, args);
            }
            return this;
        }
        public ApiLinkBuilder Id(string id, params object[] args)
        {
            if (id == null)
            {
                _link.Id = null;
            }
            else
            {
                _link.Id = string.Format(id, args);
            }
            return this;
        }

        public ApiLink Link
        {
            get
            {
                return _link;
            }
        }
    }
}
