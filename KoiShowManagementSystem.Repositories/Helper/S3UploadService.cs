﻿using Microsoft.Extensions.Configuration;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.IO;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KoiShowManagementSystem.Repositories.Helper
{
    public class S3UploadService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        public S3UploadService(IConfiguration configuration)
        {
            var awsAccessKey = configuration["AWS:AccessKey"];
            var awsSecretKey = configuration["AWS:SecretKey"];
            var awsRegion = configuration["AWS:Region"];
            _bucketName = configuration["AWS:BucketName"]!;
            if(Environment.GetEnvironmentVariable("AWS_AccessKey") != null)
            {
                awsAccessKey = Environment.GetEnvironmentVariable("AWS_AccessKey");
            }
            if (Environment.GetEnvironmentVariable("AWS_SecretKey") != null)
            {
                awsSecretKey = Environment.GetEnvironmentVariable("AWS_SecretKey");
            }
            if (Environment.GetEnvironmentVariable("AWS_Region") != null)
            {
                awsRegion = Environment.GetEnvironmentVariable("AWS_Region");
            }
            if (Environment.GetEnvironmentVariable("AWS_BucketName") != null)
            {
                _bucketName = Environment.GetEnvironmentVariable("AWS_BucketName")!;
            }
            _s3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, RegionEndpoint.GetBySystemName(awsRegion));
        }


        private async Task<string> UploadFileAsync(Stream fileStream, string keyName, string contentType)
        {
            try
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = keyName,
                    InputStream = fileStream,
                    ContentType = contentType,
                    AutoCloseStream = true
                    //CannedACL = S3CannedACL.PublicRead 
                };

                var response = await _s3Client.PutObjectAsync(putRequest);

                return $"https://{_bucketName}.s3.amazonaws.com/{keyName}";
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message: '{0}'", e.Message);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message: '{0}'", e.Message);
                throw;
            }
        }


        private async Task<bool> DoesObjectExistAsync(string keyName)
        {
            try
            {
                var response = await _s3Client.GetObjectMetadataAsync(_bucketName, keyName);
                return response != null; // If we got a response, the object exists
            }
            catch (AmazonS3Exception e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false; // The object does not exist
            }
            catch (Exception)
            {
                // Handle other exceptions as needed
                throw;
            }
        }



        public async Task<string> UploadRegistrationImage(IFormFile koiImage)
        {

            if (koiImage == null || koiImage.Length == 0)
            {
                throw new ArgumentException("The provided banner image is invalid.");
            }

            var ImageName = Path.GetFileName(koiImage.FileName);
            var contentType = koiImage.ContentType;

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(contentType))
            {
                throw new NotSupportedException("Unsupported file type.");
            };


            string uniqueFileName;
            string keyName;

            do
            {
                uniqueFileName = $"{Guid.NewGuid()}_{ImageName}";
                keyName = $"koi-registrations/{uniqueFileName}";
            } while (await DoesObjectExistAsync(keyName));


            var ImageStream = koiImage.OpenReadStream();
            return await UploadFileAsync(ImageStream, keyName, contentType);

        }

        public async Task<string> UploadKoiImage(IFormFile koiImage)
        {

            if (koiImage == null || koiImage.Length == 0)
            {
                throw new ArgumentException("The provided banner image is invalid.");
            }

            var ImageName = Path.GetFileName(koiImage.FileName);
            var contentType = koiImage.ContentType;

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(contentType))
            {
                throw new NotSupportedException("Unsupported file type.");
            };


            string uniqueFileName;
            string keyName;

            do
            {
                uniqueFileName = $"{Guid.NewGuid()}_{ImageName}";
                keyName = $"koi/{uniqueFileName}";
            } while (await DoesObjectExistAsync(keyName));


            var ImageStream = koiImage.OpenReadStream();
            return await UploadFileAsync(ImageStream, keyName, contentType);

        }

        public async Task<string> UploadShowBannerImage(IFormFile bannerImage)
        {

            if (bannerImage == null || bannerImage.Length == 0)
            {
                throw new ArgumentException("The provided banner image is invalid.");
            }

            var bannerImageName = Path.GetFileName(bannerImage.FileName);
            var contentType = bannerImage.ContentType;

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(contentType))
            {
                throw new NotSupportedException("Unsupported file type.");
            };


            string uniqueFileName;
            string keyName;

            do
            {
                uniqueFileName = $"{Guid.NewGuid()}_{bannerImageName}";
                keyName = $"show-banners/{uniqueFileName}";
            } while (await DoesObjectExistAsync(keyName));


            var bannerImageStream = bannerImage.OpenReadStream();
            return await UploadFileAsync(bannerImageStream, keyName, contentType);

        }




        // implement method with parameter a url to a image in S3 bucket, 1 IFormFile image, overwrite the image in S3 bucket with the new image
        //public async Task<string> UpdateImageAsync(string key, IFormFile newImage)
        //{
        //    if (newImage == null || newImage.Length == 0)
        //    {
        //        throw new ArgumentException("The provided image is invalid.");
        //    }

        //    var ImageName = Path.GetFileName(newImage.FileName);
        //    var contentType = newImage.ContentType;

        //    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
        //    if (!allowedTypes.Contains(contentType))
        //    {
        //        throw new NotSupportedException("Unsupported file type.");
        //    };

        //    var ImageStream = newImage.OpenReadStream();
        //    return await UploadFileAsync(ImageStream, key, contentType);
        //}

        // update image by delete old image and upload new image
        public async Task<string> UpdateImageAsync(string key, IFormFile newImage)
        {
            string keyName = key.Substring(key.LastIndexOf('/') + 1);
            return await UploadFileAsync(newImage.OpenReadStream(), keyName, newImage.ContentType);
        }

        public async Task DeleteImageAsync(string key)
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);
        }

    }
}
