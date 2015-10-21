using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Nelibur.ObjectMapper;

namespace Benchmark.SqlDataReaders
{
    public static class DynamicMethodDataRowMapper
    {

        public static List<T> GetByCodeDomByCaching<T>(this DataTable dt)
        {
            //
            // Map one by one
            //

            return (from DataRow row in dt.Rows select row.Map<T>()).ToList();
        }
    }
}
