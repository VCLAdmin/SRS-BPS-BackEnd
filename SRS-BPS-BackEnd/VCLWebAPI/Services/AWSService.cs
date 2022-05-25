using Amazon;
using Amazon.S3;
//using Amazon.S3.IO;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
//using System.Web;
using VCLWebAPI.Models.AWS;

namespace VCLWebAPI.Services
{
    public class AWSService
    {
        //private static readonly string _awsAccessKey = " AKIAIMZ7C62JIVKC6DXQ";
        //private static readonly string _awsSecretKey = "UZ9XSKc0/LcFGjjJTNDUH+sI9mZoljr";
        //private static readonly string _bucketName = "vcl-design-com";
        //private static readonly RegionEndpoint _awsEndpoint = RegionEndpoint.USEast1;
        //private static readonly string DELIMITER = "/";
        //private static IAmazonS3 s3Client;

        //public AWSService()
        //{
        //    s3Client = new AmazonS3Client(_awsEndpoint);
        //}

        //public GetObjectResponse GetFile(string key)
        //{
        //    GetObjectRequest request = new GetObjectRequest
        //    {
        //        BucketName = _bucketName,
        //        Key = key
        //    };

        //    GetObjectResponse response = s3Client.GetObject(request);

        //    return response;
        //}

        //public string GeneratePreSignedURL(string key, int expires = -1)
        //{
        //    string urlString = "";

        //    expires = expires > 0 ? expires : 5;

        //    try
        //    {
        //        GetPreSignedUrlRequest request1 = new GetPreSignedUrlRequest
        //        {
        //            BucketName = _bucketName,
        //            Key = key,
        //            Expires = DateTime.Now.AddMinutes(expires)
        //        };
        //        urlString = s3Client.GetPreSignedURL(request1);
        //    }
        //    catch (AmazonS3Exception e)
        //    {
        //        Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
        //    }
        //    return urlString;
        //}

        //public void DeleteFile(string key)
        //{
        //    try
        //    {
        //        var deleteObjectRequest = new DeleteObjectRequest
        //        {
        //            BucketName = _bucketName,
        //            Key = key
        //        };

        //        Console.WriteLine("Deleting an object");
        //        s3Client.DeleteObject(deleteObjectRequest);
        //    }
        //    catch (AmazonS3Exception e)
        //    {
        //        Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
        //    }
        //}

        //public void UploadFile(HttpPostedFileBase file, string key)
        //{
        //    var transferUtility = new TransferUtility(s3Client);

        //    try
        //    {
        //        var request = new TransferUtilityUploadRequest
        //        {
        //            BucketName = _bucketName,
        //            CannedACL = S3CannedACL.PublicRead, //PERMISSION TO FILE PUBLIC ACCESIBLE
        //            Key = key,//"Models/Designer/skysphere/" + file.FileName
        //            InputStream = file.InputStream
        //        };
        //        transferUtility.Upload(request);
        //    }
        //    catch (AmazonS3Exception amazonS3Exception)
        //    {
        //        if (amazonS3Exception.ErrorCode != null &&
        //            (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
        //            ||
        //            amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
        //        {
        //            throw new Exception("Check the provided AWS Credentials.");
        //        }
        //        else
        //        {
        //            throw new Exception("Error occurred: " + amazonS3Exception.Message);
        //        }
        //    }
        //}

        //public List<S3DirectoryListing> GetDirectoryListings(string key)
        //{
        //    var files = new List<S3DirectoryListing>();
        //    var s3key = String.Empty;
        //    S3DirectoryListing directoryListing = null;
        //    S3DirectoryInfo dir = new S3DirectoryInfo(s3Client, _bucketName, key);//"Models/Designer"
        //    foreach (IS3FileSystemInfo file in dir.GetFileSystemInfos())
        //    {
        //        directoryListing = new S3DirectoryListing();
        //        directoryListing.Name = file.Name;
        //        directoryListing.Extension = file.Extension;
        //        directoryListing.Key = file.FullName.Replace(_bucketName + ":\\", "").Replace("\\", "/");
        //        files.Add(directoryListing);
        //    }

        //    return files;
        //}

        //public string GetPresignedUrlFolder()//string folderName
        //{
        //    DateTime expiration = DateTime.Now.AddMinutes(90);
        //    string urlString = "";
        //    try
        //    {
        //        GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
        //        {
        //            BucketName = _bucketName,
        //            Key = "Models/PassiveTower/Model.obj",
        //            Expires = DateTime.Now.AddMinutes(90)
        //        };
        //        urlString = s3Client.GetPreSignedURL(request);
        //    }
        //    catch (AmazonS3Exception e)
        //    {
        //        Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
        //    }
        //    return urlString;
        //}

        //public void Dispose()
        //{
        //    s3Client.Dispose();
        //}
    }
}