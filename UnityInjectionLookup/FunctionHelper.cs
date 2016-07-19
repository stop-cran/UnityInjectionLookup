using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity.Utility;

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
            Guard.ArgumentNotNull(function, "function");
            Guard.ArgumentNotNull(parameterTypes, "parameterTypes");
            Guard.ArgumentNotNull(resultType, "resultType");

            var parameters = parameterTypes.Select(Expression.Parameter).ToList();

            return Expression.Lambda(
                    Expression.Convert(
                    function.Target == null
                        ? Expression.Call(function.Method, ToObjectArray(parameters))
                        : Expression.Call(
                            Expression.Constant(function.Target),
                            function.Method,
                            ToObjectArray(parameters)),
                        resultType),
                    function.Method.Name + "_Typed",
                    parameters)
                .Compile();
        }

        static NewArrayExpression ToObjectArray(IEnumerable<Expression> values)
        {
            return Expression.NewArrayInit(typeof(object),
                values.Select(parameter => Expression.Convert(parameter, typeof(object))));
        }
    }
}
