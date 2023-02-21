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
using Microsoft.Azure.Documents.Linq;
using RatingFunctions.Models;
using System.Linq;
using System.Collections.Generic;

namespace RatingFunctions
{
    public static class GetRatings2
    {
        [FunctionName("GetRatings2")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "ratingsdb", collectionName: "ratings", ConnectionStringSetting = "CosmosDbConnectionString")] DocumentClient cosmosClient,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string userId = req.Query["userId"];

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri("ratingsdb", "ratings");

            var options = new FeedOptions { EnableCrossPartitionQuery = true };

            IDocumentQuery<CreateRatingResponse> query = cosmosClient.CreateDocumentQuery<CreateRatingResponse>(collectionUri, options).Where(r => r.UserId == userId).AsDocumentQuery();

            var ratingList = new List<CreateRatingResponse>();

            while (query.HasMoreResults)
            {
                foreach (CreateRatingResponse result in await query.ExecuteNextAsync())
                {
                    ratingList.Add(result);
                }
                return new OkObjectResult(ratingList);
            }


            return new NotFoundObjectResult($"No ratings found with User Id: {userId}");
        }
    }
}
