using UnityEngine.SceneManagement;
using com.github.lhervier.ksp.steaminput.model;
using com.github.lhervier.ksp.shared;

namespace com.github.lhervier.ksp.steaminput 
{
    // <summary>
    //  This class is a context daemon that detects when the game is in EVA construction mode
    // </summary>
    public class EVAConstructionModeCtxDaemon : BaseContextDaemon
    {
        private static readonly ModLogger LOGGER = new ModLogger("EVAConstructionModeCtxDaemon");

        public override EActionGroup CorrespondingActionGroup()
        {
            return EActionGroup.EvaConstructionModeControls;
        }

        public void Start()
        {
            LOGGER.LogInfo("Start");
            GameEvents.OnEVAConstructionMode.Add(OnEVAConstructionModeChanged);
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            this.FireContextEnterOrLeave(false);
        }

        public void OnDestroy()
        {
            LOGGER.LogInfo("OnDestroy");
            GameEvents.OnEVAConstructionMode.Remove(OnEVAConstructionModeChanged);
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        // ============================================================

        protected void OnSceneUnloaded(Scene scene)
        {
            LOGGER.LogDebug("OnSceneUnloaded : " + scene.name);
            if( scene.name.ToUpper() != "PFLIGHT4" ) return;

            this.FireContextEnterOrLeave(false);
        }

        protected void OnEVAConstructionModeChanged(bool mode)
        {
            LOGGER.LogTrace("=> OnEVAConstructionModeChanged : " + mode);
            FireContextEnterOrLeave(mode);
        }
    }
}
