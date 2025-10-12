using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServicesApp.DataStructures
{
    public class CustomSet<T>
    {
        private HashTable<T, byte> _map = new HashTable<T, byte>();

        public bool Add(T item)
        {
            bool exists = _map.ContainsKey(item);
            _map.AddOrUpdate(item, (byte)1);
            return !exists;
        }

        public bool Contains(T item) { return _map.ContainsKey(item); }

        public T[] ToArray() { return _map.Keys(); }
    }
}
