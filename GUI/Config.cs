using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI
{
    public static class Config
    {
        // Lazily reads from App.config the first time it's needed
        public static string ConnectionString =>
            ConfigurationManager
               .ConnectionStrings["database"]
               .ConnectionString;
    }
}
