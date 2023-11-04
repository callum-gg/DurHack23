using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace spellCheck
{
    internal class Program
    {
        static async Task Main()
        {
            // URL of your locally hosted web server
            string url = "http://localhost:8081/v2/check"; // Replace with your server's URL

            // Create a dictionary to hold key-value pairs
            var formData = new Dictionary<string, string>
        {
            { "language", "en-GB" },
            { "text", "Check this text for spelling mistkes please." },
            // Add more key-value pairs as needed
        };

            // Create an instance of HttpClient
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    // Create content with the key-value pairs
                    var content = new FormUrlEncodedContent(formData);

                    // Send a POST request to the server
                    HttpResponseMessage response = await httpClient.PostAsync(url, content);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("POST request successful!");
                        Console.WriteLine("Response from the server:");
                        Console.WriteLine(responseBody);
                    }
                    else
                    {
                        Console.WriteLine($"POST request failed with status code: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"POST request failed: {e.Message}");
                }
            }
        }
    }
}
