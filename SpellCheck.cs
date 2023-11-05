using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

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
            string url = "https://api.bing.microsoft.com/v7.0/spellcheck"; // Replace with your server's URL

            // Create a dictionary to hold key-value pairs
            var formData = new Dictionary<string, string>
            {
                { "text", text },
            };

            // Create an instance of HttpClient
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    // Create content with the key-value pairs
                    var content = new FormUrlEncodedContent(formData);

                    // Add custom headers to the request
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "6e2302c0e20b4c4095cc7a793a059b3a");

                    // Send a POST request to the server
                    HttpResponseMessage response = await httpClient.PostAsync(url, content);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        FlaggedTokens[] matches = JsonSerializer.Deserialize<RootObject>(responseBody).flaggedTokens;

                        List<Mistake> mistakes = new List<Mistake>();
                        for (int i = 0; i < matches.Count(); i++)
                        {
                            Mistake mistake = new Mistake();
                            mistake.Index = matches[i].offset;
                            mistake.Length = matches[i].token.Length;

                            List<string> corrections = new List<string>();
                            for (int j = 0; j < matches[i].suggestions.Count(); j++)
                            {
                                corrections.Add(matches[i].suggestions[j].suggestion); 
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
            public FlaggedTokens[] flaggedTokens { get; set; }
        }

        public class FlaggedTokens
        {
            public int offset {  get; set; }
            public string token { get; set; }
            public Suggestions[] suggestions { get; set; }
        }

        public class Suggestions
        {
            public string suggestion { get; set; }
        }
    }
}
