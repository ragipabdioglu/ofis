using System;
using System.Collections.Generic;
using OFIS.Roles;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UiText = UnityEngine.UI.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endif

namespace OFIS.Playable
{
    public sealed class LocalPlayableSessionRunner : MonoBehaviour
    {
        [SerializeField] private bool autoBoot = true;
        [SerializeField] private bool suppressLegacyDebugBehaviours = true;

        private readonly PlayableSessionService service = new PlayableSessionService();
        private readonly Dictionary<string, UiText> labels = new Dictionary<string, UiText>();
        private readonly Dictionary<string, Button> buttons = new Dictionary<string, Button>();

        private Canvas rootCanvas;
        private Canvas hudCanvas;
        private Canvas modalCanvas;
        private Canvas debugCanvas;
        private UiText statusLabel;
        private UiText hudLabel;
        private UiText modalLabel;
        private UiText debugLabel;
        private Image menuArt;
        private Image lobbyArt;
        private Image roleArt;
        private Image resultArt;

        public PlayableSessionService Service => service;

        private void Awake()
        {
            if (suppressLegacyDebugBehaviours)
                DisableLegacyDebugBehaviours();

            EnsureEventSystem();
            BuildCanvases();
            BuildUi();
        }

        private void Start()
        {
            if (autoBoot)
                Boot();
        }

        private void Update()
        {
            if (WasKeyPressed(KeyCode.F5))
                Boot();
            if (WasKeyPressed(KeyCode.F6))
                RunHappyPathShortcut();
        }

        private static bool WasKeyPressed(KeyCode keyCode)
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current == null)
                return false;

            return keyCode == KeyCode.F5
                ? Keyboard.current.f5Key.wasPressedThisFrame
                : keyCode == KeyCode.F6 && Keyboard.current.f6Key.wasPressedThisFrame;
#else
            return Input.GetKeyDown(keyCode);
#endif
        }

        public void Boot()
        {
            service.BootToMainMenu();
            Refresh();
        }

        private void StartLocalMatch()
        {
            Apply(service.EnterLobby());
        }

        private void ReadyAll()
        {
            Apply(service.ReadyLocalPlayerAndAutoReadyBots());
        }

        private void RevealRoles()
        {
            Apply(service.AssignRolesAndReveal());
        }

        private void EnterOffice()
        {
            Apply(service.AcknowledgeRoleReveal());
        }

        private void CompleteTask()
        {
            Apply(service.CompleteTask());
        }

        private void KillOrScript()
        {
            Apply(service.KillOrScriptVictim());
        }

        private void CarryCorpse()
        {
            Apply(service.CarryCorpse());
        }

        private void DropCorpse()
        {
            Apply(service.DropCorpse());
        }

        private void InspectCorpse()
        {
            Apply(service.InspectCorpse());
        }

        private void StartSabotage()
        {
            Apply(service.StartSabotage());
        }

        private void RepairSabotage()
        {
            Apply(service.RepairSabotage());
        }

        private void StartMeeting()
        {
            Apply(service.StartMeeting());
        }

        private void ResolveVote()
        {
            Apply(service.ResolveMeetingVote());
        }

        private void ReturnOffice()
        {
            Apply(service.ReturnToOffice());
        }

        private void FinalAccusation()
        {
            Apply(service.EnterFinalAccusation());
        }

        private void SubmitCorrectFinal()
        {
            Apply(service.SubmitFinalAccusation(true));
        }

        private void SubmitWrongFinal()
        {
            Apply(service.SubmitFinalAccusation(false));
        }

        private void Cleanup()
        {
            Apply(service.CleanupToMainMenu());
        }

        private void Apply(PlayableActionResult result)
        {
            if (!result.Passed)
                Debug.LogWarning($"[LocalPlayableSessionRunner] Blocked: {result.Message}");
            else
                Debug.Log($"[LocalPlayableSessionRunner] {result.Message}");

            Refresh();
        }

        private void RunHappyPathShortcut()
        {
            service.BootToMainMenu();
            service.EnterLobby();
            service.ReadyLocalPlayerAndAutoReadyBots();
            service.AssignRolesAndReveal();
            service.AcknowledgeRoleReveal();
            service.CompleteTask();
            service.KillOrScriptVictim();
            service.InspectCorpse();
            service.StartSabotage();
            service.RepairSabotage();
            service.StartMeeting();
            service.ResolveMeetingVote();
            service.ReturnToOffice();
            service.EnterFinalAccusation();
            service.SubmitFinalAccusation(true);
            Refresh();
        }

        private void Refresh()
        {
            var snapshot = service.Snapshot;
            statusLabel.text = $"OFIS Playable Integration\nState: {snapshot.State}\nStatus: {snapshot.Status}";
            hudLabel.text =
                $"HUD\nPlayer: {snapshot.ActivePlayerName}\nRole: {snapshot.ActiveRole}\nCompany: {snapshot.CompanyHealth}\nTasks: {snapshot.CompletedTasks}\nMeeting: {snapshot.MeetingCount}\nVoice: Local placeholder\nPrompt: Use buttons or F6 happy path";
            modalLabel.text = BuildModalText(snapshot);
            debugLabel.text =
                $"DebugCanvas\nParticipants: {snapshot.Participants.Count}/8\nMeeting transient clean: {snapshot.MeetingTransientClean}\nCorpse: {snapshot.CorpseSpawned}, inspected: {snapshot.CorpseInspected}\nSabotage: {snapshot.SabotageActive}\nResult ready: {snapshot.ResultReady}\nF5 reset, F6 happy path";

            labels["participants"].text = BuildParticipantText(snapshot);
            RefreshAssetVisibility(snapshot.State, snapshot.ActiveRole, service.ResultWinnerKey);
            UpdateButtons(snapshot.State);
        }

        private void RefreshAssetVisibility(PlayableSessionState state, PlayerRole activeRole, string resultWinnerKey)
        {
            menuArt.gameObject.SetActive(state == PlayableSessionState.MainMenu);
            lobbyArt.gameObject.SetActive(state == PlayableSessionState.Lobby);
            roleArt.gameObject.SetActive(state == PlayableSessionState.RoleReveal);
            resultArt.gameObject.SetActive(state == PlayableSessionState.MatchResult);

            if (state == PlayableSessionState.RoleReveal)
            {
                roleArt.sprite = LoadSprite(activeRole == PlayerRole.Killer
                    ? "Assets/assets-img/product-phase-2/icons/roles/role_killer.png"
                    : activeRole == PlayerRole.Victim
                        ? "Assets/assets-img/product-phase-2/icons/roles/role_victim.png"
                        : "Assets/assets-img/product-phase-2/icons/roles/role_detective.png");
            }

            if (state == PlayableSessionState.MatchResult)
            {
                resultArt.sprite = LoadSprite(resultWinnerKey == "result.good_side_win"
                    ? "Assets/assets-img/product-phase-2/result/result_good_win_banner.png"
                    : "Assets/assets-img/product-phase-2/result/result_killer_win_banner.png");
            }
        }

        private static string BuildModalText(PlayableSessionSnapshot snapshot)
        {
            switch (snapshot.State)
            {
                case PlayableSessionState.MainMenu:
                    return "Main Menu\nStart Local Match opens the local 8-player lobby.";
                case PlayableSessionState.Lobby:
                    return "Lobby Ready UX\nLocal player ready triggers deterministic bot auto-ready.";
                case PlayableSessionState.RoleReveal:
                    return snapshot.ActiveRole == PlayerRole.Killer
                        ? "Role Reveal\nYou are Killer. Only victim targets are known to you."
                        : $"Role Reveal\nYou are {snapshot.ActiveRole}. No killer or target list is shown.";
                case PlayableSessionState.Meeting:
                    return "Meeting\nChoose an official action, resolve vote, then return to office.";
                case PlayableSessionState.FinalAccusation:
                    return "Final Accusation\nSubmit exactly the remaining killer list. Use correct/wrong buttons for local proof.";
                case PlayableSessionState.MatchResult:
                    return $"Match Result\n{snapshot.Status}\nUse Cleanup / Reset to prove second-match restart.";
                default:
                    return "Onboarding\nTask, corpse, sabotage, meeting and final actions are available through UI.";
            }
        }

        private static string BuildParticipantText(PlayableSessionSnapshot snapshot)
        {
            var lines = new List<string> { "Lobby / Participants" };
            for (var i = 0; i < snapshot.Participants.Count; i++)
            {
                var p = snapshot.Participants[i];
                lines.Add($"{p.DisplayName} | {(p.IsLocalPlayer ? "LOCAL" : "BOT")} | Ready={p.IsReady} | Alive={p.IsAlive} | Tasks={p.CompletedTasks}");
            }

            return string.Join("\n", lines);
        }

        private void UpdateButtons(PlayableSessionState state)
        {
            SetButton("start", state == PlayableSessionState.MainMenu);
            SetButton("ready", state == PlayableSessionState.Lobby);
            SetButton("reveal", state == PlayableSessionState.Lobby && service.CanStartMatch());
            SetButton("office", state == PlayableSessionState.RoleReveal);
            SetButton("task", state == PlayableSessionState.Office);
            SetButton("kill", state == PlayableSessionState.Office);
            SetButton("carry", state == PlayableSessionState.Office);
            SetButton("drop", state == PlayableSessionState.Office);
            SetButton("inspect", state == PlayableSessionState.Office);
            SetButton("sabotage", state == PlayableSessionState.Office);
            SetButton("repair", state == PlayableSessionState.Office);
            SetButton("meeting", state == PlayableSessionState.Office);
            SetButton("vote", state == PlayableSessionState.Meeting);
            SetButton("return", state == PlayableSessionState.Meeting);
            SetButton("final", state == PlayableSessionState.Office || state == PlayableSessionState.Meeting);
            SetButton("correct", state == PlayableSessionState.FinalAccusation);
            SetButton("wrong", state == PlayableSessionState.FinalAccusation);
            SetButton("cleanup", state == PlayableSessionState.MatchResult);
        }

        private void SetButton(string key, bool interactable)
        {
            if (buttons.TryGetValue(key, out var button))
                button.interactable = interactable;
        }

        private void BuildCanvases()
        {
            rootCanvas = CreateCanvas("PlayableRootCanvas", 20);
            hudCanvas = CreateCanvas("HudCanvas", 30);
            modalCanvas = CreateCanvas("ModalCanvas", 40);
            debugCanvas = CreateCanvas("DebugCanvas", 50);
        }

        private Canvas CreateCanvas(string canvasName, int sortingOrder)
        {
            var go = new GameObject(canvasName);
            go.transform.SetParent(transform, false);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;
            go.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            go.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private void BuildUi()
        {
            statusLabel = CreateText(rootCanvas.transform, "Status", new Rect(18, -18, 520, 96), 18, TextAnchor.UpperLeft);
            labels["participants"] = CreateText(rootCanvas.transform, "Participants", new Rect(18, -122, 520, 300), 15, TextAnchor.UpperLeft);
            hudLabel = CreateText(hudCanvas.transform, "Hud", new Rect(-420, -18, 400, 190), 16, TextAnchor.UpperLeft, new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1));
            modalLabel = CreateText(modalCanvas.transform, "Modal", new Rect(-330, 150, 660, 150), 20, TextAnchor.MiddleCenter, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(.5f, .5f));
            debugLabel = CreateText(debugCanvas.transform, "Debug", new Rect(-360, 18, 342, 180), 13, TextAnchor.UpperLeft, new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0));
            menuArt = CreateAssetImage(modalCanvas.transform, "MenuArt", "Assets/assets-img/product-phase-2/menu/ofis_logo_title.png", new Rect(-260, 315, 520, 120));
            lobbyArt = CreateAssetImage(modalCanvas.transform, "LobbyArt", "Assets/assets-img/product-phase-2/lobby/lobby_player_slot_frame.png", new Rect(-260, 315, 520, 120));
            roleArt = CreateAssetImage(modalCanvas.transform, "RoleArt", "Assets/assets-img/product-phase-2/icons/roles/role_detective.png", new Rect(-70, 310, 140, 140));
            resultArt = CreateAssetImage(modalCanvas.transform, "ResultArt", "Assets/assets-img/product-phase-2/result/result_good_win_banner.png", new Rect(-260, 315, 520, 146));

            var x = 560f;
            var y = -18f;
            CreateButton("start", "Start Local Match", x, y, StartLocalMatch);
            CreateButton("ready", "Ready + Auto Bots", x, y - 42, ReadyAll);
            CreateButton("reveal", "Reveal Roles", x, y - 84, RevealRoles);
            CreateButton("office", "Enter Office", x, y - 126, EnterOffice);
            CreateButton("task", "Complete Task", x, y - 190, CompleteTask);
            CreateButton("kill", "Kill / Script Kill", x, y - 232, KillOrScript);
            CreateButton("carry", "Carry Corpse", x, y - 274, CarryCorpse);
            CreateButton("drop", "Drop Corpse", x, y - 316, DropCorpse);
            CreateButton("inspect", "Inspect Corpse", x, y - 358, InspectCorpse);
            CreateButton("sabotage", "Start Sabotage", x + 190, y - 190, StartSabotage);
            CreateButton("repair", "Repair Sabotage", x + 190, y - 232, RepairSabotage);
            CreateButton("meeting", "Start Meeting", x + 190, y - 274, StartMeeting);
            CreateButton("vote", "Resolve Vote", x + 190, y - 316, ResolveVote);
            CreateButton("return", "Return Office", x + 190, y - 358, ReturnOffice);
            CreateButton("final", "Final Accusation", x + 380, y - 190, FinalAccusation);
            CreateButton("correct", "Submit Correct", x + 380, y - 232, SubmitCorrectFinal);
            CreateButton("wrong", "Submit Wrong", x + 380, y - 274, SubmitWrongFinal);
            CreateButton("cleanup", "Cleanup / Reset", x + 380, y - 316, Cleanup);
        }

        private UiText CreateText(Transform parent, string name, Rect rect, int size, TextAnchor alignment, Vector2? anchorMin = null, Vector2? anchorMax = null, Vector2? pivot = null)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var image = go.AddComponent<Image>();
            image.color = new Color(0.04f, 0.07f, 0.09f, 0.78f);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin ?? new Vector2(0, 1);
            rt.anchorMax = anchorMax ?? new Vector2(0, 1);
            rt.pivot = pivot ?? new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(rect.x, rect.y);
            rt.sizeDelta = new Vector2(rect.width, rect.height);

            var textGo = new GameObject($"{name}Text", typeof(RectTransform));
            textGo.transform.SetParent(go.transform, false);
            var text = textGo.AddComponent<UiText>();
            var font = ResolveUiFont();
            if (font != null)
                text.font = font;

            text.fontSize = size;
            text.color = Color.white;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            var textRt = textGo.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = new Vector2(10, 8);
            textRt.offsetMax = new Vector2(-10, -8);
            return text;
        }

        private void CreateButton(string key, string label, float x, float y, UnityEngine.Events.UnityAction onClick)
        {
            var go = new GameObject(label, typeof(RectTransform));
            go.transform.SetParent(rootCanvas.transform, false);
            var image = go.AddComponent<Image>();
            image.color = new Color(0.14f, 0.2f, 0.25f, 0.94f);
            var button = go.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(onClick);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(x, y);
            rt.sizeDelta = new Vector2(176, 34);

            var textGo = new GameObject("Text", typeof(RectTransform));
            textGo.transform.SetParent(go.transform, false);
            var text = textGo.AddComponent<UiText>();
            var font = ResolveUiFont();
            if (font != null)
                text.font = font;

            text.fontSize = 14;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            text.text = label;
            var textRt = textGo.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;

            buttons[key] = button;
        }

        private static Font ResolveUiFont()
        {
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font != null)
                return font;

            return Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        private Image CreateAssetImage(Transform parent, string name, string assetPath, Rect rect)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var image = go.AddComponent<Image>();
            image.color = Color.white;
            image.sprite = LoadSprite(assetPath);
            image.preserveAspect = true;
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(.5f, .5f);
            rt.anchorMax = new Vector2(.5f, .5f);
            rt.pivot = new Vector2(.5f, .5f);
            rt.anchoredPosition = new Vector2(rect.x, rect.y);
            rt.sizeDelta = new Vector2(rect.width, rect.height);
            return image;
        }

        private static Sprite LoadSprite(string assetPath)
        {
#if UNITY_EDITOR
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture == null)
                return null;

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 100f);
#else
            return null;
#endif
        }

        private static void EnsureEventSystem()
        {
            var existing = FindAnyObjectByType<EventSystem>();
            if (existing != null)
            {
#if ENABLE_INPUT_SYSTEM
                var standalone = existing.GetComponent<StandaloneInputModule>();
                if (standalone != null)
                {
                    standalone.enabled = false;
                    Destroy(standalone);
                }

                if (existing.GetComponent<InputSystemUIInputModule>() == null)
                    existing.gameObject.AddComponent<InputSystemUIInputModule>();
#endif
                return;
            }

            var eventSystem = new GameObject("PlayableEventSystem");
            eventSystem.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
            eventSystem.AddComponent<InputSystemUIInputModule>();
#else
            eventSystem.AddComponent<StandaloneInputModule>();
#endif
        }

        private void DisableLegacyDebugBehaviours()
        {
            var behaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include);
            for (var i = 0; i < behaviours.Length; i++)
            {
                var behaviour = behaviours[i];
                if (behaviour == null || behaviour == this)
                    continue;

                var type = behaviour.GetType();
                if (type.Namespace != null && type.Namespace.StartsWith("OFIS.Playable", StringComparison.Ordinal))
                    continue;

                var name = type.Name;
                if (name.Contains("Validator")
                    || name.Contains("DebugHarness")
                    || name.Contains("DebugHud")
                    || name.Contains("HudStub")
                    || name == "MvpAllPhasesValidationRunner")
                {
                    behaviour.enabled = false;
                }
            }
        }
    }
}
