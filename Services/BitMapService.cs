using Microsoft.Maui.Graphics.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace adventuredesign8puzzle.Services
{
    public class BitMapService
    {
        ImageSource imgSource;

        public BitMapService(ImageSource imageSource)
        {
            this.imgSource = imageSource;
        }

        public Dictionary<int,IImage> divideNxN(IImage image,int size, int n)
        {
            var dict = new Dictionary<int, IImage>();

            //이미지를 size크기로 만들고 n*n개로 나누기
            var bitmap = image.ToBitmap(size, size);
            var width = bitmap.Width;
            var height = bitmap.Height;
            var widthPerPiece = width / n;
            var heightPerPiece = height / n;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    var piece = bitmap.GetSubBitmap(j * widthPerPiece, i * heightPerPiece, widthPerPiece, heightPerPiece);
                    dict.Add(i * n + j, piece);
                }
            }



            return dict;
        }

        private IImage getImage(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            var image = PlatformImage.FromStream(stream);
            return image;
        }
    }
}
