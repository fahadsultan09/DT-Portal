﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PRDCustomerLedger
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.sami.com/FormPDF", ConfigurationName="PRDCustomerLedger.ServerPORespOut")]
    public interface ServerPORespOut
    {
        
        // CODEGEN: Generating message contract since the wrapper namespace (urn:sap-com:document:sap:rfc:functions) of message ServerPORespOutRequest does not match the default value (http://www.sami.com/FormPDF)
        [System.ServiceModel.OperationContractAttribute(Action="http://sap.com/xi/WebService/soap1.1", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        PRDCustomerLedger.ServerPORespOutResponse ServerPORespOut(PRDCustomerLedger.ServerPORespOutRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://sap.com/xi/WebService/soap1.1", ReplyAction="*")]
        System.Threading.Tasks.Task<PRDCustomerLedger.ServerPORespOutResponse> ServerPORespOutAsync(PRDCustomerLedger.ServerPORespOutRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ZSS_CUST_LEDGER_BAPI", WrapperNamespace="urn:sap-com:document:sap:rfc:functions", IsWrapped=true)]
    public partial class ServerPORespOutRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string BUDAT_HIGH;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string BUDAT_LOW;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=2)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string BUKRS;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=3)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string KUNNR_LOW;
        
        public ServerPORespOutRequest()
        {
        }
        
        public ServerPORespOutRequest(string BUDAT_HIGH, string BUDAT_LOW, string BUKRS, string KUNNR_LOW)
        {
            this.BUDAT_HIGH = BUDAT_HIGH;
            this.BUDAT_LOW = BUDAT_LOW;
            this.BUKRS = BUKRS;
            this.KUNNR_LOW = KUNNR_LOW;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ZSS_CUST_LEDGER_BAPI.Response", WrapperNamespace="urn:sap-com:document:sap:rfc:functions", IsWrapped=true)]
    public partial class ServerPORespOutResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LV_STRING;
        
        public ServerPORespOutResponse()
        {
        }
        
        public ServerPORespOutResponse(string LV_STRING)
        {
            this.LV_STRING = LV_STRING;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public interface ServerPORespOutChannel : PRDCustomerLedger.ServerPORespOut, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public partial class ServerPORespOutClient : System.ServiceModel.ClientBase<PRDCustomerLedger.ServerPORespOut>, PRDCustomerLedger.ServerPORespOut
    {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public ServerPORespOutClient(EndpointConfiguration endpointConfiguration) : 
                base(ServerPORespOutClient.GetBindingForEndpoint(endpointConfiguration), ServerPORespOutClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public ServerPORespOutClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(ServerPORespOutClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public ServerPORespOutClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(ServerPORespOutClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public ServerPORespOutClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PRDCustomerLedger.ServerPORespOutResponse PRDCustomerLedger.ServerPORespOut.ServerPORespOut(PRDCustomerLedger.ServerPORespOutRequest request)
        {
            return base.Channel.ServerPORespOut(request);
        }
        
        public string ServerPORespOut(string BUDAT_HIGH, string BUDAT_LOW, string BUKRS, string KUNNR_LOW)
        {
            PRDCustomerLedger.ServerPORespOutRequest inValue = new PRDCustomerLedger.ServerPORespOutRequest();
            inValue.BUDAT_HIGH = BUDAT_HIGH;
            inValue.BUDAT_LOW = BUDAT_LOW;
            inValue.BUKRS = BUKRS;
            inValue.KUNNR_LOW = KUNNR_LOW;
            PRDCustomerLedger.ServerPORespOutResponse retVal = ((PRDCustomerLedger.ServerPORespOut)(this)).ServerPORespOut(inValue);
            return retVal.LV_STRING;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<PRDCustomerLedger.ServerPORespOutResponse> PRDCustomerLedger.ServerPORespOut.ServerPORespOutAsync(PRDCustomerLedger.ServerPORespOutRequest request)
        {
            return base.Channel.ServerPORespOutAsync(request);
        }
        
        public System.Threading.Tasks.Task<PRDCustomerLedger.ServerPORespOutResponse> ServerPORespOutAsync(string BUDAT_HIGH, string BUDAT_LOW, string BUKRS, string KUNNR_LOW)
        {
            PRDCustomerLedger.ServerPORespOutRequest inValue = new PRDCustomerLedger.ServerPORespOutRequest();
            inValue.BUDAT_HIGH = BUDAT_HIGH;
            inValue.BUDAT_LOW = BUDAT_LOW;
            inValue.BUKRS = BUKRS;
            inValue.KUNNR_LOW = KUNNR_LOW;
            return ((PRDCustomerLedger.ServerPORespOut)(this)).ServerPORespOutAsync(inValue);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.HTTP_Port))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            if ((endpointConfiguration == EndpointConfiguration.HTTPS_Port))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                result.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.HTTP_Port))
            {
                return new System.ServiceModel.EndpointAddress("http://s049sappoprd.samipharma.com.pk:51100/XISOAPAdapter/MessageServlet?senderPa" +
                        "rty=&senderService=NSAP_PRD&receiverParty=&receiverService=&interface=ServerPORe" +
                        "spOut&interfaceNamespace=http%3A%2F%2Fwww.sami.com%2FFormPDF");
            }
            if ((endpointConfiguration == EndpointConfiguration.HTTPS_Port))
            {
                return new System.ServiceModel.EndpointAddress("https://s049sappoprd.samipharma.com.pk:51101/XISOAPAdapter/MessageServlet?senderP" +
                        "arty=&senderService=NSAP_PRD&receiverParty=&receiverService=&interface=ServerPOR" +
                        "espOut&interfaceNamespace=http%3A%2F%2Fwww.sami.com%2FFormPDF");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        public enum EndpointConfiguration
        {
            
            HTTP_Port,
            
            HTTPS_Port,
        }
    }
}
