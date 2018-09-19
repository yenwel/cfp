using System;
using Xunit;
using dotnet;
using System.Threading;

namespace dotnet_test
{
    public class CategoryTheoryForProgrammersTests
    {
        [Fact]
        public void CompositionPreservesIdentity()
        {            
            var one = CategoryTheoryForProgrammers.Compose<int,int,int>(CategoryTheoryForProgrammers.Identity,(int a) => -a);
            var two = CategoryTheoryForProgrammers.Compose<int,int,int>((int a) => -a, CategoryTheoryForProgrammers.Identity);
            Assert.True(one(8)==two(8));
            Assert.True(one(-8)==two(-8));
        }

        [Fact]
        public void MemoizeWorks()
        {            
            Func<int,int> addOneLongRunning = (int a) => { Thread.Sleep(50); return a++; };
            var addOneLongRunningMemoized = CategoryTheoryForProgrammers.Memoize(addOneLongRunning);
            var time = DateTime.Now;        
            Console.WriteLine(time);  
            Assert.True(addOneLongRunning(42)==addOneLongRunning(42));
            var runtimeOne = ( DateTime.Now - time);
            time = DateTime.Now;
            Assert.True(addOneLongRunning(42)==addOneLongRunning(42)); 
            var runtimeTwo = (DateTime.Now - time );
            Assert.True(runtimeTwo < runtimeOne );
        }

        [Fact]
        public void MemoizeRandomDoesNotWork()
        {            
            Func<int,double> random = (int _) => { var rand = new Random(); return rand.NextDouble();};
            var randomMemoized = CategoryTheoryForProgrammers.Memoize(random);
            Assert.True(randomMemoized(42)!=random(42));
            Assert.True(randomMemoized(42)!=random(42));
        }
        
        [Fact]
        public void MemoizeRandomSeededWorks()
        {            
            Func<int,double> randomSeeded = (int seed) => { var rand = new Random(seed); return rand.NextDouble();};
            var randomSeededMemoized = CategoryTheoryForProgrammers.Memoize(randomSeeded);
            Assert.True(randomSeededMemoized(42)==randomSeeded(42));            
            Assert.True(randomSeededMemoized(42)==randomSeeded(42));
        }

        private void assertPurityForInput<Tin,Tout>(Func<Tin,Tout> func,Tin input, bool isPure)
        where Tout: IEquatable<Tout>
        {            
            var funcMemoized = CategoryTheoryForProgrammers.Memoize(func);
            Assert.True(funcMemoized(input).Equals(func(input))==isPure);            
            Assert.True(funcMemoized(input).Equals(func(input))==isPure);
        }

        [Fact]
        public void FactorialIsPure()
        {            
           assertPurityForInput(
               (int n) => 
               {
                   int i; 
                   var result = 1; 
                   for(i=2; i<=n; ++i) result *= i;
                   return result;
                },
                42,
                true);
        }

        /*[Fact]
        public void ReadKeyIsImpure()
        {            
           assertPurityForInput(
               (int _) => 
               {
                   return Console.ReadKey(true).KeyChar;
                },
                42,
                false);
        }*/

        [Fact]
        public void ReturnTrueIsPure()
        {            
           assertPurityForInput(
                (int _) => 
                {
                    Console.WriteLine("Hello!");
                    return true;
                },
                42,
                true);
        }


        [Fact]
        public void IncrementWithStaticInClosureScopeIsImpure()
        {            
            var y = 0;
            assertPurityForInput(
                (int x) => 
                {
                    y+=x;
                    return y;
                },
                42,
                false);
        }
    }
}
