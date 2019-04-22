using System;
using System.Collections.Generic;
using System.Text;

namespace Features
{
    public static class MyLinq
    {
        // Extension methods pust be a public static method on a public static class
        // They are accessible to any data type that satisfies the data type of the first parameter of the extension method
        // the first parameter must have the keyword "this" at the front
        // this signifies that the first parameter variable is a reference to the object calling it
        // extension methods can have additional parameters as well that would need to be explicitly passed
        public static int Count<T>(this IEnumerable<T> sequence)
        {
            int count = 0;
            foreach (var item in sequence)
            {
                count += 1;
            }
            return count;
        }
    }
}
