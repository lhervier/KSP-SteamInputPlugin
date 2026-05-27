using System.Collections.Generic;
using com.github.lhervier.ksp.model;
using NUnit.Framework;

namespace com.github.lhervier.ksp.Tests
{
    /// <summary>
    /// Covers the preset / physical-zone reading: GetPresetZones (incl. the
    /// Switch -> Bumpers expansion and modeshift handling) and GetGamepadZones.
    /// Both rely on the private ParseGroupBinding ("active [modeshift] &lt;zone&gt;").
    /// </summary>
    [TestFixture]
    public class PresetZonesTests : DaemonTestBase
    {
        // ===============================================================================================
        // GetPresetZones
        // ===============================================================================================

        [Test]
        public void GetPresetZones_ReturnsEmpty_WhenPresetNotFound()
        {
            var daemon = NewDaemonWithVdf(@"
                ""controller_mappings""
                {
                    ""preset""
                    {
                        ""name""    ""EditorControls""
                        ""group_source_bindings""
                        {
                            ""10""   ""button_diamond active""
                        }
                    }
                }
            ");

            Assert.That(daemon.GetPresetZones(EActionGroup.FlightControls), Is.Empty);
        }

        [Test]
        public void GetPresetZones_MapsZoneToGroupId()
        {
            var daemon = NewDaemonWithVdf(@"
                ""controller_mappings""
                {
                    ""preset""
                    {
                        ""name""    ""FlightControls""
                        ""group_source_bindings""
                        {
                            ""10""   ""button_diamond active""
                            ""11""   ""joystick active""
                        }
                    }
                }
            ");

            var zones = daemon.GetPresetZones(EActionGroup.FlightControls);

            Assert.That(zones, Has.Count.EqualTo(2));
            Assert.That(zones.ContainsKey(EGamepadZone.ButtonDiamond));
            Assert.That(zones[EGamepadZone.ButtonDiamond].GroupId, Is.EqualTo("10"));
            Assert.That(zones[EGamepadZone.ButtonDiamond].ModeshiftGroupIds, Is.Empty);
            Assert.That(zones.ContainsKey(EGamepadZone.Joystick));
            Assert.That(zones[EGamepadZone.Joystick].GroupId, Is.EqualTo("11"));
        }

        [Test]
        public void GetPresetZones_ExpandsSwitchToBumpers_WithSameGroupId()
        {
            var daemon = NewDaemonWithVdf(@"
                ""controller_mappings""
                {
                    ""preset""
                    {
                        ""name""    ""FlightControls""
                        ""group_source_bindings""
                        {
                            ""20""   ""switch active""
                        }
                    }
                }
            ");

            var zones = daemon.GetPresetZones(EActionGroup.FlightControls);

            Assert.That(zones, Has.Count.EqualTo(2));
            Assert.That(zones.ContainsKey(EGamepadZone.Switch));
            Assert.That(zones[EGamepadZone.Switch].GroupId, Is.EqualTo("20"));
            Assert.That(zones.ContainsKey(EGamepadZone.Bumpers));
            Assert.That(zones[EGamepadZone.Bumpers].GroupId, Is.EqualTo("20"));
        }

        [Test]
        public void GetPresetZones_PutsModeshiftBinding_IntoModeshiftGroupIds()
        {
            var daemon = NewDaemonWithVdf(@"
                ""controller_mappings""
                {
                    ""preset""
                    {
                        ""name""    ""FlightControls""
                        ""group_source_bindings""
                        {
                            ""30""   ""button_diamond active""
                            ""31""   ""button_diamond active modeshift""
                        }
                    }
                }
            ");

            var zones = daemon.GetPresetZones(EActionGroup.FlightControls);

            Assert.That(zones, Has.Count.EqualTo(1));
            VdfPresetZone diamond = zones[EGamepadZone.ButtonDiamond];
            Assert.That(diamond.GroupId, Is.EqualTo("30"));
            Assert.That(diamond.ModeshiftGroupIds, Is.EquivalentTo(new[] { "31" }));
        }

        [Test]
        public void GetPresetZones_IgnoresBinding_WithoutActive()
        {
            var daemon = NewDaemonWithVdf(@"
                ""controller_mappings""
                {
                    ""preset""
                    {
                        ""name""    ""FlightControls""
                        ""group_source_bindings""
                        {
                            ""40""   ""button_diamond""
                        }
                    }
                }
            ");

            Assert.That(daemon.GetPresetZones(EActionGroup.FlightControls), Is.Empty);
        }

        [Test]
        public void GetPresetZones_IgnoresBinding_WithUnknownZone()
        {
            var daemon = NewDaemonWithVdf(@"
                ""controller_mappings""
                {
                    ""preset""
                    {
                        ""name""    ""FlightControls""
                        ""group_source_bindings""
                        {
                            ""50""   ""not_a_zone active""
                        }
                    }
                }
            ");

            Assert.That(daemon.GetPresetZones(EActionGroup.FlightControls), Is.Empty);
        }

        [Test]
        public void GetPresetZones_SelectsCorrectPreset_AmongSeveral()
        {
            var daemon = NewDaemonWithVdf(@"
                ""controller_mappings""
                {
                    ""preset""
                    {
                        ""name""    ""EditorControls""
                        ""group_source_bindings""
                        {
                            ""1""   ""dpad active""
                        }
                    }
                    ""preset""
                    {
                        ""name""    ""FlightControls""
                        ""group_source_bindings""
                        {
                            ""2""   ""joystick active""
                        }
                    }
                }
            ");

            var zones = daemon.GetPresetZones(EActionGroup.FlightControls);

            Assert.That(zones, Has.Count.EqualTo(1));
            Assert.That(zones.ContainsKey(EGamepadZone.Joystick));
            Assert.That(zones[EGamepadZone.Joystick].GroupId, Is.EqualTo("2"));
        }

        // ===============================================================================================
        // GetGamepadZones
        // ===============================================================================================

        [Test]
        public void GetGamepadZones_ReturnsDistinctZonesAcrossPresets()
        {
            var daemon = NewDaemonWithVdf(@"
                ""controller_mappings""
                {
                    ""preset""
                    {
                        ""name""    ""FlightControls""
                        ""group_source_bindings""
                        {
                            ""1""   ""button_diamond active""
                            ""2""   ""joystick active""
                        }
                    }
                    ""preset""
                    {
                        ""name""    ""EditorControls""
                        ""group_source_bindings""
                        {
                            ""3""   ""joystick active""
                            ""4""   ""dpad active""
                        }
                    }
                }
            ");

            List<EGamepadZone> zones = daemon.GetGamepadZones();

            Assert.That(zones, Is.EquivalentTo(new[]
            {
                EGamepadZone.ButtonDiamond,
                EGamepadZone.Joystick,
                EGamepadZone.Dpad,
            }));
        }

        [Test]
        public void GetGamepadZones_DoesNotExpandSwitchToBumpers()
        {
            var daemon = NewDaemonWithVdf(@"
                ""controller_mappings""
                {
                    ""preset""
                    {
                        ""name""    ""FlightControls""
                        ""group_source_bindings""
                        {
                            ""1""   ""switch active""
                        }
                    }
                }
            ");

            List<EGamepadZone> zones = daemon.GetGamepadZones();

            Assert.That(zones, Is.EquivalentTo(new[] { EGamepadZone.Switch }));
        }

        [Test]
        public void GetGamepadZones_IgnoresUnknownAndInactiveBindings()
        {
            var daemon = NewDaemonWithVdf(@"
                ""controller_mappings""
                {
                    ""preset""
                    {
                        ""name""    ""FlightControls""
                        ""group_source_bindings""
                        {
                            ""1""   ""dpad active""
                            ""2""   ""not_a_zone active""
                            ""3""   ""joystick""
                        }
                    }
                }
            ");

            List<EGamepadZone> zones = daemon.GetGamepadZones();

            Assert.That(zones, Is.EquivalentTo(new[] { EGamepadZone.Dpad }));
        }
    }
}
