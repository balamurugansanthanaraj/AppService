using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Email
{
    public static class EmailAlertFunction
    {
        [FunctionName("Alert")]
        public static void Run([TimerTrigger("* 0/2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            var emailAlerts = new List<EmailAlert>();
            //using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("EmailAlertDbConnectionString")))
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
                        EmailSent= (bool)reader["EmailSent"]
                    };
                    emailAlerts.Add(task);
                }
            }

            foreach(var emailAlert in emailAlerts)
            {
                log.LogInformation($"email not sent for id {emailAlert.Id}");

                using (SqlConnection connection = new SqlConnection(Environment.GetEnvironmentVariable("EmailAlertDbConnectionString")))
                {
                    connection.Open();
                    var query = @"Update EmailAlerts Set EmailSent = @EmailSent where ID=@Id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmailSent", true);
                    command.Parameters.AddWithValue("@Id", emailAlert.Id);
                    command.ExecuteNonQuery();
                }
            }
           

        }
    }
}
