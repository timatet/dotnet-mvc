using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_mvc.Helpers
{
    public static class Globals
    {
        public static string RELEASE_VERSION = "DEV";

        public static void Initialize() {
            RELEASE_VERSION = Environment.GetEnvironmentVariable("RELEASE_VERSION_ENVIRONMENT");        
        }
        
    }
}