using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_mvc.Helpers
{
    public static class Globals
    {
        public static string RELEASE_VERSION = "DEV";
        public static string DATABASE_SERVER = "localhost";
        public static string DATABASE_NAME = "mvc";
        public static string DATABASE_USERNAME = "sa";
        public static string DATABASE_PASSWORD = "None";

        public static void Initialize() {
            RELEASE_VERSION = Environment.GetEnvironmentVariable("RELEASE_VERSION_ENVIRONMENT");
            DATABASE_SERVER = Environment.GetEnvironmentVariable("DATABASE_SERVER");
            DATABASE_NAME = Environment.GetEnvironmentVariable("DATABASE_NAME");
            DATABASE_USERNAME = Environment.GetEnvironmentVariable("DATABASE_USERNAME");
            DATABASE_PASSWORD = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
        }
        
    }
}