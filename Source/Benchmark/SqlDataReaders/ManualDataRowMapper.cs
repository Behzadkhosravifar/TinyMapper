using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Model.Entities;

namespace Benchmark.SqlDataReaders
{
    public static class ManualDataRowMapper
    {
        private static T GetManual<T>(DataRow row)
        {
            if (typeof(T) == typeof(User))
            {

                User user = new User();
                user.Address = new Address();
                object obj;

                obj = row["Name"];
                if (DBNull.Value != obj)
                {
                    user.Name = (string)obj;
                }

                obj = row["LastName"];
                if (DBNull.Value != obj)
                {
                    user.LastName = (string)row["LastName"];
                }

                obj = row["Street"];
                if (DBNull.Value != obj)
                {
                    user.Address.Street = (string)row["Street"];
                }

                obj = row["City"];
                if (DBNull.Value != obj)
                {
                    user.Address.City = (string)row["City"];
                }

                obj = row["Country"];
                if (DBNull.Value != obj)
                {
                    user.Address.Country = (string)row["Country"];
                }

                obj = row["No"];
                if (DBNull.Value != obj)
                {
                    user.Address.No = (int)row["No"];
                }


                return (T)Convert.ChangeType(user, typeof(T));
            }
            else if (typeof(T) == typeof(object))
            {
                object res = new
                {
                    ID = row.IsNull(0) ? default(System.Int32) : (System.Int32)row[0],
                    Name = row.IsNull(1) ? default(System.String) : (System.String)row[1],
                    LastName = row.IsNull(2) ? default(System.String) : (System.String)row[2],
                    UserId = row.IsNull(3) ? default(System.Int32) : (System.Int32)row[3],
                    Street = row.IsNull(4) ? default(System.String) : (System.String)row[4],
                    City = row.IsNull(5) ? default(System.String) : (System.String)row[5],
                    Country = row.IsNull(6) ? default(System.String) : (System.String)row[6],
                    No = row.IsNull(7) ? default(System.Int32) : (System.Int32)row[7]
                };

                return (T)res;
            }

            return Activator.CreateInstance<T>();
        }

        public static List<T> GetManually<T>(this DataTable dt)
        {
            return (from DataRow row in dt.Rows select GetManual<T>(row)).ToList();
        }
    }
}