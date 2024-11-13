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
    using CRAI.NeuralNetworkHost;
    using CRAI.NeuralNetworkHost.Configurations;

    public class ConverterTextDigits : ConverterText
    {
        private int _StartIndex = 65; // 65 -> A, 48 -> 0, 97 -> a

        public override byte[] Convert(StimulusSet stimulusSet, ConvertSettings convertSettings)
        {
            var stringBuilder = new StringBuilder();

            foreach(var stimulus in stimulusSet.Values)
            {
                var stringBuilderSingleCharacter = new StringBuilder();

                var index = stimulus.Values.ToList().FindIndex(v => v > 0.7);

                var character = (char)(_StartIndex + index);

                stringBuilder.Append(character);
            }

            var text = stringBuilder.ToString();

            var raw = Encoding.Unicode.GetBytes(text);

            return raw;
        }

        public override void Setup(IDictionary<String, Object> parameters)
        {
            base.Setup(parameters);

            _StartIndex = parameters.TryGetValueFallback("StartIndex", 65);
        }

        public override IEnumerable<KeyValuePair<String, Object>> ExtractParameters()
        {
            foreach(var parameter in base.ExtractParameters())
            {
                yield return parameter;
            }

            yield return new KeyValuePair<String, Object>("StartIndex", _StartIndex);
        }

    }
}
