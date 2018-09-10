using System;
using Xunit;
using dotnet;

namespace dotnet_test
{
    public class CategoryTheoryForProgrammersTests
    {
        [Fact]
        public void CompositionPreservesIdentity()
        {
            var f_one = CategoryTheoryForProgrammers.Compose<int,int,int>(CategoryTheoryForProgrammers.Identity,(int a) => -a);
            var f_two = CategoryTheoryForProgrammers.Compose<int,int,int>(CategoryTheoryForProgrammers.Identity,(int a) => -a);
            Assert.True(f_one(8)==f_two(8));
            Assert.True(f_one(-8)==f_two(-8));
        }
    }
}
