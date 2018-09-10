using System;
using System.Collections.Concurrent;

namespace dotnet
{
    public static class CategoryTheoryForProgrammers
    {
        public static T Identity<T> (T me ) => me;

        public static Func<Ta,Tc> Compose<Ta,Tb,Tc> (Func<Ta,Tb> fa,Func<Tb,Tc> fb) => (Ta a) => fb(fa(a));

        public static Func<Ta,Tb> Memoize<Ta,Tb> (Func<Ta,Tb> fun){
            var dict = new ConcurrentDictionary<Ta,Tb>();
            Func<Ta,Tb> memoFun = (Ta a) => {
                if(dict.ContainsKey(a) & dict.TryGetValue(a, out var b)) 
                {
                    return b;
                } else {
                    var result = fun(a);
                    dict.TryAdd(a, result);
                    return result;
                }
            };
            return memoFun;
        }
    }
}
