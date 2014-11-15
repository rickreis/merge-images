using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;

namespace MergeImage
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Options size?\r\n[1] 48x48\r\n[2] 120x120\r\n[3] 180x180\r\n[4] 400x400");

            int optionSize = Convert.ToInt32(Console.ReadLine());

            int size = 0;

            switch (optionSize)
            {
                case 1:
                    size = 48;
                    break;
                case 2:
                    size = 120;
                    break;
                case 4:
                    size = 400;
                    break;
                default:
                    size = 180;
                    break;
            }

            string urlLoremPixel = "http://lorempixel.com/{0}/{0}";

            //limit 4
            string[] urls = new string[] { String.Format(urlLoremPixel, size), String.Format(urlLoremPixel, size), String.Format(urlLoremPixel, size), String.Format(urlLoremPixel, size) };

            while (true)
            {
                Console.WriteLine("Enter number images or zero to quit: ");

                int quantity = Convert.ToInt32(Console.ReadLine());

                if (quantity == 0)
                {
                    break;
                }

                if (quantity == 1)
                {
                    Console.WriteLine("Value must be greather than 1");
                    continue;
                }

                ICollection<Stream> files = new HashSet<Stream>();

                try
                {
                    using (var wc = new WebClient())
                    {
                        byte[] data = null;

                        for (int i = 0; i < quantity; i++)
                        {
                            data = wc.DownloadData(urls[i]);

                            MemoryStream memoryStream = new MemoryStream(data);

                            files.Add(memoryStream);
                        }

                        //image resizer bundle (merge with image-plus)
                        Bitmap bitmap = MergeImageResizer.CombineBitmap(width: size, height: size, files: files);

                        string fileName = String.Concat(AppDomain.CurrentDomain.BaseDirectory, files.Count, "_", size, "x", size, ".jpg");

                        bitmap.Save(fileName);

                        Console.WriteLine("Image save successfully! Verify file: {0},", fileName);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    foreach (var item in files)
                    {
                        item.Dispose();
                    }
                }
            }

            Console.WriteLine("Good bye!");
        }
    }
}
