using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using System;

using System.Data.SqlClient;
using System.Threading.Tasks;

namespace MinkFunctionVisualStudio
{
    public static class Function1
    {
        private static HttpClient client = new HttpClient();
        [FunctionName("Function1")]
        public static async Task Run([IoTHubTrigger("messages/events", Connection = "ConnectionString")]EventData message, ILogger log)
        {
            string messageString = Encoding.UTF8.GetString(message.Body.Array);
            log.LogInformation($"C# IoT Hub trigger function processed a message: {messageString}");
            Console.WriteLine(messageString);

            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            using (SqlConnection conn = new SqlConnection(str)) 
            {
                conn.Open();
                Console.WriteLine("Connection is bueno");
                var text = "UPDATE dbo.test_table " +
                "SET deviceColour = 'yellow'  WHERE deviceColour = 'blue';";
                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    // Execute the command and log the # rows affected.
                    var rows = await cmd.ExecuteNonQueryAsync();
                    log.LogInformation($"{rows} rows were updated");
                }
            }

            }


        
    }
}