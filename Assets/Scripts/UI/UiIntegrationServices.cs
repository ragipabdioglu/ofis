using System.Collections.Generic;
using OFIS.Roles;

namespace OFIS.UI
{
    public sealed class UiHudComposerService
    {
        public HudViewModel Build(PlayerRole ownRole, string departmentKey, int secondsRemaining, int companyHealth, string voiceStatusKey, int totalTasks, int completedTasks)
        {
            int minutes = secondsRemaining < 0 ? 0 : secondsRemaining / 60;
            int seconds = secondsRemaining < 0 ? 0 : secondsRemaining % 60;
            string timerText = $"{minutes:00}:{seconds:00}";

            return new HudViewModel(ownRole, departmentKey, timerText, companyHealth, voiceStatusKey, totalTasks, completedTasks, true);
        }
    }

    public sealed class UiRoleRevealService
    {
        public RoleRevealViewModel Build(PlayerRole ownRole, IReadOnlyList<string> victimTargetIds)
        {
            IReadOnlyList<string> targets = ownRole == PlayerRole.Killer ? victimTargetIds : new List<string>();
            return new RoleRevealViewModel(ownRole, targets, true);
        }
    }

    public sealed class UiPanelComposerService
    {
        public UiPanelState BuildKillerPanel(IReadOnlyList<string> targetIds, float killCooldownSeconds)
        {
            bool enabled = targetIds != null && targetIds.Count > 0 && killCooldownSeconds <= 0f;
            return Build(
                "ui.panel.killer",
                UiAudienceType.KillerOnly,
                true,
                true,
                new[] { new UiActionBinding(UiActionCommandType.KillTarget, "ui.action.kill_target", enabled, enabled ? string.Empty : "ui.error.kill_cooldown") },
                new[] { "ui.killer.targets", "ui.killer.cooldown" });
        }

        public UiPanelState BuildTaskPanel(bool hasActiveTask)
        {
            return Build(
                "ui.panel.task",
                UiAudienceType.OwnerOnly,
                true,
                true,
                new[] { new UiActionBinding(UiActionCommandType.StartTask, "ui.action.start_task", hasActiveTask, hasActiveTask ? string.Empty : "ui.error.no_task") },
                new[] { "ui.task.list", "ui.task.progress" });
        }

        public UiPanelState BuildMeetingPanel(bool canJoinMeeting)
        {
            return Build(
                "ui.panel.meeting",
                UiAudienceType.OwnerOnly,
                true,
                true,
                new[] { new UiActionBinding(UiActionCommandType.JoinMeeting, "ui.action.join_meeting", canJoinMeeting, canJoinMeeting ? string.Empty : "ui.error.meeting_locked") },
                new[] { "ui.meeting.timer", "ui.meeting.status" });
        }

        public UiPanelState BuildVotingPanel(bool canVote)
        {
            return Build(
                "ui.panel.voting",
                UiAudienceType.OwnerOnly,
                true,
                true,
                new[] { new UiActionBinding(UiActionCommandType.CastVote, "ui.action.cast_vote", canVote, canVote ? string.Empty : "ui.error.vote_blocked") },
                new[] { "ui.voting.proposals", "ui.voting.majority" });
        }

        public UiPanelState BuildReportPanel(bool hasReport)
        {
            return Build(
                "ui.panel.report",
                UiAudienceType.OwnerOnly,
                hasReport,
                true,
                new[] { new UiActionBinding(UiActionCommandType.OpenReport, "ui.action.open_report", hasReport, hasReport ? string.Empty : "ui.error.no_report") },
                new[] { "ui.report.summary", "ui.report.confidence" });
        }

        public UiPanelState BuildFinalAccusationPanel(bool canSubmit)
        {
            return Build(
                "ui.panel.final_accusation",
                UiAudienceType.OwnerOnly,
                true,
                true,
                new[] { new UiActionBinding(UiActionCommandType.SubmitFinalAccusation, "ui.action.submit_final_accusation", canSubmit, canSubmit ? string.Empty : "ui.error.final_list_invalid") },
                new[] { "ui.final_accusation.selection", "ui.final_accusation.remaining_count" });
        }

        public UiPanelState BuildCorpseInteractionPanel(bool canInspect, bool canAnnounce)
        {
            return Build(
                "ui.panel.corpse_interaction",
                UiAudienceType.OwnerOnly,
                true,
                true,
                new[]
                {
                    new UiActionBinding(UiActionCommandType.InspectCorpse, "ui.action.inspect_corpse", canInspect, canInspect ? string.Empty : "ui.error.inspect_blocked"),
                    new UiActionBinding(UiActionCommandType.AnnounceCorpse, "ui.action.announce_corpse", canAnnounce, canAnnounce ? string.Empty : "ui.error.announce_blocked")
                },
                new[] { "ui.corpse.inspect", "ui.corpse.announce" });
        }

        public UiPanelState BuildCorpseCarryPanel(bool canCarry, bool carrying)
        {
            return Build(
                "ui.panel.corpse_carry",
                UiAudienceType.KillerOnly,
                true,
                true,
                new[]
                {
                    new UiActionBinding(UiActionCommandType.CarryCorpse, "ui.action.carry_corpse", canCarry && !carrying, canCarry ? string.Empty : "ui.error.carry_blocked"),
                    new UiActionBinding(UiActionCommandType.DropCorpse, "ui.action.drop_corpse", carrying, carrying ? string.Empty : "ui.error.not_carrying")
                },
                new[] { "ui.corpse.carry", "ui.corpse.drop" });
        }

        public UiPanelState BuildSabotageRepairPanel(bool canSabotage, bool canRepair)
        {
            return Build(
                "ui.panel.sabotage_repair",
                UiAudienceType.OwnerOnly,
                true,
                true,
                new[]
                {
                    new UiActionBinding(UiActionCommandType.StartSabotage, "ui.action.start_sabotage", canSabotage, canSabotage ? string.Empty : "ui.error.sabotage_blocked"),
                    new UiActionBinding(UiActionCommandType.StartRepair, "ui.action.start_repair", canRepair, canRepair ? string.Empty : "ui.error.repair_blocked")
                },
                new[] { "ui.sabotage.alert", "ui.repair.progress" });
        }

        public UiPanelState BuildDetectiveDashboard(bool isDetective)
        {
            return Build(
                "ui.panel.detective_dashboard",
                UiAudienceType.DetectiveOnly,
                isDetective,
                true,
                new[] { new UiActionBinding(UiActionCommandType.PinEvidence, "ui.action.pin_evidence", isDetective, isDetective ? string.Empty : "ui.error.detective_only") },
                new[] { "ui.detective.pins", "ui.detective.flags", "ui.detective.timeline" });
        }

        public UiPanelState BuildVictimNotePanel(bool canWriteNote)
        {
            return Build(
                "ui.panel.victim_note",
                UiAudienceType.VictimOnly,
                true,
                true,
                new[] { new UiActionBinding(UiActionCommandType.SaveVictimNote, "ui.action.save_victim_note", canWriteNote, canWriteNote ? string.Empty : "ui.error.note_blocked") },
                new[] { "ui.victim_note.editor", "ui.victim_note.remaining" });
        }

        public UiPanelState BuildDeadPlayerPanel(bool isDead)
        {
            return Build(
                "ui.panel.dead_player",
                UiAudienceType.DeadOnly,
                isDead,
                true,
                new[] { new UiActionBinding(UiActionCommandType.CompleteDeadTask, "ui.action.complete_dead_task", isDead, isDead ? string.Empty : "ui.error.alive_only") },
                new[] { "ui.dead.spectator", "ui.dead.tasks" });
        }

        public UiPanelState Build(string panelId, UiAudienceType audienceType, bool visible, bool ownerOnly, IEnumerable<UiActionBinding> actions, IEnumerable<string> textKeys)
        {
            return new UiPanelState(panelId, audienceType, visible, ownerOnly, actions, textKeys);
        }
    }

    public sealed class UiMatchResultService
    {
        public MatchResultViewModel Build(string winnerKey, IReadOnlyList<string> revealRows)
        {
            return new MatchResultViewModel(winnerKey, revealRows, true);
        }
    }

    public sealed class UiTooltipErrorMappingService
    {
        private readonly Dictionary<string, string> _mapping = new Dictionary<string, string>
        {
            { "ui.error.kill_cooldown", "ui.tooltip.kill_cooldown" },
            { "ui.error.meeting_locked", "ui.tooltip.meeting_locked" },
            { "ui.error.final_list_invalid", "ui.tooltip.final_list_invalid" },
            { "ui.error.sabotage_blocked", "ui.tooltip.sabotage_blocked" },
            { "ui.error.detective_only", "ui.tooltip.detective_only" }
        };

        public string ResolveTooltipKey(string errorKey)
        {
            if (string.IsNullOrWhiteSpace(errorKey))
                return "ui.tooltip.none";

            return _mapping.TryGetValue(errorKey, out string tooltipKey) ? tooltipKey : "ui.tooltip.generic_error";
        }
    }

    public sealed class UiLocalizationKeyRegistry
    {
        private readonly HashSet<string> _keys = new HashSet<string>
        {
            "ui.hud.role_mini",
            "ui.hud.department",
            "ui.hud.timer",
            "ui.hud.company",
            "ui.hud.voice",
            "ui.hud.task_list",
            "ui.role_reveal.title",
            "ui.panel.killer",
            "ui.panel.task",
            "ui.panel.meeting",
            "ui.panel.voting",
            "ui.panel.report",
            "ui.panel.final_accusation",
            "ui.panel.corpse_interaction",
            "ui.panel.corpse_carry",
            "ui.panel.sabotage_repair",
            "ui.panel.detective_dashboard",
            "ui.panel.victim_note",
            "ui.panel.dead_player",
            "ui.panel.match_result"
        };

        public bool Contains(string key)
        {
            return !string.IsNullOrWhiteSpace(key) && _keys.Contains(key);
        }

        public int Count => _keys.Count;
    }

    public sealed class UiRoleLeakGuardService
    {
        private static readonly string[] ForbiddenPublicTokens =
        {
            "killer",
            "victim",
            "detective",
            "murderer",
            "saboteur",
            "role:",
            "target:"
        };

        public bool IsPanelSafeForPublic(UiPanelState panel)
        {
            if (panel == null)
                return false;

            if (panel.AudienceType != UiAudienceType.Public)
                return panel.IsOwnerOnly;

            if (ContainsForbiddenToken(panel.PanelId))
                return false;

            foreach (string key in panel.TextKeys)
            {
                if (ContainsForbiddenToken(key))
                    return false;
            }

            foreach (UiActionBinding action in panel.Actions)
            {
                if (ContainsForbiddenToken(action.LocalizationKey) || ContainsForbiddenToken(action.ErrorKey))
                    return false;
            }

            return true;
        }

        public bool IsHudOwnerSafe(HudViewModel hud)
        {
            return hud.IsOwnerOnly && hud.TotalTaskCount >= hud.CompletedTaskCount;
        }

        public bool IsRoleRevealOwnerSafe(RoleRevealViewModel reveal)
        {
            if (!reveal.IsOwnerOnly)
                return false;

            if (reveal.OwnRole != PlayerRole.Killer)
                return reveal.TargetIds.Count == 0;

            return reveal.TargetIds.Count > 0;
        }

        private static bool ContainsForbiddenToken(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            string lower = value.ToLowerInvariant();
            for (int i = 0; i < ForbiddenPublicTokens.Length; i++)
            {
                if (lower.Contains(ForbiddenPublicTokens[i]))
                    return true;
            }

            return false;
        }
    }
}
