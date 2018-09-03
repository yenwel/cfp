using System;

namespace dotnet
{
    public static class CategoryTheoryForProgrammers
    {
        public static T Identity<T>(T me) => me;

        public static Func<Ta,Tc> Compose<Ta,Tb,Tc> (Func<Ta,Tb> fa,Func<Tb,Tc> fb) => (Ta a) => fb(fa(a));
    }
}
