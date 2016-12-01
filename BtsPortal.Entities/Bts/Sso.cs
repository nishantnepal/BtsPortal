using System.Collections.Generic;

namespace BtsPortal.Entities.Bts
{
    public class SsoApplication
    {
        public string Name { get; set; }
        public string ContactInfo { get; set; }
    }

    public class SsoApplicationDataVm
    {
        public SsoApplicationDataVm()
        {
            ApplicationDatas = new List<SsoApplicationData>();
        }
        public string ApplicationName { get; set; }
        public bool IsBtdf { get; set; }
        public List<SsoApplicationData> ApplicationDatas { get; set; }
    }

    public class SsoApplicationData
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}