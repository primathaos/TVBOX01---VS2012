﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.18063
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace TVBOX01.ServiceReference1 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference1.WebServiceSoap")]
    public interface WebServiceSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/HelloWorld", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string HelloWorld();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/addition", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        double addition(double i, double j);
        
        // CODEGEN: 参数“fs”需要其他方案信息，使用参数模式无法捕获这些信息。特定特性为“System.Xml.Serialization.XmlElementAttribute”。
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/UploadFile", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        TVBOX01.ServiceReference1.UploadFileResponse UploadFile(TVBOX01.ServiceReference1.UploadFileRequest request);
        
        // CODEGEN: 参数“DownloadFileResult”需要其他方案信息，使用参数模式无法捕获这些信息。特定特性为“System.Xml.Serialization.XmlElementAttribute”。
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/DownloadFile", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        TVBOX01.ServiceReference1.DownloadFileResponse DownloadFile(TVBOX01.ServiceReference1.DownloadFileRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="UploadFile", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class UploadFileRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] fs;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string path;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=2)]
        public string fileName;
        
        public UploadFileRequest() {
        }
        
        public UploadFileRequest(byte[] fs, string path, string fileName) {
            this.fs = fs;
            this.path = path;
            this.fileName = fileName;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="UploadFileResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class UploadFileResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public bool UploadFileResult;
        
        public UploadFileResponse() {
        }
        
        public UploadFileResponse(bool UploadFileResult) {
            this.UploadFileResult = UploadFileResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="DownloadFile", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class DownloadFileRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        public string strFilePath;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=1)]
        public string path;
        
        public DownloadFileRequest() {
        }
        
        public DownloadFileRequest(string strFilePath, string path) {
            this.strFilePath = strFilePath;
            this.path = path;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="DownloadFileResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class DownloadFileResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://tempuri.org/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
        public byte[] DownloadFileResult;
        
        public DownloadFileResponse() {
        }
        
        public DownloadFileResponse(byte[] DownloadFileResult) {
            this.DownloadFileResult = DownloadFileResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface WebServiceSoapChannel : TVBOX01.ServiceReference1.WebServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class WebServiceSoapClient : System.ServiceModel.ClientBase<TVBOX01.ServiceReference1.WebServiceSoap>, TVBOX01.ServiceReference1.WebServiceSoap {
        
        public WebServiceSoapClient() {
        }
        
        public WebServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public WebServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WebServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WebServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string HelloWorld() {
            return base.Channel.HelloWorld();
        }
        
        public double addition(double i, double j) {
            return base.Channel.addition(i, j);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        TVBOX01.ServiceReference1.UploadFileResponse TVBOX01.ServiceReference1.WebServiceSoap.UploadFile(TVBOX01.ServiceReference1.UploadFileRequest request) {
            return base.Channel.UploadFile(request);
        }
        
        public bool UploadFile(byte[] fs, string path, string fileName) {
            TVBOX01.ServiceReference1.UploadFileRequest inValue = new TVBOX01.ServiceReference1.UploadFileRequest();
            inValue.fs = fs;
            inValue.path = path;
            inValue.fileName = fileName;
            TVBOX01.ServiceReference1.UploadFileResponse retVal = ((TVBOX01.ServiceReference1.WebServiceSoap)(this)).UploadFile(inValue);
            return retVal.UploadFileResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        TVBOX01.ServiceReference1.DownloadFileResponse TVBOX01.ServiceReference1.WebServiceSoap.DownloadFile(TVBOX01.ServiceReference1.DownloadFileRequest request) {
            return base.Channel.DownloadFile(request);
        }
        
        public byte[] DownloadFile(string strFilePath, string path) {
            TVBOX01.ServiceReference1.DownloadFileRequest inValue = new TVBOX01.ServiceReference1.DownloadFileRequest();
            inValue.strFilePath = strFilePath;
            inValue.path = path;
            TVBOX01.ServiceReference1.DownloadFileResponse retVal = ((TVBOX01.ServiceReference1.WebServiceSoap)(this)).DownloadFile(inValue);
            return retVal.DownloadFileResult;
        }
    }
}
