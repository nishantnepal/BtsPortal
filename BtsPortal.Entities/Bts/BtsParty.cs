using System;
using System.Collections.Generic;

namespace BtsPortal.Entities.Bts
{
    public class BtsParty
    {
        public bool IsHomeOrganization { get; set; }
        public string Name { get; set; }
        public int PartnerId { get; set; }
        public string Identities { get; set; }
        public List<string> SendPorts { get; set; }
        public List<BtsPartyBusinessProfile> BusinessProfiles { get; set; }

        public BtsParty()
        {
            SendPorts = new List<string>();
            BusinessProfiles = new List<BtsPartyBusinessProfile>();
        }
    }

    public class BtsPartyBusinessProfile
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public List<BtsPartyBusinessProfileAgreement> Agreements { get; set; }
        public List<BtsPartyBusinessProfileIdentity> ProfileIdentities { get; set; }

        public BtsPartyBusinessProfile()
        {
            Agreements = new List<BtsPartyBusinessProfileAgreement>();
            ProfileIdentities = new List<BtsPartyBusinessProfileIdentity>();
        }

    }

    public class BtsPartyBusinessProfileAgreement
    {
        public int AgreementId { get; set; }
        public string Name { get; set; }
        public string Protocol { get; set; }
        public string Description { get; set; }
        public string SendingPartner { get; set; }
        public string SendingProfile { get; set; }
        public string ReceivingPartner { get; set; }
        public string ReceivingProfile { get; set; }
        public bool Enabled { get; set; }

        // public List<BtsPartyBusinessProfileOneWayAgreement> OneWayAgreements { get; set; }
        public BtsPartyBusinessProfileOneWayAgreement HomeOrgToPartnerOneWayAgreement { get; set; }
        public BtsPartyBusinessProfileOneWayAgreement PartnerToHomeOrgOneWayAgreement { get; set; }

        public override string ToString()
        {
            return string.Format("{0} : Protocol: {5} Sending Partner (Profile) : {1}({2}) Receiving Partner (Profile) : {3}({4})",
                Name, SendingPartner, SendingProfile, ReceivingPartner, ReceivingProfile, Protocol);
        }

        public BtsPartyBusinessProfileAgreement()
        {
            //OneWayAgreements = new List<BtsPartyBusinessProfileOneWayAgreement>();
        }
    }

    public class BtsPartyBusinessProfileOneWayAgreement
    {
        public int Id { get; set; }
        public string SendingProfileName { get; set; }
        public string SendingPartnerName { get; set; }
        public string ReceivingProfileName { get; set; }
        public string ReceivingPartnerName { get; set; }
        public List<BtsPartyBusinessProfileOneWayAgreementBatch> Batches { get; set; }

        public BtsPartyBusinessProfileOneWayAgreement()
        {
            Batches = new List<BtsPartyBusinessProfileOneWayAgreementBatch>();
        }
    }

    public class BtsPartyBusinessProfileOneWayAgreementBatch
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public long Id { get; set; }
        public Guid? OrchestrationId { get; set; }
        public bool PendingStart { get; set; }
        public bool PendingPamControlOperation { get; set; }
        public BtsAgreementBatchReleaseCriteria ReleaseCriteria { get; set; }
        public List<BtsAgreementBatchFilterStatement> FilterStatements { get; set; }

        public BtsEdiBatchOrchestrationStatus Status
        {
            get
            {
                if (PendingPamControlOperation)
                {
                    return BtsEdiBatchOrchestrationStatus.PendingControlMessageProcessing;
                }
                return OrchestrationId.HasValue  ? BtsEdiBatchOrchestrationStatus.Started : BtsEdiBatchOrchestrationStatus.Stopped;
            }
            
        }

        public BtsPartyBusinessProfileOneWayAgreementBatch()
        {
            FilterStatements = new List<BtsAgreementBatchFilterStatement>();
            ReleaseCriteria = new BtsAgreementBatchReleaseCriteria();
        }
    }

    public class BtsAgreementBatchReleaseCriteria
    {
        public string Type { get; set; }
        public BtsAgreementBatchReleaseCriteriaMessageCount CriteriaMessageCount { get; set; }
        public BtsAgreementBatchReleaseCriteriaInterchangeSize CriteriaInterchangeSize { get; set; }
        public BtsAgreementBatchReleaseCriteriaTimeBasedRelease CriteriaTimeBasedRelease { get; set; }

    }

    public class BtsAgreementBatchReleaseCriteriaTimeBasedRelease
    {
        public DateTime FirstRelease { get; set; }
        public bool SendEmptyBatchSignal { get; set; }
        public TimeSpan RecurrencePeriod { get; set; }

        public BtsEdiRecurrenceType RecurrenceType { get; set; }
    }

    public class BtsAgreementBatchReleaseCriteriaMessageCount
    {
        public string Scope { get; set; }
        public int MessageCount { get; set; }
    }

    public class BtsAgreementBatchReleaseCriteriaInterchangeSize
    {
        public long CharacterCount { get; set; }
    }

    public class BtsAgreementBatchFilterStatement
    {
        public string Property { get; set; }
        public BatchFilterOperator BatchFilterOperator { get; set; }
        public string FilterValue { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", Property, BatchFilterOperator.ToString(), FilterValue, Environment.NewLine);
        }
    }

    public class BtsPartyBusinessProfileIdentity
    {
        public string Qualifier { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }

    }

    public class DbAgreement
    {
        public int Id { get; set; }
        public int SenderOnewayAgreementId { get; set; }
        public int ReceiverOnewayAgreementId { get; set; }

    }

    public class DbOneWayAgreement
    {
        public int Id { get; set; }
        public string SendingProfileName { get; set; }
        public string SendingPartnerName { get; set; }
        public string ReceivingProfileName { get; set; }
        public string ReceivingPartnerName { get; set; }

    }

    public class DbOneWayAgreementBatchingOrch
    {
        public string BatchName { get; set; }
        public Guid BatchOrchestrationId { get; set; }
        public int ControlBatchId { get; set; }
       

    }
}