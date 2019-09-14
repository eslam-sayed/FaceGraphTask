using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FaceGraph.API.Services;
using Microsoft.Azure.Storage.Blob;

namespace FaceGraph.API.Common
{
    public interface IImagesService
    {
        Task<IEnumerable<IListBlobItem>> GetImages();
        Task<bool> UploadImage(Stream fileStream, string fileName);
        Task<bool> DeleteImages();
        Task<bool> DeleteImage(string imageName);
    }
}