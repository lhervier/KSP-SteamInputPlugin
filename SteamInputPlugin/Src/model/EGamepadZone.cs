using System;
using System.Collections.ObjectModel;

namespace com.github.lhervier.ksp.steaminput
{
    /// <summary>
    /// Zone physique Steam Input (nom tel qu'apparaît dans le VDF / la config).
    /// </summary>
    public sealed class EGamepadZone : IEquatable<EGamepadZone>
    {
        public static readonly EGamepadZone ButtonDiamond = new EGamepadZone("button_diamond");
        public static readonly EGamepadZone Dpad = new EGamepadZone("dpad");
        public static readonly EGamepadZone LeftTrigger = new EGamepadZone("left_trigger");
        public static readonly EGamepadZone RightTrigger = new EGamepadZone("right_trigger");
        public static readonly EGamepadZone Joystick = new EGamepadZone("joystick");
        public static readonly EGamepadZone RightJoystick = new EGamepadZone("right_joystick");
        public static readonly EGamepadZone LeftTrackpad = new EGamepadZone("left_trackpad");
        public static readonly EGamepadZone RightTrackpad = new EGamepadZone("right_trackpad");
        public static readonly EGamepadZone Switch = new EGamepadZone("switch");

        private static readonly EGamepadZone[] AllZones =
        {
            ButtonDiamond,
            Dpad,
            LeftTrigger,
            RightTrigger,
            Joystick,
            RightJoystick,
            LeftTrackpad,
            RightTrackpad,
            Switch,
        };

        private static readonly ReadOnlyCollection<EGamepadZone> AllReadOnly = new ReadOnlyCollection<EGamepadZone>(AllZones);

        /// <summary>Toutes les zones connues, dans l'ordre par défaut de l'aide manette.</summary>
        public static ReadOnlyCollection<EGamepadZone> All
        {
            get { return AllReadOnly; }
        }

        /// <summary>Nom de la zone dans les fichiers VDF et la config KSP.</summary>
        public string Name { get; private set; }

        private EGamepadZone(string zoneName)
        {
            if (string.IsNullOrEmpty(zoneName))
            {
                throw new ArgumentException("Zone name cannot be empty.", "zoneName");
            }
            Name = zoneName;
        }

        public static bool TryParse(string zoneName, out EGamepadZone zone)
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

        public bool Equals(EGamepadZone other)
        {
            return ReferenceEquals(this, other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EGamepadZone);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(EGamepadZone left, EGamepadZone right)
        {
            return ReferenceEquals(left, right);
        }

        public static bool operator !=(EGamepadZone left, EGamepadZone right)
        {
            return !ReferenceEquals(left, right);
        }
    }
}
