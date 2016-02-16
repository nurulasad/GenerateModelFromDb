using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    public class MyDbContext : DbContext
    {
        public MyDbContext()
        {
            Database.Connection.ConnectionString = "data source=devsql02;initial catalog=PCS_Development;user id=pcs; password=test1234;";
        }
    }
}
