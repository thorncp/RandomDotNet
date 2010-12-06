using System;
using System.Collections.Generic;
using System.Linq;

namespace RandomDotNet
{
    public class SelectIfSuccess
    { 
        public static void Run()
        {
            var nums = new [] { 1, 2, 3, 4, 5, 6 };
            var evenSquares = nums.SelectIfSuccess<int, int>(SquareIfEven);
            Console.Out.WriteLine(evenSquares.Join(", "));

            string[] strs = new [] { "hello", "HI THERE", "YO DAWG", "mmhmm" };
            var yep = strs.SelectIfSuccess<string, string>(DowncaseIfUpcased);
            Console.Out.WriteLine(yep.Join(", "));

            var evenStrings = nums.SelectIfSuccess<int, string>(StringIfEven);
            Console.Out.WriteLine(evenStrings.Join(", "));
        }

        public static bool SquareIfEven(int x, out int result)
        {
            if (x % 2 == 0)
            {
                result = x * x;
                return true;
            }
            
            result = default(int);
            return false;
        }

        public static bool DowncaseIfUpcased(string inStr, out string outStr)
        {
            if (inStr == inStr.ToUpper())
            {
                outStr = inStr.ToLower();
                return true;
            }

            outStr = null;
            return false;
        }

        public static bool StringIfEven(int x, out string result)
        {
            if (x % 2 == 0)
            {
                result = x.ToString();
                return true;
            }
            
            result = null;
            return false;
        }
    }

    public static class Haxoring
    {
        public delegate bool OutDelegate<in TIn, TOut>(TIn input, out TOut output);

        public static IEnumerable<TOut> SelectIfSuccess<TIn, TOut>(this IEnumerable<TIn> coll, OutDelegate<TIn, TOut> func)
        {
            List<TOut> outs = new List<TOut>();
            foreach (TIn v in coll)
            {
                TOut o;
                if (func(v, out o)) outs.Add(o);
            }

            return outs;
        }
    }

    public static class IEnumerableExtensions
    {
        public static string Join<T>(this IEnumerable<T> enumerable, string separator)
        {
            string str = null;

            if (enumerable != null && enumerable.Count() > 0)
            {
                str = enumerable.Select(t => t.ToString()).Aggregate((join, next) => join + separator + next);
            }

            return str;
        }
    }
}
