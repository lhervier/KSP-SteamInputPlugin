using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using com.github.lhervier.ksp.steaminput.model;
using com.github.lhervier.ksp.steaminput.ui;
using com.github.lhervier.ksp.steaminput.ui.model;
using com.github.lhervier.ksp.steaminput.Vdf;
using NUnit.Framework;

namespace com.github.lhervier.ksp.steaminput.Tests
{
    [TestFixture]
    public class CheatSheetViewModelTests : DaemonTestBase
    {
        private static readonly FieldInfo DaemonRootField = typeof(GamepadConfigDaemon)
            .GetField("_root", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo VmDaemonField = typeof(CheatSheetViewModel)
            .GetField("_gamepadConfigDaemon", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo VmGroupsCacheField = typeof(CheatSheetViewModel)
            .GetField("_groupsCache", BindingFlags.NonPublic | BindingFlags.Instance);

        private static CheatSheetViewModel NewViewModelWithVdf(string vdfContent)
        {
            var daemon = (GamepadConfigDaemon)FormatterServices.GetUninitializedObject(typeof(GamepadConfigDaemon));
            DaemonRootField.SetValue(daemon, VdfParser.Parse(vdfContent));

            var vm = (CheatSheetViewModel)FormatterServices.GetUninitializedObject(typeof(CheatSheetViewModel));
            VmDaemonField.SetValue(vm, daemon);
            VmGroupsCacheField.SetValue(vm, new DictionaryValueList<string, VdfGroup>());
            return vm;
        }

        // ===============================================================================================

        [Test]
        public void ReturnsEmpty_WhenGroupDoesNotExist()
        {
            var vm = NewViewModelWithVdf(@"
                ""controller_mappings""
                {
                    ""group""
                    {
                        ""id""    ""1""
                        ""mode""  ""four_buttons""
                    }
                }
            ");

            Assert.That(vm.GetActivators("does-not-exist"), Is.Empty);
        }

        [Test]
        public void ReturnsEmpty_WhenGroupHasNoInputs()
        {
            var vm = NewViewModelWithVdf(@"
                ""controller_mappings""
                {
                    ""group""
                    {
                        ""id""    ""1""
                        ""mode""  ""four_buttons""
                    }
                }
            ");

            Assert.That(vm.GetActivators("1"), Is.Empty);
        }

        [Test]
        public void SetsBindingTextFromLabel_OnSingleNonModeshiftBinding()
        {
            var vm = NewViewModelWithVdf(@"
                ""controller_mappings""
                {
                    ""group""
                    {
                        ""id""    ""1""
                        ""mode""  ""four_buttons""
                        ""inputs""
                        {
                            ""button_a""
                            {
                                ""activators""
                                {
                                    ""Full_Press""
                                    {
                                        ""bindings""
                                        {
                                            ""binding""    ""key_press SPACE, Stage, , ""
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            ");

            List<UIActivator> result = vm.GetActivators("1");

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Input, Is.EqualTo(EInput.ButtonA));
            Assert.That(result[0].LongPress, Is.False);
            Assert.That(result[0].ModeShift, Is.False);
            Assert.That(result[0].BindingText, Is.EqualTo("Stage"));
        }

        [Test]
        public void UsesLastLabel_WhenMultipleNonModeshiftBindings()
        {
            // Two non-modeshift bindings on the same activator (e.g. CTRL+Y combo).
            // Expected: BindingText = label of the LAST binding.
            var vm = NewViewModelWithVdf(@"
                ""controller_mappings""
                {
                    ""group""
                    {
                        ""id""    ""1""
                        ""mode""  ""switches""
                        ""inputs""
                        {
                            ""button_escape""
                            {
                                ""activators""
                                {
                                    ""Long_Press""
                                    {
                                        ""bindings""
                                        {
                                            ""binding""    ""key_press LEFT_ALT, Mod, , ""
                                        }
                                    }
                                    ""Long_Press""
                                    {
                                        ""bindings""
                                        {
                                            ""binding""    ""key_press F12, Debug Console, , ""
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            ");

            List<UIActivator> result = vm.GetActivators("1");

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].LongPress, Is.True);
            Assert.That(result[0].ModeShift, Is.False);
            Assert.That(result[0].BindingText, Is.EqualTo("Debug Console"));
        }

        [Test]
        public void SetsModeShift_WhenAnyBindingIsModeShift()
        {
            var vm = NewViewModelWithVdf(@"
                ""controller_mappings""
                {
                    ""group""
                    {
                        ""id""    ""1""
                        ""mode""  ""switches""
                        ""inputs""
                        {
                            ""button_back_right""
                            {
                                ""activators""
                                {
                                    ""Full_Press""
                                    {
                                        ""bindings""
                                        {
                                            ""binding""    ""mode_shift button_diamond 42""
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            ");

            List<UIActivator> result = vm.GetActivators("1");

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Input, Is.EqualTo(EInput.ButtonBackRight));
            Assert.That(result[0].ModeShift, Is.True);
            Assert.That(result[0].BindingText, Is.Null);
        }

        [Test]
        public void SetsModeShiftAndLabel_WhenMixedBindings()
        {
            // One modeshift + one normal binding on same activator.
            // Expected: ModeShift=true AND BindingText set from the non-modeshift binding.
            var vm = NewViewModelWithVdf(@"
                ""controller_mappings""
                {
                    ""group""
                    {
                        ""id""    ""1""
                        ""mode""  ""switches""
                        ""inputs""
                        {
                            ""button_back_right""
                            {
                                ""activators""
                                {
                                    ""Full_Press""
                                    {
                                        ""bindings""
                                        {
                                            ""binding""    ""mode_shift button_diamond 42""
                                        }
                                    }
                                    ""Full_Press""
                                    {
                                        ""bindings""
                                        {
                                            ""binding""    ""key_press SPACE, Stage, , ""
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            ");

            List<UIActivator> result = vm.GetActivators("1");

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].ModeShift, Is.True);
            Assert.That(result[0].BindingText, Is.EqualTo("Stage"));
        }

        [Test]
        public void ReturnsTwoLines_WhenSameInputHasFullPressAndLongPress()
        {
            var vm = NewViewModelWithVdf(@"
                ""controller_mappings""
                {
                    ""group""
                    {
                        ""id""    ""1""
                        ""mode""  ""switches""
                        ""inputs""
                        {
                            ""button_escape""
                            {
                                ""activators""
                                {
                                    ""Full_Press""
                                    {
                                        ""bindings""
                                        {
                                            ""binding""    ""key_press ESCAPE, Pause, , ""
                                        }
                                    }
                                    ""Long_Press""
                                    {
                                        ""bindings""
                                        {
                                            ""binding""    ""key_press F12, Debug Console, , ""
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            ");

            List<UIActivator> result = vm.GetActivators("1");

            Assert.That(result, Has.Count.EqualTo(2));

            UIActivator fullPress = result[0];
            Assert.That(fullPress.Input, Is.EqualTo(EInput.ButtonEscape));
            Assert.That(fullPress.LongPress, Is.False);
            Assert.That(fullPress.BindingText, Is.EqualTo("Pause"));

            UIActivator longPress = result[1];
            Assert.That(longPress.Input, Is.EqualTo(EInput.ButtonEscape));
            Assert.That(longPress.LongPress, Is.True);
            Assert.That(longPress.BindingText, Is.EqualTo("Debug Console"));
        }

        [Test]
        public void DropsHoldLayerBinding_WhenLayerCannotBeResolved()
        {
            // dpad_west: a "Clic droit" mouse binding plus a "hold_layer" pointing at position 13.
            // No preset exists at that position, so the layer cannot be resolved: no extra row is
            // emitted and only "Clic droit" remains.
            var vm = NewViewModelWithVdf(@"
                ""controller_mappings""
                {
                    ""group""
                    {
                        ""id""    ""1""
                        ""mode""  ""dpad""
                        ""inputs""
                        {
                            ""dpad_west""
                            {
                                ""activators""
                                {
                                    ""Full_Press""
                                    {
                                        ""bindings""
                                        {
                                            ""binding""    ""mouse_button RIGHT, Clic droit, ""
                                        }
                                    }
                                    ""Full_Press""
                                    {
                                        ""bindings""
                                        {
                                            ""binding""    ""controller_action hold_layer 13 0 0, , ""
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            ");

            List<UIActivator> result = vm.GetActivators("1");

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Input, Is.EqualTo(EInput.DpadWest));
            Assert.That(result[0].BindingText, Is.EqualTo("Clic droit"));
            Assert.That(result[0].ActionText, Is.EqualTo("Clic droit"));
        }

        [Test]
        public void SurfacesHoldLayerBinding_AsExtraHighlightedRow()
        {
            // Same dpad_west, but now the hold_layer points at a real layer (position 2). The real
            // action "Clic droit" keeps its own row, and the layer activation gets a second,
            // highlighted row (carrying no real binding). The layer title resolution itself is
            // covered in ActionLayersTests; here we only assert the extra row is surfaced.
            var vm = NewViewModelWithVdf(@"
                ""controller_mappings""
                {
                    ""action_layers""
                    {
                        ""FlightRightClickControls""
                        {
                            ""title""             ""RightClick""
                            ""parent_set_name""   ""FlightControls""
                        }
                    }
                    ""preset""
                    {
                        ""name""    ""FlightControls""
                    }
                    ""preset""
                    {
                        ""name""    ""FlightRightClickControls""
                    }
                    ""group""
                    {
                        ""id""    ""1""
                        ""mode""  ""dpad""
                        ""inputs""
                        {
                            ""dpad_west""
                            {
                                ""activators""
                                {
                                    ""Full_Press""
                                    {
                                        ""bindings""
                                        {
                                            ""binding""    ""mouse_button RIGHT, Clic droit, ""
                                        }
                                    }
                                    ""Full_Press""
                                    {
                                        ""bindings""
                                        {
                                            ""binding""    ""controller_action hold_layer 2 0 0, , ""
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            ");

            List<UIActivator> result = vm.GetActivators("1");

            Assert.That(result, Has.Count.EqualTo(2));

            UIActivator real = result[0];
            Assert.That(real.Input, Is.EqualTo(EInput.DpadWest));
            Assert.That(real.BindingText, Is.EqualTo("Clic droit"));
            Assert.That(real.Highlighted, Is.False);

            UIActivator layer = result[1];
            Assert.That(layer.Input, Is.EqualTo(EInput.DpadWest));
            Assert.That(layer.BindingText, Is.Null);
            Assert.That(layer.ModeShift, Is.False);
            Assert.That(layer.Highlighted, Is.True);
        }

        [Test]
        public void OmitsMainRow_WhenActivatorOnlyHoldsALayer()
        {
            // An input whose only binding is a hold_layer must not produce a blank main row:
            // it yields exactly one row, the highlighted layer row.
            var vm = NewViewModelWithVdf(@"
                ""controller_mappings""
                {
                    ""action_layers""
                    {
                        ""FlightRightClickControls""
                        {
                            ""title""             ""RightClick""
                            ""parent_set_name""   ""FlightControls""
                        }
                    }
                    ""preset""
                    {
                        ""name""    ""FlightControls""
                    }
                    ""preset""
                    {
                        ""name""    ""FlightRightClickControls""
                    }
                    ""group""
                    {
                        ""id""    ""1""
                        ""mode""  ""dpad""
                        ""inputs""
                        {
                            ""dpad_west""
                            {
                                ""activators""
                                {
                                    ""Full_Press""
                                    {
                                        ""bindings""
                                        {
                                            ""binding""    ""controller_action hold_layer 2 0 0, , ""
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            ");

            List<UIActivator> result = vm.GetActivators("1");

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Highlighted, Is.True);
            Assert.That(result[0].BindingText, Is.Null);
        }

        // ===============================================================================================
        // IsMouseGroup
        // ===============================================================================================

        [Test]
        public void IsMouseGroup_True_ForJoystickMouseMode()
        {
            var vm = NewViewModelWithVdf(@"
                ""controller_mappings""
                {
                    ""group""
                    {
                        ""id""    ""1""
                        ""mode""  ""joystick_mouse""
                    }
                }
            ");

            Assert.That(vm.IsMouseGroup("1"), Is.True);
        }

        [Test]
        public void IsMouseGroup_True_ForAbsoluteMouseMode()
        {
            var vm = NewViewModelWithVdf(@"
                ""controller_mappings""
                {
                    ""group""
                    {
                        ""id""    ""1""
                        ""mode""  ""absolute_mouse""
                    }
                }
            ");

            Assert.That(vm.IsMouseGroup("1"), Is.True);
        }

        [Test]
        public void IsMouseGroup_False_ForNonMouseMode()
        {
            var vm = NewViewModelWithVdf(@"
                ""controller_mappings""
                {
                    ""group""
                    {
                        ""id""    ""1""
                        ""mode""  ""four_buttons""
                    }
                }
            ");

            Assert.That(vm.IsMouseGroup("1"), Is.False);
        }

        [Test]
        public void IsMouseGroup_False_WhenGroupDoesNotExist()
        {
            var vm = NewViewModelWithVdf(@"
                ""controller_mappings""
                {
                    ""group""
                    {
                        ""id""    ""1""
                        ""mode""  ""joystick_mouse""
                    }
                }
            ");

            Assert.That(vm.IsMouseGroup("does-not-exist"), Is.False);
        }

        // ===============================================================================================
        // IsSectionEmpty / HasNonEmptySection
        // ===============================================================================================

        private const string TwoGroupsVdf = @"
            ""controller_mappings""
            {
                ""group""
                {
                    ""id""    ""empty""
                    ""mode""  ""four_buttons""
                }
                ""group""
                {
                    ""id""    ""withBinding""
                    ""mode""  ""four_buttons""
                    ""inputs""
                    {
                        ""button_a""
                        {
                            ""activators""
                            {
                                ""Full_Press""
                                {
                                    ""bindings""
                                    {
                                        ""binding""    ""key_press SPACE, Stage, , ""
                                    }
                                }
                            }
                        }
                    }
                }
                ""group""
                {
                    ""id""    ""mouse""
                    ""mode""  ""joystick_mouse""
                }
            }
        ";

        [Test]
        public void IsSectionEmpty_True_WhenGroupHasNoBinding()
        {
            Assert.That(NewViewModelWithVdf(TwoGroupsVdf).IsSectionEmpty("empty"), Is.True);
        }

        [Test]
        public void IsSectionEmpty_False_WhenGroupHasBinding()
        {
            Assert.That(NewViewModelWithVdf(TwoGroupsVdf).IsSectionEmpty("withBinding"), Is.False);
        }

        [Test]
        public void IsSectionEmpty_False_WhenMouseGroup_EvenWithoutBinding()
        {
            Assert.That(NewViewModelWithVdf(TwoGroupsVdf).IsSectionEmpty("mouse"), Is.False);
        }

        [Test]
        public void IsSectionEmpty_True_WhenGroupIdNullOrMissing()
        {
            var vm = NewViewModelWithVdf(TwoGroupsVdf);
            Assert.That(vm.IsSectionEmpty((string) null), Is.True);
            Assert.That(vm.IsSectionEmpty("does-not-exist"), Is.True);
        }

        [Test]
        public void HasNonEmptySection_False_WhenAllSectionsEmpty()
        {
            var vm = NewViewModelWithVdf(TwoGroupsVdf);
            var zone = new UIPhysicalZone();
            zone.Sections.Add(
                new UISection{
                    GroupId = "empty", 
                    Modeshift = false,
                }
            );
            zone.Sections.Add(
                new UISection
                {
                    GroupId = "does-not-exist", 
                    Modeshift = true,
                }
            );
            
            Assert.That(vm.HasNonEmptySection(zone), Is.False);
        }

        [Test]
        public void HasNonEmptySection_True_WhenOnlyModeshiftIsNonEmpty()
        {
            var vm = NewViewModelWithVdf(TwoGroupsVdf);
            var zone = new UIPhysicalZone();
            zone.Sections.Add(
                new UISection{
                    GroupId = "empty", 
                    Modeshift = false,
                }
            );
            zone.Sections.Add(
                new UISection
                {
                    GroupId = "withBinding", 
                    Modeshift = true,
                }
            );
            
            Assert.That(vm.HasNonEmptySection(zone), Is.True);
        }
    }
}
