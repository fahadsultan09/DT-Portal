﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PRDSaleReturnCreditNote
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.sami.com/DP_Credit_Note", ConfigurationName="PRDSaleReturnCreditNote.POserRespOut")]
    public interface POserRespOut
    {
        
        // CODEGEN: Generating message contract since the wrapper namespace (urn:sap-com:document:sap:rfc:functions) of message POserRespOutRequest does not match the default value (http://www.sami.com/DP_Credit_Note)
        [System.ServiceModel.OperationContractAttribute(Action="http://sap.com/xi/WebService/soap1.1", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        PRDSaleReturnCreditNote.POserRespOutResponse POserRespOut(PRDSaleReturnCreditNote.POserRespOutRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://sap.com/xi/WebService/soap1.1", ReplyAction="*")]
        System.Threading.Tasks.Task<PRDSaleReturnCreditNote.POserRespOutResponse> POserRespOutAsync(PRDSaleReturnCreditNote.POserRespOutRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ZSS_CREDIT_NOTE", WrapperNamespace="urn:sap-com:document:sap:rfc:functions", IsWrapped=true)]
    public partial class POserRespOutRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string P_KUNAG;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string P_VBELN;
        
        public POserRespOutRequest()
        {
        }
        
        public POserRespOutRequest(string P_KUNAG, string P_VBELN)
        {
            this.P_KUNAG = P_KUNAG;
            this.P_VBELN = P_VBELN;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ZSS_CREDIT_NOTE.Response", WrapperNamespace="urn:sap-com:document:sap:rfc:functions", IsWrapped=true)]
    public partial class POserRespOutResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LV_STRING;
        
        public POserRespOutResponse()
        {
        }
        
        public POserRespOutResponse(string LV_STRING)
        {
            this.LV_STRING = LV_STRING;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public interface POserRespOutChannel : PRDSaleReturnCreditNote.POserRespOut, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public partial class POserRespOutClient : System.ServiceModel.ClientBase<PRDSaleReturnCreditNote.POserRespOut>, PRDSaleReturnCreditNote.POserRespOut
    {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public POserRespOutClient(EndpointConfiguration endpointConfiguration) : 
                base(POserRespOutClient.GetBindingForEndpoint(endpointConfiguration), POserRespOutClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public POserRespOutClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(POserRespOutClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public POserRespOutClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(POserRespOutClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public POserRespOutClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PRDSaleReturnCreditNote.POserRespOutResponse PRDSaleReturnCreditNote.POserRespOut.POserRespOut(PRDSaleReturnCreditNote.POserRespOutRequest request)
        {
            return base.Channel.POserRespOut(request);
        }
        
        public string POserRespOut(string P_KUNAG, string P_VBELN)
        {
            PRDSaleReturnCreditNote.POserRespOutRequest inValue = new PRDSaleReturnCreditNote.POserRespOutRequest();
            inValue.P_KUNAG = P_KUNAG;
            inValue.P_VBELN = P_VBELN;
            PRDSaleReturnCreditNote.POserRespOutResponse retVal = ((PRDSaleReturnCreditNote.POserRespOut)(this)).POserRespOut(inValue);
            return retVal.LV_STRING;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<PRDSaleReturnCreditNote.POserRespOutResponse> PRDSaleReturnCreditNote.POserRespOut.POserRespOutAsync(PRDSaleReturnCreditNote.POserRespOutRequest request)
        {
            return base.Channel.POserRespOutAsync(request);
        }
        
        public System.Threading.Tasks.Task<PRDSaleReturnCreditNote.POserRespOutResponse> POserRespOutAsync(string P_KUNAG, string P_VBELN)
        {
            PRDSaleReturnCreditNote.POserRespOutRequest inValue = new PRDSaleReturnCreditNote.POserRespOutRequest();
            inValue.P_KUNAG = P_KUNAG;
            inValue.P_VBELN = P_VBELN;
            return ((PRDSaleReturnCreditNote.POserRespOut)(this)).POserRespOutAsync(inValue);
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
                        "rty=&senderService=NSAP_PRD&receiverParty=&receiverService=&interface=POserRespO" +
                        "ut&interfaceNamespace=http%3A%2F%2Fwww.sami.com%2FDP_Credit_Note");
            }
            if ((endpointConfiguration == EndpointConfiguration.HTTPS_Port))
            {
                return new System.ServiceModel.EndpointAddress("https://s049sappoprd.samipharma.com.pk:51101/XISOAPAdapter/MessageServlet?senderP" +
                        "arty=&senderService=NSAP_PRD&receiverParty=&receiverService=&interface=POserResp" +
                        "Out&interfaceNamespace=http%3A%2F%2Fwww.sami.com%2FDP_Credit_Note");
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
