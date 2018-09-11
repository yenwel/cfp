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
    }
}
