using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetus.Web.Data;

namespace Tweetus.Web.Managers
{
    public class BaseManager
    {
        protected IMongoDbRepository _repository;

        public BaseManager(IMongoDbRepository repository)
        {
            _repository = repository;
        }
    }
}
