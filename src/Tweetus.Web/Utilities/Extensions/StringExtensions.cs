using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Tweetus.Web.Utilities.Extensions
{
    public static class StringExtensions
    {
        public static ObjectId ToObjectId(this string str)
        {
            return new ObjectId(str);
        }
    }
}
