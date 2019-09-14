using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace FaceGraph.CMD
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Azure Blob Storage - .NET quickstart sample\n");

            // Run the examples asynchronously, wait for the results before proceeding
            ProcessAsync().GetAwaiter().GetResult();

            Console.WriteLine("Press any key to exit the sample application.");
            Console.ReadLine();
        }

        private static async Task ProcessAsync()
        {
            // Retrieve the connection string for use with the application. The storage 
            // connection string is stored in an environment variable on the machine 
            // running the application called CONNECT_STR. If the 
            // environment variable is created after the application is launched in a 
            // console or with Visual Studio, the shell or application needs to be closed
            // and reloaded to take the environment variable into account.
            string storageConnectionString = Environment.GetEnvironmentVariable("CONNECT_STR");

            // Check whether the connection string can be parsed.
            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                // If the connection string is valid, proceed with operations against Blob
                // storage here.
                // ADD OTHER OPERATIONS HERE
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer =cloudBlobClient.GetContainerReference("images");
                // List the blobs in the container.
                Console.WriteLine("List blobs in container.");
                BlobContinuationToken blobContinuationToken = null;
                do
                {
                    var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                    // Get the value of the continuation token returned by the listing call.
                    blobContinuationToken = results.ContinuationToken;
                    foreach (IListBlobItem item in results.Results)
                    {
                        Console.WriteLine(item.Uri);
                    }
                } while (blobContinuationToken != null); // Loop while the continuation token is not null.


                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Connection is correct!");
            }
            else
            {
                // Otherwise, let the user know that they need to define the environment variable.
                Console.WriteLine(
                    "A connection string has not been defined in the system environment variables. " +
                    "Add an environment variable named 'CONNECT_STR' with your storage " +
                    "connection string as a value.");
                Console.WriteLine("Press any key to exit the application.");
                Console.ReadLine();
            }
        }
    }
}
