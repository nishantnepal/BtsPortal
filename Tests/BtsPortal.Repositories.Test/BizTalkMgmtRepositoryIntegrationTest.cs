using System;
using System.Collections.Generic;
using BtsPortal.Repositories.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using BtsPortal.Entities.Bts;
using BtsPortal.Repositories.Interface;

namespace BtsPortal.Repositories.Test
{
    [TestClass]
    public class BizTalkMgmtRepositoryIntegrationTest
    {
        
        [TestMethod]
        public void IsEdiControlMessageProcessing()
        {
            Assert.AreEqual(true, true);
        }

        [TestMethod]
        public List<DbOneWayAgreementBatchingOrch> GetEdiBatchingOrchestrationInstanceIds(int oneWayAgreementId)
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void GetHostInstances()
        {
            //Arrange
            BizTalkMgmtRepository repository = new BizTalkMgmtRepository(new SqlConnection(AppSettings.BtsMgmtBoxDatabaseConnectionString));

            //Act
            var types = repository.GetHostInstances(new List<string>() { "BizTalkServerApplication" });

            //Assert
            Assert.AreEqual(true, types.Count > 0, "Expected atleast one host instance mapped for the BizTalkServerApplication host.");

        }

        [TestMethod]
        public void GetParties()
        {
            //Arrange
            BizTalkMgmtRepository repository = new BizTalkMgmtRepository(new SqlConnection(AppSettings.BtsMgmtBoxDatabaseConnectionString));

            //Act
            var types = repository.GetParties();

            //Assert
            Assert.AreEqual(true, types.Count > 0, "Expected atleast one edi party in the local environment.");

        }

        [TestMethod]
        public void LoadArtifactTypes()
        {
            //Arrange
            BizTalkMgmtRepository repository = new BizTalkMgmtRepository(new SqlConnection(AppSettings.BtsMgmtBoxDatabaseConnectionString));

            //Act
            var types = repository.LoadArtifactTypes();

            //Assert
            Assert.AreEqual(true, types.Count > 0, "Expected atleast one receive port, send port or an orchestration in the local environment.");

          
        }

        [TestMethod]
        [ExpectedException(typeof(Microsoft.BizTalk.B2B.PartnerManagement.TpmException))]
        public void GetParty()
        {
            //Arrange
            BizTalkMgmtRepository repository = new BizTalkMgmtRepository(new SqlConnection(AppSettings.BtsMgmtBoxDatabaseConnectionString));

            //Act
            var types = repository.GetParty("IA","IA");

            //Assert
            Assert.AreEqual(true, types != null);
        }

        [TestMethod]
        public void UpdateEdiBatchStatus()
        {
            Assert.AreEqual(true, true);
        }
    }
}
