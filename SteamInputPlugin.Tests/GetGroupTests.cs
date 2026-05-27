using com.github.lhervier.ksp.model;
using com.github.lhervier.ksp.ui.model;
using NUnit.Framework;

namespace com.github.lhervier.ksp.Tests
{
    [TestFixture]
    public class GetGroupTests : DaemonTestBase
    {
        // ===============================================================================================

        [Test]
        public void ReturnsNull_WhenGroupIdNotFound()
        {
            var daemon = NewDaemonWithVdf(@"
                ""controller_mappings""
                {
                    ""group""
                    {
                        ""id""    ""123""
                        ""mode""  ""four_buttons""
                    }
                }
                ");
            Assert.That(daemon.GetGroup("does-not-exist"), Is.Null);
        }

        [Test]
        public void ReturnsGroupWithModeAndEmptyInputs_WhenGroupHasNoInputs()
        {
            var daemon = NewDaemonWithVdf(@"
                ""controller_mappings""
                {
                    ""group""
                    {
                        ""id""    ""123""
                        ""mode""  ""absolute_mouse""
                    }
                }
                ");
            VdfGroup group = daemon.GetGroup("123");

            Assert.That(group, Is.Not.Null);
            Assert.That(group.GroupId, Is.EqualTo("123"));
            Assert.That(group.Mode, Is.EqualTo("absolute_mouse"));
            Assert.That(group.Inputs, Is.Empty);
        }

        [Test]
        public void FindsSecondGroup_WhenSeveralGroupsExist()
        {
            var daemon = NewDaemonWithVdf(@"
                ""controller_mappings""
                {
                    ""group""
                    {
                        ""id""    ""1""
                        ""mode""  ""four_buttons""
                    }
                    ""group""
                    {
                        ""id""    ""2""
                        ""mode""  ""dpad""
                    }
                }
                ");
            VdfGroup group = daemon.GetGroup("2");

            Assert.That(group, Is.Not.Null);
            Assert.That(group.Mode, Is.EqualTo("dpad"));
        }

        [Test]
        public void MergesActivators_WhenSameInputDeclaredTwice()
        {
            // SteamController V2 (dpadZone/vessel-camera): dpad_north is declared twice in the same
            // group — once with Full_Press, once with Long_Press. They must merge into one input.
            var daemon = NewDaemonWithVdf(@"
                ""controller_mappings""
                {
                    ""group""
                    {
                        ""id""    ""1""
                        ""mode""  ""dpad""
                        ""inputs""
                        {
                            ""dpad_north""
                            {
                                ""activators""
                                {
                                    ""Full_Press""
                                    {
                                        ""bindings""
                                        {
                                            ""binding""    ""key_press C, Vue de la caméra, , ""
                                        }
                                    }
                                }
                            }
                            ""dpad_north""
                            {
                                ""activators""
                                {
                                    ""Long_Press""
                                    {
                                        ""bindings""
                                        {
                                            ""binding""    ""key_press V, Caméra suivante, , ""
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            ");

            VdfGroup group = daemon.GetGroup("1");

            Assert.That(group.Inputs, Has.Count.EqualTo(1));
            VdfInput input = group.Inputs[0];
            Assert.That(input.Name, Is.EqualTo(EInput.DpadNorth));
            Assert.That(input.Activators, Has.Count.EqualTo(2));

            Assert.That(input.Activators[0].Name, Is.EqualTo(EActivator.FullPress));
            Assert.That(input.Activators[0].Bindings[0].Label, Is.EqualTo("Vue de la caméra"));
            Assert.That(input.Activators[1].Name, Is.EqualTo(EActivator.LongPress));
            Assert.That(input.Activators[1].Bindings[0].Label, Is.EqualTo("Caméra suivante"));
        }

        [Test]
        public void ParsesInputsAndActivators()
        {
            var daemon = NewDaemonWithVdf(@"
                ""controller_mappings""
                {
                    ""group""
                    {
                        ""id""    ""42""
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
                                            ""binding""		""mode_shift button_diamond 42""
                                        }
                                    }
                                    ""Full_Press""
                                    {
                                        ""bindings""
                                        {
                                            ""binding""		""mode_shift joystick 38""
                                        }
                                    }
                                }
                            }
                            ""button_back_left""
                            {
                                ""activators""
                                {
                                    ""Full_Press""
                                    {
                                        ""bindings""
                                        {
                                            ""binding""		""key_press LEFT_ALT, Mod Key, , ""
                                        }
                                    }
                                }
                            }
                            ""button_escape""
                            {
                                ""activators""
                                {
                                    ""Full_Press""
                                    {
                                        ""bindings""
                                        {
                                            ""binding""		""key_press ESCAPE, Pause, , ""
                                        }
                                    }
                                    ""Long_Press""
                                    {
                                        ""bindings""
                                        {
                                            ""binding""		""key_press LEFT_ALT, Debug Console, , ""
                                        }
                                    }
                                    ""Long_Press""
                                    {
                                        ""bindings""
                                        {
                                            ""binding""		""key_press F12, Debug Console, ""
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            ");

            VdfGroup group = daemon.GetGroup("42");
            Assert.That(group, Is.Not.Null);
            Assert.That(group.Mode, Is.EqualTo("switches"));
            Assert.That(group.Inputs, Has.Count.EqualTo(3));

            // button_back_right

            VdfInput input = group.Inputs[0];
            Assert.That(input.Name, Is.EqualTo(EInput.ButtonBackRight));
            Assert.That(input.Activators, Has.Count.EqualTo(1));

            VdfActivator activator = input.Activators[0];
            Assert.That(activator.Name, Is.EqualTo(EActivator.FullPress));
            Assert.That(activator.Bindings, Has.Count.EqualTo(2));
            
            VdfBinding binding = activator.Bindings[0];
            Assert.That(binding.ModeShift, Is.EqualTo(true));
            Assert.That(binding.Zone, Is.EqualTo(EGamepadZone.ButtonDiamond));
            Assert.That(binding.GroupId, Is.EqualTo("42"));

            binding = activator.Bindings[1];
            Assert.That(binding.ModeShift, Is.EqualTo(true));
            Assert.That(binding.Zone, Is.EqualTo(EGamepadZone.Joystick));
            Assert.That(binding.GroupId, Is.EqualTo("38"));

            // button back_left

            input = group.Inputs[1];
            Assert.That(input.Name, Is.EqualTo(EInput.ButtonBackLeft));
            Assert.That(input.Activators, Has.Count.EqualTo(1));

            activator = input.Activators[0];
            Assert.That(activator.Name, Is.EqualTo(EActivator.FullPress));
            Assert.That(activator.Bindings, Has.Count.EqualTo(1));

            binding = activator.Bindings[0];
            Assert.That(binding.ModeShift, Is.EqualTo(false));
            Assert.That(binding.EventType, Is.EqualTo("key_press"));
            Assert.That(binding.Action, Is.EqualTo("LEFT_ALT"));
            Assert.That(binding.Label, Is.EqualTo("Mod Key"));
            
            // button_escape

            input = group.Inputs[2];
            Assert.That(input.Name, Is.EqualTo(EInput.ButtonEscape));
            Assert.That(input.Activators, Has.Count.EqualTo(2));

            activator = input.Activators[0];
            Assert.That(activator.Name, Is.EqualTo(EActivator.FullPress));
            Assert.That(activator.Bindings, Has.Count.EqualTo(1));

            binding = activator.Bindings[0];
            Assert.That(binding.ModeShift, Is.EqualTo(false));
            Assert.That(binding.EventType, Is.EqualTo("key_press"));
            Assert.That(binding.Action, Is.EqualTo("ESCAPE"));
            Assert.That(binding.Label, Is.EqualTo("Pause"));

            activator = input.Activators[1];
            Assert.That(activator.Name, Is.EqualTo(EActivator.LongPress));
            Assert.That(activator.Bindings, Has.Count.EqualTo(2));

            binding = activator.Bindings[0];
            Assert.That(binding.ModeShift, Is.EqualTo(false));
            Assert.That(binding.EventType, Is.EqualTo("key_press"));
            Assert.That(binding.Action, Is.EqualTo("LEFT_ALT"));
            Assert.That(binding.Label, Is.EqualTo("Debug Console"));

            binding = activator.Bindings[1];
            Assert.That(binding.ModeShift, Is.EqualTo(false));
            Assert.That(binding.EventType, Is.EqualTo("key_press"));
            Assert.That(binding.Action, Is.EqualTo("F12"));
            Assert.That(binding.Label, Is.EqualTo("Debug Console"));
        }
    }
}
