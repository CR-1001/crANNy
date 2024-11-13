/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Algebra
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using CRAI.Common;
    using System.Xml.Serialization;

    [DataContract] [Serializable]
    public class Matrix : NotifyBase<Matrix>
    {
        private volatile int _CountColumns = -1;

        private volatile int _CountRows = -1;

        #region public double[,] Values
        private double[,] _Values;
        [IgnoreDataMember]
        public double[,] Values
        {
            get { return _Values; }
            set
            {
                if (_Values == value) return;
                _Values = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(this, m => m.ValuesJagged);
            }
        }
        #endregion public double[,] Values

        [DataMember(Name="Values")]
        [XmlIgnore]
        public double[][] ValuesJagged
        {
            get
            {
                return MatrixOperations.ToTwoDimensionalJaggedArray(Values);
            }
            set
            {
                Values = MatrixOperations.ToTwoDimensionalArray(value);
            }
        }

        public int CountColumns
        {
            get
            {
                if (_CountColumns >= -1)
                {
                    lock (this)
                    {
                        if (_CountColumns >= -1)
                        {
                            if (Values == null) return 0;
                            _CountColumns = Values.GetLength(1);
                        }
                    }
                }

                return _CountColumns;
            }
            set
            {
                lock (this)
                {
                    _CountColumns = value;
                }
            }
        }

        public int CountRows
        {
            get
            {
            if (_CountRows >= -1)
                {
                    lock (this)
                    {
                        if (_CountRows >= -1)
                        {
                            if (Values == null) return 0;
                            _CountRows = Values.GetLength(0);
                        }
                    }
                }

                return _CountRows;
            }
            set
            {
                lock (this)
                {
                    _CountRows = value;
                }
            }
        }

        public double[,] ToArray()
        {
            return (double[,])Values.Clone();
        }

        public double this[int row, int column]
        {
            get { return Values[row, column]; }
            set { Values[row, column] = value; }
        }

        public Matrix()
        {
            _Values = new double[0, 0];
        }

        public Matrix(int rows, int columns)
        {
            Values = new double[rows, columns];
        }

        public Matrix(double[,] values)
        {
            Values = values;
        }

        public Matrix Copy()
        {
            var values = MatrixOperations.Copy(Values);

            return new Matrix(values);
        }


        public IEnumerator<Vector> GetEnumerator()
        {
            return MatrixOperations
                .GetRows(this)
                .GetEnumerator();
        }

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return MatrixOperations
        //        .GetRows(this)
        //        .GetEnumerator();
        //}

        public override bool Equals(Object obj)
        {
            var matrix = obj as Matrix;

            if (matrix == null) return false;

            if (matrix.CountColumns != CountColumns) return false;
            if (matrix.CountRows != CountRows) return false;

            for (var row = 0; row < CountRows; row++)
            {
                for (var column = 0; column < CountColumns; column++)
                {
                    var value1 = _Values[row, column];
                    var value2 = matrix.Values[row, column];

                    if (value1 != value2) return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            return Values.GetHashCode();
        }

        public string ToString(int precision, int rowsMax, int columnsMax)
        {
            return MatrixOperations.ToString(Values, precision, rowsMax, columnsMax);
        }

        public override string ToString()
        {
            return ToString(2, 10, 10);
        }

        public string ToStringComplete()
        {
            return MatrixOperations.ToString(Values, -1, -1, -1);
        }
    }
}
