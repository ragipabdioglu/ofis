using System.Collections.Generic;

namespace OFIS.Meetings
{
    public sealed class MeetingActionProposalService
    {
        private readonly List<MeetingActionProposalData> _proposals =
            new List<MeetingActionProposalData>();

        private readonly MeetingActionValidationService _validationService;

        public IReadOnlyList<MeetingActionProposalData> Proposals => _proposals;
        public int ProposalCount => _proposals.Count;

        public MeetingActionProposalService()
        {
            _validationService = new MeetingActionValidationService();
        }

        public MeetingActionProposalService(
            MeetingActionValidationService validationService)
        {
            _validationService = validationService ?? new MeetingActionValidationService();
        }

        public MeetingActionProposalCreateResult CreateProposal(
            string meetingId,
            MeetingActionRequestData request)
        {
            MeetingActionProposalData proposal = new MeetingActionProposalData(
                request.ActionId,
                meetingId,
                request,
                MeetingActionProposalStatus.Open);

            if (string.IsNullOrWhiteSpace(meetingId))
                return MeetingActionProposalCreateResult.Failed(
                    proposal,
                    "Meeting id is missing.");

            MeetingActionValidationResult validationResult =
                _validationService.Validate(request);

            if (!validationResult.IsValid)
                return MeetingActionProposalCreateResult.Failed(
                    proposal,
                    validationResult.Message);

            if (HasProposal(request.ActionId))
                return MeetingActionProposalCreateResult.Failed(
                    proposal,
                    "Proposal id already exists.");

            _proposals.Add(proposal);

            return new MeetingActionProposalCreateResult(
                true,
                proposal,
                "Meeting action proposal created.");
        }

        public bool HasProposal(string proposalId)
        {
            if (string.IsNullOrWhiteSpace(proposalId))
                return false;

            for (int i = 0; i < _proposals.Count; i++)
            {
                if (_proposals[i].ProposalId == proposalId)
                    return true;
            }

            return false;
        }

        public bool TryGetProposal(
            string proposalId,
            out MeetingActionProposalData proposal)
        {
            for (int i = 0; i < _proposals.Count; i++)
            {
                if (_proposals[i].ProposalId == proposalId)
                {
                    proposal = _proposals[i];
                    return true;
                }
            }

            proposal = default(MeetingActionProposalData);
            return false;
        }

        public bool TryUpdateProposalStatus(
            string proposalId,
            MeetingActionProposalStatus status,
            out MeetingActionProposalData updatedProposal)
        {
            for (int i = 0; i < _proposals.Count; i++)
            {
                if (_proposals[i].ProposalId == proposalId)
                {
                    updatedProposal = _proposals[i].WithStatus(status);
                    _proposals[i] = updatedProposal;
                    return true;
                }
            }

            updatedProposal = default(MeetingActionProposalData);
            return false;
        }

        public void ClearProposals()
        {
            _proposals.Clear();
        }
    }
}
