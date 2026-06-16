using OFIS.Players;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Communication
{
    public sealed class CommunicationPhaseFourteenDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private CommunicationPhaseFourteenPackageType packageType;
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly MockVoiceProviderAdapter _mockProvider = new MockVoiceProviderAdapter();
        private readonly ProximityVoiceService _proximityVoiceService = new ProximityVoiceService();
        private readonly RoomAcousticPenaltyService _roomPenaltyService = new RoomAcousticPenaltyService();
        private readonly MeetingVoiceRouteService _meetingVoiceRouteService = new MeetingVoiceRouteService();
        private readonly DeadVoiceRouteService _deadVoiceRouteService = new DeadVoiceRouteService();
        private readonly ExposedKillerVoiceRouteService _exposedKillerVoiceRouteService = new ExposedKillerVoiceRouteService();
        private readonly PushToTalkService _pushToTalkService = new PushToTalkService();
        private readonly VoiceLocalSettingsService _localSettingsService = new VoiceLocalSettingsService();
        private readonly VoiceUiStatusService _uiStatusService = new VoiceUiStatusService();
        private readonly VoiceReconnectRestoreService _reconnectRestoreService = new VoiceReconnectRestoreService();

        private void Start()
        {
            if (validateOnStart)
                ValidatePackage();
        }

        [ContextMenu("Validate Communication Phase 14 Package")]
        public void ValidatePackage()
        {
            switch (packageType)
            {
                case CommunicationPhaseFourteenPackageType.VoiceProviderAdapter:
                    ValidateVoiceProviderAdapter();
                    break;
                case CommunicationPhaseFourteenPackageType.MockVoiceTest:
                    ValidateMockVoiceTest();
                    break;
                case CommunicationPhaseFourteenPackageType.ProximityVoice:
                    ValidateProximityVoice();
                    break;
                case CommunicationPhaseFourteenPackageType.RoomAcousticPenalty:
                    ValidateRoomAcousticPenalty();
                    break;
                case CommunicationPhaseFourteenPackageType.MeetingVoiceEligibility:
                    ValidateMeetingVoiceEligibility();
                    break;
                case CommunicationPhaseFourteenPackageType.MeetingRoomExitCutsVoice:
                    ValidateMeetingRoomExitCutsVoice();
                    break;
                case CommunicationPhaseFourteenPackageType.LateObserverNoVoice:
                    ValidateLateObserverNoVoice();
                    break;
                case CommunicationPhaseFourteenPackageType.DeadVoice:
                    ValidateDeadVoice();
                    break;
                case CommunicationPhaseFourteenPackageType.ExposedKillerDeadVoice:
                    ValidateExposedKillerDeadVoice();
                    break;
                case CommunicationPhaseFourteenPackageType.PushToTalkDefault:
                    ValidatePushToTalkDefault();
                    break;
                case CommunicationPhaseFourteenPackageType.LocalMuteDeafen:
                    ValidateLocalMuteDeafen();
                    break;
                case CommunicationPhaseFourteenPackageType.VoiceUiStatus:
                    ValidateVoiceUiStatus();
                    break;
                case CommunicationPhaseFourteenPackageType.ReconnectVoiceRestore:
                    ValidateReconnectVoiceRestore();
                    break;
                case CommunicationPhaseFourteenPackageType.PhaseClosure:
                    ValidatePhaseClosure();
                    break;
            }
        }

        private void ValidateVoiceProviderAdapter()
        {
            VoiceProviderState state = _mockProvider.Connect("player_14a");
            LogResult("VoiceProviderAdapter", state.IsConnected && state.ProviderName == "MockVoice", state.ProviderName);
        }

        private void ValidateMockVoiceTest()
        {
            VoiceProviderState state = _mockProvider.Connect("player_14b");
            LogResult("MockVoiceTest", state.IsConnected, "Mock voice connection accepted.");
        }

        private void ValidateProximityVoice()
        {
            VoiceRouteResult full = _proximityVoiceService.Evaluate(4.5f);
            VoiceRouteResult fade = _proximityVoiceService.Evaluate(5.5f);
            VoiceRouteResult cut = _proximityVoiceService.Evaluate(6.6f);
            bool passed = full.Volume == 1f && fade.Volume > 0f && fade.Volume < 1f && !cut.CanReceive;
            LogResult("ProximityVoice", passed, $"Full={full.Volume:0.##}, Fade={fade.Volume:0.##}, Cut={cut.CanReceive}");
        }

        private void ValidateRoomAcousticPenalty()
        {
            float sameRoom = _roomPenaltyService.ApplyPenalty(1f, true);
            float otherRoom = _roomPenaltyService.ApplyPenalty(1f, false);
            LogResult("RoomAcousticPenalty", sameRoom == 1f && otherRoom < sameRoom, $"OtherRoom={otherRoom:0.##}");
        }

        private void ValidateMeetingVoiceEligibility()
        {
            VoiceRouteResult route = _meetingVoiceRouteService.Evaluate(true, true, true, false);
            LogResult("MeetingVoiceEligibility", route.CanTransmit && route.CanReceive && route.ChannelType == VoiceChannelType.Meeting, route.Reason);
        }

        private void ValidateMeetingRoomExitCutsVoice()
        {
            VoiceRouteResult route = _meetingVoiceRouteService.Evaluate(true, false, true, false);
            LogResult("MeetingRoomExitCutsVoice", !route.CanTransmit && !route.CanReceive, route.Reason);
        }

        private void ValidateLateObserverNoVoice()
        {
            VoiceRouteResult route = _meetingVoiceRouteService.Evaluate(true, true, true, true);
            LogResult("LateObserverNoVoice", !route.CanTransmit && !route.CanReceive, route.Reason);
        }

        private void ValidateDeadVoice()
        {
            VoiceRouteResult deadToDead = _deadVoiceRouteService.Evaluate(PlayerLifeState.Dead, PlayerLifeState.Dead);
            VoiceRouteResult deadToLiving = _deadVoiceRouteService.Evaluate(PlayerLifeState.Dead, PlayerLifeState.Alive);
            LogResult("DeadVoice", deadToDead.CanTransmit && deadToDead.CanReceive && !deadToLiving.CanTransmit, deadToLiving.Reason);
        }

        private void ValidateExposedKillerDeadVoice()
        {
            VoiceChannelType channel = _exposedKillerVoiceRouteService.ResolveChannel(true);
            LogResult("ExposedKillerDeadVoice", channel == VoiceChannelType.Dead, channel.ToString());
        }

        private void ValidatePushToTalkDefault()
        {
            LogResult("PushToTalkDefault", _pushToTalkService.DefaultKey == "V", _pushToTalkService.DefaultKey);
        }

        private void ValidateLocalMuteDeafen()
        {
            VoiceRouteResult route = _proximityVoiceService.Evaluate(1f);
            VoiceLocalSettings muted = _localSettingsService.Build(true, false, "V");
            VoiceLocalSettings deafened = _localSettingsService.Build(false, true, "V");
            VoiceRouteResult mutedRoute = _localSettingsService.ApplyLocalSettings(route, muted);
            VoiceRouteResult deafenedRoute = _localSettingsService.ApplyLocalSettings(route, deafened);
            LogResult("LocalMuteDeafen", !mutedRoute.CanTransmit && !deafenedRoute.CanReceive && deafenedRoute.Volume == 0f, "Mute/deafen applied.");
        }

        private void ValidateVoiceUiStatus()
        {
            VoiceRouteResult route = _proximityVoiceService.Evaluate(1f);
            VoiceUiStatus status = _uiStatusService.Build(route, _localSettingsService.Build(false, false, "V"));
            LogResult("VoiceUiStatus", status.IsSpeaking && status.Label.Contains("Proximity"), status.Label);
        }

        private void ValidateReconnectVoiceRestore()
        {
            VoiceReconnectSnapshot alive = _reconnectRestoreService.Restore("player_alive", PlayerLifeState.Alive, _localSettingsService.Build(false, false, "V"));
            VoiceReconnectSnapshot dead = _reconnectRestoreService.Restore("player_dead", PlayerLifeState.Dead, _localSettingsService.Build(true, false, "V"));
            LogResult("ReconnectVoiceRestore", alive.RestoredChannel == VoiceChannelType.Proximity && dead.RestoredChannel == VoiceChannelType.Dead && dead.LocalSettings.IsMuted, dead.RestoredChannel.ToString());
        }

        private void ValidatePhaseClosure()
        {
            ValidateVoiceProviderAdapter();
            ValidateMockVoiceTest();
            ValidateProximityVoice();
            ValidateRoomAcousticPenalty();
            ValidateMeetingVoiceEligibility();
            ValidateMeetingRoomExitCutsVoice();
            ValidateLateObserverNoVoice();
            ValidateDeadVoice();
            ValidateExposedKillerDeadVoice();
            ValidatePushToTalkDefault();
            ValidateLocalMuteDeafen();
            ValidateVoiceUiStatus();
            ValidateReconnectVoiceRestore();

            LogResult("PhaseClosure", true, "MVP Faz 14 packages 14A-14M are represented.");
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[CommunicationPhaseFourteenDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[CommunicationPhaseFourteenDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
