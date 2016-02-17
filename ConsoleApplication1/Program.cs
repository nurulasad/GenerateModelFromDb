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
                //if (i == 1)
                //    break;

                var tableName = names[i].Name;

                //tableName = "tblParts";





                var commandText = "select t.name [Table] ,c.name [Column] ,c2.data_type  [DataType], c2.IS_NULLABLE [IsNullable] from sys.columns c inner join sys.tables t  on t.object_id = c.object_id inner join INFORMATION_SCHEMA.COLUMNS c2 on (c.name = c2.COLUMN_NAME and t.name = c2.TABLE_NAME) where t.name = '" + tableName + "' and t.type = 'U' order by t.name,c.column_id";


                var columns = new List<ColumnDefinition>();
                using (var context = new MyDbContext())
                {
                    columns = context.Database.SqlQuery<ColumnDefinition>(commandText).ToList();

                }
                var directoryName = "ViewModels";
                var publicClassName = tableName.TrimStart("tbl".ToCharArray()).TrimEnd("s".ToCharArray());

                var fileName = publicClassName + ".cs";
                if (File.Exists(fileName))
                {
                    File.Delete(directoryName+"/"+ fileName);
                }
                //var file = File.Create(fileName);

                
                var lines = new List<string>();
                lines.Add("using PCS.DataModel;");
                lines.Add("using System;");
                lines.Add("namespace PCS.WebAPI.ViewModel");
                lines.Add("{");
                lines.Add("public class " + publicClassName);
                lines.Add("{");

                for (var j = 0; j < columns.Count; j++)
                {
                    var current = columns[j];
                    lines.Add("public " + GetApplicationDataType(current.DataType, current.IsNullable) + " " + SanitizeColumnNameForAppModel(current.Column) + " { get; set; }");

                }

                //conversion 1

                lines.Add("public static explicit operator " + publicClassName + "(tbl" + publicClassName + " " + publicClassName.ToLower() + ")");
                lines.Add("{");

                lines.Add("return new " + publicClassName);
                lines.Add("{");

                for (var j = 0; j < columns.Count; j++)
                {
                    var current = columns[j];

                    var lineDelimeter = ",";
                    if (j > columns.Count - 2)
                        lineDelimeter = string.Empty;

                    lines.Add(SanitizeColumnNameForAppModel(current.Column) + " = " + publicClassName.ToLower() + "." + SanitizeColumnNameForDataModel(current.Column) + lineDelimeter);

                }

                lines.Add("};");

                lines.Add("}");

                //conversion 2
                lines.Add("public static explicit operator tbl" + publicClassName + "("+ publicClassName + " " + publicClassName.ToLower() + ")");
                lines.Add("{");
                lines.Add("return new tbl" + publicClassName);
                lines.Add("{");
                for (var j = 0; j < columns.Count; j++)
                {
                    var current = columns[j];

                    var lineDelimeter = ",";
                    if (j > columns.Count - 2)
                        lineDelimeter = string.Empty;

                    lines.Add(SanitizeColumnNameForDataModel(current.Column) + " = " + publicClassName.ToLower() + "." + SanitizeColumnNameForAppModel(current.Column) + lineDelimeter);

                }
                lines.Add("};");

                lines.Add("}");

                //conversion 3
                lines.Add("public static "+ publicClassName + " toModel(tbl"+ publicClassName +" "+ publicClassName.ToLower()+")");
                lines.Add("{");
                lines.Add("return (" + publicClassName + ")" + publicClassName.ToLower() + ";");
                lines.Add("}");



                lines.Add("}");
                lines.Add("}");

                
                Directory.CreateDirectory(directoryName);
                System.IO.File.WriteAllLines(directoryName+"/" +fileName, lines.ToArray());



            }

            Console.WriteLine("Press any key ...");
            Console.ReadKey();
        }

        private static string GetApplicationDataType(string dbDataType, string isNullable)
        {
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
                appDataType = "bool";
            }
            else if (dbDataType == "timestamp")
            {
                appDataType = "byte[]";
            }
            else if (dbDataType == "float")
            {
                appDataType = "double";
            }

            // dummy data below

            else if (dbDataType == "tinyint")
            {
                appDataType = "short";
            }
            else if (dbDataType == "varbinary")
            {
                appDataType = "byte[]";
            }
            else if (dbDataType == "bigint")
            {
                appDataType = "long";
            }
            else if (dbDataType == "varchar")
            {
                appDataType = "varchar";
            }
            else if (dbDataType == "date")
            {
                appDataType = "date";
            }
            else if (dbDataType == "time")
            {
                appDataType = "time";
            }
            else
            {
                //undefineDataList += dbDataType+",";
                throw new Exception("Undefined conversion : " + dbDataType);
            }

            if (appDataType != "string")
            {
                if (isNullable == "YES")
                {
                    appDataType += "?";
                }
            }
            


            return appDataType;
        }

        private static string undefineDataList = string.Empty;

        private static string SanitizeColumnNameForAppModel(string columnName)
        {
            var sanitizedName = columnName;
            if (columnName.EndsWith("%"))
            {
                sanitizedName = sanitizedName.TrimEnd("%".ToCharArray()) + "Percentage";
            }
            return sanitizedName;
        }

        private static string SanitizeColumnNameForDataModel(string columnName)
        {
            var sanitizedName = columnName;
            if (columnName.EndsWith("%"))
            {
                sanitizedName = sanitizedName.TrimEnd("%".ToCharArray()) + "_";
            }
            return sanitizedName;
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
