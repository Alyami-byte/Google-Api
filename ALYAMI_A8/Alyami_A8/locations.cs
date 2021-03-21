using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace Alyami_A8
{
    public static class locations
    {


        [FunctionName("locations")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)

        {
            log.LogInformation("C# HTTP trigger function processed a request.");
           
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
               
            string location = req.Query["location"];

            location = location ?? data?.location;
           
            
            WebRequest client = WebRequest.CreateHttp(createApiUrl(location));
           
            if (location != null)
            {
                var reqest = client.GetResponse();
                var response = reqest.GetResponseStream();
                string content = new StreamReader(response).ReadToEnd();

                var parsedObj = JsonConvert.DeserializeObject<GooleLocation>(content);
              
                var locationData = parsedObj.results?[0].geometry.location;

             
               return new OkObjectResult(locationData);

            }  else
                {
                    return new BadRequestObjectResult("Location wasn not found");
                }
        }

        private static String createApiUrl(String location)
        {
            String url = String.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}",
                 Uri.EscapeUriString(location), Key);
            Console.WriteLine(url);
            return url;
        }
    }
}
