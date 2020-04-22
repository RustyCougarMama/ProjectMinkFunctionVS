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
using Newtonsoft.Json.Linq;

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
            //JObject json = JObject.Parse(messageString);
            var temp = JObject.Parse(messageString)["temperature"].ToString().Replace(',', '.');
            var humid = JObject.Parse(messageString)["humidity"].ToString().Replace(',', '.');
            var time = JObject.Parse(messageString)["nowTime"].ToString().Replace(',', '.');
            DateTime timeDate = DateTime.ParseExact(time, "dd/MM/yy - HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            time = timeDate.ToString("yy-MM-dd HH:mm:ss").Replace('.', ':');
            time = "20"+time;
            Console.WriteLine(time);
            //var temp = JObject.Parse(messageString)["temperature"];

            var str = Environment.GetEnvironmentVariable("sqldb_connection");
            using (SqlConnection conn = new SqlConnection(str))

            {
                conn.Open();
                Console.WriteLine("Connection is bueno");


                var text = "INSERT INTO dbo.mink_sensor_table (temperature, humidity, recordedtime)" +
                 $"VALUES ({temp}, {humid}, '{time}')";
                Console.WriteLine(text);
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