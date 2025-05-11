using UnityEngine;
using KSP.UI.Screens;
using JetBrains.Annotations;

namespace com.github.lhervier.ksp 
{
    public class LoggingUI : MonoBehaviour
    {
        private const int WINDOW_ID = 0x4C4F4747; // "LOGG" en hexadécimal
        private static readonly SteamControllerLogger LOGGER = new SteamControllerLogger("LoggingUI");
        private ApplicationLauncherButton button;
        private bool showWindow = false;
        private Rect windowRect = new Rect(20, 20, 200, 150);
        private LogLevel currentLogLevel;
        private bool isInitialized = false;

        public void Awake()
        {
            LOGGER.LogInfo("Awake");
            DontDestroyOnLoad(this);
        }

        public void Start() 
        {
            LOGGER.LogInfo("Start");
            currentLogLevel = SteamControllerLogger.GetGlobalLogLevel();
            GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
        }

        public void OnDestroy()
        {
            GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
            if (button != null)
            {
                LOGGER.LogInfo("RemoveModApplication");
                ApplicationLauncher.Instance.RemoveModApplication(button);
            }
            isInitialized = false;
            LOGGER.LogInfo("OnDestroy");
        }

        private void OnGUIAppLauncherReady()
        {
            if (isInitialized)
            {
                return;
            }
            if (!ApplicationLauncher.Ready) 
            {
                LOGGER.LogWarning("ApplicationLauncher not Ready");
                return;
            }
            LOGGER.LogInfo("Adding mod to ApplicationLauncher");
            button = ApplicationLauncher.Instance.AddModApplication(
                OnDisplay,
                OnHide,
                null,
                null,
                null,
                null,
                ApplicationLauncher.AppScenes.ALWAYS,
                GameDatabase.Instance.GetTexture("SteamController/Textures/logging_icon", false)
            );
            isInitialized = true;
        }

        private void OnDisplay()
        {
            LOGGER.LogDebug("Displaying window");
            showWindow = true;
        }

        private void OnHide()
        {
            LOGGER.LogDebug("Hiding window");
            showWindow = false;
        }

        void OnGUI()
        {
            if (!showWindow)
            {
                return;
            }
            windowRect = GUILayout.Window(
                WINDOW_ID, 
                windowRect, 
                DrawWindow, 
                "Logging Settings"
            );
        }

        private void DrawWindow(int windowID)
        {
            GUILayout.BeginVertical();

            GUILayout.Label("Log Level:");
            if (GUILayout.Button("None"))
            {
                SetLogLevel(LogLevel.None);
            }
            if (GUILayout.Button("Error"))
            {
                SetLogLevel(LogLevel.Error);
            }
            if (GUILayout.Button("Warning"))
            {
                SetLogLevel(LogLevel.Warning);
            }
            if (GUILayout.Button("Info"))
            {
                SetLogLevel(LogLevel.Info);
            }
            if (GUILayout.Button("Debug"))
            {
                SetLogLevel(LogLevel.Debug);
            }

            GUILayout.Space(10);
            GUILayout.Label("Current: " + currentLogLevel.ToString());

            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        private void SetLogLevel(LogLevel level)
        {
            LOGGER.LogDebug($"Setting log level to {level}");
            currentLogLevel = level;
            SteamControllerLogger.SetGlobalLogLevel(level);
            LOGGER.LogInfo($"Log level set to {level}");
        }
    }
} 