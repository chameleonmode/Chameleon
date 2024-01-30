using System;
using System.Collections.Generic;

namespace Chameleon.Interfaces.Repository
{
    public class EntityEventArgs : EventArgs
    {
        private List<object> _entities;
        public IReadOnlyList<object> Entities 
            => _entities.AsReadOnly();

        public EntityEventArgs(object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }
            _entities = new List<object>() { entity };
        }

        public EntityEventArgs(ICollection<object> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException();
            }
            _entities = new List<object>(entities);
        }
    }
}
