/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Converter.ConverterText
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using AForge;
    using AForge.Imaging;
    using AForge.Imaging.Filters;
    using CRAI.Common;
    using CRAI.Converter.ConverterImage;
    using CRAI.NeuralNetwork;
    using CRAI.NeuralNetwork.Algebra;
    using CRAI.NeuralNetwork.Randomizer;
    using CRAI.NeuralNetworkHost;


    public static class ImageExtracter
    {
        
        public static IEnumerable<IEnumerable<Bitmap>> ExtractBitmaps(
            Bitmap bitmapOriginal, 
            int widthCharacterMin, 
            int heightCharacterMin, 
            bool heightQuality,
            int widthResizeSingleCharacter,
            int heightResizeSingleCharacter)
        {
            var bitmapInverted = ImageProcessor.MakeInverted(bitmapOriginal);

            var colorFiltering = new ColorFiltering();

            var threshold = 120;

            colorFiltering.Red = new IntRange(0, threshold);
            colorFiltering.Green = new IntRange(0, threshold);
            colorFiltering.Blue = new IntRange(0, threshold);
            colorFiltering.FillOutsideRange = false;

            colorFiltering.ApplyInPlace(bitmapInverted);

            var blobCounter = new BlobCounter();

            blobCounter.MinWidth = widthCharacterMin;
            blobCounter.MinHeight = heightCharacterMin;

            blobCounter.ProcessImage(bitmapInverted);

            var rectangles = blobCounter.GetObjectsRectangles();

            var rectanglesOrdered 
                = OrderRows(rectangles, widthCharacterMin, heightCharacterMin)
                .ToList();

            var bitmapRows = new List<List<Bitmap>>(rectanglesOrdered.Count);

            foreach (var rectangleRow in rectanglesOrdered)
            {
                var bitmaps = new List<Bitmap>();

                bitmapRows.Add(bitmaps);

                var rectanglePrevious = default(Rectangle);

                var top = rectangleRow.Min(r => r.Top);
                var bottom = rectangleRow.Min(r => r.Bottom);
                
                var widthAverage = rectangleRow.Average(r => r.Width);
                var heightAverage = rectangleRow.Average(r => r.Height);

                foreach (var rectangle in rectangleRow)
                {
                    var spaceBetweenPrevious 
                        = rectanglePrevious != default(Rectangle)
                        ? rectangle.Left - rectanglePrevious.Right
                        : 0;

                    if (spaceBetweenPrevious > widthAverage)
                    {
                        var widthBetween = (int)Math.Min(spaceBetweenPrevious, widthAverage);

                        var rectangleSpace = new Rectangle(
                            rectanglePrevious.Right, top, widthBetween, bottom - top);
                        
                        var bitmapSubSpace = ExtractBitmap(
                            bitmapOriginal, 
                            rectangleSpace, 
                            widthResizeSingleCharacter, 
                            heightResizeSingleCharacter, 
                            heightQuality);

                        bitmaps.Add(bitmapSubSpace);
                    }
                    
                    var bitmapSub = ExtractBitmap(
                        bitmapOriginal, 
                        rectangle, 
                        widthResizeSingleCharacter, 
                        heightResizeSingleCharacter, 
                        heightQuality);

                    bitmaps.Add(bitmapSub);

                    if(rectangle == rectangleRow.Last())
                    {
                        var right = (int)Math.Min(rectangle.Right + widthAverage, bitmapOriginal.Width);

                        var left = rectangle.Right;

                        var widthBetween = (int)(right - left);

                        var rectangleSpaceLast = new Rectangle(
                            left, top, widthBetween, bottom - top);

                        var bitmapSubSpaceLast = ExtractBitmap(
                            bitmapOriginal, 
                            rectangleSpaceLast,
                            widthResizeSingleCharacter,
                            heightResizeSingleCharacter, 
                            heightQuality);

                        bitmaps.Add(bitmapSubSpaceLast);
                    }

                    rectanglePrevious = rectangle;
                }
            }

            foreach(var bitmapRow in bitmapRows)
            {
                var bitmapRowFiltered = bitmapRow.Where(b => b != null).ToList();

                if(bitmapRowFiltered.Any())
                {
                    yield return bitmapRowFiltered;
                }
            }
        }

        private static Bitmap ExtractBitmap(
            Bitmap bitmapOriginal, 
            Rectangle rectangle, 
            int width, 
            int height, 
            bool highQuality)
        {
            var bitmapGray = ImageProcessor.MakeGrayscale(bitmapOriginal);

            var bitmapGrayPartial = ImageProcessor.Crop(bitmapGray, rectangle);

            var bitmapGrayPartialCropped = ImageProcessor.CropLightness(
                bitmapGrayPartial, 0.5);

            if(bitmapGrayPartialCropped == null)
            {
                bitmapGrayPartialCropped = bitmapGrayPartial;
            }

            var bitmapPartialGrayCroppedResized = ImageProcessor.Resize(
                bitmapGrayPartialCropped, width, height, highQuality);

            return bitmapPartialGrayCroppedResized;
        }

        private static IEnumerable<IEnumerable<Rectangle>> OrderRows(
            IEnumerable<Rectangle> rectanglesOriginal,
            int widthMin,
            int heightMin)
        {
            var rectanglesProcessed
                = rectanglesOriginal
                .OrderBy(r => r.X)
                .ToList();

            var lines = new Dictionary<Double, List<Rectangle>>();

            var rectanglesProcessedSub = new List<Rectangle>();

            foreach (var rectangle in rectanglesOriginal)
            {
                if (rectangle.Height < heightMin
                    || rectangle.Width < widthMin
                    || rectanglesProcessedSub.Contains(rectangle))
                {
                    continue;
                }

                var rectanclesLine
                    = rectanglesOriginal
                    .Where(r =>
                        {
                            if (r.Height < heightMin
                                || r.Width < widthMin
                                || rectanglesProcessedSub.Contains(r)
                                || lines.SelectMany(l => l.Value).Any(rp => rp.IntersectsWith(r)))
                            {
                                return false;
                            }

                            var r1 = new Rectangle(0, r.Y, 1, r.Height);
                            var r2 = new Rectangle(0, rectangle.Y, 1, rectangle.Height);

                            return r1.IntersectsWith(r2);
                        })
                    .OrderBy(r => r.Left)
                    .ToList();

                if (rectanclesLine.Any())
                {
                    rectanglesProcessedSub.AddRange(rectanclesLine);

                    var lineCenter = rectanclesLine.Average(r => (r.Top + (r.Height / 2)));

                    var y = rectanclesLine.Min(r => r.Top);

                    var height = rectanclesLine.Max(r => r.Bottom) - y;

                    var rectanclesLineAdjusted
                        = rectanclesLine
                        .Select(r => new Rectangle(r.X, y, r.Width, height))
                        .ToList();

                    lines[lineCenter] = rectanclesLineAdjusted;
                }
            }

            foreach (var line in lines.OrderBy(l => l.Key))
            {
                yield return line.Value;
            }
        }


        public static StimulusSet FromTextToStimulusSet(
            String input, int digits = 16, bool addNoise = true, double noise = 0.4)
        {
            var binary = FromTextToBinary(input, digits, addNoise, noise);

            return StimulusSet.FromString(binary);
        }
        
        public static String[] FromTextToBinary(
            String input, int digits = 16, bool addNoise = true, double noise = 0.4)
        {

            var randomizer = new Randomizer();
            randomizer.Setup(-noise, +noise, null);

            var letters = new List<String>();

            foreach (var character in input)
            {
                var letter = (int)character;
                var letterBinary = Convert.ToString(letter, 2);
                var letterBinaryFilled = letterBinary.PadLeft(digits, '0');
                var letterBinaryFilledReplaced
                    = letterBinaryFilled
                    .Select(l => 
                    {
                        return
                            addNoise 
                            ? Int16.Parse(l.ToString()) + randomizer.Next() + "|" 
                            : l + "|";
                    })
                    .Aggregate((l1, l2) => l1 + l2);

                letters.Add(letterBinaryFilledReplaced);
            }

            return letters.ToArray();
        }

        public static String[] FromTextToDigits(
            String input, 
            int startIndex,
            int digits)
        {
            var letters = new List<String>();

            foreach (var character in input)
            {
                var stringBuilder = new StringBuilder();

                var letter = (int)character - startIndex;

                for(var i = 0; i < digits; i++)
                {
                    stringBuilder.Append((i == letter ? 1 : 0) + "|");
                }

                letters.Add(stringBuilder.ToString());
            }

            return letters.ToArray();
        }

    }
}
