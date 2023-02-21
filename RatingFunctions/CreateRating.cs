using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RatingFunctions.Models;
using System.Net.Http;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Linq;
using System.Web.Http;

namespace RatingFunctions
{
    public static class CreateRating
    {
        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [CosmosDB("ratingsdb", "ratings", ConnectionStringSetting = "CosmosDbConnectionString")] IAsyncCollector<CreateRatingRequest> createRatingCollector,
            [CosmosDB(databaseName: "ratingsdb", collectionName: "ratings", ConnectionStringSetting = "CosmosDbConnectionString")] DocumentClient cosmosClient,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<CreateRatingRequest>(requestBody);

            var client = new HttpClient();
            var userCheckResult = await client.GetAsync($"https://serverlessohapi.azurewebsites.net/api/GetUser?userId={data.UserId}");
            if (!userCheckResult.IsSuccessStatusCode)
            {
                return new NotFoundObjectResult("User not found");
            }

            var productCheckResult = await client.GetAsync($"https://serverlessohapi.azurewebsites.net/api/GetProduct?productId={data.ProductId}");
            if (!productCheckResult.IsSuccessStatusCode)
            {
                return new NotFoundObjectResult("Product not found");
            }

            data.Id = Guid.NewGuid().ToString();

            await createRatingCollector.AddAsync(data);


            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("ratingsdb", "ratings");
            
            IDocumentQuery<CreateRatingResponse> query = cosmosClient.CreateDocumentQuery<CreateRatingResponse>(collectionUri).Where(r => r.Id == data.Id).AsDocumentQuery();

            while (query.HasMoreResults)
            {
                foreach (CreateRatingResponse result in await query.ExecuteNextAsync())
                {
                    return new OkObjectResult(result);
                }
            }

            return new BadRequestObjectResult("There is a problem in processing this request");

        }
    }
}
