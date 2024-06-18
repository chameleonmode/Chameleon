using System.Collections.Generic;

namespace Chameleon.Interfaces.Repository
{
    public class UpsertItems<T>
    {
        public List<T> Inserted { get; }
            = [];

        public List<T> Updated { get; }
            = [];

        public List<T> Deleted { get; }
            = [];

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
