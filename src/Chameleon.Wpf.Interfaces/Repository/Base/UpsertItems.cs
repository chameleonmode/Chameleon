using System.Collections.Generic;

namespace Chameleon.Interfaces.Repository
{
    public class UpsertItems<T>
    {
        public List<T> Inserted { get; }
            = new List<T>();

        public List<T> Updated { get; }
            = new List<T>();

        public List<T> Deleted { get; }
            = new List<T>();

        public T[] Upserted
        {
            get
            {
                var items = new List<T>();
                items.AddRange(Inserted);
                items.AddRange(Updated);
                return items.ToArray();
            }
        }

        public bool IsEmpty =>
            Inserted.Count == 0
            && Updated.Count == 0
            && Deleted.Count == 0;
    }
}
