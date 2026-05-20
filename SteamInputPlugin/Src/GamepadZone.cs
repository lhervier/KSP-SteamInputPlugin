using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace com.github.lhervier.ksp
{
    /// <summary>
    /// Zone physique Steam Input (nom tel qu'apparaît dans le VDF / la config).
    /// </summary>
    public sealed class GamepadZone : IEquatable<GamepadZone>
    {
        public static readonly GamepadZone ButtonDiamond = new GamepadZone("button_diamond");
        public static readonly GamepadZone Dpad = new GamepadZone("dpad");
        public static readonly GamepadZone LeftTrigger = new GamepadZone("left_trigger");
        public static readonly GamepadZone RightTrigger = new GamepadZone("right_trigger");
        public static readonly GamepadZone Bumpers = new GamepadZone("bumpers");
        public static readonly GamepadZone Joystick = new GamepadZone("joystick");
        public static readonly GamepadZone RightJoystick = new GamepadZone("right_joystick");
        public static readonly GamepadZone LeftTrackpad = new GamepadZone("left_trackpad");
        public static readonly GamepadZone RightTrackpad = new GamepadZone("right_trackpad");
        public static readonly GamepadZone Switch = new GamepadZone("switch");

        private static readonly GamepadZone[] AllZones =
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

        private static readonly ReadOnlyCollection<GamepadZone> AllReadOnly =
            new ReadOnlyCollection<GamepadZone>(AllZones);

        /// <summary>Toutes les zones connues, dans l'ordre par défaut de l'aide manette.</summary>
        public static ReadOnlyCollection<GamepadZone> All
        {
            get { return AllReadOnly; }
        }

        /// <summary>Nom de la zone dans les fichiers VDF et la config KSP.</summary>
        public string Name { get; private set; }

        private GamepadZone(string zoneName)
        {
            if (string.IsNullOrEmpty(zoneName))
            {
                throw new ArgumentException("Zone name cannot be empty.", "zoneName");
            }
            Name = zoneName;
        }

        public static bool TryParse(string zoneName, out GamepadZone zone)
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

        public bool Equals(GamepadZone other)
        {
            return ReferenceEquals(this, other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GamepadZone);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(GamepadZone left, GamepadZone right)
        {
            return ReferenceEquals(left, right);
        }

        public static bool operator !=(GamepadZone left, GamepadZone right)
        {
            return !ReferenceEquals(left, right);
        }
    }
}
