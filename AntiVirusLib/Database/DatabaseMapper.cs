using System;
using System.Collections.Generic;
using AntiVirusLib.Models;

namespace AntiVirusLib.Database
{
    public class DatabaseMapper: IMapper<FileModel>
    {
        public string Source { get; set; }

        public DatabaseMapper(string source)
        {
            Source = source;
        }

        public IEnumerable<FileModel> ListAll()
        {
            throw new NotImplementedException();
        }

        public FileModel FindOne(string key)
        {
            throw new NotImplementedException();
        }

        public void Insert(FileModel model)
        {
            throw new NotImplementedException();
        }
    }
}