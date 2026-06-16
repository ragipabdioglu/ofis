namespace OFIS.Meetings
{
    public sealed class MeetingActionValidationService
    {
        public MeetingActionValidationResult Validate(MeetingActionRequestData request)
        {
            if (string.IsNullOrWhiteSpace(request.ActionId))
                return Failed(request, "Action id is missing.");

            if (string.IsNullOrWhiteSpace(request.ProposerPlayerId))
                return Failed(request, "Proposer player id is missing.");

            switch (request.ActionType)
            {
                case MeetingActionType.PersonnelAudit:
                    return RequirePlayerTarget(request, "Personnel audit requires a player target.");

                case MeetingActionType.RoomInspection:
                    return RequireRoomTarget(request, "Room inspection requires a room target.");

                case MeetingActionType.TaskReportAudit:
                    if (request.Target.HasPlayerTarget || request.Target.HasDepartmentTarget)
                        return Passed(request, "Task report audit target accepted.");

                    return Failed(
                        request,
                        "Task report audit requires a player or department target.");

                case MeetingActionType.SecurityRecordReview:
                    return RequireSecurityAreaTarget(
                        request,
                        "Security record review requires a security area target.");

                case MeetingActionType.OfficialAccusation:
                    return RequirePlayerTarget(
                        request,
                        "Official accusation requires a player target.");

                case MeetingActionType.NoAction:
                    if (request.Target.IsEmpty)
                        return Passed(request, "No action accepted without target.");

                    return Failed(request, "No action must not include a target.");

                case MeetingActionType.None:
                default:
                    return Failed(request, "Action type is invalid.");
            }
        }

        private static MeetingActionValidationResult RequirePlayerTarget(
            MeetingActionRequestData request,
            string failureMessage)
        {
            return request.Target.HasPlayerTarget
                ? Passed(request, "Player target accepted.")
                : Failed(request, failureMessage);
        }

        private static MeetingActionValidationResult RequireRoomTarget(
            MeetingActionRequestData request,
            string failureMessage)
        {
            return request.Target.HasRoomTarget
                ? Passed(request, "Room target accepted.")
                : Failed(request, failureMessage);
        }

        private static MeetingActionValidationResult RequireSecurityAreaTarget(
            MeetingActionRequestData request,
            string failureMessage)
        {
            return request.Target.HasSecurityAreaTarget
                ? Passed(request, "Security area target accepted.")
                : Failed(request, failureMessage);
        }

        private static MeetingActionValidationResult Passed(
            MeetingActionRequestData request,
            string message)
        {
            return new MeetingActionValidationResult(request, true, message);
        }

        private static MeetingActionValidationResult Failed(
            MeetingActionRequestData request,
            string message)
        {
            return new MeetingActionValidationResult(request, false, message);
        }
    }
}
