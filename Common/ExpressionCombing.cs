/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Common
{
    using System;
    using System.Linq.Expressions;

    public static class ExpressionCombing
    {
        public static Expression<Func<T, bool>> True<T>() { return f => true; }

        public static Expression<Func<T, bool>> False<T>() { return f => false; }

        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> expression1, 
            Expression<Func<T, bool>> expression2)
        {
            var expressionInvoked 
                = Expression.Invoke(expression2, expression1.Parameters);

            var expressionElse
                = Expression.Lambda<Func<T, bool>>(
                    Expression.OrElse(expression1.Body, expressionInvoked),
                    expression1.Parameters);

            return expressionElse;
        }

        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> expression1, 
            Expression<Func<T, bool>> expression2)
        {
            var expressionInvoked 
                = Expression.Invoke(expression2, expression1.Parameters);

            var expressionAnd 
                = Expression.Lambda<Func<T, bool>>(
                    Expression.AndAlso(expression1.Body, expressionInvoked), 
                    expression1.Parameters);

            return expressionAnd;
        }

    }
}
