using UnityEngine;
using UnityEngine.UI;
using com.github.lhervier.ksp.ui.styles;
using UnityEngine.Events;
using com.github.lhervier.ksp.ui.ugui.sprites;
using com.github.lhervier.ksp.ui.ugui.titleBar;
using System;
using com.github.lhervier.ksp.ui.ugui.menu;
using com.github.lhervier.ksp.ui.ugui.body;
using System.Collections;

namespace com.github.lhervier.ksp.ui.ugui
{
    public class ModPopupDialogBuilder
    {
        private const string DIALOG_ID = "SteamInputCheatSheetUGUI";
        private CheatSheetViewModel _viewModel;
        private bool _hasPosition = false;
        private Vector2 _initialPosition;
        private TitleBarBuilder _titleBarBuilder;
        private OverlayBuilder _overlayBuilder;
        private MenuBuilder _menuBuilder;
        private BodyBuilder _bodyBuilder;

        public ModPopupDialogBuilder(CheatSheetViewModel viewModel)
        {
            this._viewModel = viewModel;
            this._titleBarBuilder = new TitleBarBuilder(viewModel);
            this._overlayBuilder = new OverlayBuilder(viewModel);
            this._menuBuilder = new MenuBuilder(viewModel);
            this._bodyBuilder = new BodyBuilder(viewModel);
        }

        public ModPopupDialogBuilder(
            CheatSheetViewModel viewModel,
            Vector2 initialPosition
        ) : this(viewModel)
        {
            this._initialPosition = initialPosition;
            this._hasPosition = true;
        }

        /// <summary>
        /// Spawn the cheat-sheet popup window and return its controller, or null if KSP failed to spawn
        /// it. The caller drives the window through the returned controller.
        /// </summary>
        public ModPopupDialogController Create()
        {
            // Creates a ultra minimal MultiOptionDialog. We will not use it.
            var pos = NormalizedWindowPos(
                SteamInputPalette.WindowInitialPositionX, 
                SteamInputPalette.WindowInitialPositionY, 
                SteamInputPalette.WindowWidth,
                SteamInputPalette.WindowHeight
            );
            var content = new DialogGUIVerticalLayout();
            MultiOptionDialog multiOptionDialog = new MultiOptionDialog(
                DIALOG_ID,
                string.Empty,
                string.Empty,
                HighLogic.UISkin,
                pos,
                new DialogGUIBase[]
                {
                    new DialogGUIBox(null, -1, -1, () => true, content)
                }
            );
            
            // Creates the popup dialog
            PopupDialog popupDialog = PopupDialog.SpawnPopupDialog(
                multiOptionDialog,
                true,
                HighLogic.UISkin,
                false,
                string.Empty
            );
            if( popupDialog == null || popupDialog.popupWindow == null )
            {
                return null;
            }
            ModPopupDialogController controller = popupDialog.popupWindow.AddComponent<ModPopupDialogController>();
            controller.Initialize(_viewModel);
            controller.BindPopupDialog(popupDialog);
            controller.BindOverlayBuilder(_overlayBuilder);
            controller.BindMenuBuilder(_menuBuilder);
            if (_hasPosition)
            {
                controller.InitializePosition(_initialPosition);
            }

            // Remove KSP default title
            var title = popupDialog.popupWindow.transform.Find("Title");
            if (title != null)
            {
                title.gameObject.SetActive(false);
            }

            // Keep the window hidden until it has been positioned. KSP re-applies the initial
            // spawn position on every layout pass, so the window would otherwise flicker at the
            // default position before being moved to the saved one. The controller reveals it
            // (alpha 1) from Show(), once the layout has settled and the position has been applied.
            var canvasGroup = popupDialog.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                controller.BindCanvasGroup(canvasGroup);
            }

            // Set windows border color 
            var windowGo = popupDialog.popupWindow;
            var windowImage = windowGo.GetComponent<Image>();
            if (windowImage != null)
            {
                windowImage.sprite = SpritesPopupDialog.WindowChromeSprite;
                windowImage.type = Image.Type.Sliced;
                windowImage.color = Color.white;

                // Raycast to prevent mouse event to be sent to the game
                windowImage.raycastTarget = true;
            }

            // Set windows background color
            foreach (var image in windowGo.GetComponentsInChildren<Image>(true))
            {
                if (image == windowImage)
                {
                    continue;
                }

                image.sprite = SpritesGlobal.FillSprite;
                image.type = Image.Type.Simple;
                image.color = SteamInputPalette.WindowBodyColor;
            }

            // Add the body (scrollable content). First in z-order so the overlay/menu draw above it.
            BodyBuilder.BodyController bodyController = this._bodyBuilder.Create();
            bodyController.transform.SetParent(popupDialog.popupWindow.transform, false);

            // Add the title bar
            TitleBarBuilder.TitleBarController titleBarController = this._titleBarBuilder.Create();
            titleBarController.transform.SetParent(popupDialog.popupWindow.transform, false);

            return controller;
        }

        /// <summary>
        /// Normalized position from screen top-left, expressed as a percentage of the screen width and height.
        /// </summary>
        private static Rect NormalizedWindowPos(float screenX, float screenYFromTop, float width, float height)
        {
            var centerX = screenX + width * 0.5f;
            var centerY = Screen.height - screenYFromTop - height * 0.5f;
            return new Rect(centerX / Screen.width, centerY / Screen.height, width, height);
        }

        // ==============================================================
        // Controller
        // ==============================================================

        public class ModPopupDialogController : BaseSteamInputController
        {
            private OverlayBuilder _overlayBuilder;
            private MenuBuilder _menuBuilder;

            private OverlayBuilder.OverlayController _overlayController = null;
            private MenuBuilder.MenuController _menuController = null;

            private CanvasGroup _canvasGroup;
            private PopupDialog _popupDialog;

            private bool _hasPosition = false;
            private Vector2 _position;
            public EventData<Vector2> OnPositionCaptured = new EventData<Vector2>("SteamInput.CheatSheetUGUIWindow.OnMoved");
            public EventVoid OnClosed = new EventVoid("SteamInput.CheatSheetUGUIWindow.OnClosed");

            // =========================
            // Life cycle
            // =========================

            // Dependencies injected by the builder right after AddComponent, before Start() runs.

            /// <summary>Inject the overlay builder.</summary>
            public void BindOverlayBuilder(OverlayBuilder builder)
            {
                this._overlayBuilder = builder;
            }

            /// <summary>Inject the menu builder.</summary>
            public void BindMenuBuilder(MenuBuilder builder)
            {
                this._menuBuilder = builder;
            }

            /// <summary>Inject the KSP popup this controller drives.</summary>
            public void BindPopupDialog(PopupDialog popupDialog)
            {
                this._popupDialog = popupDialog;
            }

            /// <summary>Inject the popup's canvas group.</summary>
            public void BindCanvasGroup(CanvasGroup canvasGroup)
            {
                this._canvasGroup = canvasGroup;
            }

            public void InitializePosition(Vector2 pos)
            {
                this._position = pos;
                this._hasPosition = true;
            }

            /// <summary>
            /// Unity callback. Sets up the controller; its counterpart is <see cref="OnDestroy"/>.
            /// </summary>
            public void Start()
            {
                ViewModel?.OnShowMenu.Add(OnShowMenu);
                if( ViewModel != null )
                {
                    OnShowMenu(ViewModel.MenuDisplayed);
                }

                _popupDialog?.onDestroy.AddListener(OnPopupDestroyed);
                GameEvents.onLevelWasLoaded.Add(OnLevelWasLoaded);
            }

            /// <summary>
            /// Unity callback. Tears down what <see cref="Start"/> set up.
            /// </summary>
            public void OnDestroy()
            {
                // Pure cleanup: do NOT dismiss the dialog here. This runs on both teardown paths (the
                // owner-driven Dismiss and KSP destroying the popup itself), and in both the popup is
                // already being destroyed — dismissing again here would re-enter the teardown.
                ViewModel?.OnShowMenu.Remove(OnShowMenu);
                GameEvents.onLevelWasLoaded.Remove(OnLevelWasLoaded);
                _popupDialog?.onDestroy.RemoveListener(OnPopupDestroyed);
            }

            /// <summary>
            /// Close the window on the owner's request. The controller is destroyed as a result, so the
            /// owner must drop its reference afterwards.
            /// </summary>
            public void Dismiss()
            {
                // Unhook our KSP listener first: dismissing triggers the destruction, and we must not
                // re-enter OnPopupDestroyed to notify the owner of a close it requested itself.
                _popupDialog?.onDestroy.RemoveListener(OnPopupDestroyed);
                _popupDialog?.Dismiss();
            }

            /// <summary>
            /// Called when KSP destroys the popup on its own (e.g. the user presses Escape) — a close not
            /// initiated through <see cref="Hide"/> or <see cref="Dismiss"/>.
            /// </summary>
            private void OnPopupDestroyed()
            {
                // Grab the position while the transform is still alive, then let the owner resync
                // (reset the toolbar toggle, close the rest of the UI).
                CaptureWindowPosition();
                OnClosed.Fire();
            }

            /// <summary>
            /// Re-asserts the window's interactivity after a KSP scene change (no-op if it is closed).
            /// </summary>
            private void OnLevelWasLoaded(GameScenes scene)
            {
                this.RestoreInteractivity();
            }

            // =====================
            // Public API
            // =====================

            /// <summary>
            /// Show the window at its last saved position. Also used to re-open it after a
            /// <see cref="Hide"/>.
            /// </summary>
            public void Show()
            {
                _popupDialog?.gameObject.SetActive(true);
                // Restore the dragged position one frame later: KSP re-applies the spawn position on the
                // layout pass that follows activation, so applying it now would be overwritten.
                StartCoroutine(_ApplyUguiPositionAfterLayout());
            }

            /// <summary>
            /// Apply the saved window position and reveal the window.
            /// </summary>
            private IEnumerator _ApplyUguiPositionAfterLayout()
            {
                // Wait one frame so KSP's layout pass (which re-applies the spawn position) has settled.
                yield return null;
                if( _hasPosition )
                {
                    SetPosition(_position);
                }
                Reveal();
            }

            /// <summary>
            /// Hide the window, saving its position. The controller stays alive so the window can be
            /// shown again later.
            /// </summary>
            public void Hide()
            {
                CaptureWindowPosition();
                _popupDialog?.gameObject.SetActive(false);
            }

            // =====================
            // Internal API
            // =====================
            
            /// <summary>Move the window to the given position, preserving its current z.</summary>
            private void SetPosition(Vector2 position)
            {
                if( _popupDialog.RTrf == null ) return;
                Vector3 lp = _popupDialog.RTrf.localPosition;
                _popupDialog.RTrf.localPosition = new Vector3(position.x, position.y, lp.z);
            }

            /// <summary>Make the window visible.</summary>
            private void Reveal()
            {
                if (_canvasGroup != null)
                {
                    _canvasGroup.alpha = 1f;
                }
            }

            /// <summary>Re-enable pointer interaction on the window.</summary>
            private void RestoreInteractivity()
            {
                // KSP bug: on a scene change, UIMasterController.OnSceneChange clears the modal stack via
                // UnregisterModalDialogs() WITHOUT restoring blocksRaycasts on the surviving non-modal
                // dialogs. Our window persists across scenes, so if a modal dialog was up before the
                // transition (e.g. the KSC "exit to main menu" confirmation), the window stays visible
                // but non-interactive. We re-assert the resting state of a non-modal dialog.
                if (_canvasGroup != null)
                {
                    _canvasGroup.blocksRaycasts = true;
                }
            }

            /// <summary>Report the window's current position so the owner can persist it.</summary>
            private void CaptureWindowPosition()
            {
                if (_popupDialog != null && _popupDialog.RTrf != null)
                {
                    _position = _popupDialog.RTrf.localPosition;
                    _hasPosition = true;
                    OnPositionCaptured.Fire(_position);
                }
            }

            /// <summary>Show or hide the menu and its overlay.</summary>
            private void OnShowMenu(bool show)
            {
                if( _overlayController == null )
                {
                    _overlayController = _overlayBuilder.Create(() => ViewModel.CloseMenu());
                    _overlayController.transform.SetParent(gameObject.transform, false);
                }

                if( _menuController == null )
                {
                    _menuController = _menuBuilder.Create();
                    _menuController.transform.SetParent(gameObject.transform, false);
                }

                _overlayController.gameObject.SetActive(show);
                _menuController.gameObject.SetActive(show);
            }
        }
    }
}
