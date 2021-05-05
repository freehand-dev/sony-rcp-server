using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sony_rcp_server.Models
{
    public class BasicResult<T>
    {
        public T Result { get; set; }

        public BasicResult(T value)
        {
            this.Result = value;
        }
    }
}
