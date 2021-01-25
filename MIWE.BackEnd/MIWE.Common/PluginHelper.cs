using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

namespace MIWE.Common
{
    public class PluginHelper
    {
        public static string ExtractArchive(string archiveName, string archiveDirectoryDestination, string archiveDirectorySource)
        {
            string finalPath = Path.Combine(archiveDirectoryDestination, archiveName);
            if (!Directory.Exists(finalPath))
            {
                Directory.CreateDirectory(finalPath);
            }
            else
            {
                Directory.Delete(finalPath, true);
            
                Directory.CreateDirectory(finalPath);
                string userName = WindowsIdentity.GetCurrent().Name;
                FileInfo fileInfo = new FileInfo(finalPath);
                FileSecurity fileSecurity = fileInfo.GetAccessControl();
                fileSecurity.AddAccessRule(new FileSystemAccessRule(userName, FileSystemRights.Read, AccessControlType.Allow));
                fileSecurity.AddAccessRule(new FileSystemAccessRule(userName, FileSystemRights.FullControl, AccessControlType.Allow));
                fileInfo.SetAccessControl(fileSecurity);
            }
            ZipFile.ExtractToDirectory(archiveDirectorySource, finalPath);

            File.Delete(archiveDirectorySource);//remove archive
            return finalPath;
        }

        public static string GetLocalPluginPath(string pluginName, string azurePath, DateTime? lastModifiedDate)
        {
            string pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "plugins");
            string finalPath = Path.Combine(pluginsDirectory, pluginName);
           
            if (!Directory.Exists(finalPath))
            {
                Directory.CreateDirectory(finalPath);
                using (var client = new WebClient())
                {
                    string pathWithFile = Path.Combine(pluginsDirectory, azurePath.Split("/").Last()); 
                   
                    client.DownloadFile(azurePath, pathWithFile);
                    PluginHelper.ExtractArchive(pluginName, pluginsDirectory, pathWithFile);
                }
            }
            else
            {
                DateTime lastWriteTime = System.IO.File.GetLastWriteTimeUtc(finalPath);
                //TODO:: compare with buffer of x minutes
                if (lastModifiedDate.HasValue && lastWriteTime < lastModifiedDate.Value)//local version is out of date
                {
                    using (var client = new WebClient())
                    {
                        string pathWithFile = Path.Combine(pluginsDirectory, azurePath.Split("/").Last());
                        client.DownloadFile(azurePath, pathWithFile);

                        PluginHelper.ExtractArchive(pluginName, pluginsDirectory, pathWithFile);
                    }
                }
            }
            return finalPath;
        }
    }
}
