using System;
using System.Data.SqlClient;
using System.Linq;
using BtsPortal.Repositories.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BtsPortal.Repositories.Test
{
    [TestClass]
    public class EsbExceptionDbRepositoryIntegrationTest
    {
        [TestMethod]
        public void UpdateEsbConfiguration_Should_Handle_LongPath()
        {
            EsbExceptionDbRepository repository = new EsbExceptionDbRepository(new SqlConnection(AppSettings.EsbExceptionDbConnectionString));
            var configs = repository.GetEsbConfiguration();
            var xsltPathConfig = configs.FirstOrDefault(m => m.Name == "XsltPath");
            if (xsltPathConfig != null)
            {
                string currentPath = xsltPathConfig.Value;
                //handle long path
                string updatedPath = @"C:\temp\againtemppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppppp\somexsltname.xslt";
                repository.UpdateEsbConfiguration(xsltPathConfig.ConfigurationID, updatedPath, "test user");
                configs = repository.GetEsbConfiguration();
                xsltPathConfig = configs.FirstOrDefault(m => m.Name == "XsltPath");
                Assert.AreEqual(xsltPathConfig.Value, updatedPath);
                repository.UpdateEsbConfiguration(xsltPathConfig.ConfigurationID, currentPath, "test user");
            }
        }
    }
}
