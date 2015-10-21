using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Model.Entities;

namespace Benchmark.SqlDataReaders
{
    public static class ReflectionIDataRecordMapper
    {
        public static List<T> GetByReflection<T>(this SqlDataReader reader)
        {
            List<T> objectsList = new List<T>();

            while (reader.Read())
            {
                objectsList.Add(GetReflectorTOut<T>(reader));
            }

            return objectsList;
        }


        public static TOut GetReflectorTOut<TOut>(this IDataRecord record)
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
                    MethodInfo method = typeof(ReflectionIDataRecordMapper).GetMethod("GetReflectorTOut");
                    MethodInfo generic = method.MakeGenericMethod(property.PropertyType);
                    object obj = generic.Invoke(null, new object[] { record });
                    //
                    // Set this property
                    //
                    property.SetValue(outObject, obj);
                }
                else
                {
                    int index = record.GetOrdinal(property.Name);
                    if (index >= 0 && !record.IsDBNull(index))
                    {
                        property.SetValue(outObject, record[property.Name]);
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