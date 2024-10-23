using FileProcessing.Domain.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessing.FileConsumerApp
{
    public class Program
    {
        private const string QueueName = "file_processing_queue";
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string baseUrl = "http://localhost:5186/"; // Consider using configuration for this

        public static async Task Main(string[] args)
        {
            await StartConsumer();
        }

        private static async Task StartConsumer()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var filePath = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"Received message: {filePath}");

                    try
                    {
                        if (File.Exists(filePath))
                        {
                            Console.WriteLine($"Processing file: {filePath}");
                            var readEndpointUrl = baseUrl + "api/files/read";
                            var readResponse = await _httpClient.PostAsJsonAsync(readEndpointUrl, new { FilePath = filePath });

                            if (!readResponse.IsSuccessStatusCode)
                            {
                                Console.WriteLine($"Failed to read file: {readResponse.ReasonPhrase}");
                                return; // Early return if reading fails
                            }

                            var updateEndpointUrl = baseUrl + "api/files/updatestatus";
                            var updateResponse = await _httpClient.PostAsJsonAsync(updateEndpointUrl, new { Status = (int)FileStatus.Completed });

                            if (!updateResponse.IsSuccessStatusCode)
                            {
                                Console.WriteLine($"Failed to update status for {filePath}: {updateResponse.ReasonPhrase}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"File not found: {filePath}");
                            throw new FileNotFoundException(filePath);
                        }

                        // Acknowledge the message after processing
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        Console.WriteLine("Message acknowledged and file processed.");
                    }
                    catch (FileNotFoundException fnfEx)
                    {
                        Console.WriteLine($"File not found: {fnfEx.Message}");
                    }
                    catch (HttpRequestException httpEx)
                    {
                        Console.WriteLine($"HTTP request error: {httpEx.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing file: {ex.Message}");
                    }
                };

                channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
                Console.WriteLine("Waiting for messages...");
                Console.ReadLine();
            }
        }
    }
}
