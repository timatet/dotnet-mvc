using System;

namespace dotnet_mvc.Models.DataModels
{
  public class LogModel
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