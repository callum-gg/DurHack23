using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Translation.V2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DurHack_2023
{
    public class Dictionary
    {
        public class MeaningInfo
        {
            public List<DefinitionInfo> Definitions { get; set; }
        }

        public class DefinitionInfo
        {
            public string Definition { get; set; }
        }

        public async Task<string> GetDefinition(string wordToTranslate)
        {
            // Base URL
            string baseUrl = "https://api.dictionaryapi.dev/api/v2/entries/en/";
            string url = baseUrl + wordToTranslate;
            string result = "";
            Console.WriteLine(url);

            // Create an HttpClient
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Send a POST request to the URL
                    HttpResponseMessage response = await client.GetAsync(url);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content as a string
                        result = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                        return $"Request failed with status code: {response.StatusCode}";
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"HTTP Request Exception: {e.Message}");
                    return $"HTTP Request Exception: {e.Message}";
                }
            }

            //deal with the result here
            try
            {
                JArray jsonArray = JArray.Parse(result);
                string definition = jsonArray[0]["meanings"][0]["definitions"][0]["definition"].ToString();
                return definition;
            }
            catch (Exception e) {
                return "<err> no definition found";
            }
        }
    }
}
