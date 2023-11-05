using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Google.Cloud.Translation.V2;
using Google.Apis.Auth.OAuth2;

namespace DurHack_2023
{
    public class Translate
    {
        public async Task<string> TranslateText(string targetLanguageCode, List<ParsedWord> textObjectToTranslate)
        {
            // the JSON key for vinnie's google API
            string jsonKey = @"
                {
                    ""type"": ""service_account"",
                    ""project_id"": ""durhack-2023-404115"",
                    ""private_key_id"": ""6c03507fc60cda389e51fba0a6a70e2efd1d4fda"",
                    ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDI8nGS/vZfI9NY\nBHa5du9ajKbM+ltLgc9aktYzMHPadEFX6xqlSKOS3gvvmVCJUZ6sLmqwC9v8c5hp\n2HxqHe321LUBzom2/Bjp0qnylfobgGi2OctEK99ZdzT6Gd0W0OqZefaDZ1TpJC/l\nzYRYreLDVAdkVAjZVzKnLhu8BwR0VtaWErPK5icqv4XsJDVB3R7yHleBsjkqyWRg\n9Z9vPi/R2l58OvoaW8PkoNK6cnlaQAmer3EJYeFjQuxuVjUicfOAJpqHBcHF4tKD\nEY88bB7ue9P6i8lzFjspVwk+L8H1qaJdwLquc/DsLOI0OfNLIz/DAr8MZObAurfb\n9YaWhsPTAgMBAAECggEAIi2yiakaKKaETQU9WGBVZ9ZijJzMqHQk26f8DOrh2bdK\nYfTYLGLFh7aC2zb9js22Kl0ewmDSAwd6Vg5/YuFCV2hofS0vGVOZvTOaumsuBj16\nEfyUiTzZGXOALEX35+iS7pXjCwipVZpVF0Y0i4zP9B83vqXYd3cwY2UhTIL5C/xn\n1q15pmTCrHqrkvUNTj/0sGwKmQNhLQaipswWeNsySX9wBQWMbOpbDSfuUmTgCUO/\n8RX6v49fnKaNOm7fGaJ1X0Scoc6UrkARl6dDCtdlAevjgCgWFx1XqjFncFxFFPz4\nESy+H8DexNimOWZlXHxyjX0RhZ3cExO2F6oNvpE+KQKBgQD0VeEeWz68fLDr6c8B\niI+fpz/8GX8QAHSxfqQjFP5Cd/XU8ovaeWa0gD3ayf7zGljomHCu5xqleTN6GSFR\nEAXd237BpxdmZFWy4Bz/1Tw9fvSNlX7h47cITnRl+JSTOPdOfYrDD0KENxsiwqF0\n8p256GIALFkLO8QAo5MOP3y3RwKBgQDSikwUKiCDkcFX5imPmmheX66KsUEwRoOl\n96L2WSesBTlccgQn3OFPbptWsFOAC675C2/ulVWPjiW56usiqkhH2k3ej4XZ7TZQ\n/xj3zaaGh5lW4hX4aY9ospQmKNYxzv/KQuY5G+XJRB0jIyFUUi/5a2PizQt5Y0z+\nkQGjBOntFQKBgBjI/WUO7HZCT9Aej5kgOTGAzcTM8U62PSAkMJI+duxXLuDDGdy3\no+t7dsrS4sHWUl6F2Chl2RYgDVC9a9vFPFMdCOasBHumger7QPXSs6GpVzVljNFt\nQAvfqX8OkEO+65dStNjEm4K99Eq6Y5ZxZf6NzCMoXH//OAq4jRpM5mHRAoGAbs6S\nk9pIGnbPxy8/mHCvlVvORPhdUscJ6oOWpQjvND3bnqfzP/ekLWYNI2bRy4ZapIA8\nfNjjmxY5DMxyzF/KguaGLShkTfnmqGJaBbGafxtGvM9ouMD11SiHiD3d3YEQZqcm\n+8+O9IqAu0l19Sb8UM0QWQGJlG2KzEANGHsxkrECgYEAkkpwvBVfyJSoxKejG6MO\n/Jw4vJOol1hF0rMm1t5ATrWQtDcK2v9OWyi1pBMxck2N/UtsMxOyovDR4Ste7peB\nuPOIq8fxf7wb/wzExOY349dblKndqZHm9NgaW14+sYfpe4YgddEUD0nMmhON/dlg\nX2nDakOWQFw+vcXK+LaLXHc=\n-----END PRIVATE KEY-----\n"",
                    ""client_email"": ""durhacktranslate@durhack-2023-404115.iam.gserviceaccount.com"",
                    ""client_id"": ""103886179625779832897"",
                    ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
                    ""token_uri"": ""https://oauth2.googleapis.com/token"",
                    ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
                    ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/durhacktranslate%40durhack-2023-404115.iam.gserviceaccount.com"",
                    ""universe_domain"": ""googleapis.com""
                }";

            //new way of doing the google api credentials
            GoogleCredential credential = GoogleCredential.FromJson(jsonKey);

            //recreate the original text string from the word object list
            string textToTranslate = "";
            foreach (ParsedWord word in textObjectToTranslate)
            {
                textToTranslate += word.Text + " ";
            }

            try
            {
                TranslationClient client = TranslationClient.Create(credential);
                TranslationResult result = await client.TranslateTextAsync(textToTranslate, targetLanguage: targetLanguageCode);
                //result contains other stuff like DetectedSourceLanguage
                return result.TranslatedText;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Translation error: {e.Message}");
                return e.Message;
            }
        }
    }
}

