using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using processing;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DurHack_2023.SimpleHttpServer;

namespace DurHack_2023
{

    public class SimpleHttpServer
    {

        private HttpListener listener;
        public SimpleHttpServer(string uriPrefix)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(uriPrefix);
        }

        public async Task StartAsync()
        {
            listener.Start();
            Console.WriteLine("Listening for connections on " + string.Join(", ", listener.Prefixes));

            while (listener.IsListening)
            {
                try
                {
                    // Note: GetContextAsync() waits for a client to connect.
                    HttpListenerContext context = await listener.GetContextAsync();
                    await ProcessRequestAsync(context);
                }
                catch (HttpListenerException) // Handle the exception when the HttpListener is stopped.
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught: " + ex);
                    break;
                }
            }
        }

        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            HttpListenerResponse response = context.Response;
            string jsonString;
            string lang = "fr";

            try
            {
                // Enable CORS
                response.AppendHeader("Access-Control-Allow-Origin", "*");
                response.AppendHeader("Access-Control-Allow-Methods", "POST, OPTIONS");
                response.AppendHeader("Access-Control-Allow-Headers", "Content-Type");

                if (context.Request.HttpMethod == "OPTIONS")
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    await response.OutputStream.FlushAsync();
                }
                if (context.Request.HttpMethod == "GET")
                {
                    // Retrieve the word from the query string
                    string word = context.Request.QueryString["word"];

                    // Here, you'd typically process the word and create a response
                    // For demonstration, we're just echoing the word back in a message
                    string message = $"Received word: {word}";

                    // Create the response
                    byte[] buffer = Encoding.UTF8.GetBytes(message);
                    response.ContentLength64 = buffer.Length;
                    response.StatusCode = (int)HttpStatusCode.OK;
                    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                } else if (context.Request.HttpMethod == "POST")

                {
                    using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                    {
                        string json = await reader.ReadToEndAsync();

                        // Log the received JSON string to the console

                        RequestData requestData = JsonSerializer.Deserialize<RequestData>(json);

                        // Now you can access the mode and img properties

                        // The img property contains a base64 string of the image
                        // Depending on the length, you might not want to log the entire string
                        // Console.WriteLine($"Image Base64: {requestData.Img}");

                        // Process the image based on the mode

                        Process process = new Process();
                        List<ParsedWord> words = await process.ProcessImage(requestData.Img);

                        ImageProcessing imageProcessingInstance = new ImageProcessing(requestData.Img);
                        imageProcessingInstance.Test();


                        switch (requestData.Mode)
                        {
                            case "spell":



                                jsonString = JsonSerializer.Serialize(words);
                                response.StatusCode = (int)HttpStatusCode.OK;

                                byte[] buffer_spell = Encoding.UTF8.GetBytes(jsonString);

                                response.ContentLength64 = buffer_spell.Length;
                                await response.OutputStream.WriteAsync(buffer_spell, 0, buffer_spell.Length);

                                break;
                            case "french":
                                lang = "fr";
                                break;
                            case "spanish":
                                // Process for Spanish mode
                                lang = "es";

                                break;
                            case "mandarin":
                                // Process for Mandarin mode
                                lang = "zh-CN";

                                break;
                            default:
                                // Handle unknown mode
                                break;
                        }


                        Console.WriteLine(words);

                        //testing the translator, change "fr" for different laguages
                        Translate Translator = new Translate();
                        string translatedOutput = await Translator.TranslateText(lang, words);


                        // Create an object that includes both the original words and the translation
                        var dataToSerialize = new
                        {
                            OriginalWords = words,
                            TranslatedOutput = translatedOutput
                        };

                        // Serialize the new object to JSON
                        jsonString = JsonSerializer.Serialize(dataToSerialize);
                        response.StatusCode = (int)HttpStatusCode.OK;

                        byte[] buffer_spell_tran = Encoding.UTF8.GetBytes(jsonString);

                        response.ContentLength64 = buffer_spell_tran.Length;
                        await response.OutputStream.WriteAsync(buffer_spell_tran, 0, buffer_spell_tran.Length);

                    }



                    
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                response.OutputStream.Close();
            }
        }

        public void Stop()
        {
            listener.Stop();
            listener.Close();
        }
        public class RequestData
        {
            public string Mode { get; set; }
            public string Img { get; set; } // This will contain the entire data URI
        }

    }

    class run_server
    {
        static async Task Main_enter(string[] args)
        {
            string uriPrefix = "http://localhost:8080/";

            SimpleHttpServer server = new SimpleHttpServer(uriPrefix);

            try
            {
                await server.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Server failed to start: " + ex.Message);
            }
        }
    }
}