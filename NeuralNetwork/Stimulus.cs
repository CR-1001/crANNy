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
    using CRAI.NeuralNetwork.Algebra;

    [Serializable] [DataContract] public class Stimulus : Vector
    {
        public Stimulus()
        {

        }

        public Stimulus(Stimulus toCopy)
            : this(toCopy.Values)
        {
            Id = toCopy.Id;
            IdOriginal = toCopy.IdOriginal;
            Description = toCopy.Description;
        }

        public Stimulus(IEnumerable<double> values)
        {
            Values = values.ToArray();
        }

        public Stimulus(int count)
        {
            Values = new double[count];
        }

        #region public Guid Id
        private Guid _Id;
        [DataMember]
        public Guid Id
        {
            get { return _Id; }
            set
            {
                if (_Id == value) return;
                _Id = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public Guid Id

        #region public Guid IdOriginal
        private Guid _IdOriginal;
        [DataMember]
        public Guid IdOriginal
        {
            get { return _IdOriginal; }
            set
            {
                if (_IdOriginal == value) return;
                _IdOriginal = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public Guid IdOriginal

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


        public override string ToString()
        {
            return ToStringFormat(2, 20, -1);
        }


        public string ToStringComplete()
        {
            return ToStringFormat(-1, -1, -1);
        }

        public string ToStringFormat(int precision, int valuesToTakeMax, int valuesLineBreak)
        {
            var stringBuilder = new StringBuilder();

            if(Id != Guid.Empty)
            {
                stringBuilder.AppendFormat("{0};", Id);
            }
            
            if(IdOriginal != Guid.Empty)
            {
                stringBuilder.AppendFormat("{0};", IdOriginal);
            }

            if(Description != null)
            {
                stringBuilder.AppendFormat("{0};", Description);
            }

            var valuesSequence = VectorOperations.ToString(
                Values, valuesToTakeMax, precision, valuesLineBreak);

            if (!String.IsNullOrEmpty(valuesSequence))
            {
                stringBuilder.Append(valuesSequence);
                stringBuilder.Append(";");
            }

            return stringBuilder.ToString();
        }

        public static Stimulus FromString(String valuesSequence, bool useCurrentCulture = true)
        {
            return new Stimulus() { Values = VectorOperations.FromString(valuesSequence, useCurrentCulture)};
        }
    }
}
