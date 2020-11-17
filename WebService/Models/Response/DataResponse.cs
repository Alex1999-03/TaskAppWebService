using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebService.Models.Response
{
    public class DataResponse
    {
        public int Success { get; set; }
        public string Message { get; set; }
        public Object Data { get; set; }

        public DataResponse()
        {
            this.Success = 0;
        }
    }
}
