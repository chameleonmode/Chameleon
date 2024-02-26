using System;

namespace Chameleon.Common.Attributes
{
    public abstract class NameBaseAttribute : Attribute
    {
        public string Name { get; protected set; }
    }
}
