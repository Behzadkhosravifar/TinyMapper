using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Model.Entities;

namespace Benchmark.SqlDataReaders
{
    public static class ManualIDataRecordMapper
    {
        private static T GetManual<T>(IDataRecord record)
        {
            if (typeof(T) == typeof(User))
            {
                User user = new User();
                user.Address = new Address();
                int index;

                index = record.GetOrdinal("Name");
                if (index >= 0 && !record.IsDBNull(index))
                {
                    user.Name = (string)record["Name"];
                }

                index = record.GetOrdinal("LastName");
                if (index >= 0 && !record.IsDBNull(index))
                {
                    user.LastName = (string)record["LastName"];
                }

                index = record.GetOrdinal("Street");
                if (index >= 0 && !record.IsDBNull(index))
                {
                    user.Address.Street = (string)record["Street"];
                }

                index = record.GetOrdinal("City");
                if (index >= 0 && !record.IsDBNull(index))
                {
                    user.Address.City = (string)record["City"];
                }

                index = record.GetOrdinal("Country");
                if (index >= 0 && !record.IsDBNull(index))
                {
                    user.Address.Country = (string)record["Country"];
                }

                index = record.GetOrdinal("No");
                if (index >= 0 && !record.IsDBNull(index))
                {
                    user.Address.No = (int)record["No"];
                }


                return (T)Convert.ChangeType(user, typeof(T));
            }
            else if (typeof(T) == typeof(object))
            {
                object res = new
                {
                    ID = record.IsDBNull(0) ? default(System.Int32) : (System.Int32)record[0],
                    Name = record.IsDBNull(1) ? default(System.String) : (System.String)record[1],
                    LastName = record.IsDBNull(2) ? default(System.String) : (System.String)record[2],
                    UserId = record.IsDBNull(3) ? default(System.Int32) : (System.Int32)record[3],
                    Street = record.IsDBNull(4) ? default(System.String) : (System.String)record[4],
                    City = record.IsDBNull(5) ? default(System.String) : (System.String)record[5],
                    Country = record.IsDBNull(6) ? default(System.String) : (System.String)record[6],
                    No = record.IsDBNull(7) ? default(System.Int32) : (System.Int32)record[7]
                };

                return (T)res;
            }

            return Activator.CreateInstance<T>();
        }

        public static List<T> GetManually<T>(this SqlDataReader reader)
        {
            List<T> users = new List<T>();

            while (reader.Read())
            {
                users.Add(GetManual<T>(reader));
            }

            return users;
        }
    }
}