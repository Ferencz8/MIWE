using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MIWE.Data.Services
{
    public interface IAzureBlobRepository
    {
        Task<string> UploadAsync(string blobName, string filePath, bool overwrite = true);

        Task<string> UploadAsync(string blobName, Stream content, bool overwrite = true);

        Task DownloadAsync(string blobName, string destinationPath);
    }
}
