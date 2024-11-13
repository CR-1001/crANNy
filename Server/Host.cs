/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Threading;
    using CRAI.Common;
    using System.Text.RegularExpressions;
    using System.Collections.ObjectModel;
    using System.ServiceModel.Channels;

    [Serializable]
    public class Host : NotifyBase<Host>
    {
        public enum Protocols
        {
            HttpWebService,
            HttpsWebService,
            TcpBinary,
        }

        public enum States
        {
            None,

            Starting,
            Started,

            Closing,
            Closed,

            Failure,
        }

        private Object _Lock = new Object();

        #region public States State
        private States _State = States.None;
        public States State
        {
            get { return _State; }
            set
            {
                if (_State == value) return;
                _State = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(this, h => h.CanStart);
                NotifyPropertyChanged(this, h => h.CanStop);
            }
        }
        #endregion public States State

        #region public Service Service
        private Service _Service;
        public Service Service
        {
            get { return _Service; }
            set
            {
                if (_Service == value) return;
                _Service = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public Service Service

        #region public ServiceHost ServiceHost
        private ServiceHost _ServiceHost;
        public ServiceHost ServiceHost
        {
            get { return _ServiceHost; }
            private set
            {
                if (_ServiceHost == value) return;
                _ServiceHost = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ServiceHost ServiceHost

        #region public Int16 Port
        private Int16 _Port = 9020;
        public Int16 Port
        {
            get { return _Port; }
            set
            {
                if (_Port == value) return;
                _Port = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(this, h => h.UriBaseAddress);
                NotifyPropertyChanged(this, h => h.IsValidConfiguration);
                NotifyPropertyChanged(this, h => h.CanStart);
            }
        }
        #endregion public Int16 Port

        #region public int MaxMessageSize
        private int _MaxMessageSize = 2100000000;
        public int MaxMessageSize
        {
            get { return _MaxMessageSize; }
            set
            {
                if (_MaxMessageSize == value) return;
                _MaxMessageSize = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public int MaxMessageSize

        #region public TimeSpan TimeSpanTimeout
        private TimeSpan _TimeSpanTimeout = TimeSpan.FromMinutes(10);
        public TimeSpan TimeSpanTimeout
        {
            get { return _TimeSpanTimeout; }
            set
            {
                if (_TimeSpanTimeout == value) return;
                _TimeSpanTimeout = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public TimeSpan TimeSpanTimeout
        
        #region public Protocols Protocol
        private Protocols _Protocol = Protocols.TcpBinary;
        public Protocols Protocol
        {
            get { return _Protocol; }
            set
            {
                if (_Protocol == value) return;
                _Protocol = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(this, h => h.UriBaseAddress);
                NotifyPropertyChanged(this, h => h.IsValidConfiguration);
                NotifyPropertyChanged(this, h => h.CanStart);
            }
        }
        #endregion public Protocols Protocol

        #region public String Endpoint
        private String _Endpoint = "crai";
        public String Endpoint
        {
            get { return _Endpoint; }
            set
            {
                if (_Endpoint == value) return;
                _Endpoint = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(this, h => h.UriBaseAddress);
                NotifyPropertyChanged(this, h => h.IsValidConfiguration);
                NotifyPropertyChanged(this, h => h.CanStart);
            }
        }
        #endregion public String Endpoint

        #region public String Base
        private String _Base = "localhost";
        public String Base
        {
            get { return _Base; }
            set
            {
                if (_Base == value) return;
                _Base = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(this, h => h.UriBaseAddress);
                NotifyPropertyChanged(this, h => h.IsValidConfiguration);
                NotifyPropertyChanged(this, h => h.CanStart);
            }
        }
        #endregion public String Base

        #region public Uri UriBaseAddress
        public Uri UriBaseAddress
        {
            get
            {
                var protocol
                    = Protocol == Protocols.TcpBinary
                    ? "net.tcp"
                    : Protocol == Protocols.HttpWebService
                    ? "http"
                    : "https";

                var uri = new Uri(String.Format(
                    "{0}://{1}:{2}/{3}", protocol, Base, Port, Endpoint));

                return uri;
            }
        }
        #endregion public Uri UriBaseAddress

        #region public ObservableCollection<Exception> Exceptions
        private ObservableCollection<Exception> _Exceptions
            = new ObservableCollection<Exception>();
        public ObservableCollection<Exception> Exceptions
        {
            get { return _Exceptions; }
            set
            {
                if (_Exceptions == value) return;
                _Exceptions = value;
                NotifyPropertyChanged();
            }
        }
        #endregion public ObservableCollection<Exception> Exceptions

        #region public bool IsValidConfiguration
        public bool IsValidConfiguration
        {
            get
            {
                return Port > 0
                    && !String.IsNullOrWhiteSpace(Endpoint)
                    && Regex.IsMatch(Endpoint, "^[a-zA-Z0-9]+$")
                    && !String.IsNullOrWhiteSpace(Base)
                    && Regex.IsMatch(Base, "^[a-zA-Z0-9]+$");
            }
        }
        #endregion public bool IsValidConfiguration

        #region public bool CanStart
        public bool CanStart
        {
            get
            {
                return
                    (State == States.None
                        || State == States.Closed
                        || State == States.Failure)
                    && IsValidConfiguration;
            }
        }
        #endregion public bool CanStart

        #region public bool CanStop
        public bool CanStop
        {
            get
            {
                return State == States.Started;
            }
        }
        #endregion public bool CanStop

        public void Stop()
        {
            lock (_Lock)
            {
                if (!CanStop) return;

                try
                {
                    State = States.Closing;
                    ServiceHost.Close();
                    State = States.Closed;
                }
                catch (Exception e)
                {
                    Exceptions.Add(e);
                    State = States.Failure;
                }
            }
        }


        public void Start()
        {
            lock (_Lock)
            {
                if (!CanStart) return;

                try
                {
                    State = States.Starting;

                    Service = new Service();

                    Service.StateDetailsFormatter = new StateDetailsFormatter();

                    ServiceHost = new ServiceHost(Service, UriBaseAddress);

                    // TODO: ServiceMetadataBehavior only with 
                    // Protocols.HttpsWebService and Protocols.HttpWebService?

                    var serviceMetadataBehavior = new ServiceMetadataBehavior();

                    if (Protocol == Protocols.HttpWebService)
                    {
                        serviceMetadataBehavior.HttpGetEnabled = true;
                    }
                    else if (Protocol == Protocols.HttpsWebService)
                    {
                        serviceMetadataBehavior.HttpsGetEnabled = true;
                    }

                    serviceMetadataBehavior.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;

                    ServiceHost.Description.Behaviors.Add(serviceMetadataBehavior);

                    Binding binding = null;

                    if (Protocol == Protocols.TcpBinary)
                    {
                        binding = new NetTcpBinding(SecurityMode.Message)
                        {
                            MaxReceivedMessageSize = MaxMessageSize,
                            MaxBufferPoolSize = MaxMessageSize,
                            MaxBufferSize = MaxMessageSize,
                            ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
                            {
                                MaxArrayLength = MaxMessageSize,
                            }
                        };
                    }
                    else if (Protocol == Protocols.HttpsWebService)
                    {
                        binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport)
                        {
                            MaxReceivedMessageSize = MaxMessageSize,
                            MaxBufferPoolSize = MaxMessageSize,
                            MaxBufferSize = MaxMessageSize,
                            ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
                            {
                                MaxArrayLength = MaxMessageSize,
                            }
                        };
                    }
                    else if (Protocol == Protocols.HttpWebService)
                    {
                        binding = new BasicHttpBinding(BasicHttpSecurityMode.None)
                        {
                            MaxReceivedMessageSize = MaxMessageSize,
                            MaxBufferPoolSize = MaxMessageSize,
                            MaxBufferSize = MaxMessageSize,
                            ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas()
                            {
                                MaxArrayLength = MaxMessageSize,
                            }
                        };
                    }

                    binding.CloseTimeout = TimeSpanTimeout;
                    binding.SendTimeout = TimeSpanTimeout;
                    binding.ReceiveTimeout = TimeSpanTimeout;

                    var contractDescription = ContractDescription.GetContract(typeof(IService));

                    var serviceEndpoint
                        = new ServiceEndpoint(
                            contractDescription,
                            binding,
                            new EndpointAddress(UriBaseAddress));

                    ServiceHost.AddServiceEndpoint(serviceEndpoint);

                    ServiceHost.Open();

                    State = States.Started;
                }
                catch (Exception e)
                {
                    Exceptions.Add(e);
                    State = States.Failure;
                }

            }
        }
    }
}
