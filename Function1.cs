using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AchrafFirstAzureFucntion.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace AchrafFirstAzureFucntion
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log )
        {
            TransactionData _TransactionData = new TransactionData();

            log.LogInformation("C# HTTP trigger function processed a request.");


            //param in
            string ID = req.Query["Id"];
            string Name = req.Query["Name"];
            string TrId = req.Query["TrId"];
            string ReId = req.Query["ReId"];


            if (!string.IsNullOrEmpty(ID))
            {
                _TransactionData.ID = Int32.Parse(ID);
                _TransactionData.Name = Name;
                _TransactionData.ReId = ReId;
                _TransactionData.TrId = TrId;

                string namefile;

                namefile = Guid.NewGuid().ToString("n");

                await CreateBlob(namefile + ".json", _TransactionData, log);

            }


            return _TransactionData.ID != null && _TransactionData.ID != 0
                ? (ActionResult)new OkObjectResult($"Hello, { _TransactionData.ID} { _TransactionData.Name} the time now is :"+ System.DateTime.Now.Date 
                                                   +Environment.NewLine + JsonConvert.SerializeObject( _TransactionData))
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        private async static Task CreateBlob(string name, TransactionData data, ILogger log)
        {

            string connectionString;
            CloudStorageAccount storageAccount;
            CloudBlobClient client;
            CloudBlobContainer container;
            CloudBlockBlob blob;

            
            connectionString = "DefaultEndpointsProtocol=";
            storageAccount = CloudStorageAccount.Parse(connectionString);
            client = storageAccount.CreateCloudBlobClient();
            container = client.GetContainerReference("Demo");
            await container.CreateIfNotExistsAsync();
            blob = container.GetBlockBlobReference(name);
            blob.Properties.ContentType = "application/json";
            blob.UploadFromStreamAsync(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data))));


        }




    }
}
