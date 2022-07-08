using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using VCLWebAPI.Models;
using VCLWebAPI.Models.AWS;

namespace VCLWebAPI.Services
{
    public class DesignerService : IDisposable
    {
        ////private static readonly string DesignerFolderBasePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Content\\Models\\Designer";
        //private static readonly string DesignerFolderBasePath = @"Models\Designer\";//https://s3.amazonaws.com/vcl-design-com

        //private static readonly string[] ValidModelExtensions = { "obj", "mtl", ".obj", ".mtl" };
        //private static readonly string[] ValidModelDataExtensions = { "js", ".js" };
        //private static readonly string[] ValidSkysphereExtensions = { "jpg", ".jpg" };
        //private static readonly string[] ValidTextureThermalExtensions = { "jpg", "jpeg", "png", ".jpg", ".jpeg", ".png" };

        //private static AWSService _awsService = null;

        //public DesignerService()
        //{
        //    _awsService = new AWSService();
        //}

        //public string GetModelDirectoryListing()
        //{
        //    string thermalPath = DesignerFolderBasePath + "thermals";
        //    string skyspherePath = DesignerFolderBasePath + "skysphere";

        //    ModelDirectoryListing modelDirectoryListing = new ModelDirectoryListing
        //    {
        //        Skysphere = GetDirectoryListings(skyspherePath, ValidSkysphereExtensions),
        //        Thermals = GetDirectoryListings(thermalPath, ValidTextureThermalExtensions),
        //        Textures = GetDirectoryListings(DesignerFolderBasePath, ValidTextureThermalExtensions),
        //        Model = GetDirectoryListings(DesignerFolderBasePath, ValidModelExtensions),
        //        ModelData = GetDirectoryListings(DesignerFolderBasePath, ValidModelDataExtensions)
        //    };

        //    return JsonConvert.SerializeObject(modelDirectoryListing);
        //}

        //private List<string> GetDirectoryListings(string path, string[] validExtensions = null)
        //{
        //    List<string> filesFound = new List<string>();
        //    DirectoryInfo directoryInfo = new DirectoryInfo(path);
        //    string extension = string.Empty;
        //    var directoryListings = new List<string>();

        //    foreach (S3DirectoryListing file in _awsService.GetDirectoryListings(path))
        //    {
        //        if (validExtensions != null)
        //        {
        //            extension = file.Extension;
        //            if (validExtensions.Contains(extension.ToLower()))
        //            {
        //                filesFound.Add(file.Name);
        //            }
        //        }
        //        else
        //        {
        //            filesFound.Add(file.Name);
        //        }
        //    }

        //    return filesFound;
        //}

        //public string UploadModelOrData(IEnumerable<HttpPostedFileBase> files)
        //{
        //    JsonResponse response = new JsonResponse
        //    {
        //        Success = false,
        //        Message = ""
        //    };

        //    string extension = string.Empty;
        //    int count = 0;
        //    List<string> filesFailed = new List<string>();
        //    List<string> filesSucceeded = new List<string>();
        //    List<string> filesProcessed = new List<string>();

        //    if (files != null)
        //    {
        //        foreach (HttpPostedFileBase file in files)
        //        {
        //            extension = Path.GetExtension(file.FileName);
        //            if (ValidModelExtensions.Contains(extension.ToLower()))
        //            {
        //                Upload(file, GetBareDesignerBasePath());
        //                response.Success = true;
        //                filesSucceeded.Add(file.FileName);
        //            }
        //            else if (ValidModelDataExtensions.Contains(extension.ToLower()))
        //            {
        //                Upload(file, GetBareDesignerBasePath(), "ModelData.js");
        //                response.Success = true;
        //                filesSucceeded.Add(file.FileName);
        //            }
        //            else
        //            {
        //                filesFailed.Add(file.FileName);
        //            }
        //            filesProcessed.Add(file.FileName);
        //            count++;
        //        }

        //        if (response.Success)
        //        {
        //            response.Message = Convert.ToString(count) + " out of " + Convert.ToString(files.Count()) + " files successfully uploaded.";
        //        }
        //        else
        //        {
        //            response.Message = "Uploaded files are the incorrect file type. Please upload .mtl/.obj for Models and .js for Model Data.";
        //        }
        //        response.Data = new
        //        {
        //            FilesFailed = filesFailed,
        //            FilesSucceeded = filesSucceeded,
        //            FilesProcessed = filesProcessed
        //        };
        //    }
        //    else
        //    {
        //        response.Message = "No files uploaded";
        //    }

        //    return JsonConvert.SerializeObject(response);
        //}

        //public string UploadTextures(IEnumerable<HttpPostedFileBase> files)
        //{
        //    JsonResponse response = new JsonResponse
        //    {
        //        Success = false,
        //        Message = ""
        //    };

        //    if (files != null)
        //    {
        //        if (DeleteValidImagesFromPath(DesignerFolderBasePath))
        //        {
        //            var path = GetBareDesignerBasePath();
        //            ValidateImagesAndUpload(files, path, ref response);
        //        }
        //        else
        //        {
        //            response.Message = "Unable to delete previous textures.";
        //        }
        //    }
        //    else
        //    {
        //        response.Message = "No files uploaded";
        //    }

        //    return JsonConvert.SerializeObject(response);
        //}

        //public string UploadThermals(IEnumerable<HttpPostedFileBase> files)
        //{
        //    JsonResponse response = new JsonResponse
        //    {
        //        Success = false,
        //        Message = ""
        //    };

        //    string path = Path.Combine(DesignerFolderBasePath, "thermals");

        //    if (files != null)
        //    {
        //        if (DeleteValidImagesFromPath(path))
        //        {
        //            ValidateImagesAndUpload(files, path, ref response);
        //        }
        //        else
        //        {
        //            response.Message = "Unable to delete previous thermals.";
        //        }
        //    }
        //    else
        //    {
        //        response.Message = "No files uploaded";
        //    }

        //    return JsonConvert.SerializeObject(response);
        //}

        //public string UploadSkysphere(IEnumerable<HttpPostedFileBase> files)
        //{
        //    JsonResponse response = new JsonResponse
        //    {
        //        Success = false,
        //        Message = ""
        //    };

        //    string path = DesignerFolderBasePath + "skysphere";
        //    string extension = string.Empty;
        //    HttpPostedFileBase file = null;
        //    if (files != null)
        //    {
        //        file = files.ToList().First();
        //        extension = Path.GetExtension(file.FileName);
        //        ValidateImagesAndUpload(files, path, ref response, "skysphere" + extension);
        //    }
        //    else
        //    {
        //        response.Message = "No files uploaded";
        //    }

        //    return JsonConvert.SerializeObject(response);
        //}

        //private bool DeleteValidImagesFromPath(string directoryPath)
        //{
        //    string extension = string.Empty;
        //    foreach (S3DirectoryListing file in _awsService.GetDirectoryListings(directoryPath))
        //    {
        //        extension = file.Extension;
        //        if (ValidImageType(extension))
        //        {
        //            _awsService.DeleteFile(file.Key);
        //        }
        //    }

        //    return true;
        //}

        //private void ValidateImagesAndUpload(IEnumerable<HttpPostedFileBase> files, string path, ref JsonResponse response, string fileName = null)
        //{
        //    string extension = string.Empty;
        //    List<string> filesFailed = new List<string>();
        //    List<string> filesSucceeded = new List<string>();
        //    List<string> filesProcessed = new List<string>();
        //    int count = 0;
        //    foreach (HttpPostedFileBase file in files)
        //    {
        //        extension = Path.GetExtension(file.FileName);
        //        if (ValidImageType(extension))
        //        {
        //            Upload(file, path, fileName);
        //            response.Success = true;
        //            filesSucceeded.Add(file.FileName);
        //        }
        //        else
        //        {
        //            filesFailed.Add(file.FileName);
        //        }

        //        filesProcessed.Add(file.FileName);
        //        count++;
        //    }

        //    if (response.Success)
        //    {
        //        response.Message = Convert.ToString(count) + " out of " + Convert.ToString(files.Count()) + " files successfully uploaded.";
        //    }
        //    else
        //    {
        //        response.Message = "Uploaded files are the incorrect file type. Please upload .png, .jpg or .jpeg textures, thermals or skyspheres.";
        //    }

        //    response.Data = new
        //    {
        //        FilesFailed = filesFailed,
        //        FilesSucceeded = filesSucceeded,
        //        FilesProcessed = filesProcessed
        //    };
        //}

        //private bool ValidImageType(string extension)
        //{
        //    return ValidTextureThermalExtensions.Contains(extension.ToLower());
        //}

        //private void Upload(HttpPostedFileBase file, string path, string fileName = null)
        //{
        //    if (file?.ContentLength > 0)
        //    {
        //        string name = fileName ?? file.FileName;
        //        string filePath = path + @"\" + name;// Path.Combine((path), Path.GetFileName(name));
        //        filePath = filePath.Replace("\\", "/");

        //        _awsService.UploadFile(file, filePath);
        //    }
        //}

        //public string GetBareDesignerBasePath()
        //{
        //    var Find = "\\";
        //    int place = DesignerFolderBasePath.LastIndexOf(Find);

        //    if (place == -1)
        //        return DesignerFolderBasePath;

        //    string path = DesignerFolderBasePath.Remove(place, Find.Length).Insert(place, "");

        //    return path;
        //}

        public void Dispose()
        {
            //_awsService.Dispose();
        }
    }
}