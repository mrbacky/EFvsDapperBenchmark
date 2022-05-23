using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    public static class Constants
    {
        public static String ConnectionStringDapper { get; } =
            "Server=localhost;Database=efdapperbenchmark;User Id=sa;Password=pass1234!@;";
        public static String ConnectionStringEF { get; } =
            "Server=localhost;Database=efdapperbenchmark;User Id=sa;Password=pass1234!@;";
    }
}
