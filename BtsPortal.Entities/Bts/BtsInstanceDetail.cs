using System;
using System.Collections.Generic;

namespace BtsPortal.Entities.Bts
{
    public class BtsInstanceDetail
    {
        public BtsInstanceDetail()
        {
            Messages = new List<BtsInstanceMessage>();
        }
        public string Server { get; set; }
        public string Status { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public string ServiceClass { get; set; }
        public string ServiceName { get; set; }
        public string InstanceId { get; set; }
        public string PendingOperation { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Application { get; set; }
        public string HostName { get; set; }
        public DateTime? SuspendDateTime { get; set; }

        

        public int TotalMessagesCount { get; set; }
        public List<BtsInstanceMessage> Messages { get; set; }
    }

    public class BtsInstanceMessage
    {
        public BtsInstanceMessage()
        {
            Contexts = new List<BtsInstanceMessageContext>();
        }
        public string MessageType { get; set; }
        public string MessageStatus { get; set; }
        public DateTime DateCreated { get; set; }
        public string Adapter { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public string MessageId { get; set; }
        public string MessageBody { get; set; }
        public int MessageBodyLength { get; set; }
        public bool MessageBodyTruncated { get; set; }
        public List<BtsInstanceMessageContext> Contexts { get; set; }

    }

    public class BtsInstanceMessageContext
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Promoted { get; set; }
        public string Namespace { get; set; }
    }

}