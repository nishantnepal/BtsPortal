using System.Xml.Serialization;

namespace BtsPortal.Entities.Bts
{
    public enum BtsArtifactType
    {
        SendPort,
        ReceivePort,
        Orchestration
    }

    public enum BtsArtifactStatus
    {
        Bound,
        Stopped,
        Started,
        Unenlisted,
        Enlisted,
    }

    public enum BtsPortType
    {
        ReceivePort,
        SendPort,
        Dynamic,

    }

    public enum BtsInstanceStatus : int
    {
        ReadyToRun = 1,
        Running = 2,
        SuspendedResumable = 4,
        Dehydrated = 8,
        SuspendedNonResumable = 32
    }

    public enum BtsHostInstanceStatus
    {
        NotConfigured,
        Disabled,
        Enabled,
        Started,
        Stopped
    }

    public enum BtsArtifactAction
    {
        Start,
        Stop,
        Restart,
        Unenlist
    }

    public enum BatchFilterOperator
    {
        [XmlEnum("0")]
        Equals = 0,
        [XmlEnum("1")]
        LessThan = 1,
        [XmlEnum("2")]
        LessThanOrEquals = 2,
        [XmlEnum("3")]
        GreaterThan = 3,
        [XmlEnum("4")]
        GreaterThanOrEquals = 4,
        [XmlEnum("5")]
        NotEqual = 5,
        [XmlEnum("6")]
        Exists = 6,
        [XmlEnum("7")]
        BitwiseAnd = 7,
    }

    public enum BtsEdiRecurrenceType
    {
        None,
        Hourly,
        Daily,
        Weekly,
    }

    public enum BtsEdiBatchOrchestrationAction
    {
        Start,
        Stop,
        Override,
        Refresh
    }

    public enum BtsEdiBatchOrchestrationStatus
    {
        PendingStart = 1,
        Stopped = 2,
        Started = 3,
        PendingControlMessageProcessing = 4
    }
    public enum BtsOrchestrationStatus
    {
        Unenlisted = 1,
        Enlisted = 2,
        Started = 3,
    }
}