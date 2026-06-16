using OFIS.Company;

namespace OFIS.Meetings
{
    public sealed class MeetingProductionApplyService
    {
        public MeetingProductionApplyResult Apply(
            MeetingProductionBridgeCommand command,
            CompanyHealthService companyHealthService)
        {
            bool hasCompanyHealthService = companyHealthService != null;
            int healthBefore = hasCompanyHealthService
                ? companyHealthService.CurrentHealth
                : command.CompanyHealthBefore;

            int healthAfter = healthBefore;
            bool appliedHealthDelta = false;

            if (command.ShouldApplyCompanyHealthDelta && command.CompanyHealthDelta != 0)
            {
                if (hasCompanyHealthService)
                {
                    companyHealthService.ApplyDelta(
                        command.CompanyHealthDelta,
                        "Meeting missing eligible player penalty");

                    healthAfter = companyHealthService.CurrentHealth;
                    appliedHealthDelta = true;
                }
                else
                {
                    healthAfter = ClampHealth(healthBefore + command.CompanyHealthDelta);
                }
            }

            return new MeetingProductionApplyResult(
                command,
                hasCompanyHealthService,
                appliedHealthDelta,
                healthBefore,
                healthAfter,
                command.ShouldCloseMeeting,
                command.ShouldResolveWinBranch,
                command.ShouldRunMeetingEndPipeline,
                command.HasSummaryUiState,
                command.SummaryUiState,
                BuildMessage(command, hasCompanyHealthService, appliedHealthDelta));
        }

        private static int ClampHealth(int value)
        {
            if (value < 0)
                return 0;

            if (value > 100)
                return 100;

            return value;
        }

        private static string BuildMessage(
            MeetingProductionBridgeCommand command,
            bool hasCompanyHealthService,
            bool appliedHealthDelta)
        {
            if (command.ShouldApplyCompanyHealthDelta && !hasCompanyHealthService)
                return "Company health delta was resolved but no CompanyHealthService was available.";

            if (appliedHealthDelta)
                return "Company health delta applied.";

            if (command.ShouldCloseMeeting)
                return "Meeting close hook requested.";

            if (command.ShouldResolveWinBranch)
                return "Final meeting win branch hook requested.";

            if (command.ShouldRunMeetingEndPipeline)
                return "Meeting end pipeline hook requested.";

            if (command.ShouldContinueMeeting)
                return "Meeting should continue.";

            return "No production apply hook requested.";
        }
    }
}
