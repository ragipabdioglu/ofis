using System.Collections.Generic;

namespace OFIS.Meetings
{
    public sealed class MeetingSceneAttendanceSnapshotService
    {
        public List<MeetingAttendancePlayerSnapshot> BuildSnapshots(
            IReadOnlyList<MeetingSceneAttendancePlayerSource> sources)
        {
            List<MeetingAttendancePlayerSnapshot> snapshots =
                new List<MeetingAttendancePlayerSnapshot>();

            if (sources == null)
                return snapshots;

            for (int i = 0; i < sources.Count; i++)
            {
                MeetingSceneAttendancePlayerSource source = sources[i];

                if (source == null)
                    continue;

                snapshots.Add(source.BuildSnapshot());
            }

            return snapshots;
        }
    }
}
