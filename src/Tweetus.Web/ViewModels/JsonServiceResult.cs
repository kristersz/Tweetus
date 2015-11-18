using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tweetus.Web.ViewModels
{
    public class JsonServiceResult<T>
    {
        public T Value { get; set; }
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }
}
