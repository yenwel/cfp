using System;
using System.Collections.Concurrent;

namespace dotnet
{
    using PartialFuncDouble = System.Func<double?,double?>; //no generic type aliases eh
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

        public static bool boolone(bool b) => b;
        public static bool booltwo(bool b) => !b;        
        public static bool boolthree(bool _b) => true;
        public static bool boolfour(bool _b) => false;

        public static PartialFuncDouble IdentityPartial(PartialFuncDouble pfd) => Identity(pfd);
        
        public static PartialFuncDouble ComposePartial(PartialFuncDouble pfdOne, PartialFuncDouble pfdTwo) => Compose<double?,double?,double?>(pfdOne, pfdTwo);
        public static double? SafeRoot(double? x){
            if(x.HasValue && x >= 0) return Math.Sqrt(x.Value);
            return null;
        }

        public static double? SafeReciprocal(double? x){
            if(x.HasValue && x != 0) return 1/x;
            return null;
        }

        public static double? SafeRootReciprocal(double? x) 
        { 
            return ComposePartial(SafeRoot,SafeReciprocal)(x);
        }
    }

    public abstract class Either<Teither>{
        public Teither Value {get; private set; }
        public Either(Teither value) { Value = value;}
    }

    public class Left<Tleft> : Either<Tleft>
    {
        public Left(Tleft value) : base(value){ }
    }

    public class Right<Tright> : Either<Tright> 
    {        
        public Right(Tright value) : base(value){ }
    }


}
