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
        /// first int : image hash code<br/>
        /// second int : size<br/>
        /// third int : tile index<br/>
        /// </summary>
        private Dictionary<(int ImageHashcode,int Size), Dictionary<int, SKData>> _imageCache = new();
        
        private int previousImageHash = 0;
        public Dictionary<int, SKData> DivideNxN(SKBitmap originalImage, int n)
        {
            int imageHashCode = originalImage.GetHashCode();
            
            //다른 이미지면 캐시 클리어
            if(previousImageHash != imageHashCode)
            {
                _imageCache.Clear();
                previousImageHash = imageHashCode;
            }
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
                    int index = n * y + x;
                    index = index == n * n - 1 ? 0 : index + 1;
                    SKRect sourceRect = new SKRect(x * size, y * size, (x + 1) * size, (y + 1) * size);
                    SKBitmap piece = new SKBitmap(size, size);

                    using (var canvas = new SKCanvas(piece))
                    {
                        canvas.DrawBitmap(originalImage, sourceRect, new SKRect(0, 0, size, size));
                        if (index == 0) {
                            using var paint = new SKPaint();
                            paint.Color = new SKColor(0, 0, 0, 128);
                            canvas.DrawRect(new SKRect(0, 0, size, size), paint);
                        }
                    }

                    var stream = piece.Encode(SKEncodedImageFormat.Png, 100);
                    
                    //퍼즐 숫자 보정
                    
                    _imageCache[cacheKey].Add(index,stream);
                }
            }
            return _imageCache[cacheKey];
        }
    }

    /// <summary>
    /// 이미지를 SKiaSharp을 써서 핸들링하는 서비스
    /// </summary>
    public interface IBitMapService
    {
        /// <summary>
        /// 이미지를 n x n 크기로 나누어서 Dictionary에 순서대로 반환
        /// </summary>
        /// <param name="originalImage">이미지</param>
        /// <param name="n">크기</param>
        /// <returns></returns>
        Dictionary<int, SKData> DivideNxN(SKBitmap originalImage, int n);

    }
}
