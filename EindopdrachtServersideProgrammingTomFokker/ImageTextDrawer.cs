using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EindopdrachtServersideProgrammingTomFokker
{
    class ImageTextDrawer
    {
        public MemoryStream DrawTextOnImage(MemoryStream memoryStream, string beerAdvice, string temperature, string windspeed)
        {
            
            Bitmap bitmap = (Bitmap)Image.FromStream(memoryStream);
            
            Graphics graphics = Graphics.FromImage(bitmap);
            
            Font arialFont = new Font("Arial", 20, FontStyle.Bold);

            graphics.DrawString(beerAdvice, arialFont, Brushes.Black, new PointF(10f, 10f));
            graphics.DrawString("Temperatuur: " + temperature + " C", arialFont, Brushes.Black, new PointF(10f, 50f));
            graphics.DrawString("Windsnelheid: " + windspeed + " m/s", arialFont, Brushes.Black, new PointF(10f, 90f));

            MemoryStream outMemoryStream = new MemoryStream();
            bitmap.Save(outMemoryStream, ImageFormat.Png);
            outMemoryStream.Position = 0;

            return outMemoryStream;
        }
    }
}
