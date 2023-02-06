using srdAutoExtractor.Logic;
using System.Collections.Generic;

namespace srdAutoExtractor.Collections
{
    internal class ServiceList<T> : List<T> where T : IService
    {
        public new void Add(T item)
        {
            base.Add(item);
            item.Start();
        }

        public new void Remove(T item)
        {
            item.Stop();
            base.Remove(item);
        }
    }
}
