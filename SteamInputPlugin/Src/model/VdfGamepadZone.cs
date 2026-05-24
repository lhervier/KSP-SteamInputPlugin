using System;
using System.Collections.ObjectModel;

namespace com.github.lhervier.ksp
{
    /// <summary>
    /// Zone physique Steam Input (nom tel qu'apparaît dans le VDF / la config).
    /// </summary>
    public sealed class VdfGamepadZone : IEquatable<VdfGamepadZone>
    {
        public static readonly VdfGamepadZone ButtonDiamond = new VdfGamepadZone("button_diamond");
        public static readonly VdfGamepadZone Dpad = new VdfGamepadZone("dpad");
        public static readonly VdfGamepadZone LeftTrigger = new VdfGamepadZone("left_trigger");
        public static readonly VdfGamepadZone RightTrigger = new VdfGamepadZone("right_trigger");
        public static readonly VdfGamepadZone Bumpers = new VdfGamepadZone("bumpers");
        public static readonly VdfGamepadZone Joystick = new VdfGamepadZone("joystick");
        public static readonly VdfGamepadZone RightJoystick = new VdfGamepadZone("right_joystick");
        public static readonly VdfGamepadZone LeftTrackpad = new VdfGamepadZone("left_trackpad");
        public static readonly VdfGamepadZone RightTrackpad = new VdfGamepadZone("right_trackpad");
        public static readonly VdfGamepadZone Switch = new VdfGamepadZone("switch");

        private static readonly VdfGamepadZone[] AllZones =
        {
            ButtonDiamond,
            Dpad,
            LeftTrigger,
            RightTrigger,
            Bumpers,
            Joystick,
            RightJoystick,
            LeftTrackpad,
            RightTrackpad,
            Switch,
        };

        private static readonly ReadOnlyCollection<VdfGamepadZone> AllReadOnly =
            new ReadOnlyCollection<VdfGamepadZone>(AllZones);

        /// <summary>Toutes les zones connues, dans l'ordre par défaut de l'aide manette.</summary>
        public static ReadOnlyCollection<VdfGamepadZone> All
        {
            get { return AllReadOnly; }
        }

        /// <summary>Nom de la zone dans les fichiers VDF et la config KSP.</summary>
        public string Name { get; private set; }

        private VdfGamepadZone(string zoneName)
        {
            if (string.IsNullOrEmpty(zoneName))
            {
                throw new ArgumentException("Zone name cannot be empty.", "zoneName");
            }
            Name = zoneName;
        }

        public static bool TryParse(string zoneName, out VdfGamepadZone zone)
        {
            zone = null;
            if (string.IsNullOrEmpty(zoneName))
            {
                return false;
            }

            string trimmed = zoneName.Trim();
            for (int i = 0; i < AllZones.Length; i++)
            {
                if (AllZones[i].Name == trimmed)
                {
                    zone = AllZones[i];
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(VdfGamepadZone other)
        {
            return ReferenceEquals(this, other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as VdfGamepadZone);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(VdfGamepadZone left, VdfGamepadZone right)
        {
            return ReferenceEquals(left, right);
        }

        public static bool operator !=(VdfGamepadZone left, VdfGamepadZone right)
        {
            return !ReferenceEquals(left, right);
        }
    }
}
