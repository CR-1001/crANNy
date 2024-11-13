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
    using CRAI.Common;
    using CRAI.Converter.ConverterImage;
    using CRAI.NeuralNetwork;
    using CRAI.NeuralNetwork.Algebra;
    using CRAI.NeuralNetworkHost;
    using CRAI.NeuralNetworkHost.Configurations;

    public class ConverterText : MarshalByRefObject, IConverter
    {
        protected int _Width = 40;
        protected int _Height = 40;

        protected int _WidthCharacterMin = 15;
        protected int _HeightCharacterMin = 40;

        protected bool _EnhanceContrast = false;

        protected bool _HighQuality = true;

        protected bool _OnlyFirstCharacterInLearningMode = true;
        protected bool _OnlyFirstCharacterInProductionMode = false;

        public virtual void Setup(IDictionary<String, Object> parameters)
        {
            _Width = (int)parameters["Width"];
            _Height = (int)parameters["Height"];

            _WidthCharacterMin = (int)parameters["WidthCharacterMin"];
            _HeightCharacterMin = (int)parameters["HeightCharacterMin"];

            _HighQuality = parameters.TryGetValueFallback("HighQuality", true);

            _EnhanceContrast = parameters.TryGetValueFallback("EnhanceContrast", false);

            _OnlyFirstCharacterInLearningMode = parameters.TryGetValueFallback("OnlyFirstCharacterInLearningMode", true);

            _OnlyFirstCharacterInProductionMode = parameters.TryGetValueFallback("OnlyFirstCharacterInProductionMode", false);
        }

        public StimulusSet Convert(byte[] rawData, ConvertSettings convertSettings)
        {
            var bitmapOriginal = ImageProcessor.GetImage(rawData);

            var bitmapRows = ImageExtracter.ExtractBitmaps(
                bitmapOriginal,
                _WidthCharacterMin,
                _HeightCharacterMin,
                _HighQuality,
                _Width,
                _Height);

            var stimuli = new List<Stimulus>();

            var row = 0;
            var stop = false;

            foreach (var bitmapRow in bitmapRows)
            {
                var column = 0;

                foreach (var bitmap in bitmapRow)
                {
                    var matrix = ImageProcessor.ToMatrixLightness(
                        bitmap);

                    if (_EnhanceContrast)
                    {
                        ImageProcessor.EnhanceContrast(matrix);
                    }

                    var vector = matrix.ToVectorByRows();

                    stimuli.Add(new Stimulus(vector.Values));

                    if((convertSettings.IsLearningMode && _OnlyFirstCharacterInLearningMode)
                        || (!convertSettings.IsLearningMode && _OnlyFirstCharacterInProductionMode))
                    {
                        stop = true;
                        break;
                    }
                }

                if(stop)
                {
                    break;
                }

                row++;

                var vectorLineBreak = new Vector(_Width * _Height);
                VectorOperations.Fill(vectorLineBreak, 0.0);

                stimuli.Add(new Stimulus(vectorLineBreak.Values));
            }

            return new StimulusSet(stimuli);
        }


        public virtual byte[] Convert(StimulusSet stimulusSet, ConvertSettings convertSettings)
        {
            var stringBuilder = new StringBuilder();

            foreach (var stimulus in stimulusSet.Values)
            {
                var stringBuilderSingleCharacter = new StringBuilder();

                foreach (var value in stimulus.Values)
                {
                    var valueRounded = Math.Round(value, 0);
                    stringBuilderSingleCharacter.Append(valueRounded);
                }

                var character = (char)System.Convert.ToInt16(stringBuilderSingleCharacter.ToString(), 2);

                stringBuilder.Append(character);
            }

            var text = stringBuilder.ToString();

            var raw = Encoding.Unicode.GetBytes(text);

            return raw;
        }

        public virtual IEnumerable<KeyValuePair<String, Object>> ExtractParameters()
        {
            yield return new KeyValuePair<String, Object>("Width", _Width);
            yield return new KeyValuePair<String, Object>("Height", _Height);

            yield return new KeyValuePair<String, Object>("HighQuality", _HighQuality);

            yield return new KeyValuePair<String, Object>("HeightCharacterMin", _HeightCharacterMin);
            yield return new KeyValuePair<String, Object>("WidthCharacterMin", _WidthCharacterMin);
            yield return new KeyValuePair<String, Object>("EnhanceContrast", _EnhanceContrast);
            yield return new KeyValuePair<String, Object>("OnlyFirstCharacterInLearningMode", _OnlyFirstCharacterInLearningMode);
            yield return new KeyValuePair<String, Object>("OnlyFirstCharacterInProductionMode", _OnlyFirstCharacterInProductionMode);
        }



    }
}
