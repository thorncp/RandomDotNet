using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;

namespace RandomDotNet
{
    public class DynamicHashVsDataTable_SimpleSearching
    {
        public static void Run()
        {
            var table = BuildTable(10000);

            var sw0 = new Stopwatch();
            sw0.Start();
            var dyno = new DynamicDataTable(table);
            sw0.Stop();
            Console.Out.WriteLine("setting up dynamic wrapper: " + sw0.Elapsed);

            int times = 1000;

            var randy = new Random();

            var sw1 = new Stopwatch();
            sw1.Start();
            for (int i = 0; i < times; i++)
            {
                table.Select("my_int = " + randy.Next(times));
            }
            sw1.Stop();
            Console.Out.WriteLine("select from DataTable: " + sw1.Elapsed);

            var sw2 = new Stopwatch();
            sw2.Start();
            for (int i = 0; i < times; i++)
            {
                dyno.AsEnumerable().Where(row => row.MyInt == randy.Next(times)).ToArray();
            }
            sw2.Stop();
            Console.Out.WriteLine("dyno.Where: " + sw2.Elapsed);
        }

        private static DataTable BuildTable(int rowCount)
        {
            DataTable table = new DataTable();
            table.Columns.Add("my_string", typeof(string));
            table.Columns.Add("my_int", typeof(int));
            table.Columns.Add("my_double", typeof(double));
            table.Columns.Add("my_date", typeof(DateTime));

            for (int i = 0; i < rowCount; i++)
            {
                table.Rows.Add("row " + i, i * i, i * i / 100.0, DateTime.Today.AddDays(-i));
            }

            return table;
        }
    }

    public class DynamicDataTable : IEnumerable<DynamicDataRow>
    {
        private readonly List<DynamicDataRow> rows;

        public DynamicDataTable(DataTable table)
        {
            var columns = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName);

            rows = table.AsEnumerable().Select(row => new DynamicDataRow(row, columns)).ToList();
        }

        public IEnumerator<DynamicDataRow> GetEnumerator()
        {
            return rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return rows.GetEnumerator();
        }

        public IEnumerable<dynamic> AsEnumerable()
        {
            return rows;
        }
    }

    public class DynamicDataRow : DynamicObject
    {
        private readonly Dictionary<string, dynamic> values;

        public DynamicDataRow(DataRow row, IEnumerable<string> columns = null)
        {
            if (columns == null)
                columns = row.Table.Columns.Cast<DataColumn>().Select(c => c.ColumnName);

            values = new Dictionary<string, dynamic>();

            foreach (var column in columns)
            {
                values[Sanitize(column)] = row[column];
            }
        }

        private static string Sanitize(string column)
        {
            return column.Replace("_", string.Empty).ToLower();
        }

        public override bool TryGetMember(GetMemberBinder binder, out dynamic result)
        {
            return values.TryGetValue(Sanitize(binder.Name), out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, dynamic value)
        {
            string sanny = Sanitize(binder.Name);

            if (!values.ContainsKey(sanny)) return false;

            values[sanny] = value;
            return true;
        }
    }
}
