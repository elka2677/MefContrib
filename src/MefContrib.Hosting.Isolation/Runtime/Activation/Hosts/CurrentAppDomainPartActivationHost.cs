using System;
using System.ServiceModel;

namespace MefContrib.Hosting.Isolation.Runtime.Activation.Hosts
{
    public class CurrentAppDomainPartActivationHost : PartActivationHostBase
    {
        private readonly ServiceHost _serviceHost;

        public CurrentAppDomainPartActivationHost(ActivationHostDescription description) 
            : base(description)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            _serviceHost = RemotingServices.CreateServiceHost(Address);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!Faulted)
            {
                Faulted = true;
                PartHost.OnFailure(this);    
            }
        }
        
        public override void Start()
        {
            if (_serviceHost.State != CommunicationState.Created)
            {
                throw new InvalidOperationException();
            }

            _serviceHost.Open();
        }

        public override void Stop()
        {
            _serviceHost.Close();
        }
    }
}