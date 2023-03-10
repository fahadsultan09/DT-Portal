//------------------------------------------------------------------------------
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
    [System.ServiceModel.ServiceContractAttribute(Namespace="https://www.sami.com/Testing", ConfigurationName="PRDSaleReturnCreditNote.CN_SerRespOut")]
    public interface CN_SerRespOut
    {
        
        // CODEGEN: Generating message contract since the wrapper namespace (urn:sap-com:document:sap:rfc:functions) of message CN_SerRespOutRequest does not match the default value (https://www.sami.com/Testing)
        [System.ServiceModel.OperationContractAttribute(Action="http://sap.com/xi/WebService/soap1.1", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        PRDSaleReturnCreditNote.CN_SerRespOutResponse CN_SerRespOut(PRDSaleReturnCreditNote.CN_SerRespOutRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://sap.com/xi/WebService/soap1.1", ReplyAction="*")]
        System.Threading.Tasks.Task<PRDSaleReturnCreditNote.CN_SerRespOutResponse> CN_SerRespOutAsync(PRDSaleReturnCreditNote.CN_SerRespOutRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ZDP_CREDITNOTE_FM", WrapperNamespace="urn:sap-com:document:sap:rfc:functions", IsWrapped=true)]
    public partial class CN_SerRespOutRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string P_KUNAG;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string P_VBELN;
        
        public CN_SerRespOutRequest()
        {
        }
        
        public CN_SerRespOutRequest(string P_KUNAG, string P_VBELN)
        {
            this.P_KUNAG = P_KUNAG;
            this.P_VBELN = P_VBELN;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="ZDP_CREDITNOTE_FM.Response", WrapperNamespace="urn:sap-com:document:sap:rfc:functions", IsWrapped=true)]
    public partial class CN_SerRespOutResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string LV_STRING;
        
        public CN_SerRespOutResponse()
        {
        }
        
        public CN_SerRespOutResponse(string LV_STRING)
        {
            this.LV_STRING = LV_STRING;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public interface CN_SerRespOutChannel : PRDSaleReturnCreditNote.CN_SerRespOut, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public partial class CN_SerRespOutClient : System.ServiceModel.ClientBase<PRDSaleReturnCreditNote.CN_SerRespOut>, PRDSaleReturnCreditNote.CN_SerRespOut
    {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public CN_SerRespOutClient(EndpointConfiguration endpointConfiguration) : 
                base(CN_SerRespOutClient.GetBindingForEndpoint(endpointConfiguration), CN_SerRespOutClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public CN_SerRespOutClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(CN_SerRespOutClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public CN_SerRespOutClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(CN_SerRespOutClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public CN_SerRespOutClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PRDSaleReturnCreditNote.CN_SerRespOutResponse PRDSaleReturnCreditNote.CN_SerRespOut.CN_SerRespOut(PRDSaleReturnCreditNote.CN_SerRespOutRequest request)
        {
            return base.Channel.CN_SerRespOut(request);
        }
        
        public string CN_SerRespOut(string P_KUNAG, string P_VBELN)
        {
            PRDSaleReturnCreditNote.CN_SerRespOutRequest inValue = new PRDSaleReturnCreditNote.CN_SerRespOutRequest();
            inValue.P_KUNAG = P_KUNAG;
            inValue.P_VBELN = P_VBELN;
            PRDSaleReturnCreditNote.CN_SerRespOutResponse retVal = ((PRDSaleReturnCreditNote.CN_SerRespOut)(this)).CN_SerRespOut(inValue);
            return retVal.LV_STRING;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<PRDSaleReturnCreditNote.CN_SerRespOutResponse> PRDSaleReturnCreditNote.CN_SerRespOut.CN_SerRespOutAsync(PRDSaleReturnCreditNote.CN_SerRespOutRequest request)
        {
            return base.Channel.CN_SerRespOutAsync(request);
        }
        
        public System.Threading.Tasks.Task<PRDSaleReturnCreditNote.CN_SerRespOutResponse> CN_SerRespOutAsync(string P_KUNAG, string P_VBELN)
        {
            PRDSaleReturnCreditNote.CN_SerRespOutRequest inValue = new PRDSaleReturnCreditNote.CN_SerRespOutRequest();
            inValue.P_KUNAG = P_KUNAG;
            inValue.P_VBELN = P_VBELN;
            return ((PRDSaleReturnCreditNote.CN_SerRespOut)(this)).CN_SerRespOutAsync(inValue);
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
                        "rty=&senderService=NSAP_PRD&receiverParty=&receiverService=&interface=CN_SerResp" +
                        "Out&interfaceNamespace=https%3A%2F%2Fwww.sami.com%2FTesting");
            }
            if ((endpointConfiguration == EndpointConfiguration.HTTPS_Port))
            {
                return new System.ServiceModel.EndpointAddress("https://s049sappoprd.samipharma.com.pk:51101/XISOAPAdapter/MessageServlet?senderP" +
                        "arty=&senderService=NSAP_PRD&receiverParty=&receiverService=&interface=CN_SerRes" +
                        "pOut&interfaceNamespace=https%3A%2F%2Fwww.sami.com%2FTesting");
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
