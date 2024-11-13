/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.NeuralNetwork.Algebra
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using CRAI.NeuralNetwork.Randomizer;

    public static class MatrixOperations
    {
        public static void AssertDimensionsEqual(Matrix A, Matrix B)
        {
            if (A.CountColumns != B.CountColumns || A.CountRows != B.CountRows)
            {
                throw new NeuralNetworkException(Errors.MatrixDimensionsMissmatch);
            }
        }

        public static double[,] Copy(double[,] values)
        {
            var columns = values.GetLength(1);
            var rows = values.GetLength(0);

            var valuesCopy = new double[rows, columns];

            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    valuesCopy[row, column] = values[row, column];
                }
            }

            return valuesCopy;
        }

        public static void Replace(this Matrix matrix, Func<int, int, double, double> replace)
        {
            for (var row = 0; row < matrix.CountRows; row++)
            {
                for (var column = 0; column < matrix.CountColumns; column++)
                {
                    var value = matrix[row, column];

                    matrix[row, column] = replace(row, column, value);
                }
            }
        }

        public static Matrix RemoveRows(this Matrix matrix, params int[] rowsToDelete)
        {
            foreach (var rowToDelete in rowsToDelete)
            {
                if (rowToDelete >= matrix.CountRows)
                {
                    throw new NeuralNetworkException(Errors.MatrixDimensionsMissmatch);
                }
            }

            var values = new double[matrix.CountRows - rowsToDelete.Length, matrix.CountColumns];

            var rowStep = 0;

            for (var row = 0; row < matrix.CountRows; row++)
            {
                if (!rowsToDelete.Contains(row))
                {
                    for (var column = 0; column < matrix.CountColumns; column++)
                    {
                        values[row - rowStep, column] = matrix[row, column];
                    }
                }
                else
                {
                    rowStep++;
                }
            }

            return new Matrix(values);
        }

        public static Matrix RemoveColumns(this Matrix matrix, params int[] columnsToDelete)
        {
            foreach (var columnToDelete in columnsToDelete)
            {
                if (columnToDelete >= matrix.CountColumns)
                {
                    throw new NeuralNetworkException(Errors.MatrixDimensionsMissmatch);
                }
            }

            var values = new double[matrix.CountRows, matrix.CountColumns - columnsToDelete.Length];

            for (var row = 0; row < matrix.CountRows; row++)
            {
                var columnStep = 0;

                for (var column = 0; column < matrix.CountColumns; column++)
                {
                    if (!columnsToDelete.Contains(column))
                    {
                        values[row, column - columnStep] = matrix[row, column];
                    }
                    else
                    {
                        columnStep++;
                    }
                }

            }

            return new Matrix(values);
        }

        public static Matrix Add(this Matrix matrix1, Matrix matrix2)
        {
            MatrixOperations.AssertDimensionsEqual(matrix1, matrix2);

            var values = new double[matrix2.CountRows, matrix2.CountColumns];

            for (var row = 0; row < matrix2.CountRows; row++)
            {
                for (var column = 0; column < matrix2.CountColumns; column++)
                {
                    values[row, column] = matrix1[row, column] + matrix2[row, column];
                }
            }

            return new Matrix(values);
        }

        public static Matrix Subtract(this Matrix matrix1, Matrix matrix2)
        {
            MatrixOperations.AssertDimensionsEqual(matrix1, matrix2);

            var values = new double[matrix2.CountRows, matrix2.CountColumns];

            for (var row = 0; row < matrix2.CountRows; row++)
            {
                for (var column = 0; column < matrix2.CountColumns; column++)
                {
                    values[row, column] = matrix1[row, column] - matrix2[row, column];
                }
            }

            return new Matrix(values);
        }

        public static Matrix Negate(this Matrix matrix1)
        {
            return Multiply(matrix1, -1);
        }

        public static Matrix Multiply(this Matrix matrix, double scalar)
        {
            var values = new double[matrix.CountRows, matrix.CountColumns];

            for (var row = 0; row < matrix.CountRows; row++)
            {
                for (var column = 0; column < matrix.CountColumns; column++)
                {
                    values[row, column] = scalar * matrix[row, column];
                }
            }

            return new Matrix(values);
        }

        public static Matrix Multiply(this Matrix matrix1,
            Matrix matrix2)
        {
            if (matrix1.CountColumns != matrix2.CountRows)
            {
                throw new NeuralNetworkException(Errors.MatrixDimensionsMissmatch);
            }

            var values = new double[matrix1.CountRows, matrix2.CountColumns];

            for (var i = 0; i < matrix1.CountRows; ++i)
            {
                for (var j = 0; j < matrix2.CountColumns; ++j)
                {
                    for (var k = 0; k < matrix1.CountColumns; ++k)
                    {
                        values[i, j] += matrix1[i, k] * matrix2[k, j];
                    }
                }
            }

            return new Matrix(values);
        }

        public static Matrix Sub(this Matrix matrix, int rowStart, int rowEnd, int columnStart, int columnEnd)
        {
            var rows = rowEnd - rowStart + 1;
            var columns = columnEnd - columnStart + 1;

            var values = new double[rows, columns];

            for (var row = rowStart; row <= rowEnd; row++)
            {
                for (var column = columnStart; column <= columnEnd; column++)
                {
                    var value = matrix[row, column];
                    values[row - rowStart, column - columnStart] = value;
                }
            }

            return new Matrix(values);
        }

        public static Matrix Sub(this Matrix matrix, IEnumerable<int> rowsToTake, IEnumerable<int> columnsToTake)
        {
            var rows = rowsToTake.ToArray();
            var columns = columnsToTake.ToArray();

            var values = new double[rows.Length, columns.Length];

            for (var row = 0; row < rows.Length; row++)
            {
                for (var column = 0; column < columns.Length; column++)
                {
                    var rowIndex = rows[row];
                    var columnIndex = columns[column];

                    var value = matrix[rowIndex, columnIndex];

                    values[row, column] = value;
                }
            }

            return new Matrix(values);
        }

        public static Matrix Transpose(this Matrix matrix)
        {
            var values = new double[matrix.CountColumns, matrix.CountRows];

            for (var row = 0; row < matrix.CountRows; row++)
            {
                for (var column = 0; column < matrix.CountColumns; column++)
                {
                    values[column, row] = matrix[row, column];
                }
            }

            return new Matrix(values);
        }

        public static Matrix Divide(this Matrix matrix1, Matrix matrix2)
        {
            MatrixOperations.AssertDimensionsEqual(matrix1, matrix2);

            var values = new double[matrix1.CountRows, matrix1.CountColumns];

            for (var row = 0; row < matrix1.CountRows; row++)
            {
                for (var column = 0; column < matrix1.CountColumns; column++)
                {
                    var value1 = matrix1[row, column];
                    var value2 = matrix2[row, column];

                    values[row, column] = value1 / value2;
                }
            }

            return new Matrix(values);
        }

        public static void Fill(this Matrix matrix, double value)
        {
            for (var row = 0; row < matrix.CountRows; row++)
            {
                for (var column = 0; column < matrix.CountColumns; column++)
                {
                    matrix[row, column] = value;
                }
            }
        }

        public static void SetColumn(this Matrix matrix, int column, Vector vector)
        {

            if (vector.Dimension != matrix.CountRows || column >= matrix.CountColumns || column < 0)
            {
                throw new NeuralNetworkException(Errors.MatrixDimensionsMissmatch);
            }

            for (var row = 0; row < matrix.CountRows; row++)
            {
                matrix[row, column] = vector[row];
            }
        }

        public static void SetRow(this Matrix matrix, int row, Vector vector)
        {

            if (vector.Dimension != matrix.CountColumns || row >= matrix.CountRows || row < 0)
            {
                throw new NeuralNetworkException(Errors.MatrixDimensionsMissmatch);
            }

            for (var column = 0; column < matrix.CountColumns; column++)
            {
                matrix[row, column] = vector[column];
            }
        }

        public static Vector GetColumn(this Matrix matrix, int column)
        {
            if (column >= matrix.CountColumns || column < 0)
            {
                throw new NeuralNetworkException(Errors.MatrixDimensionsMissmatch);
            }

            var values = new double[matrix.CountRows];

            for (var row = 0; row < matrix.CountRows; row++)
            {
                values[row] = matrix[row, column];
            }

            return new Vector(values);
        }

        public static Vector GetRow(this Matrix matrix, int row)
        {
            if (row >= matrix.CountRows || row < 0)
            {
                throw new NeuralNetworkException(Errors.MatrixDimensionsMissmatch);
            }

            var values = new double[matrix.CountColumns];

            for (var column = 0; column < matrix.CountColumns; column++)
            {
                values[column] = matrix[row, column];
            }

            return new Vector(values);
        }

        public static IEnumerable<Vector> GetRows(this Matrix matrix)
        {
            for (var row = 0; row < matrix.CountRows; row++)
            {
                var vector = GetRow(matrix, row);

                yield return vector;
            }
        }

        public static IEnumerable<Vector> GetColumns(this Matrix matrix)
        {
            for (var column = 0; column < matrix.CountColumns; column++)
            {
                var vector = GetColumn(matrix, column);

                yield return vector;
            }
        }

        public static void RandomChange(
            this Matrix matrix,
            IRandomizer randomizer,
            double minValue,
            double maxValue,
            IDictionary<String, Object> parameters)
        {
            randomizer.Setup(minValue, maxValue, parameters);

            var rows = matrix.CountRows;
            var columns = matrix.CountColumns;

            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    matrix[row, column] *= randomizer.Next();
                }
            }
        }

        public static void Random(
            this Matrix matrix,
            IRandomizer randomizer,
            double minValue,
            double maxValue,
            IDictionary<String, Object> parameters)
        {
            randomizer.Setup(minValue, maxValue, parameters);

            var rows = matrix.CountRows;
            var columns = matrix.CountColumns;

            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    matrix[row, column] = randomizer.Next();
                }
            }
        }

        public static Vector ToVectorByRows(this Matrix matrix)
        {
            var rows = matrix.CountRows;
            var columns = matrix.CountColumns;

            var values = new double[rows * columns];

            var index = 0;

            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    values[index++] = matrix[row, column];
                }
            }

            return new Vector(values);
        }

        public static double[][] ToTwoDimensionalJaggedArray(this double[,] values)
        {
            var rows = values.GetLength(0);
            var columns = values.GetLength(1);

            var valuesJagged = new double[rows][];

            for (var row = 0; row < rows; row++)
            {
                valuesJagged[row] = new double[columns];

                for (var column = 0; column < columns; column++)
                {

                    valuesJagged[row][column] = values[row, column];
                }
            }

            return valuesJagged;
        }

        public static double[,] ToTwoDimensionalArray(this double[][] valuesJagged)
        {
            var rows = valuesJagged.Length;
            var columns = rows == 0 ? 0 : valuesJagged[0].Length;

            var values = new double[rows, columns];

            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {

                    values[row, column] = valuesJagged[row][column];
                }
            }

            return values;
        }

        private static readonly char[] separators
            = new char[] { '|', ' ', '/' };

        public static String ToString(
            double[,] values,
            int precision)
        {
            return ToString(values, precision, -1, -1);
        }

        public static String ToString(
            double[,] values,
            int precision,
            int rowsMax,
            int columnsMax)
        {
            var stringBuilder = new StringBuilder();

            var rows = values.GetLength(0);
            var columns = values.GetLength(1);

            if (rowsMax < 0)
            {
                rowsMax = rows;
            }
            else
            {
                rowsMax = Math.Min(rowsMax, rows);
            }

            if (columnsMax < 0)
            {
                columnsMax = columns;
            }
            else
            {
                columnsMax = Math.Min(columnsMax, columns);
            }

            var valueFormat
                = (precision < 0
                ? "{0}"
                : "{0:0." + new String('0', precision) + "}")
                + separators[0];

            for (var row = 0; row < rowsMax; row++)
            {
                for (var column = 0; column < columnsMax; column++)
                {
                    var value = values[row, column];

                    stringBuilder.AppendFormat(
                        CultureInfo.InvariantCulture, valueFormat, value);
                }

                stringBuilder.AppendLine();
            }

            if (rows != rowsMax || columns != columnsMax)
            {
                stringBuilder.AppendLine("...");
            }

            return stringBuilder.ToString();
        }

        public static Matrix FromString(String valuesSequence, bool useCurrentCulture = true)
        {
            var values
                = valuesSequence
                .Split(new [] { Environment.NewLine, ";" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(v => VectorOperations.FromString(v, useCurrentCulture))
                .ToArray();

            if(!values.Any() || values[0].Length == 0)
            {
                return null;
            }

            if(values.Select(v => v.Length).Distinct().Count() != 1)
            {
                throw new NeuralNetworkException(Errors.MatrixDimensionsMissmatch);
            }

            var matrix = new Matrix(values.Length, values[0].Length);

            for(var row = 0; row < values.Length; row++)
            {
                for(var column = 0; column < values[0].Length; column++)
                {
                    matrix[row, column] = values[row][column];
                }
            }

            return matrix;
        }

    }


}
