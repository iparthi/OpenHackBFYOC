using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using RatingFunctions.Models;
using Microsoft.Azure.Documents.Linq;
using System.Linq;

namespace RatingFunctions
{
    public static class GetRating2
    {
        [FunctionName("GetRating2")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "ratingsdb", collectionName: "ratings", ConnectionStringSetting = "CosmosDbConnectionString")] DocumentClient cosmosClient,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string ratingId = req.Query["ratingId"];

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("ratingsdb", "ratings");

            IDocumentQuery<CreateRatingResponse> query = cosmosClient.CreateDocumentQuery<CreateRatingResponse>(collectionUri).Where(r => r.Id == ratingId).AsDocumentQuery();

            while (query.HasMoreResults)
            {
                foreach (CreateRatingResponse result in await query.ExecuteNextAsync())
                {
                    return new OkObjectResult(result);
                }
            }


            return new NotFoundObjectResult($"No rating found with Rating Id: {ratingId}");
        }
    }
}
