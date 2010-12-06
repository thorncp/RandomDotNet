using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace RandomDotNet
{
    public class SendAndRespondTo
    {
        public static void Run()
        {
            Console.Out.WriteLine(1.RespondTo("ToString"));
            Console.Out.WriteLine(1.Send("ToString").GetType());

            Console.Out.WriteLine(1.RespondTo("Whoops"));
            try
            {
                Console.Out.WriteLine(1.Send("Whoops", 42, "hi there"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            var list = new List<string>();
            list.Send("Add", "hello there");
            Console.Out.WriteLine(list.Join(", "));
        }
    }

    public static class ObjectExtensions
    {
        public static dynamic Send(this object obj, string name, params object[] args)
        {
            var argTypes = args.Select(a => a.GetType()).ToArray();
            MethodInfo methodInfo = obj.GetType().GetMethod(name, argTypes);

            if (methodInfo == null)
                throw new MissingMemberException(string.Format("{0} does not respond to: {1}({2})", obj.GetType(), name, argTypes.Join(", ")));

            return methodInfo.Invoke(obj, args);
        }

        public static bool RespondTo(this object obj, string name)
        {
            return obj.GetType().GetMethods().Any(m => m.Name == name);
        }
    }

}
