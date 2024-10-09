using Microsoft.Maui.Graphics.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;
using System.Drawing;
using Microsoft.Maui.Controls.Shapes;
using IImage = Microsoft.Maui.Graphics.IImage;
using System.Reflection;
using SkiaSharp;
namespace adventuredesign8puzzle.Services
{
    public class BitMapService
    {

        public BitMapService()
        {
            

        }

        public Dictionary<int, SKBitmap> divideNxN(SKBitmap originalImage, int n)
        {
            var dict = new Dictionary<int, SKBitmap>();
            int pieceWidth = originalImage.Width / n;
            int pieceHeight = originalImage.Height / n;

            int size = Math.Min(pieceWidth, pieceHeight);


            for (int y = 0; y < n; y++)
            {
                for (int x = 0; x < n; x++)
                {
                    SKRect sourceRect = new SKRect(x * size, y * size, (x + 1) * size, (y + 1) * size);
                    SKBitmap piece = new SKBitmap(size, size);

                    using (var canvas = new SKCanvas(piece))
                    {
                        canvas.DrawBitmap(originalImage, sourceRect, new SKRect(0, 0, size, size));
                    }

                    dict.Add(n * y + x,piece);
                }
            }
            return dict;
        }

        private void SliceImage()
        {
            

        }
    }
}
