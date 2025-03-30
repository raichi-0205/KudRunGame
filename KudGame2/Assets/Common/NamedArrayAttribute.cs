using UnityEngine;
using System;
using System.Collections.Generic;

namespace Star.Editor
{
    public class NamedArrayAttribute : PropertyAttribute
    {
        public readonly string[] names;
        public NamedArrayAttribute(string[] names)
        {
            this.names = names;
        }

        public NamedArrayAttribute(Type _type)
        {
            this.names = Enum.GetNames(_type);
        }
    }
}