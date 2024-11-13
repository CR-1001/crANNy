/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Converter.ConverterImage
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using CMatrix = CRAI.NeuralNetwork.Algebra.Matrix;
    using CRAI.NeuralNetwork.Algebra;
    using CRAI.NeuralNetwork;

    public static class ImageProcessor
    {
        public static Bitmap GetImage(byte[] imageData)
        {
            return new Bitmap(Image.FromStream(new MemoryStream(imageData)));
        }

        public static Bitmap Resize(Image image, int width, int height, bool isHighQualityResizing = true)
        {
            var bitmapTarget = new Bitmap(width, height);
            bitmapTarget.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            var imageRectangle = new Rectangle(0, 0, width, height);

            using (var graphics = Graphics.FromImage(bitmapTarget))
            using (var imageAttributes = new ImageAttributes())
            {
                imageAttributes.SetWrapMode(WrapMode.TileFlipXY);

                graphics.CompositingMode = CompositingMode.SourceCopy;

                graphics.SmoothingMode
                    = isHighQualityResizing
                    ? SmoothingMode.HighQuality
                    : SmoothingMode.HighSpeed;

                graphics.PixelOffsetMode
                    = isHighQualityResizing
                    ? PixelOffsetMode.HighQuality
                    : PixelOffsetMode.HighSpeed;

                graphics.CompositingQuality
                    = isHighQualityResizing
                    ? CompositingQuality.HighQuality
                    : CompositingQuality.HighSpeed;

                graphics.InterpolationMode
                    = isHighQualityResizing
                    ? InterpolationMode.HighQualityBicubic
                    : InterpolationMode.Low;

                graphics.DrawImage(
                    image, 
                    imageRectangle, 
                    0, 
                    0, 
                    image.Width, 
                    image.Height, 
                    GraphicsUnit.Pixel, 
                    imageAttributes);
            }

            return bitmapTarget;
        }

        public static Bitmap MakeInverted(Bitmap bitmap)
        {
            var bitmapGray = new Bitmap(bitmap.Width, bitmap.Height);

            for (var row = 0; row < bitmap.Height; row++)
            {
                for (var column = 0; column < bitmap.Width; column++)
                {
                    var color = bitmap.GetPixel(column, row);

                    var colorInverted = Color.FromArgb(
                            255 - color.R, 
                            255 - color.G, 
                            255 - color.B);

                    bitmapGray.SetPixel(column, row, colorInverted);
                }
            }

            return bitmapGray;
        }

        public static Bitmap MakeGrayscale(Bitmap bitmap)
        {
            var bitmapGray = new Bitmap(bitmap.Width, bitmap.Height);

            for (var row = 0; row < bitmap.Height; row++)
            {
                for (var column = 0; column < bitmap.Width; column++)
                {
                    var color = bitmap.GetPixel(column, row);

                    var red = color.R * 0.3;
                    var green = color.G * 0.59;
                    var blue = color.B * 0.11;

                    var gray = (int)(red + green + blue);

                    bitmapGray.SetPixel(column, row, Color.FromArgb(gray, gray, gray));
                }
            }

            return bitmapGray;
        }

        public static Bitmap Crop(Bitmap bitmap, Rectangle rectangle)
        {

            if(rectangle.Width <= 0 || rectangle.Height <= 0)
            {
                return null;
            }

            lock (bitmap)
            {
                var bitmapCropped = bitmap.Clone(rectangle, bitmap.PixelFormat);

                return bitmapCropped;
            }
        }

        public static Bitmap CropLightness(Bitmap bitmap, double lightnessThreshold)
        {
            var matrix = ToMatrixLightness(bitmap);

            var rectangle = ExtractRectangle(matrix, lightnessThreshold);

            var bitmapCropped = Crop(bitmap, rectangle);

            return bitmapCropped;
        }

        public static Rectangle ExtractRectangle(CMatrix matrix, double threshold)
        {
            var rowStart = 0;
            var rowEnd = matrix.CountRows - 1;

            var columnStart = 0;
            var columnEnd = matrix.CountColumns - 1;

            var rows = matrix.GetRows();
            foreach (var row in rows)
            {
                if (row.Values.Any(v => v <= threshold))
                {
                    break;
                }

                rowStart++;
            }

            var rowSubReverse = rows.Skip(rowStart).Reverse();
            foreach (var row in rowSubReverse)
            {
                if (row.Values.Any(v => v <= threshold))
                {
                    break;
                }

                rowEnd--;
            }

            var columns = matrix.GetColumns();
            foreach (var column in columns)
            {
                if (column.Values.Any(v => v <= threshold))
                {
                    break;
                }

                columnStart++;
            }

            var columnsSubReverse = columns.Skip(columnStart).Reverse();
            foreach (var column in columnsSubReverse)
            {
                if (column.Values.Any(v => v <= threshold))
                {
                    break;
                }

                columnEnd--;
            }

            var x = columnStart;
            var y = rowStart;
            var width = columnEnd - columnStart + 1;
            var height = rowEnd - rowStart + 1;

            return new Rectangle(x, y, width, height);
        }

        public static CMatrix ToMatrixLightness(Bitmap bitmap)
        {
            return ToMatrix(bitmap, c => c.GetBrightness());
        }

        public static CMatrix ToMatrixRedGreenBlue(Bitmap bitmap)
        {
            return ToMatrix(bitmap, 3, c => new double[] { c.R, c.G, c.B });
        }

        public static CMatrix ToMatrixHueSaturationLightness(Bitmap bitmap)
        {
            return ToMatrix(bitmap, 3, c => new double[] { c.GetHue(), c.GetSaturation(), c.GetBrightness()});
        }
        
        public static CMatrix ToMatrix(Bitmap bitmap, Func<Color, double> pixelExtract)
        {
            return ToMatrix(bitmap, 1, c => new [] { pixelExtract(c) });
        }

        public static CMatrix ToMatrix(
            Bitmap bitmap, 
            int valuesPerPixel, 
            Func<Color, double[]> pixelExtract)
        {
            var columnsImage = bitmap.Width;
            var rowsImage = bitmap.Height;

            var matrix = new CMatrix(rowsImage, columnsImage * valuesPerPixel);

            for (var rowImage = 0; rowImage < rowsImage; rowImage++)
            {
                for (var columnImage = 0; columnImage < columnsImage; columnImage++)
                {
                    var color = bitmap.GetPixel(columnImage, rowImage);
                    
                    var values = pixelExtract(color);

                    if(values.Length != valuesPerPixel)
                    {
                        throw new NeuralNetworkException(Errors.MatrixDimensionsMissmatch);
                    }

                    var columnMatrix = columnImage * valuesPerPixel;
                        
                    foreach(var value in values)
                    {
                        matrix[rowImage, columnMatrix] = value;
                        columnMatrix++;
                    }
                }
            }

            return matrix;
        }

        public static void EnhanceContrast(CMatrix matrix)
        {
            var min = 0.0;
            var max = 1.0;
            var delta = max - min;
            var deltaThird = delta / 3.0;
            var deltaHalf = delta / 2.0;
            var mid = min + deltaHalf;
            var lower = min + deltaThird;
            var upper = max - deltaThird;

            matrix.Replace((r, c, v) => 
            {
                if (v <= lower) return min;
                if (v >= upper) return max;
                return mid;
            });
            
        }
    }
}
