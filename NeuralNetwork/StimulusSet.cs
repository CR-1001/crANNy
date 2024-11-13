/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using CRAI.Common;

    [Serializable] [DataContract] public class StimulusSet : NotifyBase<Stimulus>
    {
        public StimulusSet()
        {

        }

        public StimulusSet(StimulusSet toCopy)
        {
            Values = toCopy.Values
                .Select(s => new Stimulus(s))
                .ToArray();
        }

        public StimulusSet(double[][] values)
        {
            Values = new Stimulus[values.Length];

            for(int i = 0; i < values.Length; i++)
            {
                Values[i] = new Stimulus(values[i]);
            }
        }

        public StimulusSet(IEnumerable<Stimulus> values)
        {
            Values = values.ToArray();
        }

        public StimulusSet(int count)
        {
            Values = new Stimulus[count];
        }

        public Stimulus this[int i]
        {
            get { return Values[i]; }
            set { Values[i] = value; }
        }

        #region public Stimulus[] Values
        private Stimulus[] _Values;
        [DataMember]
        public Stimulus[] Values
        {
            get { return _Values; }
            set
            {
                if (_Values == value) return;
                _Values = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public Stimulus[] Values

        #region public String Description
        private String _Description;
        [DataMember]
        public String Description
        {
            get { return _Description; }
            set
            {
                if (_Description == value) return;
                _Description = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public String Description

        public static StimulusSet FromString(params String[] valuesSequences)
        {
            var stimulus
                = valuesSequences
                .Select(v => Stimulus.FromString(v))
                .ToList();

            return new StimulusSet(stimulus);
        }

        public String ToStringComplete()
        {
            return ToStringFormat(-1, -1, -1, -1);
        }

        public String ToStringFormat(
            int itemsToTakeMaxStimulusSet, 
            int precisionStimulus, 
            int valuesToTakeMaxStimulus, 
            int valuesLineBreak)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(
                String.Format("Values.Count = {0}", Values.Length));

            var index = 0;

            var items 
                = itemsToTakeMaxStimulusSet != -1 
                ? Values.Take(itemsToTakeMaxStimulusSet).ToArray()
                : Values;

            foreach(var item in items)
            {
                var values = item.ToStringFormat(
                    precisionStimulus,
                    valuesToTakeMaxStimulus,
                    valuesLineBreak);

                stringBuilder.AppendLine(
                    String.Format("Values[{0}] = {1}", index++, values));
            }

            if(itemsToTakeMaxStimulusSet != -1 && itemsToTakeMaxStimulusSet != Values.Length)
            {
                stringBuilder.AppendLine("...");
            }

            return stringBuilder.ToString();
        }
    }
}
