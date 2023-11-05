using System;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util; // For VectorOfByte
using System.Drawing; // For Size and PointF
using System.Diagnostics; // For Process
using System.IO; // For File and MemoryStream
using System.Drawing.Imaging;
using System.IO;

namespace processing
{

    public class ImageProcessing
    {
        private string imageBase64;

        public ImageProcessing(string base64)
        {
            imageBase64 = base64;
        }

        public void Test()
        {
            // Convert base64 to byte array
            byte[] imageBytes = Convert.FromBase64String(imageBase64);


        }


        private void ShowImage(byte[] imageBytes)
        {
            // Create a temporary file to save the image
            string tempFilePath = Path.GetTempFileName() + ".jpg";

            // Write the byte array to the temporary file
            File.WriteAllBytes(tempFilePath, imageBytes);

            // Open the image using the default viewer
            Process.Start(new ProcessStartInfo(tempFilePath) { UseShellExecute = true });
        }


    }


}
