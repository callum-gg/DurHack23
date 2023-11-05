using System;
using System.Threading.Tasks;

namespace DurHack_2023
{
    class main_enter
    {
        static bool web = true; // Make this static if it's supposed to control the program flow

        static async Task Main(string[] args)
        {
            if (web)
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
            else
            {
                // If 'web' is false, do the non-web related tasks here
                // LOUSISAAAAAAAA
            }
        }
    }
}
