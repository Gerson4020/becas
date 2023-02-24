using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BECASLC
{
    internal class ClassConnection
    {
        public static string Connectionstring()

        {

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            return config.GetValue<string>("ConnectionStrings:value");

        }
    }
}
