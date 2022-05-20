using BpsUnifiedModelLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VCLWebAPI.Models.QueuingServer
{
    public class SqsMessageBody
    {
        public string Type { get; set; }
        public string MessageId { get; set; }
        public string TopicArn { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string SignatureVersion { get; set; }
        public string Signature { get; set; }
        public string SigningCertURL { get; set; }
        public string UnsubscribeURL { get; set; }

        public SqsMessageBodyMessage GetParsedBodyMessage()
        {
            return JsonConvert.DeserializeObject<SqsMessageBodyMessage>(Message);
        }
    }

    public class RhinoQueuingMessage
    {
        public SqsMessageBodyMessage SqsMessageBodyMessage { get; set; }
        public string unifiedModel { get; set; }
    }

    public class SqsMessageBodyMessage
    {
        public List<Record> Records { get; set; }
        public string ReceiptHandle { get; set; }
    }

    public class Record
    {
        public string eventVersion { get; set; }
        public string eventSource { get; set; }
        public string awsRegion { get; set; }
        public DateTime eventTime { get; set; }
        public string eventName { get; set; }
        public Useridentity userIdentity { get; set; }
        public Requestparameters requestParameters { get; set; }
        public Responseelements responseElements { get; set; }
        public S3 s3 { get; set; }
    }

    public class Useridentity
    {
        public string principalId { get; set; }
    }

    public class Requestparameters
    {
        public string sourceIPAddress { get; set; }
    }

    public class Responseelements
    {
        public string xamzrequestid { get; set; }
        public string xamzid2 { get; set; }
    }

    public class S3
    {
        public string s3SchemaVersion { get; set; }
        public string configurationId { get; set; }
        public Bucket bucket { get; set; }
        public SqsFileObject @object { get; set; }
    }

    public class Bucket
    {
        public string name { get; set; }
        public Owneridentity ownerIdentity { get; set; }
        public string arn { get; set; }
    }

    public class Owneridentity
    {
        public string principalId { get; set; }
    }

    public class SqsFileObject
    {
        public string key { get; set; }
        public int size { get; set; }
        public string eTag { get; set; }
        public string sequencer { get; set; }
    }

}