using System.Collections.Generic;
using System.Data.SqlClient;
using Nelibur.ObjectMapper;

namespace Benchmark.SqlDataReaders
{
    public static class DynamicMethodIDataRecordMapper
    {
        public static List<T> GetByCodeDomByCaching<T>(this SqlDataReader reader)
        {
            //
            // Map one by one
            //
            List<T> objectList = new List<T>();

            while (reader.Read())
            {
                objectList.Add(reader.Map<T>());
            }

            return objectList;
        }
    }
}
