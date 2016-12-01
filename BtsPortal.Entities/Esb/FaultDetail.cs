using System;
using System.Collections.Generic;

namespace BtsPortal.Entities.Esb
{
    public class FaultDetailAggregate
    {
        public FaultDetail FaultDetail { get; set; }
        public List<FaultMessage> FaultMessages { get; set; }
        public List<FaultHistory> FaultHistories { get; set; }

        public FaultDetailAggregate()
        {
            FaultMessages = new List<FaultMessage>();
            FaultHistories = new List<FaultHistory>();
        }

    }

    public class MessageDetail
    {
        public FaultMessage Message { get; set; }
        public FaultMessageData Data { get; set; }
        public List<FaultMessageContext> FaultMessageContexts { get; set; }

    }

    public class FaultDetail
    {

        public System.Guid FaultId { get; set; }

        public string NativeMessageID { get; set; }

        public string ActivityID { get; set; }

        public string Application { get; set; }

        public string Description { get; set; }

        public string ErrorType { get; set; }

        public string FailureCategory { get; set; }

        public string FaultCode { get; set; }

        public string FaultDescription { get; set; }

        public int? FaultSeverity { get; set; }

        public string Scope { get; set; }

        public string ServiceInstanceID { get; set; }

        public string ServiceName { get; set; }

        public string FaultGenerator { get; set; }

        public string MachineName { get; set; }

        public DateTime? DateTime { get; set; }

        public string ExceptionMessage { get; set; }

        public string ExceptionType { get; set; }

        public string ExceptionSource { get; set; }

        public string ExceptionTargetSite { get; set; }

        public string ExceptionStackTrace { get; set; }

        public string InnerExceptionMessage { get; set; }

        public bool? InsertMessagesFlag { get; set; }

        public System.DateTime InsertedDate { get; set; }



    }

    public class FaultMessage
    {
        public Guid MessageID { get; set; }
        public Guid FaultId { get; set; }
        public string InterchangeID { get; set; }
        public string MessageName { get; set; }
        public string ContentType { get; set; }
        public string NativeMessageId { get; set; }

    }

    public class FaultMessageContext
    {
        public Guid MessageID { get; set; }
        public Guid ContextPropertyId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }

    public class FaultMessageData
    {
        public Guid MessageID { get; set; }
        public string MessageData { get; set; }
        public int MessageDataLength { get; set; }
        public int MaxDataLength { get; set; }
        public bool DataTruncated => MessageDataLength > MaxDataLength;
    }

    public class FaultHistory
    {
        public System.Guid FaultId { get; set; }
        public FaultStatusType FaultStatusType { get; set; }
        public string UpdatedBy { get; set; }
        public string Comment { get; set; }
        public DateTime UpdatedTimeUtc { get; set; }


    }
}