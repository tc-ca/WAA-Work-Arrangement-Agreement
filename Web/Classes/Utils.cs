using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Classes
{
    public class Utils
    {
        public IConfiguration _configuration { get; }
        private readonly IWebHostEnvironment _webHostEnvironment;
        public Utils(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        public string GetConnectionString()
        {
            //implement your solution to get DB connection string
            string connStr = @"Data Source=DbName;User Id=test;Password=pass;";
            
            return connStr;
        }

    }
}
