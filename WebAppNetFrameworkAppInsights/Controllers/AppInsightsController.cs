using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace WebAppNetFrameworkAppInsights.Controllers
{
    // https://learn.microsoft.com/en-us/azure/azure-monitor/app/api-custom-events-metrics
    // https://learn.microsoft.com/en-us/azure/azure-monitor/app/asp-net-dependencies
    // https://learn.microsoft.com/en-us/azure/azure-monitor/app/custom-operations-tracking

    public class AppInsightsController : ApiController
    {
        private TelemetryClient _telemetry = new TelemetryClient();

        // GET api/<controller>

        public async Task<IHttpActionResult> Get()  
        {

            try
            {
                
                // Custom Events
                _telemetry.TrackEvent("Get Event");

                // Custom metrics, traces etc.
                var sample = new MetricTelemetry
                {
                    Name = "queueLength",
                    Sum = 42.3
                };
                _telemetry.TrackMetric(sample);


                var sw = new Stopwatch();
                sw.Start();
                // Wywoanie serwisu WCF
                await Task.Delay(5);
                sw.Stop();

                _telemetry.TrackRequest("Odwolanie do SAP", DateTimeOffset.Now, sw.Elapsed, "200", true);
                

                _telemetry.TrackPageView("Page XYZ visited");

                // ???
                _telemetry.TrackTrace("Slow database response",
                    SeverityLevel.Warning,
                    new Dictionary<string, string> { { "database", "123" } });


                // simple dependency tracking
                var dependency = new DependencyTelemetry
                {
                    Name = "WCF GetOrders",
                    Target = "OrderProcessingService",
                    Data = "OrderID: 1",
                    Success = true,
                    Duration = TimeSpan.FromSeconds(3),
                };

                _telemetry.TrackDependency(dependency);



                // advanced dependency tracking

                var requestTelemetry = new RequestTelemetry
                {
                    Name = $"External-System",
                    Source = "Caller Name/ID",
                    Id = Guid.NewGuid().ToString(), // request ID
                };
                requestTelemetry.Context.Operation.Id = Guid.NewGuid().ToString();
                requestTelemetry.Context.Operation.ParentId = this.Request.GetCorrelationId().ToString();

                var operationTelemetry = new DependencyTelemetry
                {
                    Name = $"External-System-XYZ",
                    Id = Guid.NewGuid().ToString(), // request ID
                    Type = "HTTP",
                    Target = "http://external-system",
                    ResultCode = "200",
                    Data = "operation payload",
                    Properties = { { "property1", "value1" } }

                };

                var operation = _telemetry.StartOperation(operationTelemetry);

                await Task.Delay(555);

                _telemetry.StopOperation(operation);



            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
            
            return Ok();
        }


    }
}
