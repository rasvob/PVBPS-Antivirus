using System;
using System.Collections.Generic;

namespace AntiVirusLib.Database
{
    public interface IMapper<T>
    {
        IEnumerable<T> ListAll();
        T FindOne(string key);
        void Save(T model);
    }
}