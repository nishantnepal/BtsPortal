using System;
using System.Collections.Generic;
using System.Text;

namespace BtsPortal.Entities.Bts
{
    public class BtsHost
    {
        private StringBuilder _builder;
        public BtsHost()
        {
            HostInstances = new List<BtsHostInstance>();
            _builder = new StringBuilder();
        }
        public bool IsDefault { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string NtGroupName { get; set; }
        public List<BtsHostInstance> HostInstances { get; set; }

        public string HostMetadata
        {
            get
            {
                if (_builder.Length != 0) return _builder.ToString();
                _builder.AppendLine(string.Concat("IsDefault", " : ", IsDefault));
                _builder.AppendLine(string.Concat("NtGroupName", " : ", NtGroupName));
                _builder.AppendLine(string.Concat("Type", " : ", Type));

                return _builder.ToString();
            }
        }
    }

    public class BtsHostInstance
    {
        public DateTime DateModified { get; set; }
        public string Name { get; set; }
        public Guid UniqueId { get; set; }
        public string HostName { get; set; }
        public string Server { get; set; }
        public BtsHostInstanceStatus HostInstanceStatus { get; set; }
    }

}