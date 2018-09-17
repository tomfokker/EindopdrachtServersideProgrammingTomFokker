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
        public MemoryStream DrawTextOnImage(MemoryStream memoryStream, string temperature)
        {
            
            Bitmap bitmap = (Bitmap)Image.FromStream(memoryStream);
            
            Graphics graphics = Graphics.FromImage(bitmap);
            
            Font arialFont = new Font("Arial", 20);
            
            graphics.DrawString("Temperature: " + temperature, arialFont, Brushes.Black, new PointF(10f, 10f));
            
            MemoryStream outMemoryStream = new MemoryStream();
            bitmap.Save(outMemoryStream, ImageFormat.Png);
            outMemoryStream.Position = 0;

            return outMemoryStream;
        }
    }
}
