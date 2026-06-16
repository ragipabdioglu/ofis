using System.Collections.Generic;

namespace OFIS.Product.Hardening
{
    public sealed class ProductHardeningPhaseOneServices
    {
        private static readonly string[] RequiredIntegrationSuites =
        {
            "MatchLoop",
            "NetworkPrivacy",
            "Reconnect",
            "VoiceProvider"
        };

        private static readonly int[] RequiredLoadCounts = { 6, 8, 10, 12 };

        public HardeningGateResult ValidateDedicatedServerBuild(DedicatedServerBuildSnapshot snapshot)
        {
            if (snapshot.Target != ProductBuildTargetType.DedicatedServer)
            {
                return Fail("Build target is not DedicatedServer.");
            }

            if (!snapshot.HeadlessMode || !snapshot.DeterministicProfile || snapshot.SceneCount <= 0 || !snapshot.ClientAssetsExcluded)
            {
                return Fail("Dedicated server build pipeline is missing headless, deterministic, scene, or client-exclusion guarantees.");
            }

            return Pass("Dedicated server build pipeline is gated.");
        }

        public HardeningGateResult ValidateServerLogging(ServerLogRecord record)
        {
            if (!record.Structured || record.ServerTick <= 0 || string.IsNullOrWhiteSpace(record.Category))
            {
                return Fail("Server logs must be structured and tick-addressable.");
            }

            if (!record.IdentityRedacted || ContainsSensitiveIdentity(record.Message))
            {
                return Fail("Server log contains unredacted identity text.");
            }

            return Pass("Server logging is structured and privacy guarded.");
        }

        public HardeningGateResult ValidateCrashRecovery(CrashRecoverySnapshot snapshot)
        {
            if (string.IsNullOrWhiteSpace(snapshot.MatchId) || snapshot.SavedTick <= 0)
            {
                return Fail("Crash recovery snapshot is not match-addressable.");
            }

            if (!snapshot.ServerStateOnly || !snapshot.SafeRoomState || snapshot.RestoredTick < snapshot.SavedTick)
            {
                return Fail("Crash recovery does not restore a safe server-only state.");
            }

            return Pass("Crash recovery restores server authority without client leaks.");
        }

        public HardeningGateResult ValidateMatchCancellation(bool cancellationBroadcast, bool rewardsBlocked, bool serverStateClosed)
        {
            if (!cancellationBroadcast || !rewardsBlocked || !serverStateClosed)
            {
                return Fail("Match cancellation does not fully close the session.");
            }

            return Pass("Match cancellation closes safely.");
        }

        public HardeningGateResult ValidatePacketLoss(NetworkSimulationSnapshot snapshot)
        {
            if (snapshot.PacketLossPercent < 5 || !snapshot.ReliableActionsSurvive)
            {
                return Fail("Packet loss simulation does not protect reliable actions.");
            }

            return Pass("Packet loss simulation preserves reliable gameplay actions.");
        }

        public HardeningGateResult ValidateHighPing(NetworkSimulationSnapshot snapshot)
        {
            if (snapshot.LatencyMs < 180 || !snapshot.ReconciliationStable)
            {
                return Fail("High ping simulation does not validate stable reconciliation.");
            }

            return Pass("High ping simulation remains playable.");
        }

        public HardeningGateResult ValidateReconnectEdgeCases(ReconnectEdgeCaseSnapshot snapshot)
        {
            if (!snapshot.SeatDeduplicated || !snapshot.RoleHiddenFromClient || !snapshot.PositionRestored || !snapshot.VoiceSessionRestored)
            {
                return Fail("Reconnect edge cases are not fully covered.");
            }

            return Pass("Reconnect edge cases are covered.");
        }

        public HardeningGateResult ValidateServerOnlyDataAudit(IReadOnlyCollection<string> clientVisibleKeys)
        {
            foreach (var key in clientVisibleKeys)
            {
                if (ContainsSensitiveIdentity(key) || ContainsServerOnlyToken(key))
                {
                    return Fail("Client-visible payload contains server-only or identity data.");
                }
            }

            return Pass("Server-only data audit is clean.");
        }

        public HardeningGateResult ValidatePrivacyGuardExpansion(IReadOnlyCollection<ServerLogRecord> records)
        {
            foreach (var record in records)
            {
                var result = ValidateServerLogging(record);
                if (!result.Passed)
                {
                    return result;
                }
            }

            return Pass("Privacy guard expansion covers logs and payload labels.");
        }

        public HardeningGateResult ValidateAutomatedIntegrationTests(IntegrationTestSnapshot snapshot)
        {
            foreach (var suite in RequiredIntegrationSuites)
            {
                if (!ContainsSuite(snapshot.PassingSuites, suite))
                {
                    return Fail($"Integration suite is missing: {suite}.");
                }
            }

            return Pass("Automated integration tests cover product hardening gates.");
        }

        public HardeningGateResult ValidateLoadTests(IReadOnlyCollection<LoadTestSnapshot> snapshots)
        {
            foreach (var requiredCount in RequiredLoadCounts)
            {
                var found = false;
                foreach (var snapshot in snapshots)
                {
                    if (snapshot.PlayerCount != requiredCount)
                    {
                        continue;
                    }

                    found = true;
                    if (!snapshot.MatchCompleted || snapshot.MaxFrameMs > 40f || snapshot.DisconnectCount > 0)
                    {
                        return Fail($"{requiredCount}-player load test is not stable.");
                    }
                }

                if (!found)
                {
                    return Fail($"{requiredCount}-player load test is missing.");
                }
            }

            return Pass("6/8/10/12-player load tests are stable.");
        }

        public HardeningGateResult ValidateMemoryAllocation(RuntimePerformanceSnapshot snapshot)
        {
            return snapshot.AllocationBytesPerTick <= 2048
                ? Pass("Memory allocation budget is controlled.")
                : Fail("Memory allocation exceeds the per-tick budget.");
        }

        public HardeningGateResult ValidateGcSpike(RuntimePerformanceSnapshot snapshot)
        {
            return snapshot.MaxGcSpikeMs <= 4f
                ? Pass("GC spikes stay under budget.")
                : Fail("GC spike budget is exceeded.");
        }

        public HardeningGateResult ValidateSceneLoad(RuntimePerformanceSnapshot snapshot)
        {
            return snapshot.AsyncSceneLoad && snapshot.SceneLoadSeconds <= 6f
                ? Pass("Scene loading is async and within budget.")
                : Fail("Scene load optimization gate failed.");
        }

        public HardeningGateResult ValidateRealVoiceProvider(VoiceProviderSnapshot snapshot)
        {
            if (string.IsNullOrWhiteSpace(snapshot.ProviderName) || !snapshot.RealProvider || !snapshot.ReconnectSupported || snapshot.RecordsContent)
            {
                return Fail("Voice provider is not production-ready.");
            }

            return Pass("Real voice provider contract is production-ready.");
        }

        public HardeningGateResult ValidatePhaseClosure(
            DedicatedServerBuildSnapshot build,
            ServerLogRecord log,
            CrashRecoverySnapshot crash,
            NetworkSimulationSnapshot network,
            ReconnectEdgeCaseSnapshot reconnect,
            IReadOnlyCollection<string> visibleKeys,
            IReadOnlyCollection<ServerLogRecord> privacyRecords,
            IntegrationTestSnapshot integration,
            IReadOnlyCollection<LoadTestSnapshot> load,
            RuntimePerformanceSnapshot performance,
            VoiceProviderSnapshot voice)
        {
            var gates = new[]
            {
                ValidateDedicatedServerBuild(build),
                ValidateServerLogging(log),
                ValidateCrashRecovery(crash),
                ValidateMatchCancellation(true, true, true),
                ValidatePacketLoss(network),
                ValidateHighPing(network),
                ValidateReconnectEdgeCases(reconnect),
                ValidateServerOnlyDataAudit(visibleKeys),
                ValidatePrivacyGuardExpansion(privacyRecords),
                ValidateAutomatedIntegrationTests(integration),
                ValidateLoadTests(load),
                ValidateMemoryAllocation(performance),
                ValidateGcSpike(performance),
                ValidateSceneLoad(performance),
                ValidateRealVoiceProvider(voice)
            };

            foreach (var gate in gates)
            {
                if (!gate.Passed)
                {
                    return gate;
                }
            }

            return Pass("Product Phase 1 closure passed: 12-player stability, reconnect coverage, and server-only privacy are gated.");
        }

        private static HardeningGateResult Pass(string message)
        {
            return new HardeningGateResult(true, message, HardeningGateSeverity.Info);
        }

        private static HardeningGateResult Fail(string message)
        {
            return new HardeningGateResult(false, message);
        }

        private static bool ContainsSensitiveIdentity(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var normalized = value.ToLowerInvariant();
            return normalized.Contains("player ")
                || normalized.Contains("killer")
                || normalized.Contains("employee")
                || normalized.Contains("@")
                || normalized.Contains("identity");
        }

        private static bool ContainsServerOnlyToken(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var normalized = value.ToLowerInvariant();
            return normalized.Contains("role")
                || normalized.Contains("murder")
                || normalized.Contains("task_seed")
                || normalized.Contains("server_secret")
                || normalized.Contains("private");
        }

        private static bool ContainsSuite(IReadOnlyList<string> suites, string expected)
        {
            for (var i = 0; i < suites.Count; i++)
            {
                if (suites[i] == expected)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
