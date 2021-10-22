using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Email
{
    public static class EmailAlertFunction
    {
        [FunctionName("Alert")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            var emailAlerts = new List<EmailAlert>();
            
            using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("EmailAlertDbConnectionString")))
            {
                connection.Open();
                var query = @"Select * from EmailAlerts where EmailSent = @Id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", false);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    EmailAlert task = new EmailAlert()
                    {
                        Id = (int)reader["Id"],
                        ToAddress = reader["ToAddress"].ToString(),
                        MailBody = reader["MailBody"].ToString(),
                        MailSubject = reader["MailSubject"].ToString(),
                        EmailSent = (bool)reader["EmailSent"]
                    };
                    emailAlerts.Add(task);
                }
            }
            BlobContainerClient containerClient = new BlobContainerClient(Environment.GetEnvironmentVariable("blobConnectionString"), "emails");
            foreach (var emailAlert in emailAlerts)
            {
                log.LogInformation($"Email Not sent for id = {emailAlert.Id} ; ToAddress = {emailAlert.ToAddress} ; MailBody = {emailAlert.MailBody}");

                var emailAsString = JsonConvert.SerializeObject(emailAlert);
                string fileName = $"Email-{emailAlert.Id}" + ".txt";
                // Get a reference to a blob
                BlobClient blobClient = containerClient.GetBlobClient(fileName);

                log.LogInformation("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

                // Upload data from the local file
                using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(emailAsString)))
                {
                    blobClient.Upload(stream);
                }

                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("EmailAlertDbConnectionString")))
                {
                    connection.Open();
                    var query = @"Update EmailAlerts Set EmailSent = @EmailSent where ID=@Id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmailSent", true);
                    command.Parameters.AddWithValue("@Id", emailAlert.Id);
                    command.ExecuteNonQuery();
                }

                log.LogInformation($"Email Sent for id = {emailAlert.Id} ; ToAddress = {emailAlert.ToAddress} ; MailBody = {emailAlert.MailBody}");
            }
           

        }
    }
}
