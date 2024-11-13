/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Converter.ConverterImage
{
    using System;
    using System.Collections.Generic;
    using CRAI.Common;
    using CRAI.NeuralNetwork;
    using CRAI.NeuralNetwork.Algebra;
    using CRAI.NeuralNetworkHost;
    using CRAI.NeuralNetworkHost.Configurations;

    [Serializable] public class ConverterImage : MarshalByRefObject, IConverter
    {
        private int _Width;
        private int _Height;
        private bool _GrayScale = true;
        private bool _HighQuality = true;
        private bool _EnhanceContrast = false;
        private double _CropBrightnessThreshold = 1.0;

        public StimulusSet Convert(byte[] rawData, ConvertSettings convertSettings)
        {
            var bitmap = ImageProcessor.GetImage(rawData);

            var bitmapGray = _GrayScale
                ? ImageProcessor.MakeGrayscale(bitmap) : bitmap;

            var bitmapCropped = _CropBrightnessThreshold < 1.0
                ? ImageProcessor.CropLightness(bitmapGray, _CropBrightnessThreshold) : bitmapGray;

            if (bitmapCropped == null ||
                (bitmapCropped.Height == 1
                && bitmapCropped.Width == 1
                && bitmapCropped.GetPixel(0, 0).GetBrightness() == 1.0))
            {
                var vectorEmpty = new Vector(_Width * _Height);
                vectorEmpty.Fill(1.0);
                return new StimulusSet(new[] { new Stimulus(vectorEmpty.Values) });
            }

            var bitmapResized = ImageProcessor.Resize(bitmapCropped, _Width, _Height, _HighQuality);

            var matrix
                = _GrayScale
                ? ImageProcessor.ToMatrixLightness(bitmapResized)
                : ImageProcessor.ToMatrixHueSaturationLightness(bitmapResized);

            if(_EnhanceContrast)
            {
                ImageProcessor.EnhanceContrast(matrix);
            }

            var vector = matrix.ToVectorByRows();

            return new StimulusSet(new[] { new Stimulus(vector.Values) });
        }

        public byte[] Convert(StimulusSet stimulusSet, ConvertSettings convertSettings)
        {
            return Serialization.ToBinary(stimulusSet);
        }

        public void Setup(IDictionary<String, Object> parameters)
        {
            _Width = (int)parameters["Width"];
            _Height = (int)parameters["Height"];

            _GrayScale = parameters.TryGetValueFallback("GrayScale", true);
            _HighQuality = parameters.TryGetValueFallback("HighQuality", true);

            _EnhanceContrast = parameters.TryGetValueFallback("EnhanceContrast", false);

            _CropBrightnessThreshold = parameters.TryGetValueFallback(
                "CropBrightnessThreshold", 1.0);
        }


        public IEnumerable<KeyValuePair<String, Object>> ExtractParameters()
        {
            yield return new KeyValuePair<String, Object>("Width", _Width);
            yield return new KeyValuePair<String, Object>("Height", _Height);
            yield return new KeyValuePair<String, Object>("GrayScale", _GrayScale);
            yield return new KeyValuePair<String, Object>("HighQuality", _HighQuality);
            yield return new KeyValuePair<String, Object>("CropBrightnessThreshold", _CropBrightnessThreshold);
            yield return new KeyValuePair<String, Object>("EnhanceContrast", _EnhanceContrast);
        }
    }
}
