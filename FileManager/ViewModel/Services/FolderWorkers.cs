using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.ViewModel.Services
{
    internal class FolderWorkers
    {

        private string _curretPath;

        public static async Task<long> GetSize(DirectoryInfo d)
        {
            long size = 0;
            try
            {
                try
                {
                    List<FileInfo> files = d.GetFiles().ToList();
                    files.ForEach(x => size += Math.Max(x.Length / (1024 * 1024),0)) ;
                }
                catch (Exception ex)
                {

                }
                List<DirectoryInfo> directories = d.GetDirectories().ToList();
                directories.ForEach(async x => size += await GetSize(x));
                
                return size;
            }
            catch (Exception ex)
            {
                return size;
            }
        }
    }
}
