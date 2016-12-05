using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.BizTalk.B2B.PartnerManagement;

namespace BtsPortal.Repositories.Helpers.Edi
{
    /*
     shoutout to      
http://social.technet.microsoft.com/wiki/contents/articles/33699.create-trading-partner-agreements-in-biztalk-server-2013-r2-using-code.aspx
         */

    class TPAM
    {
        SqlConnectionStringBuilder _builder;
        public TPAM(SqlConnectionStringBuilder sqlBuilder)
        {
            _builder = sqlBuilder;
        }
        public TPAM()
        {

        }
        public bool CreatePartner(string partnerName, SqlConnectionStringBuilder builder)
        {
            var tpmCtx = TpmContext.Create(builder);
            Console.WriteLine("Creating Partner " + partnerName + "   in Biztalk.");
            Partner newPartner = tpmCtx.CreatePartner(partnerName);

            tpmCtx.SaveChanges();
            tpmCtx.Dispose();
            if (newPartner != null)
            {
                Console.WriteLine("Partner " + partnerName + " created  in Biztalk.");
                return true;
            }
            else return false;
        }
        public bool VerifyPartnerExists(string partnerName, SqlConnectionStringBuilder builder)
        {
            var tpmCtx = TpmContext.Create(builder);
            Console.WriteLine("Verifying Partner " + partnerName + " exists in Biztalk.");
            Partner existingPartner = tpmCtx.Partners.FirstOrDefault(a => a.Name == partnerName);

            tpmCtx.Dispose();
            if (existingPartner != null)
            {
                Console.WriteLine("Partner " + partnerName + " exists in Biztalk.");
                return true;
            }
            else
            {
                Console.WriteLine("Partner " + partnerName + " does not exists in Biztalk.");
                return false;
            }
        }
        public bool VerifyBusinessProfileExists(string partnerName, string businessProfileName, SqlConnectionStringBuilder builder)
        {
            var tpmCtx = TpmContext.Create(builder);
            Console.WriteLine("Verifying BusinessProfile " + businessProfileName + " exists for Partner " + partnerName + "  in Biztalk.");
            Partner existingPartner = tpmCtx.Partners.FirstOrDefault(a => a.Name == partnerName);
            if (existingPartner == null)
                return false;

            var profileList = existingPartner.GetBusinessProfiles();
            if (profileList == null)
                return false;
            var profile = profileList.FirstOrDefault(a => a.Name == businessProfileName);
            tpmCtx.Dispose();
            if (profile != null)
            {
                Console.WriteLine("BusinessProfile " + businessProfileName + "  for Partner " + partnerName + " exists in Biztalk.");

                return true;
            }
            else
            {
                Console.WriteLine("BusinessProfile " + businessProfileName + " does not exists for Partner " + partnerName + "  in Biztalk.");

                return false;
            }
        }
        public bool DeletePartner(string partnerName, SqlConnectionStringBuilder builder)
        {
            var tpmCtx = TpmContext.Create(builder);
            Console.WriteLine("Deleting Partner " + partnerName + "   in Biztalk.");
            Partner existingPartner = tpmCtx.Partners.FirstOrDefault(a => a.Name == partnerName);
            if (existingPartner != null)
                tpmCtx.DeleteObject(existingPartner);

            tpmCtx.SaveChanges();
            tpmCtx.Dispose();
            return !VerifyPartnerExists(partnerName, builder);
        }
        public bool AddBusinessProfileToPartner(string partnerName, string businessProfileName, SqlConnectionStringBuilder builder)
        {
            var tpmCtx = TpmContext.Create(builder);
            Console.WriteLine("Adding BusinessProfile " + businessProfileName + " to  Partner " + partnerName + "  in Biztalk.");

            Partner newPartner = tpmCtx.GetPartner(partnerName);
            if (String.IsNullOrEmpty(businessProfileName))
                businessProfileName = "DefaultProfile";
            BusinessProfile newPartnerProfile = newPartner.CreateBusinessProfile(businessProfileName);
            //var pcol = new X12ProtocolSettings("X12_"+partnerName+"Settings");
            //newPartnerProfile.AddProtocolSettings(pcol);
            newPartnerProfile.Description = "Partner created for transacting EDI and Xml messages";

            tpmCtx.AddToBusinessProfiles(newPartnerProfile);
            tpmCtx.SaveChanges();
            tpmCtx.Dispose();
            if (newPartnerProfile != null)
            {
                Console.WriteLine("BusinessProfile " + businessProfileName + " added  for Partner " + partnerName + "  in Biztalk.");

                return true;
            }
            else return false;
        }
        public bool AddIdentityToPartnerBusinessProfile(string partnerName, string businessProfileName, string identityQualifierValueAndDesc, SqlConnectionStringBuilder builder)
        {
            if (string.IsNullOrEmpty(businessProfileName))
                businessProfileName = "DefaultProfile";
            var tpmCtx = TpmContext.Create(builder);
            Console.WriteLine("Adding Identity " + identityQualifierValueAndDesc + " to BusinessProfile " + businessProfileName + "  for Partner " + partnerName + "  in Biztalk.");

            string identityQualifier, identityValue, identityDesc;
            identityQualifier = identityQualifierValueAndDesc.Split(':')[0].ToString();
            identityValue = identityQualifierValueAndDesc.Split(':')[1].ToString();
            identityDesc = identityQualifierValueAndDesc.Split(':')[2].ToString();

            Partner newPartner = tpmCtx.GetPartner(partnerName);
            BusinessProfile newPartnerProfile = newPartner.GetBusinessProfile(businessProfileName);
            QualifierIdentity identity = new QualifierIdentity(identityDesc, identityQualifier, identityValue);

            string protocol = "x12_edifact";
            if (identityQualifier == "AS2Identity")
                protocol = "as2";
            ReflectionHelper.SetFieldValue(identity, "_AdditionalData", protocol);

            bool suppportsPotocol = identity.SupportsProtocol("x12_edifact");
            Console.WriteLine("Identity " + identityQualifierValueAndDesc + " supports protocol X12 " + suppportsPotocol.ToString() + "  for Partner " + partnerName + "  in Biztalk.");

            newPartnerProfile.AddBusinessIdentity(identity);
            tpmCtx.AddToBusinessIdentities(identity);

            tpmCtx.SaveChanges();
            tpmCtx.Dispose();

            if (identity != null)
            {
                Console.WriteLine("Added Identity " + identityQualifierValueAndDesc + " to BusinessProfile " + businessProfileName + "  for Partner " + partnerName + "  in Biztalk.");

                return true;
            }
            else return false;
        }
        public bool VerifyIdentityExistsInPartnerBusinessProfile(string partnerName, string businessProfileName, string identityQualifierValueAndDesc, SqlConnectionStringBuilder builder)
        {
            if (string.IsNullOrEmpty(businessProfileName))
                businessProfileName = "DefaultProfile";
            var tpmCtx = TpmContext.Create(builder);
            Console.WriteLine("Verifying Identity " + identityQualifierValueAndDesc + " for BusinessProfile " + businessProfileName + "  for Partner " + partnerName + " exists in Biztalk.");

            string identityQualifier, identityValue, identityDesc;
            identityQualifier = identityQualifierValueAndDesc.Split(':')[0].ToString();
            identityValue = identityQualifierValueAndDesc.Split(':')[1].ToString();
            identityDesc = identityQualifierValueAndDesc.Split(':')[2].ToString();
            Partner newPartner = tpmCtx.GetPartner(partnerName);
            BusinessProfile newPartnerProfile = newPartner.GetBusinessProfile(businessProfileName);
            QualifierIdentity identity = new QualifierIdentity(identityDesc, identityQualifier, identityValue);
            List<BusinessIdentity> lstBusinessIdentities = newPartnerProfile.GetBusinessIdentities().ToList();
            BusinessIdentity partnerIdentity = null;
            foreach (var a in lstBusinessIdentities)
            {
                QualifierIdentity x = (QualifierIdentity)a;
                if (x.Qualifier == identityQualifier && x.Value == identityValue)
                    partnerIdentity = x;

            }
            tpmCtx.Dispose();

            if (partnerIdentity != null)
            {
                Console.WriteLine(" Identity " + identityQualifierValueAndDesc + " for BusinessProfile " + businessProfileName + "  for Partner " + partnerName + "  exists in Biztalk.");

                return true;
            }
            else
            {
                Console.WriteLine(" Identity " + identityQualifierValueAndDesc + " for BusinessProfile " + businessProfileName + "  for Partner " + partnerName + " doesnot exists in Biztalk.");

                return false;
            }
        }
        public bool CreatePartnership(string firstPartnerName, string secondPartnerName, SqlConnectionStringBuilder builder)
        {
            var tpmCtx = TpmContext.Create(builder);
            Console.WriteLine("Creating Partnership  for partner " + firstPartnerName + " and  partner  " + secondPartnerName + "   in Biztalk.");

            Partner firstPartner = tpmCtx.Partners.FirstOrDefault(a => a.Name == firstPartnerName);
            Partner secondPartner = tpmCtx.Partners.FirstOrDefault(a => a.Name == secondPartnerName);
            Partnership partnership = tpmCtx.CreatePartnership(firstPartner, secondPartner);



            tpmCtx.SaveChanges();
            tpmCtx.Dispose();
            if (partnership != null)
            {
                Console.WriteLine("Created Partnership  for partner " + firstPartnerName + " and  partner  " + secondPartnerName + "   in Biztalk.");

                return true;
            }
            else return false;
        }
        public bool VerifyPartnershipExists(string firstPartnerName, string secondPartnerName, SqlConnectionStringBuilder builder)
        {
            var tpmCtx = TpmContext.Create(builder);
            Console.WriteLine("Verifying Partnership  for partner " + firstPartnerName + " and  partner  " + secondPartnerName + "  exists in Biztalk.");

            Partner firstPartner = tpmCtx.Partners.FirstOrDefault(a => a.Name == firstPartnerName);
            Partner secondPartner = tpmCtx.Partners.FirstOrDefault(a => a.Name == secondPartnerName);
            Partnership partnership = null;
            try
            {
                partnership = tpmCtx.GetPartnership(firstPartner, secondPartner);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("not exist"))
                    Console.WriteLine(" Partnership  for partner " + firstPartnerName + " and  partner  " + secondPartnerName + " doesnot  exists in Biztalk.");
                return false;

            }




            tpmCtx.Dispose();
            if (partnership != null)
            {
                Console.WriteLine("Partnership  for partner " + firstPartnerName + " and  partner  " + secondPartnerName + "  exists in Biztalk.");
                return true;
            }
            else return false;
        }

        public bool VerifyAgreementExists(string agreementName, SqlConnectionStringBuilder builder)
        {
            var tpmCtx = TpmContext.Create(builder);

            Console.WriteLine("Verifying Agreement  " + agreementName + "  exists in Biztalk.");
            Agreement agreement = tpmCtx.Agreements.FirstOrDefault(a => a.Name == agreementName);



            tpmCtx.Dispose();
            if (agreement != null)
            {
                Console.WriteLine(" Agreement  " + agreementName + "  exists in Biztalk.");
                return true;
            }
            else
            {
                Console.WriteLine(" Agreement  " + agreementName + " does not  exists in Biztalk.");
                return false;
            }
        }

        public bool CreateX12Agreement(string firstPartnerNameAndBusinessProfile, string secondPartnerNameAndBusinessProfile, string firstPartnerIdentityValue, string secondPartyIdentityValue, string agreementName, SqlConnectionStringBuilder builder)
        {
            Console.WriteLine("Creating X12 Agreement  " + agreementName + " between Partner with business profile " + firstPartnerNameAndBusinessProfile + " and partner with business profile " + secondPartnerNameAndBusinessProfile + "   in Biztalk.");
            Console.WriteLine("ISA 05 -06  values are  " + firstPartnerIdentityValue + " and ISA 07-08 values are " + secondPartyIdentityValue);
            var tpmCtx = TpmContext.Create(builder);
            string firstPartnerName = "", secondPartnerName = "", firstPartnerBusinessProfile = "DefaultProfile", secondPartnerBusinessProfile = "DefaultProfile";
            firstPartnerName = firstPartnerNameAndBusinessProfile.Split(':')[0].ToString();
            secondPartnerName = secondPartnerNameAndBusinessProfile.Split(':')[0].ToString();
            firstPartnerBusinessProfile = firstPartnerNameAndBusinessProfile.Split(':')[1] == null ? firstPartnerBusinessProfile : firstPartnerNameAndBusinessProfile.Split(':')[1].ToString();
            secondPartnerBusinessProfile = secondPartnerNameAndBusinessProfile.Split(':')[1] == null ? secondPartnerBusinessProfile : secondPartnerNameAndBusinessProfile.Split(':')[1].ToString();
            Partner firstPartner = tpmCtx.Partners.FirstOrDefault(a => a.Name == firstPartnerName);
            Partner secondPartner = tpmCtx.Partners.FirstOrDefault(a => a.Name == secondPartnerName);
            Partnership partnership = tpmCtx.GetPartnership(firstPartner, secondPartner);
            PartnerAgreementContext firstPartyCtx = new PartnerAgreementContext(firstPartnerName, firstPartnerBusinessProfile, null);

            PartnerAgreementContext secondPartyCtx = new PartnerAgreementContext(secondPartnerName, secondPartnerBusinessProfile, null);
            if (string.IsNullOrEmpty(agreementName))
                agreementName = firstPartnerName + "_" + secondPartnerName + "_X12Agreement";

            List<BusinessIdentity> lstBusinessIdentities = firstPartner.GetBusinessProfile(firstPartnerBusinessProfile).GetBusinessIdentities().ToList();
            BusinessIdentity secondPartnerIdentity = null, firstPartnerIdentity = null;
            foreach (var a in lstBusinessIdentities)
            {
                QualifierIdentity x = (QualifierIdentity)a;
                if (x.Qualifier == firstPartnerIdentityValue.Split(':')[0].ToString() && x.Value == firstPartnerIdentityValue.Split(':')[1].ToString())
                    firstPartnerIdentity = x;

            }

            lstBusinessIdentities = secondPartner.GetBusinessProfile(secondPartnerBusinessProfile).GetBusinessIdentities().ToList();
            foreach (var a in lstBusinessIdentities)
            {
                QualifierIdentity x = (QualifierIdentity)a;
                if (x.Qualifier == secondPartyIdentityValue.Split(':')[0].ToString() && x.Value == secondPartyIdentityValue.Split(':')[1].ToString())
                    secondPartnerIdentity = x;


            }
            Agreement agreement = partnership.CreateAgreement<X12ProtocolSettings>((Microsoft.BizTalk.B2B.PartnerManagement.TpmContext)tpmCtx, (string)agreementName, (Microsoft.BizTalk.B2B.PartnerManagement.PartnerAgreementContext)firstPartyCtx, (Microsoft.BizTalk.B2B.PartnerManagement.PartnerAgreementContext)secondPartyCtx);

            tpmCtx.AddToAgreements(agreement);
            OnewayAgreement onewayAgreementAtoB = agreement.GetOnewayAgreement(firstPartnerName, secondPartnerName);

            onewayAgreementAtoB.SenderIdentity = firstPartnerIdentity;
            onewayAgreementAtoB.ReceiverIdentity = secondPartnerIdentity;
            onewayAgreementAtoB.GetProtocolSettings<X12ProtocolSettings>().ValidationSettings.AllowLeadingAndTrailingSpacesAndZeroes = true;
            onewayAgreementAtoB.GetProtocolSettings<X12ProtocolSettings>().EnvelopeSettings.UsageIndicator = UsageIndicator.Production;

            OnewayAgreement onewayAgreementBtoA = agreement.GetOnewayAgreement(secondPartnerName, firstPartnerName);
            onewayAgreementBtoA.SenderIdentity = secondPartnerIdentity;
            onewayAgreementBtoA.ReceiverIdentity = firstPartnerIdentity;
            onewayAgreementBtoA.GetProtocolSettings<X12ProtocolSettings>().ValidationSettings.AllowLeadingAndTrailingSpacesAndZeroes = true;
            onewayAgreementBtoA.GetProtocolSettings<X12ProtocolSettings>().EnvelopeSettings.UsageIndicator = UsageIndicator.Production;

            tpmCtx.SaveChanges();
            tpmCtx.Dispose();
            if (agreement != null)
            {
                Console.WriteLine("Created X12 Agreement  " + agreementName + " between Partner with business profile " + firstPartnerNameAndBusinessProfile + " and partner with business profile " + secondPartnerNameAndBusinessProfile + "   in Biztalk.");

                return true;
            }
            else return false;
        }
        public bool CreateAS2Agreement(string firstPartnerNameAndBusinessProfile, string secondPartnerNameAndBusinessProfile, string firstPartnerIdentityValue, string secondPartyIdentityValue, string agreementName, SqlConnectionStringBuilder builder)
        {
            Console.WriteLine("Creating AS2 Agreement  " + agreementName + " between Partner with business profile " + firstPartnerNameAndBusinessProfile + " and partner with business profile " + secondPartnerNameAndBusinessProfile + "   in Biztalk.");
            Console.WriteLine("AS2 Identity of sender  " + firstPartnerIdentityValue + " and AS2 Identity of receiver are " + secondPartyIdentityValue);
            var tpmCtx = TpmContext.Create(builder);
            string firstPartnerName = "", secondPartnerName = "", firstPartnerBusinessProfile = "DefaultProfile", secondPartnerBusinessProfile = "DefaultProfile";
            firstPartnerName = firstPartnerNameAndBusinessProfile.Split(':')[0].ToString();
            secondPartnerName = secondPartnerNameAndBusinessProfile.Split(':')[0].ToString();
            firstPartnerBusinessProfile = firstPartnerNameAndBusinessProfile.Split(':')[1] == null ? firstPartnerBusinessProfile : firstPartnerNameAndBusinessProfile.Split(':')[1].ToString();
            secondPartnerBusinessProfile = secondPartnerNameAndBusinessProfile.Split(':')[1] == null ? secondPartnerBusinessProfile : secondPartnerNameAndBusinessProfile.Split(':')[1].ToString();
            Partner firstPartner = tpmCtx.Partners.FirstOrDefault(a => a.Name == firstPartnerName);
            Partner secondPartner = tpmCtx.Partners.FirstOrDefault(a => a.Name == secondPartnerName);
            Partnership partnership = tpmCtx.GetPartnership(firstPartner, secondPartner);
            PartnerAgreementContext firstPartyCtx = new PartnerAgreementContext(firstPartnerName, firstPartnerBusinessProfile, null);

            PartnerAgreementContext secondPartyCtx = new PartnerAgreementContext(secondPartnerName, secondPartnerBusinessProfile, null);
            if (string.IsNullOrEmpty(agreementName))
                agreementName = firstPartnerName + "_" + secondPartnerName + "_AS2Agreement";

            List<BusinessIdentity> lstBusinessIdentities = firstPartner.GetBusinessProfile(firstPartnerBusinessProfile).GetBusinessIdentities().ToList();
            BusinessIdentity secondPartnerIdentity = null, firstPartnerIdentity = null;
            foreach (var a in lstBusinessIdentities)
            {
                QualifierIdentity x = (QualifierIdentity)a;
                if (x.Qualifier == firstPartnerIdentityValue.Split(':')[0].ToString() && x.Value == firstPartnerIdentityValue.Split(':')[1].ToString())
                    firstPartnerIdentity = x;

            }

            lstBusinessIdentities = secondPartner.GetBusinessProfile(secondPartnerBusinessProfile).GetBusinessIdentities().ToList();
            foreach (var a in lstBusinessIdentities)
            {
                QualifierIdentity x = (QualifierIdentity)a;
                if (x.Qualifier == secondPartyIdentityValue.Split(':')[0].ToString() && x.Value == secondPartyIdentityValue.Split(':')[1].ToString())
                    secondPartnerIdentity = x;


            }
            Agreement agreement = partnership.CreateAgreement<AS2ProtocolSettings>((Microsoft.BizTalk.B2B.PartnerManagement.TpmContext)tpmCtx, (string)agreementName, (Microsoft.BizTalk.B2B.PartnerManagement.PartnerAgreementContext)firstPartyCtx, (Microsoft.BizTalk.B2B.PartnerManagement.PartnerAgreementContext)secondPartyCtx);

            tpmCtx.AddToAgreements(agreement);

            OnewayAgreement onewayAgreementAtoB = agreement.GetOnewayAgreement(firstPartnerName, secondPartnerName);

            onewayAgreementAtoB.SenderIdentity = firstPartnerIdentity;
            onewayAgreementAtoB.ReceiverIdentity = secondPartnerIdentity;
            onewayAgreementAtoB.GetProtocolSettings<AS2ProtocolSettings>().ValidationSettings.MessageEncrypted = true;
            onewayAgreementAtoB.GetProtocolSettings<AS2ProtocolSettings>().ValidationSettings.MessageSigned = true;


            OnewayAgreement onewayAgreementBtoA = agreement.GetOnewayAgreement(secondPartnerName, firstPartnerName);
            onewayAgreementBtoA.SenderIdentity = secondPartnerIdentity;
            onewayAgreementBtoA.ReceiverIdentity = firstPartnerIdentity;
            onewayAgreementBtoA.GetProtocolSettings<AS2ProtocolSettings>().ValidationSettings.MessageEncrypted = true;
            onewayAgreementBtoA.GetProtocolSettings<AS2ProtocolSettings>().ValidationSettings.MessageSigned = true;

            tpmCtx.SaveChanges();
            tpmCtx.Dispose();
            if (agreement != null)
            {
                Console.WriteLine("Created AS2 Agreement  " + agreementName + " between Partner with business profile " + firstPartnerNameAndBusinessProfile + " and partner with business profile " + secondPartnerNameAndBusinessProfile + "   in Biztalk.");

                return true;
            }
            else return false;
        }
        public bool EnableMDNForAS2Agreement(string AS2AgreementName, string firstPartnerName, string secondPartnerName, string encryptionForMDN, string receiptDeliveryOptionURL, SqlConnectionStringBuilder builder)
        {
            if (string.IsNullOrEmpty (receiptDeliveryOptionURL))
                Console.WriteLine("Enabling syncronous MDN settings " + encryptionForMDN + " for AS2 Agreement  " + AS2AgreementName + " between Partner  " + firstPartnerName + " and partner " + secondPartnerName + "   in Biztalk.");
            else
                Console.WriteLine("Enabling asyncronous MDN settings " + encryptionForMDN + " for AS2 Agreement  " + AS2AgreementName + " between Partner  " + firstPartnerName + " and partner " + secondPartnerName + "   in Biztalk.");
           
            var tpmCtx = TpmContext.Create(builder);
            Agreement agreement = tpmCtx.Agreements.FirstOrDefault(a => a.Name == AS2AgreementName);
            OnewayAgreement onewayAgreementAtoB = agreement.GetOnewayAgreement(firstPartnerName, secondPartnerName);

            AS2ProtocolSettings as2Settings = onewayAgreementAtoB.GetProtocolSettings<AS2ProtocolSettings>();

           
            as2Settings.MDNSettings.SendInboundMDNToMessageBox = true;
            as2Settings.MDNSettings.NeedMDN = true;
            if (!string.IsNullOrEmpty(encryptionForMDN))
            {
                as2Settings.MDNSettings.SignMDN = true;
                if (encryptionForMDN=="SHA1")
                as2Settings.MDNSettings.MicHashingAlgorithm = HashingAlgorithm.SHA1;
                else
                    as2Settings.MDNSettings.MicHashingAlgorithm = HashingAlgorithm.MD5;
            }
             if (!string.IsNullOrEmpty(receiptDeliveryOptionURL))
            {
            as2Settings.MDNSettings.SendMDNAsynchronously=true;
            UriBuilder uribuilder=new UriBuilder(receiptDeliveryOptionURL);
            as2Settings.MDNSettings.ReceiptDeliveryUrl = uribuilder.Uri;
            as2Settings.MDNSettings.DispositionNotificationTo=receiptDeliveryOptionURL;
             }
            tpmCtx.SaveChanges();
            tpmCtx.Dispose();
            if (agreement != null)
            {
                if (string.IsNullOrEmpty(receiptDeliveryOptionURL))
                    Console.WriteLine("Enabled syncronous MDN settings for AS2 Agreement  " + AS2AgreementName + " between Partner  " + firstPartnerName + " and partner " + secondPartnerName + "   in Biztalk.");
                else
                    Console.WriteLine("Enabled asyncronous MDN settings for AS2 Agreement  " + AS2AgreementName + " between Partner  " + firstPartnerName + " and partner " + secondPartnerName + "   in Biztalk.");
           
           
                return true;
            }
            else return false;
        }

        public bool EnableFunctionalAckForX12Agreement(string x12AgreementName, string firstPartnerName, string secondPartnerName, string functionalAckVersion, SqlConnectionStringBuilder builder)
        {
            Console.WriteLine("Enabling FunctionalAck version" + functionalAckVersion + " for X12 Agreement  " + x12AgreementName + " between Partner  " + firstPartnerName + " and partner " + secondPartnerName + "   in Biztalk.");

            var tpmCtx = TpmContext.Create(builder);
            Agreement agreement = tpmCtx.Agreements.FirstOrDefault(a => a.Name == x12AgreementName);
            OnewayAgreement onewayAgreementAtoB = agreement.GetOnewayAgreement(firstPartnerName, secondPartnerName);

            X12ProtocolSettings x12Settings = onewayAgreementAtoB.GetProtocolSettings<X12ProtocolSettings>();
            //X12EnvelopeSettings envSettings = x12Settings.EnvelopeSettings;
            X12AcknowledgementSettings ackSettings = x12Settings.AcknowledgementSettings;
            x12Settings.AcknowledgementSettings.NeedFunctionalAcknowledgement = true;
            x12Settings.AcknowledgementSettings.FunctionalAcknowledgementVersion = functionalAckVersion;
            x12Settings.AcknowledgementSettings.NeedLoopForValidMessages = true;
            tpmCtx.SaveChanges();
            tpmCtx.Dispose();
            if (agreement != null)
            {
                Console.WriteLine("Enabled FunctionalAck version" + functionalAckVersion + " for X12 Agreement  " + x12AgreementName + " between Partner  " + firstPartnerName + " and partner " + secondPartnerName + "   in Biztalk.");

                return true;
            }
            else return false;
        }

        public bool AddAgreementAliasToAgreement(string agreementName, string firstPartnerName, string secondPartnerName, string agreementAliasKey, string agreementAliasValue, SqlConnectionStringBuilder builder)
        {
            Console.WriteLine("Add agreement alias " + agreementAliasKey + " " + agreementAliasValue + " for  Agreement  " + agreementName + " between Partner  " + firstPartnerName + " and partner " + secondPartnerName + "   in Biztalk.");

            var tpmCtx = TpmContext.Create(builder);
            Agreement agreement = tpmCtx.Agreements.FirstOrDefault(a => a.Name == agreementName);
            OnewayAgreement onewayAgreementAtoB = agreement.GetOnewayAgreement(firstPartnerName, secondPartnerName);
            AgreementAlias alias = new AgreementAlias(agreementAliasKey, agreementAliasValue);
            onewayAgreementAtoB.AddAlias(alias);


            tpmCtx.SaveChanges();
            tpmCtx.Dispose();
            if (alias != null)
            {
                Console.WriteLine("Added agreement alias " + agreementAliasKey + " " + agreementAliasValue + " for  Agreement  " + agreementName + " between Partner  " + firstPartnerName + " and partner " + secondPartnerName + "   in Biztalk.");

                return true;
            }
            else return false;
        }

        public bool VerifyAgreementAliasExistsForAgreement(string agreementName, string firstPartnerName, string secondPartnerName, string agreementAliasKey, string agreementAliasValue, SqlConnectionStringBuilder builder)
        {
            Console.WriteLine("Verifying  agreement alias " + agreementAliasKey + " " + agreementAliasValue + " for  Agreement  " + agreementName + " between Partner  " + firstPartnerName + " and partner " + secondPartnerName + " exists  in Biztalk.");

            var tpmCtx = TpmContext.Create(builder);
            Agreement agreement = tpmCtx.Agreements.FirstOrDefault(a => a.Name == agreementName);
            OnewayAgreement onewayAgreementAtoB = agreement.GetOnewayAgreement(firstPartnerName, secondPartnerName);
            List<AgreementAlias> aliasList = null;
            try
            {
                aliasList = onewayAgreementAtoB.GetAliases().ToList();

            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("exist"))
                {
                    Console.WriteLine("Agreement alias " + agreementAliasKey + " " + agreementAliasValue + " for  Agreement  " + agreementName + " between Partner  " + firstPartnerName + " and partner " + secondPartnerName + " doesnot exist  in Biztalk.");
                    return false;
                }
            }
            var alias = aliasList.FirstOrDefault(a => a.Key == agreementAliasKey && a.Value == agreementAliasValue);


            tpmCtx.Dispose();
            if (alias != null)
            {
                Console.WriteLine("Agreement alias " + agreementAliasKey + " " + agreementAliasValue + " for  Agreement  " + agreementName + " between Partner  " + firstPartnerName + " and partner " + secondPartnerName + " exists  in Biztalk.");

                return true;
            }
            else
            {
                Console.WriteLine("Agreement alias " + agreementAliasKey + " " + agreementAliasValue + " for  Agreement  " + agreementName + " between Partner  " + firstPartnerName + " and partner " + secondPartnerName + " doesnot exist  in Biztalk.");

                return false;
            }
        }

        public bool SetCharacterSetAndSeparatorForX12Agreement(string x12AgreementName, string firstPartnerName, string secondPartnerName, string dataElementSeparatorComponentSeparatorSegmentTerminator, SqlConnectionStringBuilder builder)
        {
            Console.WriteLine("Enabling DataElementSeparator,ComponentSeparator,SegmentTerminator" + dataElementSeparatorComponentSeparatorSegmentTerminator + " for X12 Agreement  " + x12AgreementName + " between Partner  " + firstPartnerName + " and partner " + secondPartnerName + "   in Biztalk.");

            var tpmCtx = TpmContext.Create(builder);
            Agreement agreement = tpmCtx.Agreements.FirstOrDefault(a => a.Name == x12AgreementName);
            OnewayAgreement onewayAgreementAtoB = null;
            try
            {
                onewayAgreementAtoB = agreement.GetOnewayAgreement(firstPartnerName, secondPartnerName);
            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("not exist"))
                    return false;
            }
            string dataElementSeparator = "", componentSeparator = "", segmentTerminator = "";
            dataElementSeparator = !String.IsNullOrEmpty(dataElementSeparatorComponentSeparatorSegmentTerminator.Split(':')[0]) ? dataElementSeparatorComponentSeparatorSegmentTerminator.Split(':')[0].ToString() : "126";
            componentSeparator = !String.IsNullOrEmpty(dataElementSeparatorComponentSeparatorSegmentTerminator.Split(':')[1]) ? dataElementSeparatorComponentSeparatorSegmentTerminator.Split(':')[1].ToString() : "124";
            segmentTerminator = !String.IsNullOrEmpty(dataElementSeparatorComponentSeparatorSegmentTerminator.Split(':')[2]) ? dataElementSeparatorComponentSeparatorSegmentTerminator.Split(':')[2].ToString() : "18";
            X12ProtocolSettings x12Settings = onewayAgreementAtoB.GetProtocolSettings<X12ProtocolSettings>();
            x12Settings.FramingSettings.DataElementSeparator = Convert.ToInt32(dataElementSeparator);
            x12Settings.FramingSettings.SegmentTerminator = Convert.ToInt32(segmentTerminator);
            x12Settings.FramingSettings.ComponentSeparator = Convert.ToInt32(componentSeparator);
            x12Settings.FramingSettings.SegmentTerminatorSuffix = SegmentTerminatorSuffix.CRLF;

            tpmCtx.SaveChanges();
            tpmCtx.Dispose();
            if (agreement != null)
            {
                Console.WriteLine("Enabled DataElementSeparator,ComponentSeparator,SegmentTerminator" + dataElementSeparatorComponentSeparatorSegmentTerminator + " for X12 Agreement  " + x12AgreementName + " between Partner  " + firstPartnerName + " and partner " + secondPartnerName + "   in Biztalk.");

                return true;
            }
            else return false;
        }

        public bool AddSendPortToOneWayAgreementAgreement(string partnerNameOfSendPorts, string agreementName, string firstPartnerName, string secondPartnerName, string sendportName, SqlConnectionStringBuilder builder)
        {
            var tpmCtx = TpmContext.Create(builder);
            Console.WriteLine("Adding SendPort " + sendportName + " for Agreement  name " + agreementName + " between Partner  " + firstPartnerName + " and partner " + secondPartnerName + "   in Biztalk.");

            var partner = tpmCtx.Partners.FirstOrDefault(a => a.Name == partnerNameOfSendPorts);
            Agreement agreement = tpmCtx.Agreements.FirstOrDefault(a => a.Name == agreementName);
            OnewayAgreement onewayAgreementAtoB = agreement.GetOnewayAgreement(firstPartnerName, secondPartnerName);
            onewayAgreementAtoB.SendPortReferences.Add(partner.GetSendPortAssociations().FirstOrDefault(a => a.Name == sendportName));

            tpmCtx.SaveChanges();
            tpmCtx.Dispose();
            if (agreement != null)
            {
                Console.WriteLine("Added SendPort " + sendportName + " for Agreement  name " + agreementName + " between Partner  " + firstPartnerName + " and partner " + secondPartnerName + "   in Biztalk.");

                return true;
            }
            else return false;
        }
        public bool VerifySendPortExistsInOneWayAgreementAgreement(string agreementName, string firstPartnerName, string secondPartnerName, string sendportName, SqlConnectionStringBuilder builder)
        {
            Console.WriteLine("Verifying  SendPort " + sendportName + " for Agreement  name " + agreementName + " between Partner  " + firstPartnerName + " and partner " + secondPartnerName + " exists  in Biztalk.");

            var tpmCtx = TpmContext.Create(builder);
            Agreement agreement = tpmCtx.Agreements.FirstOrDefault(a => a.Name == agreementName);

            OnewayAgreement onewayAgreementAtoB = agreement.GetOnewayAgreement(firstPartnerName, secondPartnerName);
            var sendport = onewayAgreementAtoB.SendPortReferences.FirstOrDefault(a => a.Name == sendportName);


            tpmCtx.Dispose();
            if (sendport != null)
            {
                Console.WriteLine("  SendPort " + sendportName + " for Agreement  name " + agreementName + " between Partner  " + firstPartnerName + " and partner " + secondPartnerName + " exists  in Biztalk.");
                return true;
            }
            else
            {
                Console.WriteLine("SendPort " + sendportName + " for Agreement  name " + agreementName + " between Partner  " + firstPartnerName + " and partner " + secondPartnerName + " does not exists  in Biztalk.");

                return false;
            }
        }

        public bool AddSendPortToPartner(string partnerName, string sendportName, SqlConnectionStringBuilder builder)
        {
            Console.WriteLine(" Adding  SendPort " + sendportName + " for  Partner  " + partnerName + "   in Biztalk.");

            var tpmCtx = TpmContext.Create(builder);
            var partner = tpmCtx.Partners.FirstOrDefault(a => a.Name == partnerName);
            partner.AddSendPortAssociation(SendPortReference.CreateSendPortReference(sendportName));
            tpmCtx.SaveChanges();
            tpmCtx.Dispose();
            if (partner != null)
            {
                Console.WriteLine(" Added SendPort " + sendportName + " for  Partner  " + partnerName + "   in Biztalk.");

                return true;
            }
            else return false;
        }

        public bool VerifySendPortExistsInPartner(string partnerName, string sendportName, SqlConnectionStringBuilder builder)
        {
            Console.WriteLine(" Verifying  SendPort " + sendportName + " for  Partner  " + partnerName + " exists  in Biztalk.");

            var tpmCtx = TpmContext.Create(builder);
            var partner = tpmCtx.Partners.FirstOrDefault(a => a.Name == partnerName);

            List<SendPortReference> sendportreferences = null;

            try
            {
                sendportreferences = partner.GetSendPortAssociations().ToList();
            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("exist"))
                {
                    Console.WriteLine("   SendPort " + sendportName + " for  Partner  " + partnerName + " does not exists  in Biztalk.");
                    return false;
                }
            }
            var sendport = sendportreferences.FirstOrDefault(a => a.Name == sendportName);

            tpmCtx.Dispose();
            if (sendport != null)
            {
                Console.WriteLine("SendPort " + sendportName + " for  Partner  " + partnerName + " exists  in Biztalk.");

                return true;
            }
            else
            {
                Console.WriteLine("SendPort " + sendportName + " for  Partner  " + partnerName + " does not exists  in Biztalk.");

                return false;
            }
        }

        public bool AddEnvelopesForX12OneWayAgreement(string x12AgreementName, string firstPartnerName, string secondPartnerName, string messageIdProtocolVersion, string targetNamespace, string receiverId, string senderId, string dateFormatAndTimeFormat, string responsibleAgency, string fucIdentifierCode, SqlConnectionStringBuilder builder)
        {
            Console.WriteLine("Adding Envelope " + messageIdProtocolVersion + targetNamespace + "  for  X12 Agreement Name  " + x12AgreementName + "   in Biztalk.");

            var tpmCtx = TpmContext.Create(builder);
            Agreement agreement = tpmCtx.Agreements.FirstOrDefault(a => a.Name == x12AgreementName);
            OnewayAgreement onewayAgreementAtoB = null;

            onewayAgreementAtoB = agreement.GetOnewayAgreement(firstPartnerName, secondPartnerName);

            X12ProtocolSettings x12Settings = onewayAgreementAtoB.GetProtocolSettings<X12ProtocolSettings>();
            
            string messageId = "", protocolVersion = "";
            messageId = messageIdProtocolVersion.Split(':')[0].ToString();
            protocolVersion = messageIdProtocolVersion.Split(':')[1].ToString();

            
            X12EnvelopeOverrides env = new X12EnvelopeOverrides(messageId, protocolVersion, targetNamespace);
            if (!String.IsNullOrEmpty(dateFormatAndTimeFormat))
            {
                string dateFormat = dateFormatAndTimeFormat.Split(':')[0].ToString();
                env.DateFormat = dateFormat == "CCYYMMDD" ? X12DateFormat.CCYYMMDD : X12DateFormat.YYMMDD;
                string timeFormat = dateFormatAndTimeFormat.Split(':')[1].ToString();
                env.TimeFormat = timeFormat == "HHMM" ? X12TimeFormat.HHMM : X12TimeFormat.HHMMSS;
            }
            else
            {
                env.DateFormat = X12DateFormat.CCYYMMDD;
                env.TimeFormat = X12TimeFormat.HHMM;
            }

            env.ReceiverApplicationId = receiverId;
            env.SenderApplicationId = senderId;
            if (!String.IsNullOrEmpty(responsibleAgency))
            {
                env.ResponsibleAgencyCode = responsibleAgency == "T" ? "T" : "X";
            }
            else
                env.ResponsibleAgencyCode = "X";
            env.HeaderVersion = protocolVersion + "0";
            env.FunctionalIdentifierCode = fucIdentifierCode;

            x12Settings.EnvelopeSettings.AddOverride(env);


            tpmCtx.SaveChanges();
            tpmCtx.Dispose();
            if (x12Settings != null)
            {
                Console.WriteLine("Added Envelope " + messageIdProtocolVersion + targetNamespace + "  for  X12 Agreement Name  " + x12AgreementName + "   in Biztalk.");

                return true;
            }
            else return false;
        }
        public bool VerifyEnvelopeExistsForX12OneWayAgreement(string x12AgreementName, string firstPartnerName, string secondPartnerName, string messageIdProtocolVersion, string targetNamespace, SqlConnectionStringBuilder builder)
        {
            Console.WriteLine("Verifying Envelope " + messageIdProtocolVersion + targetNamespace + "  for  X12 Agreement Name  " + x12AgreementName + "  exists  in Biztalk.");

            var tpmCtx = TpmContext.Create(builder);
            Agreement agreement = tpmCtx.Agreements.FirstOrDefault(a => a.Name == x12AgreementName);
            OnewayAgreement onewayAgreementAtoB = null;

            onewayAgreementAtoB = agreement.GetOnewayAgreement(firstPartnerName, secondPartnerName);

            X12ProtocolSettings x12Settings = onewayAgreementAtoB.GetProtocolSettings<X12ProtocolSettings>();

            string messageId = "", protocolVersion = "";
            messageId = messageIdProtocolVersion.Split(':')[0].ToString();
            protocolVersion = messageIdProtocolVersion.Split(':')[1].ToString();
            X12EnvelopeOverrides env = null;

            
            try
            {
                
                env = x12Settings.EnvelopeSettings.GetOverride(messageId, protocolVersion, targetNamespace);

            }
            catch (Exception ex)
            {


                if (ex.Message.Contains("exist"))
                {
                    Console.WriteLine(" Envelope " + messageIdProtocolVersion + targetNamespace + "  for  X12 Agreement Name  " + x12AgreementName + " does not exists  in Biztalk.");
                    return false;
                }
            }
            tpmCtx.Dispose();
            if (env != null)
            {
                Console.WriteLine(" Envelope " + messageIdProtocolVersion + targetNamespace + "  for  X12 Agreement Name  " + x12AgreementName + " exists  in Biztalk.");

                return true;
            }
            else return false;
        }

        public bool VerifyBatchExistsForX12OneWayAgreement(string x12AgreementName, string firstPartnerName, string secondPartnerName, string batchName, SqlConnectionStringBuilder builder)
        {
            Console.WriteLine("Verifying Batch" + batchName + "  for  X12 Agreement Name  " + x12AgreementName + "  exists  in Biztalk.");

            var tpmCtx = TpmContext.Create(builder);
            Agreement agreement = tpmCtx.Agreements.FirstOrDefault(a => a.Name == x12AgreementName);
            OnewayAgreement onewayAgreementAtoB = null;


            BatchDescription batchDesc = null;
            try
            {
                onewayAgreementAtoB = agreement.GetOnewayAgreement(firstPartnerName, secondPartnerName);



                

                batchDesc = onewayAgreementAtoB.GetBatch(batchName);
               

            }
            catch (Exception ex)
            {


                if (ex.Message.Contains("exist"))
                {
                    Console.WriteLine(" Batch " + batchName + "  for  X12 Agreement Name  " + x12AgreementName + " does not exists  in Biztalk.");
                    return false;
                }
            }
            tpmCtx.Dispose();
            if (batchDesc != null)
            {
                Console.WriteLine(" Batch " + batchName + "  for  X12 Agreement Name  " + x12AgreementName + " exists  in Biztalk.");

                return true;
            }
            else return false;
        }

        /*
        public bool AddBatchForX12OneWayAgreement(string x12AgreementName, string firstPartnerName, string secondPartnerName, Biztalk2013R2PartnerAgreementManager.XmlSettings.Batch batchAgreementSettings, SqlConnectionStringBuilder builder)
        {
            Console.WriteLine("Adding Batch " + batchAgreementSettings.Name + "  for  X12 Agreement Name  " + x12AgreementName + "   in Biztalk.");

            var tpmCtx = TpmContext.Create(builder);
            Agreement agreement = tpmCtx.Agreements.FirstOrDefault(a => a.Name == x12AgreementName);
            OnewayAgreement onewayAgreementAtoB = null;

            onewayAgreementAtoB = agreement.GetOnewayAgreement(firstPartnerName, secondPartnerName);
            BatchDescription batchDesc = onewayAgreementAtoB.CreateBatch(batchAgreementSettings.Name);
            
            batchDesc.Description = "Automated Creation Batch " + batchAgreementSettings.Name;
            FilterGroup grp;
            FilterPredicate filterPredicate = new FilterPredicate();
            foreach (var batch in batchAgreementSettings.FilterGroups)
            {
                //batchDesc.Protocol.s = "x12";
                
                grp = new FilterGroup();
                foreach (var filter in batch.Filters)
                {
                   
                    FilterStatement stat = new FilterStatement(filter.FilterProperty, FilterOperator.Equals, filter.FilterValue);
                   

                    grp.Statements.Add(stat);
                    
                }
                filterPredicate.Groups.Add(grp);
             
            }
            batchDesc.SetFilterPredicate(filterPredicate);
            batchDesc.StartDate = System.DateTime.Now;
            HourlyRecurrence oc = new HourlyRecurrence(1,5);
            
            TimeBasedReleaseCriteria cr=new TimeBasedReleaseCriteria(System.DateTime.Now,oc,false);
            batchDesc.SetReleaseCriteria(cr);
            tpmCtx.SaveChanges();

            using (var cmd = new SqlCommand(@" INSERT INTO [dbo].[PAM_Control]
                                                                       ([EdiMessageType]
                                                                       ,[ActionType]
                                                                       ,[ActionDateTime]
                                                                       ,[UsedOnce]
                                                                       ,[BatchId]
                                                                       ,[BatchName]
                                                                       ,[SenderPartyName], ReceiverPartyName, AgreementName)
                                                                    SELECT 0 as EDIMessageType
                                                                  ,'EdiBatchActivate' as 'ActionType'
                                                                  ,GetDate() as 'ActionDateTime'
                                                                  ,0 as 'UsedOnce' 
                                                                  ," + batchDesc.Id + @" as [BatchId]
                                                                  ,'" + batchDesc.Name + @"' as [BatchName], '" + firstPartnerName + @"', '" +
  secondPartnerName + @"', '" + x12AgreementName + "'", new SqlConnection(builder.ConnectionString)))
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
               
           
            tpmCtx.Dispose();
            if (batchDesc != null)
            {
                Console.WriteLine("Added Batch " + batchAgreementSettings.Name + "  for  X12 Agreement Name  " + x12AgreementName + "   in Biztalk.");

                return true;
            }
            else return false;
        }
        */

        public bool AddSchemaOveridesForX12OneWayAgreement(string x12AgreementName, string firstPartnerName, string secondPartnerName, string messageIdVersion, string targetNamespace, string senderId, SqlConnectionStringBuilder builder)
        {
            Console.WriteLine("Adding Schema Overide " + messageIdVersion + targetNamespace + "  for  X12 Agreement Name  " + x12AgreementName + "   in Biztalk.");

            var tpmCtx = TpmContext.Create(builder);
            Agreement agreement = tpmCtx.Agreements.FirstOrDefault(a => a.Name == x12AgreementName);
            OnewayAgreement onewayAgreementAtoB = null;

            onewayAgreementAtoB = agreement.GetOnewayAgreement(firstPartnerName, secondPartnerName);

            X12ProtocolSettings x12Settings = onewayAgreementAtoB.GetProtocolSettings<X12ProtocolSettings>();

            string messageId = "", protocolVersion = "";
            messageId = messageIdVersion.Split(':')[0].ToString();
            protocolVersion = messageIdVersion.Split(':')[1].ToString();


            X12SchemaOverrides env = new X12SchemaOverrides(messageId, senderId);
            
            if (!String.IsNullOrEmpty(targetNamespace))
            {
                env.TargetNamespace = targetNamespace;
            }
            

           
          
           

            x12Settings.SchemaSettings.AddOverride(env);


            tpmCtx.SaveChanges();
            tpmCtx.Dispose();
            if (x12Settings != null)
            {
                Console.WriteLine("Added Schema Overide " + messageIdVersion + targetNamespace + "  for  X12 Agreement Name  " + x12AgreementName + "   in Biztalk.");

                return true;
            }
            else return false;
        }
        public bool VerifySchemaOverideExistsForX12OneWayAgreement(string x12AgreementName, string firstPartnerName, string secondPartnerName, string messageIdProtocolVersion, string senderId, SqlConnectionStringBuilder builder)
        {
            Console.WriteLine("Verifying SchemaOveride " + messageIdProtocolVersion  +" sender id" +senderId+"  for  X12 Agreement Name  " + x12AgreementName + "  exists  in Biztalk.");

            var tpmCtx = TpmContext.Create(builder);
            Agreement agreement = tpmCtx.Agreements.FirstOrDefault(a => a.Name == x12AgreementName);
            OnewayAgreement onewayAgreementAtoB = null;

            onewayAgreementAtoB = agreement.GetOnewayAgreement(firstPartnerName, secondPartnerName);

            X12ProtocolSettings x12Settings = onewayAgreementAtoB.GetProtocolSettings<X12ProtocolSettings>();

            string messageId = "", protocolVersion = "";
            messageId = messageIdProtocolVersion.Split(':')[0].ToString();
            protocolVersion = messageIdProtocolVersion.Split(':')[1].ToString();
            List<X12SchemaOverrides> schemaOveridesLst = null;
            X12SchemaOverrides schemaOveride= null;


            try
            {

                schemaOveridesLst = x12Settings.SchemaSettings.GetOverrides().ToList();
                schemaOveride = schemaOveridesLst.FirstOrDefault(a => a.MessageId == messageId && a.SenderApplicationId == senderId);

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                   
                if (ex.Message.Contains("exist"))
                {
                    Console.WriteLine(" Schema Overide " + messageIdProtocolVersion  + "  for  X12 Agreement Name  " + x12AgreementName + " does not exists  in Biztalk.");
                    return false;
                }
            }
            tpmCtx.Dispose();
            if (schemaOveride != null)
            {
                Console.WriteLine(" Schema Overide " + messageIdProtocolVersion  + "  for  X12 Agreement Name  " + x12AgreementName + " exists  in Biztalk.");

                return true;
            }
            else return false;
        }

    }

    
}
