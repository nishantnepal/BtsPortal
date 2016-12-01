using System;

namespace BtsPortal.Web.Models.Exceptions
{
    public class TerminateAndMigrateToEsbException : Exception
    {
        public TerminateAndMigrateToEsbException(string message):base(message)
        {
            
        }
    }
}