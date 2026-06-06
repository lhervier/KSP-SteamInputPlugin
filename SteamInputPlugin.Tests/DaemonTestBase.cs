using System;
using System.Reflection;
using System.Runtime.Serialization;
using com.github.lhervier.ksp;
using com.github.lhervier.ksp.Vdf;
using NUnit.Framework;
using UnityEngine;

namespace com.github.lhervier.ksp.Tests
{
    /// <summary>
    /// Base class for tests that drive a <see cref="GamepadConfigDaemon"/> from a VDF string.
    /// Redirects the Unity logger to the test console, forces Trace log level, and builds a
    /// daemon with an injected <c>_root</c> (without running the MonoBehaviour ctor).
    /// </summary>
    public abstract class DaemonTestBase
    {
        private static readonly FieldInfo RootField = typeof(GamepadConfigDaemon)
            .GetField("_root", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Build a daemon without invoking the MonoBehaviour ctor and inject a parsed VDF
        /// as its <c>_root</c>. Allows hitting the read methods without a Unity runtime.
        /// </summary>
        protected static GamepadConfigDaemon NewDaemonWithVdf(string vdfContent)
        {
            var root = VdfParser.Parse(vdfContent);
            var daemon = (GamepadConfigDaemon)FormatterServices.GetUninitializedObject(typeof(GamepadConfigDaemon));
            RootField.SetValue(daemon, root);
            return daemon;
        }

        [OneTimeSetUp]
        public void RedirectUnityLogger()
        {
            Debug.unityLogger.logHandler = new TestConsoleLogHandler();
        }

        [SetUp]
        public void SetUp()
        {
            SteamInputSettings.SetLogLevel(LogLevel.Trace);
        }

        private class TestConsoleLogHandler : ILogHandler
        {
            public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
            {
                TestContext.Progress.WriteLine("[" + logType + "] " + string.Format(format, args));
            }

            public void LogException(Exception exception, UnityEngine.Object context)
            {
                TestContext.Progress.WriteLine("[Exception] " + exception);
            }
        }
    }
}
