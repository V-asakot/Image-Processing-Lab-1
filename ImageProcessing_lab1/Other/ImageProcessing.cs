using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessing_lab1
{
    class ImageProcessing
    {
        public static Bitmap Process(Bitmap firstBitmap, Bitmap secondBitmap, bool[] selectedChannels,Mode mode)
        {
            var newSize = secondBitmap != null ? new Size(Math.Max(firstBitmap.Width, secondBitmap.Width), Math.Max(firstBitmap.Height, secondBitmap.Height)) : firstBitmap.Size;
            firstBitmap = ResizeImage(firstBitmap, newSize);
            secondBitmap = secondBitmap != null ? ResizeImage(secondBitmap, newSize): null;
            return ProcessPixels(firstBitmap, secondBitmap, selectedChannels,mode);        
        }
        public static bool isMaskMode(Mode mode)
        {
            return (int)mode > 4;
        }

        private static Bitmap ResizeImage(Bitmap input, Size size)
        {
            try
            {
                Bitmap bitmap = new Bitmap(size.Width, size.Height);
                using (Graphics g = Graphics.FromImage((Image)bitmap))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(input, 0, 0, size.Width, size.Height);
                }
                return bitmap;
            }
            catch
            {
                Console.WriteLine("Bitmap could not be resized");
                return input;
            }
        }

        private static Bitmap ProcessPixels(Bitmap firstBitmap, Bitmap secondBitmap, bool[] selectedChannels, Mode mode)
        {
            var size = firstBitmap.Size;
            var result = new Bitmap(size.Width, size.Height);
            var mask = CreateMask(size,mode);
            for (int i=0;i< size.Width;i++)
            {
                for (int j = 0; j < size.Height; j++)
                {
                    var pix1 = firstBitmap.GetPixel(i, j);
                    var color1 = Color.FromArgb(
                        selectedChannels[0] ? pix1.R : 0,
                        selectedChannels[1] ? pix1.G : 0,
                        selectedChannels[2] ? pix1.B : 0
                    );

                    var pix2 = secondBitmap != null ? secondBitmap.GetPixel(i, j) : Color.White;
                    var color2 = secondBitmap != null ? Color.FromArgb(
                        selectedChannels[0] ? pix2.R : 0,
                        selectedChannels[1] ? pix2.G : 0,
                        selectedChannels[2] ? pix2.B : 0
                    ) : Color.White;

                    int maskColor = mask != null ? mask.GetPixel(i, j).R: 0;

                    var color = mode switch 
                    { 
                        Mode.Summ => SummProcess(color1,color2),
                        Mode.Mult => MultProcess(color1, color2),
                        Mode.Avg => AvgProcess(color1, color2),
                        Mode.Min => MinProcess(color1, color2),
                        Mode.Max => MaxProcess(color1, color2),
                        Mode.Cirle => MaskProcess(color1, maskColor),
                        Mode.Square => MaskProcess(color1, maskColor),
                        Mode.Rectangle => MaskProcess(color1, maskColor)
                    };
                    result.SetPixel(i, j, color);
                }
            }

            return result;
        }

        private static Color SummProcess(Color color1, Color color2) => Color.FromArgb(Clamp(color1.R + color2.R), Clamp(color1.G + color2.G), Clamp(color1.B + color2.B));
        private static Color MultProcess(Color color1, Color color2) => Color.FromArgb(Clamp(color1.R * color2.R / 256), Clamp(color1.G * color2.G / 256), Clamp(color1.B * color2.B / 256));
        private static Color AvgProcess(Color color1, Color color2) => Color.FromArgb((color1.R + color2.R) / 2, (color1.G + color2.G) / 2, (color1.B + color2.B) / 2);
        private static Color MinProcess(Color color1, Color color2) => Color.FromArgb(Math.Min(color1.R, color2.R), Math.Min(color1.G, color2.G), Math.Min(color1.B, color2.B));
        private static Color MaxProcess(Color color1, Color color2) => Color.FromArgb(Math.Max(color1.R, color2.R), Math.Max(color1.G, color2.G), Math.Max(color1.B, color2.B));
        private static Color MaskProcess(Color color, int maskColor) => Color.FromArgb(maskColor, color.R, color.G, color.B);
        private static Bitmap CreateMask(Size size,Mode mode)
        {
            if (!isMaskMode(mode)) return null;
            Bitmap result = new Bitmap(size.Width, size.Height);
            int sizeSize = (int) (Math.Min(size.Width, size.Height)*0.8f);
            using (Graphics gr = Graphics.FromImage(result))
            {
                gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                switch (mode) 
                {
                    case Mode.Cirle: gr.FillEllipse(Brushes.White, Convert.ToInt32((size.Width - sizeSize) / 2), Convert.ToInt32((size.Height - sizeSize) / 2), sizeSize, sizeSize);break;
                    case Mode.Square: gr.FillRectangle(Brushes.White, Convert.ToInt32((size.Width - sizeSize) / 2), Convert.ToInt32((size.Height - sizeSize) / 2), sizeSize, sizeSize); break;
                    case Mode.Rectangle: gr.FillRectangle(Brushes.White, Convert.ToInt32((size.Width - sizeSize) / 2), Convert.ToInt32((size.Height - (sizeSize/2)) / 2), sizeSize, sizeSize/2); break;
                }
            }
            return result;
        }

        private static int Clamp(float val)
        {
            if (val.CompareTo(0) < 0) return 0;
            else if (val.CompareTo(255) > 0) return 255;
            else return (int) val;
        }

    }
}
