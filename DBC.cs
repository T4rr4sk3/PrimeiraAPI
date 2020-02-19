using System.Data.SqlClient;
using System.IO;

namespace BD
{
    class DBC
    {
        private SqlConnection _con;
        public SqlConnection Con
        {
            get => _con;
            set => _con = value;
        }

        public DBC(string bd)
        {
            string d = GetThisDirectory();
            string s = "Data Source=(LocalDB)\\MSSQLLocalDB; AttachDbFilename=" + @d + "\\" + bd + ".mdf;Integrated Security=True;Connect Timeout=45";
            
            Con = new SqlConnection(s);
        }
  
        public void Open()
        {
            Con.Open();
        }

        public void Close()
        {
            Con.Close();
        }

        public string GetThisDirectory()
        {
            return Directory.GetCurrentDirectory();
        }
    }
}
