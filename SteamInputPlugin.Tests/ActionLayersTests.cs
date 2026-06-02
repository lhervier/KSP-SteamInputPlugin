using System.Collections.Generic;
using com.github.lhervier.ksp.model;
using NUnit.Framework;

namespace com.github.lhervier.ksp.Tests
{
    /// <summary>
    /// Covers the layer reading: GetActionLayers (filtered by parent_set_name) and
    /// GetActionLayerZones (resolves the layer by title, then reads its preset zones —
    /// a layer being a preset, this reuses the same GetPresetZones path as the base set).
    /// </summary>
    [TestFixture]
    public class ActionLayersTests : DaemonTestBase
    {
        // A base preset (FlightControls) with two RightClick layers, one of which targets it.
        private const string LayersVdf = @"
            ""controller_mappings""
            {
                ""action_layers""
                {
                    ""FlightRightClickControls""
                    {
                        ""title""             ""RightClick""
                        ""set_layer""         ""1""
                        ""parent_set_name""   ""FlightControls""
                    }
                    ""EditorRightClickControls""
                    {
                        ""title""             ""RightClick""
                        ""set_layer""         ""1""
                        ""parent_set_name""   ""EditorControls""
                    }
                }
                ""preset""
                {
                    ""name""    ""FlightControls""
                    ""group_source_bindings""
                    {
                        ""35""   ""right_joystick active""
                    }
                }
                ""preset""
                {
                    ""name""    ""FlightRightClickControls""
                    ""group_source_bindings""
                    {
                        ""112""   ""right_joystick active""
                        ""113""   ""button_diamond active modeshift""
                    }
                }
            }
        ";

        // ===============================================================================================
        // GetActionLayers
        // ===============================================================================================

        [Test]
        public void GetActionLayers_ReturnsOnlyLayers_WhoseParentIsTheActionGroup()
        {
            List<VdfLayer> layers = NewDaemonWithVdf(LayersVdf).GetActionLayers(EActionGroup.FlightControls);

            Assert.That(layers, Has.Count.EqualTo(1));
            Assert.That(layers[0].Name, Is.EqualTo("FlightRightClickControls"));
            Assert.That(layers[0].Title, Is.EqualTo("RightClick"));
            Assert.That(layers[0].ParentSetName, Is.EqualTo("FlightControls"));
        }

        [Test]
        public void GetActionLayers_ReturnsEmpty_WhenNoLayerTargetsTheActionGroup()
        {
            Assert.That(NewDaemonWithVdf(LayersVdf).GetActionLayers(EActionGroup.MenuControls), Is.Empty);
        }

        [Test]
        public void GetActionLayers_ReturnsEmpty_WhenNoActionLayersAtAll()
        {
            var daemon = NewDaemonWithVdf(@"
                ""controller_mappings""
                {
                    ""preset""
                    {
                        ""name""    ""FlightControls""
                        ""group_source_bindings""
                        {
                            ""1""   ""joystick active""
                        }
                    }
                }
            ");

            Assert.That(daemon.GetActionLayers(EActionGroup.FlightControls), Is.Empty);
        }

        // ===============================================================================================
        // GetActionLayerZones
        // ===============================================================================================

        [Test]
        public void GetActionLayerZones_ReadsTheLayerPresetZones()
        {
            var zones = NewDaemonWithVdf(LayersVdf).GetActionLayerZones(EActionGroup.FlightControls, "RightClick");

            // The layer remaps right_joystick (normal) and adds a button_diamond modeshift.
            Assert.That(zones, Has.Count.EqualTo(2));
            Assert.That(zones[EGamepadZone.RightJoystick].GroupId, Is.EqualTo("112"));
            Assert.That(zones[EGamepadZone.ButtonDiamond].ModeshiftGroupIds, Is.EquivalentTo(new[] { "113" }));
        }

        [Test]
        public void GetActionLayerZones_ReturnsEmpty_WhenLayerTitleNotFound()
        {
            var zones = NewDaemonWithVdf(LayersVdf).GetActionLayerZones(EActionGroup.FlightControls, "DoesNotExist");

            Assert.That(zones, Is.Empty);
        }

        [Test]
        public void GetActionLayerZones_ReturnsEmpty_WhenLayerTargetsAnotherActionGroup()
        {
            // "RightClick" exists, but only as a layer of EditorControls here (no Editor preset
            // is declared), so resolving it against MenuControls must yield nothing.
            var zones = NewDaemonWithVdf(LayersVdf).GetActionLayerZones(EActionGroup.MenuControls, "RightClick");

            Assert.That(zones, Is.Empty);
        }
    }
}
