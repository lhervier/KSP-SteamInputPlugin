using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace com.github.lhervier.ksp.ui.model
{
    public sealed class EInput : IEquatable<EInput>
    {
        public static readonly EInput Click = new EInput("click");
        
        // DPAD
        public static readonly EInput DpadEast = new EInput("dpad_east");
        public static readonly EInput DpadWest = new EInput("dpad_west");
        public static readonly EInput DpadNorth = new EInput("dpad_north");
        public static readonly EInput DpadSouth = new EInput("dpad_south");

        // FOUR BUTTONS
        public static readonly EInput ButtonA = new EInput("button_a");
        public static readonly EInput ButtonB = new EInput("button_b");
        public static readonly EInput ButtonX = new EInput("button_x");
        public static readonly EInput ButtonY = new EInput("button_y");

        // HotBar
        public static readonly EInput TouchMenuButton1 = new EInput("touch_menu_button_1");
        public static readonly EInput TouchMenuButton2 = new EInput("touch_menu_button_2");
        public static readonly EInput TouchMenuButton3 = new EInput("touch_menu_button_3");
        public static readonly EInput TouchMenuButton4 = new EInput("touch_menu_button_4");
        public static readonly EInput TouchMenuButton5 = new EInput("touch_menu_button_5");
        public static readonly EInput TouchMenuButton6 = new EInput("touch_menu_button_6");
        public static readonly EInput TouchMenuButton7 = new EInput("touch_menu_button_7");
        public static readonly EInput TouchMenuButton8 = new EInput("touch_menu_button_8");
        public static readonly EInput TouchMenuButton9 = new EInput("touch_menu_button_9");
        public static readonly EInput TouchMenuButton10 = new EInput("touch_menu_button_10");
        public static readonly EInput TouchMenuButton11 = new EInput("touch_menu_button_11");
        public static readonly EInput TouchMenuButton12 = new EInput("touch_menu_button_12");
        public static readonly EInput TouchMenuButton13 = new EInput("touch_menu_button_13");
        public static readonly EInput TouchMenuButton14 = new EInput("touch_menu_button_14");
        public static readonly EInput TouchMenuButton15 = new EInput("touch_menu_button_15");
        public static readonly EInput TouchMenuButton16 = new EInput("touch_menu_button_16");

        // SWITCHES
        public static readonly EInput ButtonBackLeft = new EInput("button_back_left");
        public static readonly EInput ButtonBackRight = new EInput("button_back_right");
        public static readonly EInput ButtonEscape = new EInput("button_escape");
        public static readonly EInput ButtonMenu = new EInput("button_menu");
        public static readonly EInput LeftBumper = new EInput("left_bumper");
        public static readonly EInput RightBumper = new EInput("right_bumper");

        // TRIGGERS
        public static readonly EInput Edge = new EInput("edge");

        private static readonly EInput[] AllInputs =
        {
            Click,
            DpadEast,
            DpadWest,
            DpadNorth,
            DpadSouth,
            ButtonA,
            ButtonB,
            ButtonX,
            ButtonY,
            TouchMenuButton1,
            TouchMenuButton2,
            TouchMenuButton3,
            TouchMenuButton4,
            TouchMenuButton5,
            TouchMenuButton6,
            TouchMenuButton7,
            TouchMenuButton8,
            TouchMenuButton9,
            TouchMenuButton10,
            TouchMenuButton11,
            TouchMenuButton12,
            TouchMenuButton13,
            TouchMenuButton14,
            TouchMenuButton15,
            TouchMenuButton16,
            ButtonBackLeft,
            ButtonBackRight,
            ButtonEscape,
            ButtonMenu,
            LeftBumper,
            RightBumper,
        };
        
        private static readonly ReadOnlyCollection<EInput> AllReadOnly = new ReadOnlyCollection<EInput>(AllInputs);

        public static ReadOnlyCollection<EInput> All
        {
            get { return AllReadOnly; }
        }

        public string Name { get; private set; }
        
        private EInput(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Input name cannot be empty.", "name");
            }
            Name = name;
        }

        public static bool TryParse(string name, out EInput input)
        {
            input = null;
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            string trimmed = name.Trim();
            for (int i = 0; i < AllInputs.Length; i++)
            {
                if (AllInputs[i].Name == trimmed)
                {
                    input = AllInputs[i];
                    return true;
                }
            }

            return false;
        }

        public string GetLabel(string controllerType)
        {
            string baseKey = "SteamInput_inputs_" + Name;
            string key = baseKey + "_" + controllerType;
            string translated = ModLocalization.GetString(key);
            if( !string.IsNullOrEmpty(translated) ) return translated;
            return ModLocalization.GetString(baseKey);
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(EInput other)
        {
            return ReferenceEquals(this, other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EInput);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(EInput left, EInput right)
        {
            return ReferenceEquals(left, right);
        }

        public static bool operator !=(EInput left, EInput right)
        {
            return !ReferenceEquals(left, right);
        }
    }
}