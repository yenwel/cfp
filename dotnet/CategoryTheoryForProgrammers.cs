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

    public abstract class Either<Tleft,Tright>{
        protected Tleft Left {get; private set; }
        protected Tright Right {get; private set; }
        protected Either(Tleft left) { Left = left;}        
        protected Either(Tright right) { Right = right;}
    }

    public class Left<Tleft, Tright> : Either<Tleft,Tright>
    {
        public Tleft Value { get { return Left;} }
        public Left(Tleft value) : base(value){ }
    }

    public class Right<Tleft, Tright> : Either<Tleft,Tright>
    {                
        public Tright Value { get { return Right;} }
        public Right(Tright value) : base(value){ }
    }

    public abstract class WorseThenEither<Tleft,Tright>{
        protected Tleft Left {get; private set; }
        protected Tright Right {get; private set; }
        protected WorseThenEither(Tleft left) { Left = left;}        
        protected WorseThenEither(Tright right) { Right = right;}
        protected Tleft FarLeft {get; private set; }
        protected Tright FarRight {get; private set; }
        protected WorseThenEither(Tleft left, Tleft farLeft) { Left = left; FarLeft = farLeft;}        
        protected WorseThenEither(Tright right, Tright farRight) { Right = right; FarRight = farRight;}
    }

    public class WorseThenLeft<Tleft, Tright> : WorseThenEither<Tleft,Tright>
    {
        public Tleft Value { get { return Left;} }
        public WorseThenLeft(Tleft value) : base(value){ }
    }

    public class WorseThenRight<Tleft, Tright> : WorseThenEither<Tleft,Tright>
    {                
        public Tright Value { get { return Right;} }
        public WorseThenRight(Tright value) : base(value){ }
    }

    public class WorseThenFarLeft<Tleft, Tright> : WorseThenEither<Tleft,Tright>
    {
        public Tleft Value { get { return Left;} }
        public Tleft FarValue { get { return FarLeft;} }
        public WorseThenFarLeft(Tleft value, Tleft farValue) : base(value, farValue){ }
    }

    public class WorseThenFarRight<Tleft, Tright> : WorseThenEither<Tleft,Tright>
    {                
        public Tright Value { get { return Right;} }        
        public Tright FarValue { get { return FarRight;} }
        public WorseThenFarRight(Tright value,Tright farValue) : base(value, farValue){ }
    }

    public class IntegerWithTwoInjections{

        public int Value {get; private set;}
        private IntegerWithTwoInjections(int value)
        {
            Value = value;
        }
        public static IntegerWithTwoInjections iSingleQoute(int n){
            return  new IntegerWithTwoInjections(n);
        }

        public static IntegerWithTwoInjections jSingleQoute(bool b){
            return new IntegerWithTwoInjections(b?0:1);
        }
        
        
        //IntegerWithTwoInjections clearly worse then Either because when injecting, 
        //the values from int and bool overlap at (0 and 1)
        public static IntegerWithTwoInjections mFactorize(Either<int,bool> e){
            switch(e){
                case Left<int,bool> i : return iSingleQoute(i.Value);
                case Right<int,bool> j : return jSingleQoute(j.Value);
                default : return new IntegerWithTwoInjections(0);
            }
        }
    }
}
