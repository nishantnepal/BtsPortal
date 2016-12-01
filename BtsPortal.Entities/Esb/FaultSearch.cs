using System;

namespace BtsPortal.Entities.Esb
{
    public class FaultSearchRequest
    {
        public FaultSearchRequest()
        {
            FromDateTime = DateTime.Today;
            ToDateTime = DateTime.Now;
        }

        public string Application { get; set; }
        public FaultStatusType? Status { get; set; }
        public string Message { get; set; }
        public string ErrorType { get; set; }
        public string ErrorTypeExact { get; set; }
        public string FaultId { get; set; }
        public string FaultCode { get; set; }
        public string FailureCategory { get; set; }
        public string FailureScope { get; set; }
        public DateTime? FromDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public int? PageSize { get; set; }

    }

    public class FaultSearchResponse
    {
        public string Application { get; set; }
        public FaultStatusType Status { get; set; }
        public string Message { get; set; }
        public string ErrorType { get; set; }
        public Guid FaultId { get; set; }

        public string FaultCode { get; set; }
        public string FailureCategory { get; set; }
        public string Scope { get; set; }
        public DateTime? TransactionDateTimeUtc { get; set; }
        public DateTime? TransactionDateTimeLocal => TransactionDateTimeUtc.GetValueOrDefault().ToLocalTime();
        public string ServiceName { get; set; }
        public string FaultGenerator { get; set; }
        public string MachineName { get; set; }
        public bool? InsertMessagesFlag { get; set; }
       
    }

}