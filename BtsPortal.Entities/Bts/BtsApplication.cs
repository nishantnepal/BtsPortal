using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BtsPortal.Entities.Bts
{
    public class BtsApplication
    {
        public BtsApplication()
        {
            ReceivePorts = new List<BtsReceivePort>();
            SendPorts = new List<BtsSendPort>();
            Orchestrations = new List<BtsOrchestration>();
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MessageBoxModuleId { get; set; }
        public List<BtsReceivePort> ReceivePorts { get; set; }
        public List<BtsSendPort> SendPorts { get; set; }
        public List<BtsOrchestration> Orchestrations { get; set; }

    }

    public class BtsReceivePort
    {
        public BtsReceivePort()
        {
            ReceiveLocations = new List<BtsReceiveLocation>();
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool TwoWay { get; set; }
        public bool RouteFailedMessages { get; set; }
        public List<BtsReceiveLocation> ReceiveLocations { get; set; }
    }

    public class BtsSendPort
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool TwoWay { get; set; }
        public bool RouteFailedMessages { get; set; }
        public bool Dynamic { get; set; }
        public bool OrderedDelivery { get; set; }
        public string Filter { get; set; }

        public string FilterTable
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Filter))
                {
                    return String.Empty;
                }

                StringBuilder builder = new StringBuilder();
                try
                {
                    XmlDocument dom = new XmlDocument();
                    
                    builder.Append("<table>");
                    dom.LoadXml(Filter);
                    foreach (XmlNode node in dom.SelectNodes("//Group"))
                    {
                        foreach (XmlNode statement in node.SelectNodes("Statement"))
                        {
                            string ops = statement.Attributes["Operator"].Value;
                            if (ops == "6")
                            {
                                ops = "Exists";
                            }
                            else if (ops == "0")
                            {
                                ops = "Equals";
                            }

                            string value=string.Empty;
                            if (statement.Attributes["Value"] != null)
                            {
                                value = statement.Attributes["Value"].Value;
                            }

                            builder.Append("<tr>").Append("<td>").Append(string.Format(" <span class='span-width-50'>{0}</span>", statement.Attributes["Property"].Value)).Append("</td>")
                                .Append("<td>&nbsp;<td>")
                                .Append("<td>").Append(ops).Append("</td>")
                                .Append("<td>&nbsp;<td>")
                                .Append("<td>").Append(string.Format(" <span class='span-width-50'>{0}</span>", value)).Append("</td>").Append("</tr>");
                            builder.AppendLine("<tr>").Append("<td>&nbsp;</td><td>&nbsp;</td>").AppendLine("</tr>");
                            
                        }
                    }
                    builder.Append("<table>");
                }
                catch (Exception exception)
                {
                    builder.Append("Error - " + exception.ToString());
                }
                

                
                return builder.ToString();
            }
        }

        public IList InboundTransforms { get; set; }
        public BtsArtifactStatus Status { get; set; }
        public string Adapter { get; set; }
        public string HostName { get; set; }
        public string Address { get; set; }
        public string Tracking { get; set; }
    }

    public class BtsReceiveLocation
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Primary { get; set; }
        public string Adapter { get; set; }
        public string HostName { get; set; }
        public string Address { get; set; }
        public bool ServiceWindowEnabled { get; set; }
        public bool StartEndDateEnabled { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public BtsArtifactStatus Status { get; set; }
       // public string Tracking { get; set; }
    }

    public class BtsOrchestration
    {
        public BtsOrchestration()
        {
            Ports = new List<BtsOrchestrationPort>();
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string HostName { get; set; }
        public string Tracking { get; set; }
        public BtsArtifactStatus Status { get; set; }
        public List<BtsOrchestrationPort> Ports { get; set; }
    }

    public class BtsOrchestrationPort
    {
        public BtsPortType PortType { get; set; }
        public string OrchestrationPortName { get; set; }
        public string PhysicalPortName { get; set; }
        public string Binding { get; set; }
    }
}



