using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace com.github.lhervier.ksp.steaminput.ui.model
{
    public sealed class EActivator : IEquatable<EActivator>
    {
        public static readonly EActivator FullPress = new EActivator("Full_Press");
        public static readonly EActivator LongPress = new EActivator("Long_Press");
        
        private static readonly EActivator[] AllActivators =
        {
            FullPress,
            LongPress,
        };
        
        private static readonly ReadOnlyCollection<EActivator> AllReadOnly = new ReadOnlyCollection<EActivator>(AllActivators);

        public static ReadOnlyCollection<EActivator> All
        {
            get { return AllReadOnly; }
        }

        public string Name { get; private set; }
        
        private EActivator(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Activator name cannot be empty.", "name");
            }
            Name = name;
        }

        public static bool TryParse(string name, out EActivator input)
        {
            input = null;
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            string trimmed = name.Trim();
            for (int i = 0; i < AllActivators.Length; i++)
            {
                if (AllActivators[i].Name == trimmed)
                {
                    input = AllActivators[i];
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(EActivator other)
        {
            return ReferenceEquals(this, other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EActivator);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(EActivator left, EActivator right)
        {
            return ReferenceEquals(left, right);
        }

        public static bool operator !=(EActivator left, EActivator right)
        {
            return !ReferenceEquals(left, right);
        }
    }
}