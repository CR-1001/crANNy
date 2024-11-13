/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Algebra
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using CRAI.Common;

    [Serializable] [DataContract] public class Vector : NotifyBase<Vector>, IEnumerable<double>
    {
        #region public double Values
        private double[] _Values = new double[0];
        [DataMember]
        public double[] Values
        {
            get { return _Values; }
            set
            {
                if (_Values == value) return;
                _Values = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public double Values
        
        public int Dimension
        {
            get { return Values.Length; }
        }

        public double this[int i]
        {
            get { return Values[i]; }
            set { Values[i] = value; }
        }

        public double[] ToArray()
        {
            return Values.ToArray();
        }

        public Vector()
        {
        }

        public Vector(double[] values)
        {
            Values = values;
        }

        public Vector(int dimension)
        {
            Values = new double[dimension];
        }
        
        public static Vector Construct(IEnumerable<double> values)
        {
            return new Vector(values.ToArray());
        }
       
        public Vector Copy()
        {
            return Construct(Values);
        }

        public override bool Equals(Object obj)
        {
            var vector = obj as Vector;

            if (vector == null) return false;

            if (vector.Dimension != Dimension) return false;

            for(var i = 0; i < Dimension; i++)
            {
                if(vector[i] != _Values[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return Values.GetHashCode();
        }

        public IEnumerator<double> GetEnumerator()
        {
            return Values.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        public override string ToString()
        {
            return VectorOperations.ToString(Values, 10, 2, -1);
        }
        
        public string ToStringComplete()
        {
            return VectorOperations.ToString(Values, -1, -1, -1);
        }
    }
}
