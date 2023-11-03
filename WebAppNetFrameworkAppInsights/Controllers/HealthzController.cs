using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebAppNetFrameworkAppInsights.Controllers
{
    public class HealthzController : ApiController
    {
        // Simple health check endpoint

        public IHttpActionResult Get()
        {
            return Ok();
        }
    }
}
