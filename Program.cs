using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Google.Cloud.Translation.V2;
using Newtonsoft.Json;

namespace spellCheck
{
    internal class Program
    {
        static async Task Main()
        {
            string json = "";

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
                        json = responseBody;
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
            //string json = "{\"software\":{\"name\":\"LanguageTool\",\"version\":\"6.3\",\"buildDate\":\"2023-10-06 20:34:38 +0000\",\"apiVersion\":1,\"premium\":false,\"premiumHint\":\"You might be missing errors only the Premium version can find. Contact us at support<at>languagetoolplus.com.\",\"status\":\"\"},\"warnings\":{\"incompleteResults\":false},\"language\":{\"name\":\"English (GB)\",\"code\":\"en-GB\",\"detectedLanguage\":{\"name\":\"English (US)\",\"code\":\"en-US\",\"confidence\":0.9999926,\"source\":\"+fallback\"},\"matches\":[{\"message\":\"Possible spelling mistake found.\",\"shortMessage\":\"Spelling mistake\",\"replacements\":[{\"value\":\"mistakes\"},{\"value\":\"misTKOs\"}],\"offset\":29,\"length\":7,\"context\":{\"text\":\"Check this text for spelling mistkes please.\",\"offset\":29,\"length\":7},\"sentence\":\"Check this text for spelling mistkes please.\",\"type\":{\"typeName\":\"UnknownWord\"},\"rule\":{\"id\":\"MORFOLOGIK_RULE_EN_GB\",\"description\":\"Possible spelling mistake\",\"issueType\":\"misspelling\",\"category\":{\"id\":\"TYPOS\",\"name\":\"Possible Typo\"}},\"ignoreForIncompleteSentence\":false,\"contextForSureMatch\":0}],\"sentenceRanges\":[[0,44]]}";

            try
            {
                Match[] matches = JsonConvert.DeserializeObject<RootObject>(json).matches;

                foreach (Match match in matches)
                {
                    Console.WriteLine($"Message: {match.message}");
                    Console.WriteLine($"Replacements:");
                    foreach (Replacement replacement in match.replacements)
                    {
                        Console.WriteLine($"  - {replacement.value}");
                    }
                }
            }
            catch (JsonException e)
            {
                Console.WriteLine($"JSON parsing error: {e.Message}");
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
            public Replacement[] replacements { get; set; }
        }

        public class Replacement
        {
            public string value { get; set; }
        }
    }
}
