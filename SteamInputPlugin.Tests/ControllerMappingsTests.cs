using com.github.lhervier.ksp.model;
using NUnit.Framework;

namespace com.github.lhervier.ksp.Tests
{
    /// <summary>
    /// Covers the metadata reading at the controller_mappings root:
    /// GetControllerMappings (title/type/description) and GetAction (label/legacy_set).
    /// </summary>
    [TestFixture]
    public class ControllerMappingsTests : DaemonTestBase
    {
        // ===============================================================================================
        // GetControllerMappings
        // ===============================================================================================

        [Test]
        public void GetControllerMappings_ReadsTitleTypeAndDescription()
        {
            var daemon = NewDaemonWithVdf(@"
                ""controller_mappings""
                {
                    ""title""           ""My config""
                    ""controller_type"" ""controller_ps4""
                    ""description""      ""Some description""
                }
            ");

            VdfControllerMappings mappings = daemon.GetControllerMappings();

            Assert.That(mappings.Title, Is.EqualTo("My config"));
            Assert.That(mappings.ControllerType, Is.EqualTo(EControllerType.PS4));
            Assert.That(mappings.Description, Is.EqualTo("Some description"));
        }

        [Test]
        public void GetControllerMappings_ReturnsNullControllerType_WhenUnknown()
        {
            var daemon = NewDaemonWithVdf(@"
                ""controller_mappings""
                {
                    ""title""           ""My config""
                    ""controller_type"" ""controller_unknown""
                }
            ");

            VdfControllerMappings mappings = daemon.GetControllerMappings();

            Assert.That(mappings.Title, Is.EqualTo("My config"));
            Assert.That(mappings.ControllerType, Is.Null);
            Assert.That(mappings.Description, Is.EqualTo(""));
        }

        // ===============================================================================================
        // GetAction
        // ===============================================================================================

        [Test]
        public void GetAction_ReadsLabelAndLegacySet()
        {
            var daemon = NewDaemonWithVdf(@"
                ""controller_mappings""
                {
                    ""actions""
                    {
                        ""FlightControls""
                        {
                            ""label""       ""#Flight""
                            ""legacy_set""  ""1""
                        }
                    }
                }
            ");

            VdfAction action = daemon.GetAction(EActionGroup.FlightControls);

            Assert.That(action.Label, Is.EqualTo("#Flight"));
            Assert.That(action.LegacySet, Is.EqualTo("1"));
        }

        [Test]
        public void GetAction_ReturnsEmptyStrings_WhenActionGroupMissing()
        {
            var daemon = NewDaemonWithVdf(@"
                ""controller_mappings""
                {
                    ""actions""
                    {
                        ""EditorControls""
                        {
                            ""label""       ""#Editor""
                            ""legacy_set""  ""2""
                        }
                    }
                }
            ");

            VdfAction action = daemon.GetAction(EActionGroup.FlightControls);

            Assert.That(action.Label, Is.EqualTo(""));
            Assert.That(action.LegacySet, Is.EqualTo(""));
        }
    }
}
