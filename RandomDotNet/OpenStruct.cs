using System;
using System.Collections.Generic;
using System.Dynamic;

namespace RandomDotNet
{
    public class OpenStruct : DynamicObject
    {
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        public override bool TryGetMember(GetMemberBinder binder, out dynamic result)
        {
            return values.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, dynamic value)
        {
            values[binder.Name] = value;
            return true;
        }

        public static void Run()
        {
            dynamic o = new OpenStruct();

            o.CanRunAndTellThat = true;

            Console.Out.WriteLine(o.CanRunAndTellThat);

            o.X = 56154;
            o.Y = 1337;

            Console.Out.WriteLine(o.X / o.Y);
        }
    }
}
