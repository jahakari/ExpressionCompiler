using ExpressionCompiler.Syntax.Nodes;
using System;
using System.Linq;
using System.Reflection;

namespace ExpressionCompiler.Utility
{
    public static class TypeHelper
    {
        private const string op_Addition = nameof(op_Addition);
        private const string op_Subtraction = nameof(op_Subtraction);
        private const string op_Multiply = nameof(op_Multiply);
        private const string op_Division = nameof(op_Division);
        private const string op_Modulus = nameof(op_Modulus);
        private const string op_Equality = nameof(op_Equality);
        private const string op_Inequality = nameof(op_Inequality);
        private const string op_LessThan = nameof(op_LessThan);
        private const string op_LessThanOrEqual = nameof(op_LessThanOrEqual);
        private const string op_GreaterThan = nameof(op_GreaterThan);
        private const string op_GreaterThanOrEqual = nameof(op_GreaterThanOrEqual);
        private const string op_Explicit = nameof(op_Explicit);

        private const string get_Day = nameof(get_Day);
        private const string get_Month = nameof(get_Month);
        private const string get_Year = nameof(get_Year);
        private const string get_Today = nameof(get_Today);

        private const string get_Length = nameof(get_Length);

        private static ConstructorInfo decimalConstructorFromParts;
        public static ConstructorInfo DecimalConstructorFromParts
            => decimalConstructorFromParts ??= typeof(decimal)
                .GetConstructor(new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(byte) });

        private static ConstructorInfo decimalConstructorFromDouble;
        public static ConstructorInfo DecimalConstructorFromDouble
            => decimalConstructorFromDouble ??= typeof(decimal).GetConstructor(new[] { typeof(double) });

        private static ConstructorInfo decimalConstructorFromInt;
        public static ConstructorInfo DecimalConstructorFromInt
            => decimalConstructorFromInt ??= typeof(decimal).GetConstructor(new[] { typeof(int) });

        private static MethodInfo decimalAddMethod;
        public static MethodInfo DecimalAddMethod
            => decimalAddMethod ??= GetDecimalOperatorMethod(op_Addition);

        private static MethodInfo decimalSubtractMethod;
        public static MethodInfo DecimalSubtractMethod
            => decimalSubtractMethod ??= GetDecimalOperatorMethod(op_Subtraction);

        private static MethodInfo decimalMultiplyMethod;
        public static MethodInfo DecimalMultiplyMethod
            => decimalMultiplyMethod ??= GetDecimalOperatorMethod(op_Multiply);

        private static MethodInfo decimalDivideMethod;
        public static MethodInfo DecimalDivideMethod
            => decimalDivideMethod ??= GetDecimalOperatorMethod(op_Division);

        private static MethodInfo decimalModulusMethod;
        public static MethodInfo DecimalModulusMethod
            => decimalModulusMethod ??= GetDecimalOperatorMethod(op_Modulus);

        private static MethodInfo decimalEqualsMethod;
        public static MethodInfo DecimalEqualsMethod
            => decimalEqualsMethod ??= GetDecimalOperatorMethod(op_Equality);

        private static MethodInfo decimalNotEqualsMethod;
        public static MethodInfo DecimalNotEqualsMethod
            => decimalNotEqualsMethod ??= GetDecimalOperatorMethod(op_Inequality);

        private static MethodInfo decimalLessThanMethod;
        public static MethodInfo DecimalLessThanMethod
            => decimalLessThanMethod ??= GetDecimalOperatorMethod(op_LessThan);

        private static MethodInfo decimalLessThanOrEqualMethod;
        public static MethodInfo DecimalLessThanOrEqualMethod
            => decimalLessThanOrEqualMethod ??= GetDecimalOperatorMethod(op_LessThanOrEqual);

        private static MethodInfo decimalGreaterThanMethod;
        public static MethodInfo DecimalGreaterThanMethod
            => decimalGreaterThanMethod ??= GetDecimalOperatorMethod(op_GreaterThan);

        private static MethodInfo decimalGreaterThanOrEqualMethod;
        public static MethodInfo DecimalGreaterThanOrEqualMethod
            => decimalGreaterThanOrEqualMethod ??= GetDecimalOperatorMethod(op_GreaterThanOrEqual);

        private static MethodInfo decimalToFloatExplicitCastMethod;
        public static MethodInfo DecimalToFloatExplicitCastMethod
            => decimalToFloatExplicitCastMethod ??= GetExplicitCastMethod(typeof(decimal), typeof(double));

        public static MethodInfo GetExplicitCastMethod(Type sourceType, Type returnType)
        {
            return sourceType
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == op_Explicit)
                .First(m => m.ReturnType == returnType);
        }

        private static MethodInfo GetDecimalOperatorMethod(string name)
            => typeof(decimal).GetMethod(name, BindingFlags.Public | BindingFlags.Static);

        private static MethodInfo mathPowMethod;
        public static MethodInfo MathPowMethod
            => mathPowMethod ??= typeof(Math).GetMethod(nameof(Math.Pow));

        public static MethodInfo GetDecimalMethodForOperator(OperatorType operatorType)
        {
            return operatorType switch
            {
                OperatorType.Add                => DecimalAddMethod,
                OperatorType.Subtract           => DecimalSubtractMethod,
                OperatorType.Multiply           => DecimalMultiplyMethod,
                OperatorType.Divide             => DecimalDivideMethod,
                OperatorType.Modulo             => DecimalModulusMethod,
                OperatorType.Exponent           => MathPowMethod,
                OperatorType.Equal              => DecimalEqualsMethod,
                OperatorType.NotEqual           => DecimalNotEqualsMethod,
                OperatorType.LessThan           => DecimalLessThanMethod,
                OperatorType.LessThanOrEqual    => DecimalLessThanOrEqualMethod,
                OperatorType.GreaterThan        => DecimalGreaterThanMethod,
                OperatorType.GreaterThanOrEqual => DecimalGreaterThanOrEqualMethod,
                _                               => throw new InvalidOperationException($"Operator '{operatorType}' is not supported.")
            };
        }

        public static string GetOperatorNameForOperatorType(OperatorType operatorType)
        {
            return operatorType switch
            {
                OperatorType.Add                => op_Addition,
                OperatorType.Subtract           => op_Subtraction,
                OperatorType.Multiply           => op_Multiply,
                OperatorType.Divide             => op_Multiply,
                OperatorType.Modulo             => op_Modulus,
                OperatorType.Equal              => op_Equality,
                OperatorType.NotEqual           => op_Inequality,
                OperatorType.LessThan           => op_LessThan,
                OperatorType.LessThanOrEqual    => op_LessThanOrEqual,
                OperatorType.GreaterThan        => op_GreaterThan,
                OperatorType.GreaterThanOrEqual => op_GreaterThanOrEqual,
                _                               => throw new InvalidOperationException($"Operator '{operatorType}' is not supported.")
            };
        }

        private static ConstructorInfo dateTimeConstructor;
        public static ConstructorInfo DateTimeConstructor
            => dateTimeConstructor ??= typeof(DateTime).GetConstructor(new[] { typeof(int), typeof(int), typeof(int) });

        private static MethodInfo dateTimeDayGetter;
        public static MethodInfo DateTimeDayGetter
            => dateTimeDayGetter ??= GetDatePropertyMethod(get_Day);

        private static MethodInfo dateTimeMonthGetter;
        public static MethodInfo DateTimeMonthGetter
            => dateTimeMonthGetter ??= GetDatePropertyMethod(get_Month);

        private static MethodInfo dateTimeYearGetter;
        public static MethodInfo DateTimeYearGetter
            => dateTimeYearGetter ??= GetDatePropertyMethod(get_Year);

        private static MethodInfo dateTimeTodayGetter;
        public static MethodInfo DateTimeTodayGetter
            => dateTimeTodayGetter ??= GetDatePropertyMethod(get_Today);

        private static MethodInfo GetDatePropertyMethod(string name)
            => typeof(DateTime).GetMethod(name);

        private static MethodInfo dataContextGetIntegerMethod;
        public static MethodInfo DataContextGetIntegerMethod
            => dataContextGetIntegerMethod ??= typeof(IIdentifierDataContext).GetMethod(nameof(IIdentifierDataContext.GetInteger));

        private static MethodInfo dataContextGetDecimalMethod;
        public static MethodInfo DataContextGetDecimalMethod
            => dataContextGetDecimalMethod ??= typeof(IIdentifierDataContext).GetMethod(nameof(IIdentifierDataContext.GetDecimal));

        private static MethodInfo dataContextGetDateMethod;
        public static MethodInfo DataContextGetDateMethod
            => dataContextGetDateMethod ??= typeof(IIdentifierDataContext).GetMethod(nameof(IIdentifierDataContext.GetDate));

        private static MethodInfo dataContextGetStringMethod;
        public static MethodInfo DataContextGetStringMethod
            => dataContextGetStringMethod ??= typeof(IIdentifierDataContext).GetMethod(nameof(IIdentifierDataContext.GetString));

        private static MethodInfo stringLengthGetter;
        public static MethodInfo StringLengthGetter
            => stringLengthGetter ??= typeof(string).GetMethod(get_Length);

        private static MethodInfo stringSubstringMethod;
        public static MethodInfo StringSubstringMethod
            => stringSubstringMethod ??= typeof(string).GetMethod(nameof(string.Substring), new[] { typeof(int) });

        private static MethodInfo stringSubstringWithLengthMethod;
        public static MethodInfo StringSubstringWithLengthMethod
            => stringSubstringWithLengthMethod ??= typeof(string).GetMethod(nameof(string.Substring), new[] { typeof(int), typeof(int) });
    }
}
