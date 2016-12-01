using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using BtsPortal.Entities.Bts;
using BtsPortal.Repositories.Interface;
using BtsPortal.Repositories.Utilities;
using Microsoft.BizTalk.ExplorerOM;
using Microsoft.BizTalk.Operations;
using BtsOrchestration = BtsPortal.Entities.Bts.BtsOrchestration;
using BtsPortal.Entities;
using Microsoft.BizTalk.Message.Interop;

namespace BtsPortal.Repositories.Db
{
    public class BizTalkRepository : IBizTalkRepository
    {
        readonly BtsCatalogExplorer _catalog;
        private readonly BizTalkOperations _operations;
        private readonly IDbConnection _dbConnection;

        public BizTalkRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
            _catalog = new BtsCatalogExplorer { ConnectionString = dbConnection.ConnectionString };
            //todo : maybe take in server etc?
            _operations = new BizTalkOperations();
        }

        public virtual List<BtsApplication> GetBiztalkApplications()
        {
            List<BtsApplication> applications = new List<BtsApplication>();
            foreach (Application app in _catalog.Applications)
            {
                applications.Add(new BtsApplication()
                {
                    Name = app.Name,
                    Description = app.Description
                });
            }

            return applications.OrderBy(m => m.Name).ToList();
        }


        public BtsInstanceDetail GetInstanceDetail(Guid instanceId, int maxBtsMessagesToShow = 50)
        {
            MessageBoxServiceInstance mbsi = _operations.GetServiceInstance(instanceId);
            BtsInstanceDetail detail = new BtsInstanceDetail
            {
                Status = mbsi.InstanceStatus.ToString(),
                ErrorCode = mbsi.ErrorCode,
                ErrorDescription = mbsi.ErrorDescription,
                Server = !string.IsNullOrWhiteSpace(mbsi.CurrentProcessingServer) ? mbsi.CurrentProcessingServer : mbsi.ErrorProcessingServer,
                InstanceId = instanceId.ToString(),
                PendingOperation = mbsi.PendingOperation.ToString(),
                ServiceClass = mbsi.Class.ToString(),
                ServiceName = mbsi.ServiceType.ToString(),
                CreateDateTime = mbsi.CreationTime.ToLocalTime(),
                SuspendDateTime = mbsi.SuspendTime,
                TotalMessagesCount = mbsi.Messages.Cast<BizTalkMessage>().Count(),
                Application = mbsi.Application,
                HostName = mbsi.HostName
            };

            int counter = 0;
            foreach (BizTalkMessage msg in mbsi.Messages)
            {
                counter++;
                if (counter > maxBtsMessagesToShow)
                {
                    break;
                }

                BizTalkMessage message = _operations.GetMessage(msg.MessageID, msg.InstanceID);

                BtsInstanceMessage btsMsg = new BtsInstanceMessage
                {
                    Adapter = message.AdapterName,
                    MessageId = message.MessageID.ToString(),
                    MessageType = message.MessageType,
                    Uri = message.Url,
                    MessageStatus = message.MessageStatus.ToString(),
                    DateCreated = message.CreationTime,
                    Name = message.BodyPartName

                };

                detail.Messages.Add(btsMsg);

            }

            return detail;
        }

        public BtsInstanceMessage GetInstanceMessage(Guid instanceId, Guid messageId, int maxDataLength = 40000)
        {
            BizTalkMessage message = _operations.GetMessage(messageId, instanceId);
            BtsInstanceMessage btsMsg = new BtsInstanceMessage();
            if (message.BodyPart != null)
            {
                StringBuilder sb = new StringBuilder();
                using (StreamReader streamReader = new StreamReader(message.BodyPart.Data))
                {
                    sb.Append(streamReader.ReadToEnd());
                }

                btsMsg = new BtsInstanceMessage
                {
                    MessageBodyLength = sb.Length,
                    Adapter = message.AdapterName,
                    MessageId = message.MessageID.ToString(),
                    MessageType = message.MessageType,
                    Uri = message.Url,
                    MessageStatus = message.MessageStatus.ToString(),
                    DateCreated = message.CreationTime,
                    MessageBody =
                        sb.Length > maxDataLength
                            ? sb.ToString().Substring(0, maxDataLength)
                            : sb.ToString(),
                    MessageBodyTruncated = sb.Length > maxDataLength
                };

                IBaseMessageContext ctx = message.Context;
                for (int loop = 0; loop < ctx.CountProperties; loop++)
                {
                    string name;
                    string nspace;
                    ctx.ReadAt(loop, out name, out nspace);
                    string value = ctx.Read(name, nspace).ToString();
                    bool isPromoted = ctx.IsPromoted(name, nspace);
                    btsMsg.Contexts.Add(new BtsInstanceMessageContext()
                    {
                        Value = value,
                        Name = name,
                        Namespace = nspace,
                        Promoted = isPromoted
                    });
                }


            }

            return btsMsg;

        }

        public string GetInstanceMessageBody(Guid instanceId, Guid messageId)
        {
            BizTalkMessage message = _operations.GetMessage(messageId, instanceId);
            StringBuilder sb = new StringBuilder();

            if (message.BodyPart != null)
            {
                using (StreamReader streamReader = new StreamReader(message.BodyPart.Data))
                {
                    sb.Append(streamReader.ReadToEnd());
                }

            }
            else
            {
                sb.Append("No message body in message.");
            }
            return sb.ToString();

        }

        public List<BtsHost> GetHosts()
        {
            List<BtsHost> hosts = new List<BtsHost>();
            foreach (Host host in (ReadOnlyCollectionBase)_catalog.Hosts)
            {
                hosts.Add(new BtsHost()
                {
                    Name = host.Name,
                    NtGroupName = host.NTGroupName,
                    IsDefault = host.IsDefault,
                    Type = host.Type.ToString()
                });
            }

            return hosts;
        }

        public void StartHost(string hostName)
        {
            EnumerationOptions enumOptions = new EnumerationOptions { ReturnImmediately = false };
            var searchObject = string.IsNullOrWhiteSpace(hostName)
                ? new ManagementObjectSearcher("root\\MicrosoftBizTalkServer",
                    "Select * from MSBTS_HostInstance where HostType=1", enumOptions)
                : new ManagementObjectSearcher("root\\MicrosoftBizTalkServer",
                    string.Format("Select * from MSBTS_HostInstance where HostType=1 and HostName = '{0}'", hostName),
                    enumOptions);

            //Enumerate through the result set and start each HostInstance if it is already stopped
            foreach (ManagementObject inst in searchObject.Get())
            {
                //Check if ServiceState is 'Stopped'
                if (inst["ServiceState"].ToString() == "1")
                {
                    inst.InvokeMethod("Start", null);
                }
            }
        }

        public void StopHost(string hostName)
        {
            //https://msdn.microsoft.com/en-us/library/ee268298(v=bts.10).aspx
            EnumerationOptions enumOptions = new EnumerationOptions { ReturnImmediately = false };
            var searchObject = string.IsNullOrWhiteSpace(hostName)
                ? new ManagementObjectSearcher("root\\MicrosoftBizTalkServer",
                    "Select * from MSBTS_HostInstance where HostType=1", enumOptions)
                : new ManagementObjectSearcher("root\\MicrosoftBizTalkServer",
                    string.Format("Select * from MSBTS_HostInstance where HostType=1 and HostName = '{0}'", hostName),
                    enumOptions);

            //Enumerate through the result set and start each HostInstance if it is already stopped
            foreach (ManagementObject inst in searchObject.Get())
            {
                //Check if ServiceState is 'Stopped'
                if (inst["ServiceState"].ToString() == "4")
                {
                    inst.InvokeMethod("Stop", null);
                }
            }
        }
        public BtsApplication GetBiztalkApplicationStatus(string applicationName)
        {
            BtsApplication application = new BtsApplication();

            foreach (Application app in _catalog.Applications)
            {
                if (string.Equals(app.Name, applicationName, StringComparison.OrdinalIgnoreCase))
                {
                    application.Name = app.Name;
                    application.Description = app.Description;
                    foreach (ReceivePort port in app.ReceivePorts)
                    {
                        var btsRecPort = new BtsReceivePort
                        {
                            Name = port.Name,
                            Description = port.Description,
                            TwoWay = port.IsTwoWay,
                            RouteFailedMessages = port.RouteFailedMessage
                        };

                        foreach (ReceiveLocation location in port.ReceiveLocations)
                        {
                            BtsReceiveLocation btsRecLoc = new BtsReceiveLocation()
                            {
                                Name = location.Name,
                                Primary = location.IsPrimary,
                                Status = location.Enable ? BtsArtifactStatus.Started : BtsArtifactStatus.Stopped,
                                Adapter = location.ReceiveHandler?.TransportType?.Name,
                                HostName = location.ReceiveHandler?.Host?.Name,
                                Address = location.Address,
                                ServiceWindowEnabled = location.ServiceWindowEnabled,
                                StartEndDateEnabled = location.StartDateEnabled || location.EndDateEnabled,
                                StartDate = location.StartDate,
                                EndDate = location.EndDate,
                                FromTime = location.FromTime,
                                ToTime = location.ToTime
                            };

                            btsRecPort.ReceiveLocations.Add(btsRecLoc);
                        }
                        application.ReceivePorts.Add(btsRecPort);
                    }

                    foreach (SendPort port in app.SendPorts)
                    {
                        var item = new BtsSendPort()
                        {
                            Name = port.Name,
                            Description = port.Description,
                            TwoWay = port.IsTwoWay,
                            Dynamic = port.IsDynamic,
                            OrderedDelivery = port.OrderedDelivery,
                            RouteFailedMessages = port.RouteFailedMessage,
                            Filter = port.Filter,
                            InboundTransforms = port.InboundTransforms,
                            Status = port.Status.ToBtsArtifactStatus(),
                            Adapter = port.PrimaryTransport?.SendHandler?.TransportType?.Name,
                            HostName = port.PrimaryTransport?.SendHandler?.Host?.Name,
                            Address = port.PrimaryTransport?.Address,
                            Tracking = port.Tracking.ToString(),
                        };


                        application.SendPorts.Add(item);
                    }

                    foreach (Microsoft.BizTalk.ExplorerOM.BtsOrchestration orchestration in app.Orchestrations)
                    {
                        var item = new BtsOrchestration()
                        {
                            Name = orchestration.FullName,
                            Description = orchestration.Description,
                            HostName = orchestration.Host?.Name,
                            Status = orchestration.Status.ToBtsArtifactStatus(),
                            Tracking = orchestration.Tracking.ToString()
                        };


                        foreach (OrchestrationPort port in orchestration.Ports)
                        {
                            string phyPortName = null;
                            BtsPortType portType;
                            if (port.SendPort != null || port.SendPortGroup != null)
                            {
                                phyPortName = port.SendPort?.Name;
                                portType = BtsPortType.SendPort;
                            }

                            else if (port.ReceivePort != null)
                            {
                                phyPortName = port.ReceivePort?.Name;
                                portType = BtsPortType.ReceivePort;
                            }
                            else
                            {
                                portType = BtsPortType.Dynamic;
                            }

                            item.Ports.Add(new BtsOrchestrationPort()
                            {
                                PortType = portType,
                                OrchestrationPortName = port.Name,
                                PhysicalPortName = phyPortName,
                                Binding = port.Binding.ToString()
                            });
                        }

                        application.Orchestrations.Add(item);
                    }
                }


            }

            return application;
        }

        public List<string> OrchestrationOperation(string applicationName, string orchestrationName, BtsArtifactStatus status)
        {
            List<string> affectedList = new List<string>();
            foreach (Application app in _catalog.Applications)
            {
                if (string.Equals(app.Name, applicationName, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (Microsoft.BizTalk.ExplorerOM.BtsOrchestration orchestration in app.Orchestrations)
                    {
                        if (!string.IsNullOrWhiteSpace(orchestrationName))
                        {
                            if (string.Equals(orchestration.FullName, orchestrationName, StringComparison.OrdinalIgnoreCase)
                                && orchestration.Status.ToBtsArtifactStatus() != status)
                            {
                                orchestration.Status = status.ToOrchestrationStatus();
                                affectedList.Add(orchestration.FullName);
                                _catalog.SaveChanges();
                                return affectedList;
                            }
                        }
                        else
                        {
                            if (orchestration.Status.ToBtsArtifactStatus() != status)
                            {
                                orchestration.Status = status.ToOrchestrationStatus();
                                affectedList.Add(orchestration.FullName);
                            }
                        }
                    }

                    _catalog.SaveChanges();
                }
            }
            return affectedList;
        }

        public List<string> SendPortOperation(string applicationName, string portName, BtsArtifactStatus status)
        {
            List<string> affectedList = new List<string>();
            foreach (Application app in _catalog.Applications)
            {
                if (string.Equals(app.Name, applicationName, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (SendPort port in app.SendPorts)
                    {
                        if (!string.IsNullOrWhiteSpace(portName))
                        {
                            if (string.Equals(port.Name, portName, StringComparison.OrdinalIgnoreCase)
                                && port.Status.ToBtsArtifactStatus() != status)
                            {
                                port.Status = status.ToBtsArtifactStatus();
                                affectedList.Add(port.Name);
                                _catalog.SaveChanges();
                                return affectedList;
                            }
                        }
                        else
                        {
                            if (port.Status.ToBtsArtifactStatus() != status)
                            {
                                port.Status = status.ToBtsArtifactStatus();
                                affectedList.Add(port.Name);
                            }
                        }
                    }

                    _catalog.SaveChanges();
                }
            }
            return affectedList;
        }

        public List<string> ReceiveLocationOperation(string applicationName, string portName, string locationName, bool enable)
        {
            List<string> affectedList = new List<string>();
            foreach (Application app in _catalog.Applications)
            {
                if (string.Equals(app.Name, applicationName, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (Microsoft.BizTalk.ExplorerOM.ReceivePort port in app.ReceivePorts)
                    {
                        if (!string.IsNullOrWhiteSpace(portName))
                        {
                            if (string.Equals(port.Name, portName, StringComparison.OrdinalIgnoreCase))
                            {
                                foreach (ReceiveLocation location in port.ReceiveLocations)
                                {
                                    if (!string.IsNullOrWhiteSpace(locationName))
                                    {
                                        if (string.Equals(location.Name, locationName,
                                            StringComparison.OrdinalIgnoreCase)
                                            && location.Enable != enable)
                                        {
                                            location.Enable = enable;
                                            affectedList.Add(location.Name);
                                            _catalog.SaveChanges();
                                            return affectedList;
                                        }
                                    }
                                    else
                                    {
                                        if (location.Enable != enable)
                                        {
                                            location.Enable = enable;
                                            affectedList.Add(location.Name);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (ReceiveLocation location in port.ReceiveLocations)
                            {
                                if (location.Enable != enable)
                                {
                                    location.Enable = enable;
                                    affectedList.Add(location.Name);
                                }
                            }
                        }
                    }

                    _catalog.SaveChanges();
                }
            }
            return affectedList;
        }

        public string TerminateInstance(Guid instanceId)
        {
            //todo : maybe take in server etc?
            //cannot use the local instance of operations because of bug
            //https://blogs.msdn.microsoft.com/biztalknotes/2015/05/19/biztalk-2013-r2-issues-while-terminating-and-resuming-instance-using-c-code/
            using (BizTalkOperations operations = new BizTalkOperations())
            {
                var status = operations.TerminateInstance(instanceId);
                return status.ToString();
            }

        }

    }
}