using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Model.Entities;

namespace Benchmark.SqlDataReaders
{
    public static class ReflectionDataRowMapper
    {
        public static List<T> GetByReflection<T>(this DataTable dt)
        {
            return (from DataRow row in dt.Rows select GetReflectorTOut<T>(row)).ToList();
        }


        public static TOut GetReflectorTOut<TOut>(this DataRow row)
        {
            TOut outObject = Activator.CreateInstance<TOut>();

            Type type = typeof(TOut);
            var properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (!IsMicrosoftType(property.PropertyType)) // Property Type is a user-defined class
                {
                    //
                    // Call generic method GetReflectorTOut<type of this property>
                    //
                    MethodInfo method = typeof(ReflectionDataRowMapper).GetMethod("GetReflectorTOut");
                    MethodInfo generic = method.MakeGenericMethod(property.PropertyType);
                    object obj = generic.Invoke(null, new object[] { row });
                    //
                    // Set this property
                    //
                    property.SetValue(outObject, obj);
                }
                else
                {
                    object obj = row[property.Name];
                    if (DBNull.Value != obj)
                    {
                        property.SetValue(outObject, obj);
                    }
                }
            }

            return outObject;
        } // EOF method

        private static bool IsMicrosoftType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (type.Assembly.GetName().Name.Equals("mscorlib", StringComparison.OrdinalIgnoreCase))
                return true;

            object[] atts = type.Assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
            if (atts.Length == 0)
                return false;

            AssemblyCompanyAttribute aca = (AssemblyCompanyAttribute)atts[0];
            return aca.Company != null && aca.Company.IndexOf("Microsoft Corporation", StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}