using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Bitmap.Test;

internal class Program
{
    private static void Main()
    {
        var text = "به گروه نرم افزاری آموت خوش امدید";
        SizeF textSize;
        Console.WriteLine("Enter desired width:");
        var desiredWidth = int.Parse(Console.ReadLine());

        Console.WriteLine("Enter desired height:");
        var desiredHeight = int.Parse(Console.ReadLine());
        var fontSize = CalculateFontSize(text, desiredWidth, desiredHeight);

        var font = new Font("Arial", 5, FontStyle.Bold);


        using (var tempImage = new System.Drawing.Bitmap(96, 68))
        using (var g = Graphics.FromImage(tempImage))
        {
            textSize = g.MeasureString(text, font);
        }

        var width = desiredWidth > 0 ? desiredWidth : (int)textSize.Width;
        var height = desiredHeight > 0 ? desiredHeight : (int)textSize.Height;
        using (var bitmap = new System.Drawing.Bitmap(width, height))
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.White);
            graphics.DrawString(text, font, Brushes.Black, new PointF(10, 20));

            var monochromeBitmap = ConvertToMonochrome(bitmap);

            var xbmData = GenerateXBM(monochromeBitmap, "text_image");

            File.WriteAllText("text_image.xbm", xbmData);
            Console.WriteLine(xbmData);

            var outputPath = "output.png";

            Console.WriteLine("XBM file generated: text_image.xbm");
        }
    }

    private static System.Drawing.Bitmap ConvertToMonochrome(System.Drawing.Bitmap source)
    {
        var monochrome = new System.Drawing.Bitmap(source.Width, source.Height, PixelFormat.Format1bppIndexed);

        for (var y = 0; y < source.Height; y++)
        for (var x = 0; x < source.Width; x++)
        {
            var pixel = source.GetPixel(x, y);
            var intensity = (pixel.R + pixel.G + pixel.B) / 3;
            //monochrome.SetPixel(x, y, intensity < 128 ? Color.Black : Color.White);


            //    var data = monochrome.LockBits(new Rectangle(0, 0, monochrome.Width, monochrome.Height),
            //    ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            //var bytes = new byte[data.Height * data.Stride];
            //Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            //    bytes[x * data.Stride + y] = 1; // Set the pixel at (5, 5) to the color #1

            //    Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);

            //monochrome.UnlockBits(data);


            var data = monochrome.LockBits(new Rectangle(0, 0, monochrome.Width, monochrome.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            var bytes = new byte[data.Height * data.Stride];
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            var index = y * data.Stride + x;

            bytes[index] = intensity < 128 ? (byte)0 : (byte)255; 

            Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);

            monochrome.UnlockBits(data);
        }

        return monochrome;
    }

    private static string GenerateXBM(System.Drawing.Bitmap bitmap, string name)
    {
        var width = bitmap.Width;
        var height = bitmap.Height;
        var data = new byte[(width + 7) / 8 * height];

        for (var y = 0; y < height; y++)
        for (var x = 0; x < width; x++)
        {
            var pixel = bitmap.GetPixel(x, y);
            var index = y * ((width + 7) / 8) + x / 8;

            if (pixel.R == 0) // Black pixel
                data[index] |= (byte)(1 << (x % 8));
        }

        var sb = new StringWriter();
        sb.WriteLine($"#define {name}_width {width}");
        sb.WriteLine($"#define {name}_height {height}");
        sb.WriteLine($"static unsigned char {name}_bits[] = {{");

        for (var i = 0; i < data.Length; i++)
        {
            if (i % 12 == 0) sb.Write("   ");
            sb.Write($"0x{data[i]:X2}");
            if (i < data.Length - 1) sb.Write(", ");
            if (i % 12 == 11) sb.WriteLine();
        }

        sb.WriteLine("};");
        return sb.ToString();
    }

    private static int CalculateFontSize(string text, int width, int height)
    {
        using (var tempImage = new System.Drawing.Bitmap(1, 1))
        using (var g = Graphics.FromImage(tempImage))
        {
            var fontSize = 10; // Start with a small font size
            Font testFont;
            SizeF textSize;

            do
            {
                fontSize++;
                testFont = new Font("Arial", fontSize, FontStyle.Bold);
                textSize = g.MeasureString(text, testFont);
            } while (textSize.Width < width && textSize.Height < height);

            return fontSize - 1; // Return the last fitting font size
        }
    }
}