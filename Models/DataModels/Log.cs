using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace dotnet_mvc.Models.DataModels
{
    public class Log
    {
      public DateTime _timeStamp;

        public int Id { get; set; }
        public string Author { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { 
          get { 
            return _timeStamp;
          }

          private set {
            _timeStamp = DateTime.Now;
          }
        }
    }
}