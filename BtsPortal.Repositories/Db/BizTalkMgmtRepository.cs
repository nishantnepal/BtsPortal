using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BtsPortal.Entities.Bts;
using BtsPortal.Repositories.Interface;
using BtsPortal.Repositories.Utilities;
using Dapper;
using Microsoft.BizTalk.B2B.PartnerManagement;

namespace BtsPortal.Repositories.Db
{
    public  class BizTalkMgmtRepository: IBizTalkMgmtRepository
    {
        readonly IDbConnection _dbConnection;
        
        public BizTalkMgmtRepository(IDbConnection dbConnection)
        {
            //_dbConnection = new SqlConnection(dbConnectionString);
            _dbConnection = dbConnection;
        }

        public  virtual List<BtsArtifact> LoadArtifactTypes()
        {
            const string query = @"
                        SELECT 
                        'ReceivePort' as ArtifactType
                              ,sp.[nvcName] as ArtifactName
                              ,[uidGUID] as ServiceId
                              --,sp.[DateModified]      
	                          ,bap.nvcName as ApplicationName
                          FROM [BizTalkMgmtDb].[dbo].[bts_receiveport] sp(NOLOCK)
                          inner join bts_application bap (NOLOCK) on sp.nApplicationID=bap.nID
                          UNION ALL
                          SELECT 'SendPort' as ArtifactType
                              ,sp.[nvcName] as ArtifactName
                              ,[uidGUID]      
	                          --,sp.[DateModified] 
	                          ,bap.nvcName as ApplicationName
                          FROM [BizTalkMgmtDb].[dbo].[bts_sendport] sp (NOLOCK)
                          inner join bts_application bap (NOLOCK) on sp.nApplicationID=bap.nID
                          UNION ALL
                           SELECT 'Orchestration' as ArtifactType
                              ,bo.nvcFullName as ArtifactName
                              ,[uidGUID]
	                          ,bap.nvcName as ApplicationName
                            --  ,dtModified as [DateModified]     
                          FROM [BizTalkMgmtDb].[dbo].[bts_orchestration] bo
                          inner join [dbo].[bts_assembly] ba (NOLOCK) on ba.nID = bo.[nAssemblyID]
                          inner join bts_application bap (NOLOCK) on ba.nApplicationID=bap.nID
                        ";

            var vm = _dbConnection.Query<BtsArtifact>(query).ToList();

            return vm;
        }

        public  virtual List<BtsHostInstance> GetHostInstances(List<string> hostNames)
        {
            const string query = @"
select 
adm_HostInstance.Name as Name, 
adm_HostInstance.DateModified, 
adm_HostInstance.UniqueId, 
adm_Host.Name as HostName,
adm_Server.Name as Server
 ,case ISNULL(adm_HostInstance.[DisableHostInstance],-10)
	  when -10 then 'Not Configured'
	  when -1 then 'Disabled'
	  else 'Enabled'
	  end as HostInstanceStatus
from adm_HostInstance, adm_Host, adm_Server, adm_Server2HostMapping with (nolock) 
where adm_HostInstance.Svr2HostMappingId = adm_Server2HostMapping.Id 
and adm_Host.Id = adm_Server2HostMapping.HostId 
and adm_Server.Id = adm_Server2HostMapping.ServerId 
and adm_Host.Name in @hostNames

";

            return _dbConnection.Query<BtsHostInstance>(query, new { @hostNames = hostNames }).ToList();
        }

        public  virtual List<BtsParty> GetParties()
        {
            const string query = @"
                                select distinct p.name,
                                substring(
                                        (
                                            Select ','+ Value  AS [text()]
                                            From [tpm].[BusinessIdentity] bi (NOLOCK)
			                                left outer join [tpm].[BusinessProfile] bp (NOLOCK) on p.PartnerId = bp.PartnerId
                                            Where bp.ProfileId = bi.ProfileId
                                            ORDER BY bi.ProfileId
                                            For XML PATH ('')
                                        ), 2, 1000) [Identities]
                                from tpm.partner p(NOLOCK)
                                order by p.name
                                ";
            var vm = _dbConnection.Query<BtsParty>(query).ToList();
            return vm;
        }

        protected List<BtsPartyBusinessProfile> GetEdiBusinessProfiles(int partnerId)
        {
            const string query = @"
  select
bp.Name as Name,bp.ProfileId as Id,bp.Description
from tpm.partner p(NOLOCK)
left outer join [tpm].[BusinessProfile] bp (NOLOCK) on p.PartnerId = bp.PartnerId
where p.PartnerId = @partnerId
";

            return _dbConnection.Query<BtsPartyBusinessProfile>(query, new { partnerId }).ToList();
        }

        protected List<BtsPartyBusinessProfileIdentity> GetEdiBusinessProfileIdentity(int partnerId, int profileId)
        {
            const string query = @"
  select bi.Qualifier, bi.Value,bi.Description,bi.Id
 ,bp.ProfileId as ProfileId
from tpm.partner p(NOLOCK)
left outer join [tpm].[BusinessProfile] bp (NOLOCK) on p.PartnerId = bp.PartnerId
left outer join [tpm].[BusinessIdentity] bi (NOLOCK) on bp.ProfileId = bi.ProfileId
where p.PartnerId = @partnerId and bp.ProfileId = @profileId
";

            return _dbConnection.Query<BtsPartyBusinessProfileIdentity>(query, new { partnerId, profileId }).ToList();
        }

        protected List<DbAgreement> GetEdiAgreementsPerProfile(int profileId)
        {
            const string query = @"
 SELECT  
Id,SenderOnewayAgreementId, ReceiverOnewayAgreementId
FROM [BizTalkMgmtDb].[tpm].[Agreement] a (NOLOCK)
where SenderProfileId = @profileId or ReceiverProfileId = @profileId
";

            return _dbConnection.Query<DbAgreement>(query, new { profileId }).ToList();
        }

        protected List<DbOneWayAgreement> GetEdiOneWayAgreements(List<int> agreementIds)
        {
            const string query = @"
 select a.Id,
bp1.Name as SendingProfileName,p1.Name as SendingPartnerName,
bp2.Name as ReceivingProfileName,p2.Name as ReceivingPartnerName
from tpm.OnewayAgreement a (NOLOCK)
inner join tpm.BusinessIdentity bi1 (NOLOCK) on bi1.Id = a.SenderId
inner join tpm.BusinessProfile bp1 (NOLOCK) on bi1.ProfileId = bp1.ProfileId
inner join tpm.Partner p1 (NOLOCK) on bp1.PartnerId = p1.PartnerId
inner join tpm.BusinessIdentity bi2 (NOLOCK) on bi2.Id = a.ReceiverId
inner join tpm.BusinessProfile bp2 (NOLOCK) on bi2.ProfileId = bp2.ProfileId
inner join tpm.Partner p2 (NOLOCK) on bp2.PartnerId = p2.PartnerId
where a.Id in @agreementIds
";

            return _dbConnection.Query<DbOneWayAgreement>(query, new { agreementIds }).ToList();
        }

        public  virtual List<DbOneWayAgreementBatchingOrch> GetEdiBatchingOrchestrationInstanceIds(int oneWayAgreementId)
        {
            const string query = @"
select BatchOrchestrationId,name as BatchName,ISNULL(p.BatchId,0) as ControlBatchId
from [dbo].[PAM_Batching_Log] bl (NOLOCK)
inner join [tpm].[BatchDescription] bd (NOLOCK)  on bd.id = bl.BatchId
left outer join PAM_Control p (NOLOCK) on p.BatchId = bl.BatchId and p.UsedOnce = 0
where bd.OnewayAgreementId = @oneWayAgreementId
";

            return _dbConnection.Query<DbOneWayAgreementBatchingOrch>(query, new { oneWayAgreementId }).ToList();
        }

        public  virtual bool IsEdiControlMessageProcessing(int batchId)
        {
            const string query = @"
select count(*) as Cnt from PAM_Control
where BatchId = @batchId and UsedOnce =0
";
            int cnt = _dbConnection.Query(query, new { batchId }).FirstOrDefault().Cnt;
            return cnt > 0;
        }

        public  virtual BtsParty GetParty(string partyName,string homePartyName)
        {
            BtsParty party = new BtsParty
            {
                Name = partyName,
                IsHomeOrganization = string.Equals(partyName, homePartyName, StringComparison.OrdinalIgnoreCase)
            };
            using (var tpmCtx = TpmContext.Create(new SqlConnectionStringBuilder(_dbConnection.ConnectionString)))
            {
                Partner homeOrg = tpmCtx.GetPartner(homePartyName);
                Partner partner = tpmCtx.GetPartner(partyName);
                party.PartnerId = partner.Id;

                var sendPorts = partner.GetSendPortAssociations();
                foreach (var port in sendPorts)
                {
                    party.SendPorts.Add(port.Name);
                }

                party.BusinessProfiles = GetEdiBusinessProfiles(partner.Id);
                foreach (var profile in party.BusinessProfiles)
                {
                    profile.ProfileIdentities = GetEdiBusinessProfileIdentity(partner.Id, profile.Id);

                    if (!party.IsHomeOrganization)
                    {
                        var dbAgreements = GetEdiAgreementsPerProfile(profile.Id);
                        foreach (var dbAgreement in dbAgreements)
                        {
                            var agreement = tpmCtx.Agreements.FirstOrDefault(m => m.Id == dbAgreement.Id);
                            var btsAgreement = new BtsPartyBusinessProfileAgreement()
                            {
                                AgreementId = agreement.Id,
                                Name = agreement.Name,
                                Protocol = agreement.Protocol,
                                ReceivingProfile = agreement.ReceiverDetails.Profile,
                                ReceivingPartner = agreement.ReceiverDetails.Partner,
                                Enabled = agreement.Enabled,
                                SendingPartner = agreement.SenderDetails.Partner,
                                SendingProfile = agreement.SenderDetails.Profile
                            };

                            var dbOneWayAgreements =
                                GetEdiOneWayAgreements(new List<int>()
                                {
                                    dbAgreement.ReceiverOnewayAgreementId,
                                    dbAgreement.SenderOnewayAgreementId
                                });

                            foreach (var dbOneWayAgreement in dbOneWayAgreements)
                            {
                                var onewayAgreement = tpmCtx.OnewayAgreements.FirstOrDefault(m => m.Id == dbOneWayAgreement.Id);
                                var batches = onewayAgreement.GetBatches();
                                var dbBatchInfo = GetEdiBatchingOrchestrationInstanceIds(dbOneWayAgreement.Id);
                                var onewayAg = new BtsPartyBusinessProfileOneWayAgreement()
                                {
                                    Id = onewayAgreement.Id,
                                    SendingPartnerName = dbOneWayAgreement.SendingPartnerName,
                                    ReceivingPartnerName = dbOneWayAgreement.ReceivingPartnerName,
                                    SendingProfileName = dbOneWayAgreement.SendingProfileName,
                                    ReceivingProfileName = dbOneWayAgreement.ReceivingProfileName
                                };

                                foreach (BatchDescription batch in batches)
                                {
                                    var btsBatch = new BtsPartyBusinessProfileOneWayAgreementBatch()
                                    {
                                        Name = batch.Name,
                                        Id = batch.Id,
                                        Description = batch.Description,
                                        OrchestrationId = dbBatchInfo.FirstOrDefault(m => m.BatchName == batch.Name)?.BatchOrchestrationId,
                                        PendingStart = dbBatchInfo.Any(m => m.BatchName == batch.Name) && dbBatchInfo.FirstOrDefault(m => m.BatchName == batch.Name)?.BatchOrchestrationId == null,
                                        PendingPamControlOperation = dbBatchInfo.FirstOrDefault(m => m.BatchName == batch.Name)?.ControlBatchId > 0
                                    };

                                    var filterPredicate = batch.GetFilterPredicate();
                                    foreach (FilterGroup filterGroup in filterPredicate.Groups)
                                    {
                                        foreach (FilterStatement filterStatement in filterGroup.Statements)
                                        {
                                            btsBatch.FilterStatements.Add(new BtsAgreementBatchFilterStatement()
                                            {
                                                FilterValue = filterStatement.Value,
                                                Property = filterStatement.Property,
                                                BatchFilterOperator = filterStatement.Operator.ToBatchFilterOperator()
                                            });
                                        }
                                    }

                                    var releaseCritera = batch.GetReleaseCriteria();
                                    if (releaseCritera.GetType() == typeof(MessageCountReleaseCriteria)) //MessageCountReleaseCriteria, InterchangeSizeReleaseCriteria,ManualReleaseCriteria,TimeBasedReleaseCriteria
                                    {
                                        MessageCountReleaseCriteria criteria =
                                            (MessageCountReleaseCriteria)releaseCritera;

                                        btsBatch.ReleaseCriteria.Type = "MessageCountReleaseCriteria";
                                        btsBatch.ReleaseCriteria.CriteriaMessageCount =
                                            new BtsAgreementBatchReleaseCriteriaMessageCount
                                            {
                                                MessageCount = criteria.MessageCount,
                                                Scope = criteria.Scope.ToString()
                                            };
                                    }

                                    if (releaseCritera.GetType() == typeof(InterchangeSizeReleaseCriteria)) //InterchangeSizeReleaseCriteria,ManualReleaseCriteria,TimeBasedReleaseCriteria
                                    {
                                        InterchangeSizeReleaseCriteria criteria =
                                            (InterchangeSizeReleaseCriteria)releaseCritera;

                                        btsBatch.ReleaseCriteria.Type = "InterchangeSizeReleaseCriteria";
                                        btsBatch.ReleaseCriteria.CriteriaInterchangeSize =
                                            new BtsAgreementBatchReleaseCriteriaInterchangeSize()
                                            {
                                                CharacterCount = criteria.CharacterCount
                                            };
                                    }

                                    if (releaseCritera.GetType() == typeof(TimeBasedReleaseCriteria)) //ManualReleaseCriteria,TimeBasedReleaseCriteria
                                    {
                                        TimeBasedReleaseCriteria criteria =
                                            (TimeBasedReleaseCriteria)releaseCritera;
                                        btsBatch.ReleaseCriteria.Type = "TimeBasedReleaseCriteria";
                                        btsBatch.ReleaseCriteria.CriteriaTimeBasedRelease =
                                            new BtsAgreementBatchReleaseCriteriaTimeBasedRelease()
                                            {
                                                FirstRelease = criteria.FirstRelease,
                                                SendEmptyBatchSignal = criteria.SendEmptyBatchSignal,
                                                RecurrencePeriod = criteria.RecurrenceSchedule.RecurrencePeriod,
                                                RecurrenceType = criteria.RecurrenceSchedule.ToBtsEdiRecurrenceType()
                                            };
                                    }

                                    if (releaseCritera.GetType() == typeof(ManualReleaseCriteria)) //ManualReleaseCriteria,TimeBasedReleaseCriteria
                                    {
                                        ManualReleaseCriteria criteria =
                                            (ManualReleaseCriteria)releaseCritera;

                                        btsBatch.ReleaseCriteria.Type = "ManualReleaseCriteria";
                                    }

                                    onewayAg.Batches.Add(btsBatch);
                                }

                                if (string.Equals(dbOneWayAgreement.SendingPartnerName, homePartyName, StringComparison.OrdinalIgnoreCase))
                                {
                                    btsAgreement.HomeOrgToPartnerOneWayAgreement = onewayAg;
                                }
                                else
                                {
                                    btsAgreement.PartnerToHomeOrgOneWayAgreement = onewayAg;
                                }

                            }

                            profile.Agreements.Add(btsAgreement);
                        }

                        //todo : load one way agreement

                        //var ag = tpmCtx.OnewayAgreements.Where(m=>m.);
                        //var batch = ag.GetBatches().FirstOrDefault();
                        //var releaseCritera = batch.GetReleaseCriteria();
                        //if (releaseCritera.GetType() == typeof(MessageCountReleaseCriteria)) //MessageCountReleaseCriteria, InterchangeSizeReleaseCriteria,ManualReleaseCriteria,TimeBasedReleaseCriteria
                        //{

                        //}
                        //var filterPredicate = batch.GetFilterPredicate();
                        //foreach (FilterGroup filterGroup in filterPredicate.Groups)
                        //{
                        //    foreach (FilterStatement filterStatement in filterGroup.Statements)
                        //    {
                        //        filterStatement.Operator
                        //    }
                        //}
                        //var proto = ag.GetProtocolName();
                        //X12ProtocolSettings x12Settings = ag.GetProtocolSettings<X12ProtocolSettings>();
                        //var as2Settings = ag.GetProtocolSettings<AS2ProtocolSettings>();
                        ////x12Settings.FramingSettings.  - character sets



                    }
                }



                ////Console.WriteLine("Creating Partner " + partnerName + "   in Biztalk.");
                //// profile.get
                //var partnership = tpmCtx.GetPartnership(homeOrg, partner);

                ////Console.WriteLine("profile.AgreementAsReceiver.Count " + profile.AgreementAsReceiver.Count );
                ////Console.WriteLine("profile.AgreementAsSender.Count " + profile.AgreementAsSender.Count );

                //foreach (Agreement agreement in partnership.GetAgreements())
                //{
                //    //todo : profile (receiver or sender) will match business profile name
                //    var btsAgreement = new BtsPartyBusinessProfileAgreement()
                //    {
                //        AgreementId = agreement.Id,
                //        Name = agreement.Name,
                //        Protocol = agreement.Protocol,
                //        ReceivingProfile = agreement.ReceiverDetails.Profile,
                //        ReceivingPartner = agreement.ReceiverDetails.Partner,
                //        Enabled = agreement.Enabled,
                //        SendingPartner = agreement.SenderDetails.Partner,
                //        SendingProfile = agreement.SenderDetails.Profile
                //    };
                //    bp.Agreements.Add(btsAgreement);

                //    //todo : load one way agreement

                //    //var ag = tpmCtx.OnewayAgreements.FirstOrDefault();
                //    //var batch = ag.GetBatches().FirstOrDefault();
                //    //var releaseCritera = batch.GetReleaseCriteria();
                //    //if (releaseCritera.GetType() == typeof(MessageCountReleaseCriteria)) //MessageCountReleaseCriteria, InterchangeSizeReleaseCriteria,ManualReleaseCriteria,TimeBasedReleaseCriteria
                //    //{

                //    //}
                //    //var filterPredicate = batch.GetFilterPredicate();
                //    //foreach (FilterGroup filterGroup in filterPredicate.Groups)
                //    //{
                //    //    foreach (FilterStatement filterStatement in filterGroup.Statements)
                //    //    {
                //    //        filterStatement.Operator
                //    //    }
                //    //}
                //    //var proto = ag.GetProtocolName();
                //    //X12ProtocolSettings x12Settings = ag.GetProtocolSettings<X12ProtocolSettings>();
                //    //var as2Settings = ag.GetProtocolSettings<AS2ProtocolSettings>();
                //    ////x12Settings.FramingSettings.  - character sets



                //    Console.WriteLine("agreement :  " + btsAgreement.ToString());
                //}

                ////foreach (Agreement agreement in profile.AgreementAsReceiver)
                ////{
                ////    bp.AgreementsAsReceiver.Add(new BtsPartyBusinessProfileAgreement()
                ////    {
                ////        Name = agreement.Name,
                ////        Protocol = agreement.Protocol
                ////    });

                ////    Console.WriteLine("agreement :  " + agreement.Name);
                ////}

                ////foreach (Agreement agreement in profile.AgreementAsSender)
                ////{
                ////    bp.AgreementsAsSender.Add(new BtsPartyBusinessProfileAgreement()
                ////    {
                ////        Name = agreement.Name,
                ////        Protocol = agreement.Protocol
                ////    });

                ////    Console.WriteLine("agreement :  " + agreement.Name);
                //// }

                //party.BusinessProfiles.Add(bp);
            }

            return party;
        }

        public  virtual void UpdateEdiBatchStatus(BtsEdiBatchOrchestrationAction action, int batchId, string batchName, string agreementName, string sendingPartner, string receivingPartner)
        {
            string actionString = null;
            switch (action)
            {
                case BtsEdiBatchOrchestrationAction.Start:
                    actionString = "EdiBatchActivate";
                    break;
                case BtsEdiBatchOrchestrationAction.Stop:
                    actionString = "EdiBatchTerminate";
                    break;
                case BtsEdiBatchOrchestrationAction.Override:
                    actionString = "EdiBatchOverride";
                    break;
            }

            const string query = @"
                        insert into PAM_Control
                        (
                        EdiMessageType,
                        ActionType, 
                        ActionDateTime, 
                        UsedOnce, 
                        BatchId,
                        BatchName, 
                        SenderPartyName, 
                        ReceiverPartyName, 
                        AgreementName
                        )
                        values
                        (
                        0,
                        @actionString,
                        getdate(),
                        0,
                        @batchId,
                        @batchName,
                        @sendingPartner,
                        @receivingPartner,
                        @agreementName
                        )
                        ";

            _dbConnection.Execute(query, new
            {
                actionString = actionString.ToDbStringAnsi(),
                batchId,
                batchName = batchName.ToDbStringAnsi(),
                sendingPartner = sendingPartner.ToDbStringAnsi(),
                receivingPartner = receivingPartner.ToDbStringAnsi(),
                agreementName = agreementName.ToDbStringAnsi(),

            });

        }




    }
}
