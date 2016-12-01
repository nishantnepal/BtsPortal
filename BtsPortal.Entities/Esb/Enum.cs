namespace BtsPortal.Entities.Esb
{
    public enum FaultStatusType : int
    {
        UnResolved = 1,
        Resolved = 2,
        //Resubmitted = 3,
        Flagged = 4
    }

    public enum AlertCriteria
    {
        Application,
        ErrorType,
        FailureCategory,
        FaultCode,
        FaultGenerator,
        FaultSeverity,
        MachineName,
        Scope,
        ServiceName
    }

    public enum AlertType
    {
        Transactional = 1,
        Summary
    }
}