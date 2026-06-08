using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using com.github.lhervier.ksp.steaminput;

namespace com.github.lhervier.ksp.steaminput.model
{
    public sealed class EControllerType : IEquatable<EControllerType>
    {
        public static readonly EControllerType SteamController = new EControllerType("controller_steamcontroller_gordon");
        public static readonly EControllerType SteamController2 = new EControllerType("controller_triton");
        public static readonly EControllerType HoriSteamPad = new EControllerType("controller_hori_steam");
        public static readonly EControllerType PS4 = new EControllerType("controller_ps4");
        public static readonly EControllerType XBoxElite = new EControllerType("controller_xboxelite");
        
        private static readonly EControllerType[] AllControllers =
        {
            SteamController,
            SteamController2,
            HoriSteamPad,
            PS4,
            XBoxElite
        };
        
        private static readonly ReadOnlyCollection<EControllerType> AllReadOnly = new ReadOnlyCollection<EControllerType>(AllControllers);

        public static ReadOnlyCollection<EControllerType> All
        {
            get { return AllReadOnly; }
        }

        public string Name { get; private set; }
        
        private EControllerType(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Controller type name cannot be empty.", nameof(name));
            }
            Name = name;
        }

        public static bool TryParse(string name, out EControllerType input)
        {
            input = null;
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            string trimmed = name.Trim();
            for (int i = 0; i < AllControllers.Length; i++)
            {
                if (AllControllers[i].Name == trimmed)
                {
                    input = AllControllers[i];
                    return true;
                }
            }

            return false;
        }

        public string GetLabel()
        {
            return ModLocalization.GetString("SteamInput_" + Name);
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(EControllerType other)
        {
            return ReferenceEquals(this, other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EControllerType);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(EControllerType left, EControllerType right)
        {
            return ReferenceEquals(left, right);
        }

        public static bool operator !=(EControllerType left, EControllerType right)
        {
            return !ReferenceEquals(left, right);
        }
    }
}