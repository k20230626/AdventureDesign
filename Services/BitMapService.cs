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
    public class BitMapService : IBitMapService
    {
        /// <summary>
        /// first int : image hash code
        /// second int : size
        /// third int : tile index
        /// </summary>
        private Dictionary<(int ImageHashcode,int Size), Dictionary<int, Stream>> _imageCache = new();
        public Dictionary<int, Stream> DivideNxN(SKBitmap originalImage, int n)
        {
            int imageHashCode = originalImage.GetHashCode();
            var cacheKey = (imageHashCode, n);
            
            if (_imageCache.ContainsKey(cacheKey))
                return _imageCache[cacheKey];

            _imageCache[cacheKey] = new();
            
            // 이미지를 n x n 크기로 나누어서 저장
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

                    var stream = piece.Encode(SKEncodedImageFormat.Jpeg, 100).AsStream();
                    _imageCache[cacheKey].Add(n * y + x,stream);
                }
            }
            return _imageCache[cacheKey];
        }
    }

    public interface IBitMapService
    {
        /// <summary>
        /// 이미지를 n x n 크기로 나누어서 Dictionary에 순서대로 반환
        /// </summary>
        /// <param name="originalImage">이미지</param>
        /// <param name="n">크기</param>
        /// <returns></returns>
        Dictionary<int, Stream> DivideNxN(SKBitmap originalImage, int n);

    }
}
