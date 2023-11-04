using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System.Net.Http;

namespace DurHack_2023
{
    public class Coords
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    public class ParsedWord
    {
        public string Text { get; set; }
        public List<Coords> Coords { get; set; }
        public List<string> Corrections {  get; set; }
    }
    public class OCR
    {
        public async Task<(string, List<ParsedWord>)> ReadImage(string image)
        {
            {
                // Create an instance of HttpClient
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        // Specify the URL you want to send the GET request to
                        string url = "https://vision.googleapis.com/v1/images:annotate?key=AIzaSyAZHbnnwYj9faJFNJyvfyahqNnmmANq9Go";

                        // Create a data object to send in the request (e.g., JSON data)
                        string jsonData = $"{{" +
                            $"\"requests\":  [ {{" +
                                $"\"image\": {{" +
                                   $"\"content\": \"{image}\"" +
                                $"}}," +
                                $"\"features\":  [" +
                                    $"{{ \"type\": \"TEXT_DETECTION\"}}" +
                                $"]" +
                            $"}} ]" +
                        $"}}";

                        HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                        // Send the GET request and get the response
                        HttpResponseMessage response = await client.PostAsync(url, content);

                        // Check if the request was successful
                        if (response.IsSuccessStatusCode)
                        {
                            // Read the content of the response
                            string responseContent = await response.Content.ReadAsStringAsync();

                            Root root = JsonSerializer.Deserialize<Root>(responseContent);
                            List<ParsedWord> words = new List<ParsedWord>();
                            for (int i = 1; i < root.responses[0].textAnnotations.Count; i++)
                            {
                                List<Coords> coords = new List<Coords>();
                                for (int j = 0; j < 4; j++)
                                {
                                    Coords coord = new Coords();
                                    coord.X = root.responses[0].textAnnotations[i].boundingPoly.vertices[j].x;
                                    coord.Y = root.responses[0].textAnnotations[i].boundingPoly.vertices[j].y;
                                    coords.Add(coord);
                                }

                                ParsedWord word = new ParsedWord();
                                word.Text = root.responses[0].textAnnotations[i].description;
                                word.Coords = coords;
                                word.Corrections = new List<string>();

                                words.Add(word);
                            }

                            string text = root.responses[0].textAnnotations[0].description;

                            return (text, words);
                        }
                        else
                        {
                            Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                            Console.WriteLine($"{response.Content}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                    throw new Exception("An error occured");
                }
            }
        }
    }
}
