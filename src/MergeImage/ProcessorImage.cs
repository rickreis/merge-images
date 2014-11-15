using ImageResizer;
using System;
using System.Collections.Generic;
using System.IO;

namespace MergeImage
{
    public class ProcessorImage
    {
        public ProcessorImage(int width, int height)
        {
            Size = new ImageSizeConfig { Height = height, Width = width };
        }

        public ImageSizeConfig Size { get; set; }
    }

    public class ImageSizeConfig
    {
        public int Width { get; set; }

        public int Height { get; set; }

        string _format;
        public string Format
        {
            get { return _format ?? "jpg"; }
            set { _format = value; }
        }

        int _quality;
        public int Quality
        {
            get { return (_quality == 0) ? 100 : _quality; }
            set { _quality = value; }
        }

        public string BuilderConfig()
        {
            var configs = new HashSet<string>();

            if (this.Width > 0)
            {
                configs.Add(String.Concat("width=", this.Width));
            }
            if (this.Height > 0)
            {
                configs.Add(String.Concat("height=", this.Height));
            }
            configs.Add(String.Concat("format=", this.Format));
            configs.Add(String.Concat("quality=", this.Quality));
            configs.Add("crop=auto");

            return String.Join("&", configs);
        }

        public Stream ProcessingImage(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (stream.Position > 0)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            Stream destination = new MemoryStream();

            var imageJob = new ImageJob();
            imageJob.Source = stream;
            imageJob.Dest = destination;
            imageJob.Settings = new ResizeSettings(this.BuilderConfig());
            imageJob.DisposeSourceObject = false;
            imageJob.AddFileExtension = true;
            imageJob.ResetSourceStream = true;
            ImageBuilder.Current.Build(imageJob);

            return destination;
        }
    }
}
