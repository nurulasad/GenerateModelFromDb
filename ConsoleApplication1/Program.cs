using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //System.Data.SqlClient.SqlConnection SqlCon = new System.Data.SqlClient.SqlConnection("data source=devsql02;initial catalog=PCS_Development;user id=pcs; password=test1234;");
            //SqlCon.Open();

            //SqlCommand cmd = new SqlCommand();
            //SqlDataReader reader;

            //cmd.Connection = SqlCon;



            //DataTable dt = cmd.Connection.GetSchema("Tables");

            var sql = "SELECT name FROM sysobjects WHERE xtype = 'U'";
            var names = new List<TableName>();
            using (var context = new MyDbContext())
            {
                names = context.Database.SqlQuery<TableName>(sql).ToList();
            }

            for (var i = 0; i < names.Count; i++)
            {
                if (i == 1)
                    break;

                var tableName = names[i].Name;

                tableName = "tblParts";





                var commandText = "select t.name [Table] ,c.name [Column] ,c2.data_type  [DataType], c2.IS_NULLABLE [IsNullable] from sys.columns c inner join sys.tables t  on t.object_id = c.object_id inner join INFORMATION_SCHEMA.COLUMNS c2 on (c.name = c2.COLUMN_NAME and t.name = c2.TABLE_NAME) where t.name = '" + tableName + "' and t.type = 'U'";


                var columns = new List<ColumnDefinition>();
                using (var context = new MyDbContext())
                {
                    columns = context.Database.SqlQuery<ColumnDefinition>(commandText).ToList();

                }

                var fileName = tableName + ".cs";
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                //var file = File.Create(fileName);

                var  lines = new List<string>();
                lines.Add("using PCS.DataModel;");
                lines.Add("using System;");
                lines.Add("namespace PCS.WebAPI.ViewModel");
                lines.Add("{");
                lines.Add("public class " + tableName);
                lines.Add("{");

                for (var j = 0; j < columns.Count; j++)
                {
                    var current = columns[j];
                    lines.Add("public " + GetApplicationDataType(current.DataType,current.IsNullable) + " " + current.Column + " { get; set; }");
                    
                }
                    // WriteAllLines creates a file, writes a collection of strings to the file,
                    // and then closes the file.  You do NOT need to call Flush() or Close().
                    System.IO.File.WriteAllLines(fileName, lines.ToArray());
                




            }


            Console.ReadKey();
        }

        private static string GetApplicationDataType(string dbDataType, string isNullable) {
            var appDataType = string.Empty;

            if (dbDataType == "int")
            {
                appDataType = "int";
            }
            else if (dbDataType == "nvarchar")
            {
                appDataType = "string";
            }
            else if (dbDataType == "smallint")
            {
                appDataType = "short";
            }
            else if (dbDataType == "real")
            {
                appDataType = "float";
            }
            else if (dbDataType == "money")
            {
                appDataType = "decimal";
            }
            else if (dbDataType == "datetime2")
            {
                appDataType = "DateTime";
            }
            else if (dbDataType == "bit")
            {
                appDataType = "boolean";
            }
            else if (dbDataType == "timestamp")
            {
                appDataType = "byte[]";
            }
            else
            {
                throw new Exception("Undefined conversion : " + dbDataType);
            }

            if(isNullable == "YES")
            {
                appDataType += "?";
            }


            return appDataType;
        }
    }

    public class ColumnDefinition
    {
        public string Table { get; set; }
        public string Column { get; set; }
        public string DataType { get; set; }
        public string IsNullable { get; set; }
    }

    public class TableName
    {
        public string Name { get; set; }
    }
}
