using System;
using System.Collections.Generic;
using System.Linq;

namespace Features
{
    class Program
    {
        static void Main(string[] args)
        {
            // delegate types

            //"Func" type parameters - the last will always be the return value
            Func<int, int> square = x => x * x;
            Func<int, int, int> add = (x, y) => x + y;

            // "Action" always returns void, type paramaters are just describing the function parameters
            Action<int> write = x => Console.WriteLine(x);

            Console.WriteLine(square(add(3, 5)));

            IEnumerable<Employee> developers = new Employee[]
            {
                new Employee { Id = 1, Name = "Scott" },
                new Employee { Id = 2, Name = "Chris" }
            };

            IEnumerable<Employee> sales = new List<Employee>()
            {
                new Employee { Id = 3, Name = "Alex" }
            };

            // method syntax of LINQ
            var query = developers.Where(e => e.Name.Length == 5)
                                  .OrderBy(e => e.Name);

            // query syntax of LINQ
            var query2 = from developer in developers
                         where developer.Name.Length == 5
                         orderby developer.Name
                         select developer;

            // Using an Extension method
            Console.WriteLine(developers.Count());

            // Most datastructures derive from the IEnmurator interface
            // This meves they have a "GetEnumerator" method that describes how to enumerate through the datastructure
            // Below is an example of directly using that enumerator
            // It will work on both the Array and the List above
            // "foreach" works on different datastructures because it uses their individual enumerator behind the scences

            IEnumerator<Employee> enumerator = developers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Console.WriteLine(enumerator.Current.Name);
            }

            // lambda expressions
            foreach (var employee in developers.Where(e => e.Name.StartsWith("S")))
            {
                Console.WriteLine(employee.Name);
            }

            foreach (var employee in developers.Where(e => e.Name.Length == 5)
                                               .OrderBy(e => e.Name))
            {
                Console.WriteLine(employee.Name);
            }
        }
    }
}
