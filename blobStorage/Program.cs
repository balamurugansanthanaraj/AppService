using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace blobStorage
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Azure Blob Storage v12 - .NET quickstart sample\n");

            //string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            string connectionString = $"DefaultEndpointsProtocol=https;AccountName=allartifacts;AccountKey=1lAhBbLm5eI/b8dPcWRVYROFJzo29sAKi6uST4iAWAvYY1AP0WlyAE36qxOk91Tjim1skfykIXT7OMFzXqIqzA==;EndpointSuffix=core.windows.net";

            // Create a BlobServiceClient object which will be used to create a container client
            //BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            //Create a unique name for the container
            string containerName = "quickstartblobs" + Guid.NewGuid().ToString();

            // Create the container and return a container client object
            //BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
            BlobContainerClient containerClient = new BlobContainerClient(connectionString, "emails");

            //// Create a local file in the ./data/ directory for uploading and downloading
            //string localPath = $"D:\\pocs\\AppService\\blobStorage";
            //string fileName = "quickstart" + Guid.NewGuid().ToString() + ".txt";
            //string localFilePath = Path.Combine(localPath, fileName);

            //// Write text to the file
            //await File.WriteAllTextAsync(localFilePath, "Hello, World!");

            var emails = new List<EmailAlert>();
            emails.Add(new EmailAlert
            {
                Id = 10,
                ToAddress = "Hello",
                MailBody = "bye"
            });

           

            foreach(var email in emails)
            {
                var emailAsString = JsonConvert.SerializeObject(email);
                string fileName = $"Email-{email.Id}" + ".txt";
                // Get a reference to a blob
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);
                
                // Upload data from the local file
                using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(emailAsString)))
                {
                    await blobClient.UploadAsync(stream);
                }
            }
     

            Console.WriteLine("Listing blobs...");

            // List all blobs in the container
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                Console.WriteLine("\t" + blobItem.Name);
            }
        }
    }
}
