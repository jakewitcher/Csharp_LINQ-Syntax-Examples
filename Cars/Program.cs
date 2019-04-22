using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            var cars = ProcessCars("fuel.csv");
            var manufacturers = ProcessManufacturers("manufacturers.csv");

            var query =
                cars.GroupBy(c => c.Manufacturer)
                .Select(g =>
                {
                    var results = g.Aggregate(new CarStatistics(),
                                        (acc, c) => acc.Accumulate(c),
                                        acc => acc.Compute());
                    return new
                    {
                        Name = g.Key,
                        results.Max,
                        results.Min,
                        Avg = results.Average
                    };
                })
                .OrderByDescending(r => r.Max);

            foreach (var result in query)
            {
                Console.WriteLine($"{result.Name}");
                Console.WriteLine($"\t Max: {result.Max}");
                Console.WriteLine($"\t Min: {result.Min}");
                Console.WriteLine($"\t Avg: {Math.Round(result.Avg, 2)}");
            }
        }

        private static void AggregatingDataWithQuerySyntax(List<Car> cars)
        {
            var query =
                from car in cars
                group car by car.Manufacturer into carGroup
                select new
                {
                    Name = carGroup.Key,
                    Max = carGroup.Max(c => c.Combined),
                    Min = carGroup.Min(c => c.Combined),
                    Avg = carGroup.Average(c => c.Combined)
                } into result
                orderby result.Max descending
                select result;

            foreach (var result in query)
            {
                Console.WriteLine($"{result.Name}");
                Console.WriteLine($"\t Max: {result.Max}");
                Console.WriteLine($"\t Min: {result.Min}");
                Console.WriteLine($"\t Avg: {Math.Round(result.Avg, 2)}");
            }
        }

        private static void GroupByCountryMySolution(List<Car> cars, List<Manufacturer> manufacturers)
        {
            var query =
                from manufacturer in manufacturers
                join car in cars on manufacturer.Name equals car.Manufacturer
                    into carGroup
                orderby manufacturer.Name
                group carGroup by manufacturer.Headquarters into headquarters
                orderby headquarters.Key
                select headquarters;

            foreach (var group in query)
            {
                Console.WriteLine($"{group.Key}");
                foreach (var car in group.SelectMany(c => c).OrderByDescending(c => c.Combined).Take(3))
                {
                    Console.WriteLine($"\t{car.Manufacturer}, {car.Name} : {car.Combined}");
                }
            }
        }

        private static void MethodSyntaxForGroupJoin(List<Car> cars, List<Manufacturer> manufacturers)
        {
            var query =
                manufacturers.GroupJoin(cars, m => m.Name, c => c.Manufacturer, (m, g) =>
                new
                {
                    Manufacturer = m,
                    Cars = g
                }).OrderBy(m => m.Manufacturer.Name);

            foreach (var group in query)
            {
                Console.WriteLine($"{group.Manufacturer.Name} : {group.Manufacturer.Headquarters}");
                foreach (var car in group.Cars.OrderByDescending(c => c.Combined).Take(2))
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }
        }

        private static void QuerySyntaxForGroupJoin(List<Car> cars, List<Manufacturer> manufacturers)
        {
            var query =
                from manufacturer in manufacturers
                join car in cars on manufacturer.Name equals car.Manufacturer
                    into carGroup
                select new
                {
                    Manufacturer = manufacturer,
                    Cars = carGroup
                };

            foreach (var group in query)
            {
                Console.WriteLine($"{group.Manufacturer.Name} : {group.Manufacturer.Headquarters}");
                foreach (var car in group.Cars.OrderByDescending(c => c.Combined).Take(2))
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }
        }

        private static void MethodSyntaxForGrouping(List<Car> cars)
        {
            var query =
                cars.GroupBy(c => c.Manufacturer.ToUpper())
                    .OrderBy(g => g.Key);

            foreach (var group in query)
            {
                Console.WriteLine(group.Key);
                foreach (var car in group.OrderByDescending(c => c.Combined).Take(2))
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }
        }

        private static void QuerySyntaxForGrouping(List<Car> cars)
        {
            var query =
                from car in cars
                group car by car.Manufacturer.ToUpper() into manufacturer
                orderby manufacturer.Key
                select manufacturer;

            foreach (var group in query)
            {
                Console.WriteLine(group.Key);
                foreach (var car in group.OrderByDescending(c => c.Combined).Take(2))
                {
                    Console.WriteLine($"\t{car.Name} : {car.Combined}");
                }
            }
        }

        private static void JoinOnTwoDataPointsWithMethodSyntax(List<Car> cars, List<Manufacturer> manufacturers)
        {
            var query =
                cars.Join(manufacturers,
                            c => new { c.Manufacturer, c.Year },
                            m => new { Manufacturer = m.Name, m.Year },
                            (c, m) => new
                            {
                                m.Headquarters,
                                c.Name,
                                c.Combined,
                                c.Year
                            })
                    .OrderByDescending(c => c.Combined)
                    .ThenBy(c => c.Name);

            foreach (var car in query.Take(10))
            {
                Console.WriteLine($"{car.Headquarters} {car.Name} : {car.Combined}");
            }
        }

        private static void JoinOnTwoDataPointsWithQuerySyntax(List<Car> cars, List<Manufacturer> manufacturers)
        {
            var query =
                from car in cars
                join manufacturer in manufacturers
                    on new { car.Manufacturer, car.Year }
                    equals new { Manufacturer = manufacturer.Name, manufacturer.Year }
                orderby car.Combined descending, car.Name ascending
                select new
                {
                    manufacturer.Headquarters,
                    car.Name,
                    car.Combined,
                    car.Year
                };

            foreach (var car in query.Take(10))
            {
                Console.WriteLine($"{car.Headquarters} {car.Name}, {car.Year} : {car.Combined}");
            }
        }

        private static void UsingJoinWithMethodSyntax(List<Car> cars, List<Manufacturer> manufacturers)
        {
            var query =
                cars.Join(manufacturers,
                            c => c.Manufacturer,
                            m => m.Name, 
                            (c, m) => new
                            {
                                m.Headquarters,
                                c.Name,
                                c.Combined
                            })
                    .OrderByDescending(c => c.Combined)
                    .ThenBy(c => c.Name);

            foreach (var car in query.Take(10))
            {
                Console.WriteLine($"{car.Headquarters} {car.Name} : {car.Combined}");
            }
        }

        private static void UsingJoinWithQuerySyntax(List<Car> cars, List<Manufacturer> manufacturers)
        {
            var query =
                from car in cars
                join manufacturer in manufacturers
                    on car.Manufacturer equals manufacturer.Name
                orderby car.Combined descending, car.Name ascending
                select new
                {
                    manufacturer.Headquarters,
                    car.Name,
                    car.Combined
                };

            foreach (var car in query.Take(10))
            {
                Console.WriteLine($"{car.Headquarters} {car.Name} : {car.Combined}");
            }
        }
        private static void BasicQuerySyntaxExamples(List<Car> cars)
        {
            var query =
               from car in cars
               where car.Manufacturer == "BMW" && car.Year == 2016
               orderby car.Combined descending, car.Name
               select new
               {
                   car.Manufacturer,
                   car.Name,
                   car.Combined
               };
            // this is an anonymous type, describing an truncated version of "Car" without having to write a new class

            foreach (var car in query.Take(10))
            {
                Console.WriteLine($"{car.Name} : {car.Combined}");
            }
        }

        private static void BasicMethodSyntaxExamples(List<Car> cars)
        {
            var query = cars.Where(c => c.Manufacturer == "BMW" && c.Year == 2016)
                            .OrderBy(c => c.Combined)
                            .ThenBy(c => c.Name)
                            .Select(c =>
                            new {
                                c.Manufacturer,
                                c.Name,
                                c.Combined
                            });

            foreach (var car in query.Take(10))
            {
                Console.WriteLine($"{car.Name} : {car.Combined}");
            }

        }
        private static void UsingFirst(List<Car> cars)
        {
            var top =
            cars.Where(c => c.Manufacturer == "BMW" && c.Year == 2016)
                .OrderByDescending(c => c.Combined)
                .ThenBy(c => c.Name)
                .First();
            // First returns a value rather than an enumerable

            Console.WriteLine(top.Name);
        }

        private static void UsingSelectMany(List<Car> cars)
        {
            IEnumerable<char> result3 = cars.SelectMany(c => c.Name);
            // If the query result is a list of lists, selectMany will flatten them out into a single list

            foreach (var name in result3)
            {
                Console.WriteLine(name);
            }
        }

        private static void AnyAndAll(List<Car> cars)
        {
            var result = cars.Any(c => c.Manufacturer == "Ford");
            // returns True if at least one value matches the predicate

            var result2 = cars.All(c => c.Manufacturer == "Ford");
            // returns True if all values math the predicate

            Console.WriteLine(result);
            Console.WriteLine(result2);

            Console.WriteLine();
        }

        private static List<Manufacturer> ProcessManufacturers(string path)
        {
            var query =
                File.ReadAllLines(path)
                    .Where(l => l.Length > 1)
                    .ToManufacturer();

            return query.ToList();
        }

        private static List<Car> ProcessCars(string path)
        {
            var query =

                File.ReadAllLines(path)
                    .Skip(1)
                    .Where(l => l.Length > 1)
                    .ToCar();

            return query.ToList();

            //from line in File.ReadAllLines(path).Skip(1)
            //where line.Length > 1
            //select Car.ParseFromCsv(line);

            //return
            //File.ReadAllLines(path)
            //    .Skip(1)
            //    .Where(line => line.Length > 1)
            //    .Select(line => Car.ParseFromCsv(line))
            //    .ToList();
        }
    }
}
