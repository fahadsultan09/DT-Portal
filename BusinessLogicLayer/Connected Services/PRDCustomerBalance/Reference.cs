//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PRDCustomerBalance
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.sami.com/DP_Cust_Bal", ConfigurationName="PRDCustomerBalance.CustPORespOut")]
    public interface CustPORespOut
    {
        
        // CODEGEN: Generating message contract since the operation CustPORespOut is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="http://sap.com/xi/WebService/soap1.1", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        PRDCustomerBalance.CustPORespOutResponse CustPORespOut(PRDCustomerBalance.CustPORespOutRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://sap.com/xi/WebService/soap1.1", ReplyAction="*")]
        System.Threading.Tasks.Task<PRDCustomerBalance.CustPORespOutResponse> CustPORespOutAsync(PRDCustomerBalance.CustPORespOutRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.sami.com/DP_Cust_Bal")]
    public partial class Cust_resp_out
    {
        
        private string s_KUNNR_LOWField;
        
        private string s_KUNNR_HIGHField;
        
        private string p_BUKRSField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string S_KUNNR_LOW
        {
            get
            {
                return this.s_KUNNR_LOWField;
            }
            set
            {
                this.s_KUNNR_LOWField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string S_KUNNR_HIGH
        {
            get
            {
                return this.s_KUNNR_HIGHField;
            }
            set
            {
                this.s_KUNNR_HIGHField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string P_BUKRS
        {
            get
            {
                return this.p_BUKRSField;
            }
            set
            {
                this.p_BUKRSField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZST_BALANCE
    {
        
        private string bUKRField;
        
        private string kUNNRField;
        
        private string nAME1Field;
        
        private string oRT01Field;
        
        private string cURRENCYField;
        
        private decimal bALANCEField;
        
        private bool bALANCEFieldSpecified;
        
        private string iNDIField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string BUKR
        {
            get
            {
                return this.bUKRField;
            }
            set
            {
                this.bUKRField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string KUNNR
        {
            get
            {
                return this.kUNNRField;
            }
            set
            {
                this.kUNNRField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string NAME1
        {
            get
            {
                return this.nAME1Field;
            }
            set
            {
                this.nAME1Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string ORT01
        {
            get
            {
                return this.oRT01Field;
            }
            set
            {
                this.oRT01Field = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public string CURRENCY
        {
            get
            {
                return this.cURRENCYField;
            }
            set
            {
                this.cURRENCYField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public decimal BALANCE
        {
            get
            {
                return this.bALANCEField;
            }
            set
            {
                this.bALANCEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BALANCESpecified
        {
            get
            {
                return this.bALANCEFieldSpecified;
            }
            set
            {
                this.bALANCEFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=6)]
        public string INDI
        {
            get
            {
                return this.iNDIField;
            }
            set
            {
                this.iNDIField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZSS_CUST_BALResponse
    {
        
        private ZST_BALANCE[] fINAL_TABField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public ZST_BALANCE[] FINAL_TAB
        {
            get
            {
                return this.fINAL_TABField;
            }
            set
            {
                this.fINAL_TABField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class CustPORespOutRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.sami.com/DP_Cust_Bal", Order=0)]
        public PRDCustomerBalance.Cust_resp_out Cust_resp_out;
        
        public CustPORespOutRequest()
        {
        }
        
        public CustPORespOutRequest(PRDCustomerBalance.Cust_resp_out Cust_resp_out)
        {
            this.Cust_resp_out = Cust_resp_out;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class CustPORespOutResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="ZSS_CUST_BAL.Response", Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute("ZSS_CUST_BAL.Response")]
        public PRDCustomerBalance.ZSS_CUST_BALResponse ZSS_CUST_BALResponse;
        
        public CustPORespOutResponse()
        {
        }
        
        public CustPORespOutResponse(PRDCustomerBalance.ZSS_CUST_BALResponse ZSS_CUST_BALResponse)
        {
            this.ZSS_CUST_BALResponse = ZSS_CUST_BALResponse;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public interface CustPORespOutChannel : PRDCustomerBalance.CustPORespOut, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.2")]
    public partial class CustPORespOutClient : System.ServiceModel.ClientBase<PRDCustomerBalance.CustPORespOut>, PRDCustomerBalance.CustPORespOut
    {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public CustPORespOutClient(EndpointConfiguration endpointConfiguration) : 
                base(CustPORespOutClient.GetBindingForEndpoint(endpointConfiguration), CustPORespOutClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public CustPORespOutClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(CustPORespOutClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public CustPORespOutClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(CustPORespOutClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public CustPORespOutClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PRDCustomerBalance.CustPORespOutResponse PRDCustomerBalance.CustPORespOut.CustPORespOut(PRDCustomerBalance.CustPORespOutRequest request)
        {
            return base.Channel.CustPORespOut(request);
        }
        
        public PRDCustomerBalance.ZSS_CUST_BALResponse CustPORespOut(PRDCustomerBalance.Cust_resp_out Cust_resp_out)
        {
            PRDCustomerBalance.CustPORespOutRequest inValue = new PRDCustomerBalance.CustPORespOutRequest();
            inValue.Cust_resp_out = Cust_resp_out;
            PRDCustomerBalance.CustPORespOutResponse retVal = ((PRDCustomerBalance.CustPORespOut)(this)).CustPORespOut(inValue);
            return retVal.ZSS_CUST_BALResponse;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<PRDCustomerBalance.CustPORespOutResponse> PRDCustomerBalance.CustPORespOut.CustPORespOutAsync(PRDCustomerBalance.CustPORespOutRequest request)
        {
            return base.Channel.CustPORespOutAsync(request);
        }
        
        public System.Threading.Tasks.Task<PRDCustomerBalance.CustPORespOutResponse> CustPORespOutAsync(PRDCustomerBalance.Cust_resp_out Cust_resp_out)
        {
            PRDCustomerBalance.CustPORespOutRequest inValue = new PRDCustomerBalance.CustPORespOutRequest();
            inValue.Cust_resp_out = Cust_resp_out;
            return ((PRDCustomerBalance.CustPORespOut)(this)).CustPORespOutAsync(inValue);
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
                        "rty=&senderService=NSAP_PRD&receiverParty=&receiverService=&interface=CustPOResp" +
                        "Out&interfaceNamespace=http%3A%2F%2Fwww.sami.com%2FDP_Cust_Bal");
            }
            if ((endpointConfiguration == EndpointConfiguration.HTTPS_Port))
            {
                return new System.ServiceModel.EndpointAddress("https://s049sappoprd.samipharma.com.pk:51101/XISOAPAdapter/MessageServlet?senderP" +
                        "arty=&senderService=NSAP_PRD&receiverParty=&receiverService=&interface=CustPORes" +
                        "pOut&interfaceNamespace=http%3A%2F%2Fwww.sami.com%2FDP_Cust_Bal");
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
