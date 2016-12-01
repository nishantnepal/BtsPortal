using System;

namespace BtsPortal.Entities.Esb
{
    public class EsbConfiguration
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifieDate { get; set; }
        public Guid ConfigurationID { get; set; }

    }

    public class AlertConfiguration
    {
        public int QueueBatchSize { get; set; }
        public int EmailBatchSize { get; set; }
        public bool IsEmailEnabled { get; set; }
        public bool IsQueueEnabled { get; set; }
        public string LdapRoot { get; set; }
        public int QueueInterval { get; set; }
        public int EmailInterval { get; set; }
        public string EmailServer { get; set; }
        public string Sender { get; set; }
        public string XsltPath { get; set; }
        public int ADCacheInterval { get; set; }



    }
    /*
     
			 conf._queueBatchSize = Convert.ToInt32(GetValue("QueueBatchSize", configurations), System.Globalization.CultureInfo.InvariantCulture);
			 conf._emailBatchSize = Convert.ToInt32(GetValue("EmailBatchSize", configurations), System.Globalization.CultureInfo.InvariantCulture);
			 conf._isEmailEnabled = Convert.ToBoolean(GetValue("IsEmailEnabled", configurations), System.Globalization.CultureInfo.InvariantCulture);
			 string isQueueEnabled = GetValue("IsQueueEnabled", configurations);
			 conf._isQueueEnabled = (!string.IsNullOrEmpty(isQueueEnabled) && (isQueueEnabled == "true" || isQueueEnabled == "1")) ? true : false;
			 conf._ldapRoot = GetValue("LdapRoot", configurations);
			 conf._queueInterval = Convert.ToInt32(GetValue("QueueInterval", configurations), System.Globalization.CultureInfo.InvariantCulture);
			 conf._emailInterval = Convert.ToInt32(GetValue("EmailInterval", configurations), System.Globalization.CultureInfo.InvariantCulture);
			 conf._emailServer = GetValue("EmailServer", configurations);
			 conf._sender = GetValue("Sender", configurations);
			 conf._xsltPath = GetValue("XsltPath", configurations);
			 conf._aDCacheInterval = Convert.ToInt32(GetValue("ADCacheInterval", configurations), System.Globalization.CultureInfo.InvariantCulture);
			 conf._configurationCacheInterval = Convert.ToInt32(GetValue("ConfigurationCacheInterval", configurations), System.Globalization.CultureInfo.InvariantCulture);

			 return conf;
             */
}