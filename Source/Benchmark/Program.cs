using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Benchmark.Benchmarks;
using Benchmark.SqlDataReaders;
using Model.Entities;
using Nelibur.ObjectMapper;

namespace Benchmark
{
    internal class Program
    {
        private const int Iterations = 1000;
        private const string ConnString = "Data Source=.;Initial Catalog=TinyMapperTestDB;Integrated Security=True";
        private static SqlCommand cmd;
        private static DataTable _dataTable;

        private static void Main()
        {
            //
            // AutoMapper - QuickMapper - Handwriting Benchmarks (T object ==MapTo==> T2 object)
            //
            ObjectMappingToObjectBenchmarks();

            //
            // Sql data readers benchmark (IDataRecord or DataRow ==MapTo==> T object)
            //
            SqlDataToObjectBenchmarks();

            Console.WriteLine("\n\nBenchmark End.");
            Console.WriteLine("Press any key to Exit");
            Console.ReadLine();
        }

        private static void ObjectMappingToObjectBenchmarks()
        {
            var primitiveTypeBenchmark = new PrimitiveTypeBenchmark(Iterations);
            primitiveTypeBenchmark.Measure();

            var collectionBenchmark = new CollectionBenchmark(Iterations);
            collectionBenchmark.Measure();
        }

        private static void SqlDataToObjectBenchmarks()
        {
            SqlConnection sqlConn = new SqlConnection(ConnString);
            cmd = sqlConn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = String.Format("SELECT TOP {0} *  FROM [TestDB].[dbo].[User] u LEFT JOIN [TestDB].[dbo].[Address] a ON a.UserId = u.ID", Iterations);

            #region Convert IDataRecords to User Objects

            //
            // Begin Convert IDataRecords to Objects
            //
            Console.WriteLine("--------------- Begin Convert IDataRecords to User Objects ----------------\n");
            //
            // Just load data by SqlDataReader from SQL
            //
            SqlDataReaderWithoutConverting();
            //
            // Manual Converting IDataRecord to User object
            //
            SqlDataReaderWithManualConverting();
            //
            // Reflection Converting IDataRecord to User object
            //
            SqlDataReaderWithReflectionConverting();
            //
            // DynamicFunc Converting IDataRecord to User object
            //
            SqlDataReaderWithDynamicMethodConverting();
            //
            // DynamicFunc Converting IDataRecord to User object (By one by one binding)
            //
            SqlDataReaderWithDynamicMethodCachierConverting();
            //
            // End Convert IDataRecords to Objects
            //
            Console.WriteLine("\n---------------------------------------------------------------------------\n\n\n");
            #endregion

            #region Convert DataRows to User Objects

            //
            // Begin Convert DataRows to User Objects
            //
            Console.WriteLine("----------------- Begin Convert DataRows to User Objects ------------------\n");
            //
            // Just load data by DataTable from SQL
            //
            var loadTime = GetDataTable();
            //
            // Manual Converting DataRow to User object 
            //
            DataTableWithManualConverting(loadTime);
            //
            // Reflection Converting DataRow to User object 
            //
            DataTableWithReflectionConverting(loadTime);
            //
            // DynamicFunc Converting DataRow to User object 
            //
            DataTableWithDynamicMethodConverting(loadTime);
            //
            // DynamicFunc Converting DataRow to User object (By one by one binding)
            //
            DataTableWithDynamicMethodCachierConverting(loadTime);
            //
            // End Convert DataRows to User Objects
            //
            Console.WriteLine("\n---------------------------------------------------------------------------\n\n\n");
            #endregion

            #region Convert IDataRecords to dynamic Objects
            //
            // Begin Convert IDataRecords to Dynamic Objects
            //
            Console.WriteLine("------------- Begin Convert IDataRecords to dynamic Objects ---------------\n");
            //
            // Manual Converting IDataRecords to dynamic object 
            //
            SqlDataReaderWithManualConvertingObject();
            //
            // DynamicFunc Converting IDataRecords to dynamic object 
            //
            SqlDataReaderWithDynamicMethodConvertingObject();
            //
            // End Convert IDataRecords to dynamic objects
            //
            Console.WriteLine("\n---------------------------------------------------------------------------\n\n\n");
            #endregion

            #region Convert DataRows to dynamic Objects
            //
            // Begin Convert DataRows to Dynamic Objects
            //
            Console.WriteLine("--------------- Begin Convert DataRows to dynamic Objects -----------------\n");
            //
            // Manual Converting DataRows to dynamic object 
            //
            DataTableWithManualConvertingObject(loadTime);
            //
            // DynamicFunc Converting DataRows to dynamic object 
            //
            DataTableWithDynamicMethodConvertingObject(loadTime);
            //
            // End Convert DataRows to dynamic objects
            //
            Console.WriteLine("\n---------------------------------------------------------------------------\n\n\n");
            #endregion
        }

        #region Empty Loaders

        // Empty sqlDataReader loading
        private static void SqlDataReaderWithoutConverting()
        {
            Console.WriteLine("---> SELECT TOP {0} * From SQL ==> SQLDataReader", Iterations);
            cmd.Connection.Open();
            var mappingTimer = Stopwatch.StartNew();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                // just load data from SQL server
            }
            mappingTimer.Stop();
            cmd.Connection.Close();
            ClearLine();
            Console.WriteLine("SELECT TOP {0} * From SQL ==> SQLDataReader :\t {1}ms \n", Iterations, mappingTimer.ElapsedMilliseconds);
        }


        // Empty dataTable loading
        private static long GetDataTable()
        {
            Console.WriteLine("---> SELECT TOP {0} * From SQL ==> DataTable", Iterations);

            _dataTable = new DataTable();

            var sw = new Stopwatch();
            try
            {
                cmd.Connection.Open();

                sw.Start();

                SqlDataReader reader = cmd.ExecuteReader();
                _dataTable.Load(reader);

                sw.Stop();

                ClearLine();
                Console.WriteLine("SELECT TOP {0} * From SQL ==> DataTable :\t {1}ms \n", Iterations, sw.ElapsedMilliseconds);
            }
            finally
            {
                cmd.Connection.Close();
            }

            return sw.ElapsedMilliseconds;
        }

        #endregion

        #region SqlDataReader To User object Methods

        private static void SqlDataReaderWithManualConverting()
        {
            Console.WriteLine("---> Manual\t IDataRecord   ==>   User");
            cmd.Connection.Open();
            var mappingTimer = Stopwatch.StartNew();
            cmd.ExecuteReader().GetManually<User>();
            mappingTimer.Stop();
            cmd.Connection.Close();
            ClearLine();
            Console.WriteLine("Manual\t\t\t IDataRecord   ==>   User :\t {0}ms", mappingTimer.ElapsedMilliseconds);
        }

        private static void SqlDataReaderWithReflectionConverting()
        {
            Console.WriteLine("---> Reflection\t IDataRecord   ==>   User");
            cmd.Connection.Open();
            var mappingTimer = Stopwatch.StartNew();
            cmd.ExecuteReader().GetByReflection<User>();
            mappingTimer.Stop();
            cmd.Connection.Close();
            ClearLine();
            Console.WriteLine("Reflection\t\t IDataRecord   ==>   User :\t {0}ms", mappingTimer.ElapsedMilliseconds);
        }

        private static void SqlDataReaderWithDynamicMethodConverting()
        {
            Console.WriteLine("---> DynamicFunc (Bind-Map)\t IDataRecord   ==>   User");
            cmd.Connection.Open();
            var mappingTimer = Stopwatch.StartNew();
            List<User> users = cmd.ExecuteReader().MapToList<User>();
            mappingTimer.Stop();
            cmd.Connection.Close();
            ClearLine();
            Console.WriteLine("DynamicFunc (Bind-Map)\t IDataRecord   ==>   User :\t {0}ms", mappingTimer.ElapsedMilliseconds);
        }

        private static void SqlDataReaderWithDynamicMethodCachierConverting()
        {
            Console.WriteLine("---> DynamicFunc (Map)\t IDataRecord   ==>   User");
            cmd.Connection.Open();
            var mappingTimer = Stopwatch.StartNew();
            cmd.ExecuteReader().GetByCodeDomByCaching<User>();
            mappingTimer.Stop();
            cmd.Connection.Close();
            ClearLine();
            Console.WriteLine("DynamicFunc (Map)\t IDataRecord   ==>   User :\t {0}ms", mappingTimer.ElapsedMilliseconds);
        }

        #endregion

        #region SqlDataReader To dynamic object Methods

        private static void SqlDataReaderWithManualConvertingObject()
        {
            Console.WriteLine("---> Manual\t IDataRecord   ==>   Object");
            cmd.Connection.Open();
            var mappingTimer = Stopwatch.StartNew();
            cmd.ExecuteReader().GetManually<object>();
            mappingTimer.Stop();
            cmd.Connection.Close();
            ClearLine();
            Console.WriteLine("Manual\t\t\t IDataRecord   ==>   Object :\t {0}ms", mappingTimer.ElapsedMilliseconds);
        }

        private static void SqlDataReaderWithDynamicMethodConvertingObject()
        {
            Console.WriteLine("---> DynamicFunc (Bind-Map)\t IDataRecord   ==>   Object");
            cmd.Connection.Open();
            var mappingTimer = Stopwatch.StartNew();
            List<object> objects = cmd.ExecuteReader().MapToList<object>();
            mappingTimer.Stop();
            cmd.Connection.Close();
            ClearLine();
            Console.WriteLine("DynamicFunc (Bind-Map)\t IDataRecord   ==>   Object :\t {0}ms", mappingTimer.ElapsedMilliseconds);
        }

        #endregion

        #region DataTable To User object Methods

        private static void DataTableWithManualConverting(long loadTime)
        {
            Console.WriteLine("---> Manual\t DataRow   ==>   User");
            var mappingTimer = Stopwatch.StartNew();
            var lst = _dataTable.GetManually<User>();
            mappingTimer.Stop();
            ClearLine();
            Console.WriteLine("Manual\t\t\t DataRow       ==>   User :\t {0}ms", mappingTimer.ElapsedMilliseconds + loadTime);
        }

        private static void DataTableWithReflectionConverting(long loadTime)
        {
            Console.WriteLine("---> Reflection\t\t DataRow       ==>   User");
            var mappingTimer = Stopwatch.StartNew();
            var lst = _dataTable.GetByReflection<User>();
            mappingTimer.Stop();
            ClearLine();
            Console.WriteLine("Reflection\t\t DataRow       ==>   User :\t {0}ms", mappingTimer.ElapsedMilliseconds + loadTime);
        }

        private static void DataTableWithDynamicMethodConverting(long loadTime)
        {
            Console.WriteLine("---> DynamicFunc (Bind-Map)\t DataRow       ==>   User");
            var mappingTimer = Stopwatch.StartNew();
            List<User> users = _dataTable.MapToList<User>();
            mappingTimer.Stop();
            ClearLine();
            Console.WriteLine("DynamicFunc (Bind-Map)\t DataRow       ==>   User :\t {0}ms", mappingTimer.ElapsedMilliseconds + loadTime);
        }

        private static void DataTableWithDynamicMethodCachierConverting(long loadTime)
        {
            Console.WriteLine("---> DynamicFunc (Map)\t DataRow       ==>   User");
            var mappingTimer = Stopwatch.StartNew();
            _dataTable.GetByCodeDomByCaching<User>();
            mappingTimer.Stop();
            ClearLine();
            Console.WriteLine("DynamicFunc (Map)\t DataRow       ==>   User :\t {0}ms", mappingTimer.ElapsedMilliseconds + loadTime);
        }

        #endregion

        #region DataTable To dynamic object Methods

        private static void DataTableWithManualConvertingObject(long loadTime)
        {
            Console.WriteLine("---> Manual\t DataRow   ==>   Object");
            var mappingTimer = Stopwatch.StartNew();
            var lst = _dataTable.GetManually<object>();
            mappingTimer.Stop();
            ClearLine();
            Console.WriteLine("Manual\t\t\t DataRow       ==>   Object :\t {0}ms", mappingTimer.ElapsedMilliseconds + loadTime);
        }

        private static void DataTableWithDynamicMethodConvertingObject(long loadTime)
        {
            Console.WriteLine("---> DynamicFunc (Bind-Map)\t DataRow   ==>   Object");
            var mappingTimer = Stopwatch.StartNew();
            List<object> users = _dataTable.MapToList<object>();
            mappingTimer.Stop();
            ClearLine();
            Console.WriteLine("DynamicFunc (Bind-Map)\t DataRow       ==>   Object :\t {0}ms", mappingTimer.ElapsedMilliseconds + loadTime);
        }

        #endregion


        public static void ClearLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }
    }
}
