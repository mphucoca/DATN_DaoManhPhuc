using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WH.Controllers
{
    public class TestApiController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Hello()
        {
            return Ok("Hello from Web API!");
        }
    }

}


