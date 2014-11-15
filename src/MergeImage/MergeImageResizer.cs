using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MergeImage
{
    public static class MergeImageResizer
    {
        public static Bitmap CombineBitmap(int width, int height, IEnumerable<Stream> files)
        {
            if (null == files || files.Any() == false)
            {
                return null;
            }

            //read all images into memory            
            Bitmap finalImage = null;

            ConfigBitmapImage config = BuilderConfigBitmap(width, height, files);

            try
            {
                //create a bitmap to hold the combined image
                finalImage = new Bitmap(config.Width, config.Height);

                //get a graphics object from the image so we can draw on it
                using (Graphics graphic = Graphics.FromImage(finalImage))
                {
                    //set background color
                    graphic.Clear(Color.White);

                    int x = 0;
                    int y = 0;

                    //if images conunt 2
                    if (config.CountImages == 2)
                    {
                        RenderFileTwoImages(graphic, finalImage, config);
                    }
                    else
                    {
                        RenderFileGreatherTwoImages(graphic, finalImage, config);
                    }

                    if (config.CountImages > 1)
                    {
                        string plusIcon = "plus-small.png";

                        if (width == 400)
                        {
                            plusIcon = "plus-medium.png";
                        }

                        if (width == 48)
                        {
                            plusIcon = "plus-super-small.png";
                        }

                        Bitmap plus = new Bitmap(String.Concat(AppDomain.CurrentDomain.BaseDirectory.Replace(@"bin\Debug\", @"files\"), plusIcon));

                        x = (finalImage.Width / 2) - (plus.Width / 2);
                        y = (finalImage.Height / 2) - ((plus.Height / 2));

                        graphic.DrawImage(plus, new Rectangle(x, y, plus.Width, plus.Height));
                    }
                }

                //sets o dpi para 72
                finalImage.SetResolution(72, 72);

                return finalImage;
            }
            catch (Exception ex)
            {
                if (finalImage != null)
                {
                    finalImage.Dispose();
                }

                throw ex;
            }
            finally
            {
                //clean up memory
                foreach (Bitmap image in config.Images)
                {
                    image.Dispose();
                }
            }
        }

        private static void RenderFileGreatherTwoImages(Graphics graphic, Bitmap finalImage, ConfigBitmapImage config)
        {
            int i = 1;
            int x = 0;
            int y = 0;
            //go through each image and draw it on the final image                        
            foreach (Bitmap image in config.Images)
            {
                if (i == 2)
                {
                    x += image.Width;
                }

                if (i == 3)
                {
                    y = image.Height;

                    if (config.CountImages == 3)
                    {
                        x = finalImage.Width / 4;
                    }
                    else
                    {
                        x = 0;
                    }
                }

                if (i == 4)
                {
                    x = image.Width;
                }

                graphic.DrawImage(image, new Rectangle(x, y, image.Width, image.Height));

                i++;
            }
        }

        private static void RenderFileTwoImages(Graphics graphic, Bitmap finalImage, ConfigBitmapImage config)
        {
            int i = 1;
            int x = 0;
            int y = 0;
            //go through each image and draw it on the final image                        
            foreach (Bitmap image in config.Images)
            {
                y = image.Height / 2;

                graphic.DrawImage(image, new Rectangle(x, y, image.Width, image.Height));

                x += image.Width;

                i++;
            }
        }

        private static ConfigBitmapImage BuilderConfigBitmap(int width, int height, IEnumerable<Stream> files)
        {
            ConfigBitmapImage config = new ConfigBitmapImage(width, height);

            //processo reduction images (divide 1/2)
            ProcessorImage processor = new ProcessorImage((width/2), (height/2));

            foreach (var item in files)
            {
                //processor reduction images, keep original image quality
                Stream imageProcessed = processor.Size.ProcessingImage(item);

                Bitmap bitmap = new Bitmap(imageProcessed);

                //sets o dpi para 72
                bitmap.SetResolution(72, 72);

                config.Images.Add(bitmap);
            }

            return config;
        }
    }

    public class ConfigBitmapImage
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public ICollection<Bitmap> Images { get; set; }

        public ConfigBitmapImage(int width, int height)
        {
            Width = width;
            Height = height;
            Images = new HashSet<Bitmap>();
        }

        public int CountImages
        {
            get
            {
                if (null == Images) return 0;

                return Images.Count;
            }
        }
    }
}
