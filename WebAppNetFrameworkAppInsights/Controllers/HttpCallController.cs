using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebAppNetFrameworkAppInsights.Controllers
{
    public class HttpCallController : ApiController
    {
        // GET api/<controller>
        public async Task<IHttpActionResult> Get()
        {
            var httpClient = new HttpClient();

            var response = await httpClient.GetAsync("https://www.google.com");
                 
            return Ok($"Response status code: {response.StatusCode}");
        }
    }
}