using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace UnityInjectionLookup
{
    public static class FunctionHelper
    {
        public static Type GetFuncType(int rank)
        {
            switch (rank)
            {
                case 1: return typeof(Func<>);
                case 2: return typeof(Func<,>);
                case 3: return typeof(Func<,,>);
                case 4: return typeof(Func<,,,>);
                case 5: return typeof(Func<,,,,>);
                case 6: return typeof(Func<,,,,,>);
                case 7: return typeof(Func<,,,,,,>);
                case 8: return typeof(Func<,,,,,,,>);
                case 9: return typeof(Func<,,,,,,,,>);
                default: throw new ArgumentOutOfRangeException("rank");
            }
        }

        public static Delegate MakeTyped(Func<object[], object> function, IEnumerable<Type> parameterTypes, Type resultType)
        {
            var parameters = (parameterTypes ?? throw new ArgumentNullException(nameof(parameterTypes)))
                .Select(Expression.Parameter).ToList();

            return Expression.Lambda(
                    Expression.Convert(
                    (function ?? throw new ArgumentNullException(nameof(function))).Target == null
                        ? Expression.Call(function.Method, ToObjectArray(parameters))
                        : Expression.Call(
                            Expression.Constant(function.Target),
                            function.Method,
                            ToObjectArray(parameters)),
                        resultType ?? throw new ArgumentNullException(nameof(resultType))),
                    function.Method.Name + "_Typed",
                    parameters)
                .Compile();
        }

        private static NewArrayExpression ToObjectArray(IEnumerable<Expression> values) =>
            Expression.NewArrayInit(typeof(object),
                values.Select(parameter => Expression.Convert(parameter, typeof(object))));
    }
}
