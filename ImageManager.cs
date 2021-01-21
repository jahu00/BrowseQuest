using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseQuest
{
    public class ImageManager
    {
        private static ImageManager instance { get; set; }

        public static ImageManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ImageManager();
                }

                return instance;
            }
        }

        private Dictionary<string, Image> images { get; set; }

        private ImageManager()
        {
            images = new Dictionary<string, Image>();
        }


        public Image GetImage(string name)
        {
            if (!images.ContainsKey(name))
            {
                var path = GetPath(name);
                if (!File.Exists(path))
                {
                    path = GetPath("default");
                }
                var image = Image.FromFile(path);
                images[name] = image;
            }
            return images[name];
        }

        private string GetPath(string name)
        {
            var path = $"Content/Gfx/{name}.png";
            return path;
        }
    }
}
