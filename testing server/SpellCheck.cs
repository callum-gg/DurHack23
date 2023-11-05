using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
// using Google.Cloud.Translation.V2;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DurHack_2023
{
    public class Mistake
    {
        public string Message { get; set; }
        public int Index { get; set; }
        public int Length { get; set; }
        public List<string> Corrections { get; set; }
    }
    public class SpellCheck
    {
        public async Task<List<Mistake>> CheckSpell(string text)
        {
            // URL of your locally hosted web server
            string url = "http://localhost:8081/v2/check"; // Replace with your server's URL

            // Create a dictionary to hold key-value pairs
            var formData = new Dictionary<string, string>
        {
            { "language", "en-GB" },
            { "text", text },
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

                        Match[] matches = JsonSerializer.Deserialize<RootObject>(responseBody).matches;

                        List<Mistake> mistakes = new List<Mistake>();
                        for (int i = 0; i < matches.Count(); i++)
                        {
                            Mistake mistake = new Mistake();
                            mistake.Message = matches[i].message;
                            mistake.Index = matches[i].offset;
                            mistake.Length = matches[i].length;

                            List<string> corrections = new List<string>();
                            for (int j = 0; j < matches[i].replacements.Count(); j++)
                            {
                                corrections.Add(matches[i].replacements[j].value);
                            }
                            mistake.Corrections = corrections;
                            mistakes.Add(mistake);
                        }

                        return mistakes;
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
                throw new Exception("An error occured");
            }
        }

        public class RootObject
        {
            public Match[] matches { get; set; }
        }

        public class Match
        {
            public string message { get; set; }
            public string shortMessage { get; set; }
            public int offset { get; set; }
            public int length { get; set; }
            public Replacement[] replacements { get; set; }
        }

        public class Replacement
        {
            public string value { get; set; }
        }
    }
}