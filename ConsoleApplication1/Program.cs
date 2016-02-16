using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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



            //for (var i = 0; i < dt.Rows.Count; i++)
            //{
            //    var tableName = dt.Rows[i].ItemArray[2];





            var commandText = "select t.name [Table] ,c.name [Column] ,c2.data_type  [DataType] from sys.columns c inner join sys.tables t  on t.object_id = c.object_id inner join INFORMATION_SCHEMA.COLUMNS c2 on (c.name = c2.COLUMN_NAME and t.name = c2.TABLE_NAME) where t.name = 'tblParts' and t.type = 'U'";
            //cmd.CommandType = CommandType.Text;
            //cmd.Connection = SqlCon;

            //cmd.Connection.Open();

            //reader = cmd.ExecuteReader();

            using (var context = new MyDbContext())
            {
                var blogs = context.Database.SqlQuery<ColumnDefinition>(commandText).ToList();
            }




            //}


            Console.ReadKey();
        }
    }

    public class ColumnDefinition
    {
        public string Table { get; set; }
        public string Column { get; set; }
        public string DataType { get; set; }
    }
}
