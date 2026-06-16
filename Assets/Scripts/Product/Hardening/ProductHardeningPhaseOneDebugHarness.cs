using System.Collections.Generic;
using UnityEngine;

namespace OFIS.Product.Hardening
{
    public sealed class ProductHardeningPhaseOneDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private ProductHardeningPhaseOnePackageType packageType = ProductHardeningPhaseOnePackageType.PhaseClosure;

        private readonly ProductHardeningPhaseOneServices services = new ProductHardeningPhaseOneServices();

        private void Start()
        {
            if (validateOnStart)
            {
                ValidatePackage();
            }
        }

        [ContextMenu("Validate Product Phase 1 Package")]
        public void ValidatePackage()
        {
            switch (packageType)
            {
                case ProductHardeningPhaseOnePackageType.DedicatedServerBuildPipeline:
                    LogResult("DedicatedServerBuildPipeline", services.ValidateDedicatedServerBuild(CreateBuildSnapshot()));
                    break;
                case ProductHardeningPhaseOnePackageType.ServerLogging:
                    LogResult("ServerLogging", services.ValidateServerLogging(CreateServerLog()));
                    break;
                case ProductHardeningPhaseOnePackageType.CrashRecovery:
                    LogResult("CrashRecovery", services.ValidateCrashRecovery(CreateCrashRecovery()));
                    break;
                case ProductHardeningPhaseOnePackageType.MatchCancellationHandling:
                    LogResult("MatchCancellationHandling", services.ValidateMatchCancellation(true, true, true));
                    break;
                case ProductHardeningPhaseOnePackageType.NetworkPacketLossTest:
                    LogResult("NetworkPacketLossTest", services.ValidatePacketLoss(CreateNetworkSimulation()));
                    break;
                case ProductHardeningPhaseOnePackageType.HighPingSimulation:
                    LogResult("HighPingSimulation", services.ValidateHighPing(CreateNetworkSimulation()));
                    break;
                case ProductHardeningPhaseOnePackageType.ReconnectEdgeCaseTest:
                    LogResult("ReconnectEdgeCaseTest", services.ValidateReconnectEdgeCases(CreateReconnect()));
                    break;
                case ProductHardeningPhaseOnePackageType.ServerOnlyDataAudit:
                    LogResult("ServerOnlyDataAudit", services.ValidateServerOnlyDataAudit(CreateClientVisibleKeys()));
                    break;
                case ProductHardeningPhaseOnePackageType.PrivacyGuardExpansion:
                    LogResult("PrivacyGuardExpansion", services.ValidatePrivacyGuardExpansion(CreatePrivacyRecords()));
                    break;
                case ProductHardeningPhaseOnePackageType.AutomatedIntegrationTests:
                    LogResult("AutomatedIntegrationTests", services.ValidateAutomatedIntegrationTests(CreateIntegrationTests()));
                    break;
                case ProductHardeningPhaseOnePackageType.LoadTestPlayerCounts:
                    LogResult("LoadTestPlayerCounts", services.ValidateLoadTests(CreateLoadTests()));
                    break;
                case ProductHardeningPhaseOnePackageType.MemoryAllocationOptimization:
                    LogResult("MemoryAllocationOptimization", services.ValidateMemoryAllocation(CreatePerformance()));
                    break;
                case ProductHardeningPhaseOnePackageType.GcSpikeControl:
                    LogResult("GcSpikeControl", services.ValidateGcSpike(CreatePerformance()));
                    break;
                case ProductHardeningPhaseOnePackageType.SceneLoadOptimization:
                    LogResult("SceneLoadOptimization", services.ValidateSceneLoad(CreatePerformance()));
                    break;
                case ProductHardeningPhaseOnePackageType.RealVoiceProviderImplementation:
                    LogResult("RealVoiceProviderImplementation", services.ValidateRealVoiceProvider(CreateVoiceProvider()));
                    break;
                case ProductHardeningPhaseOnePackageType.PhaseClosure:
                    LogResult("PhaseClosure", services.ValidatePhaseClosure(
                        CreateBuildSnapshot(),
                        CreateServerLog(),
                        CreateCrashRecovery(),
                        CreateNetworkSimulation(),
                        CreateReconnect(),
                        CreateClientVisibleKeys(),
                        CreatePrivacyRecords(),
                        CreateIntegrationTests(),
                        CreateLoadTests(),
                        CreatePerformance(),
                        CreateVoiceProvider()));
                    break;
            }
        }

        private static DedicatedServerBuildSnapshot CreateBuildSnapshot()
        {
            return new DedicatedServerBuildSnapshot(ProductBuildTargetType.DedicatedServer, true, true, 1, true);
        }

        private static ServerLogRecord CreateServerLog()
        {
            return new ServerLogRecord("match.lifecycle", "server tick accepted for redacted connection", 128, true, true);
        }

        private static CrashRecoverySnapshot CreateCrashRecovery()
        {
            return new CrashRecoverySnapshot("match-redacted-01", 144, 145, true, true);
        }

        private static NetworkSimulationSnapshot CreateNetworkSimulation()
        {
            return new NetworkSimulationSnapshot(8, 220, true, true);
        }

        private static ReconnectEdgeCaseSnapshot CreateReconnect()
        {
            return new ReconnectEdgeCaseSnapshot(true, true, true, true);
        }

        private static IReadOnlyCollection<string> CreateClientVisibleKeys()
        {
            return new[] { "room_id", "public_position", "interaction_prompt", "connection_quality" };
        }

        private static IReadOnlyCollection<ServerLogRecord> CreatePrivacyRecords()
        {
            return new[]
            {
                new ServerLogRecord("audit.network", "redacted connection rejoined active match", 164, true, true),
                new ServerLogRecord("audit.privacy", "server-only state remained hidden", 165, true, true)
            };
        }

        private static IntegrationTestSnapshot CreateIntegrationTests()
        {
            return new IntegrationTestSnapshot(new[] { "MatchLoop", "NetworkPrivacy", "Reconnect", "VoiceProvider" });
        }

        private static IReadOnlyCollection<LoadTestSnapshot> CreateLoadTests()
        {
            return new[]
            {
                new LoadTestSnapshot(6, true, 24f, 0),
                new LoadTestSnapshot(8, true, 27f, 0),
                new LoadTestSnapshot(10, true, 32f, 0),
                new LoadTestSnapshot(12, true, 36f, 0)
            };
        }

        private static RuntimePerformanceSnapshot CreatePerformance()
        {
            return new RuntimePerformanceSnapshot(1024, 2.5f, 4.2f, true);
        }

        private static VoiceProviderSnapshot CreateVoiceProvider()
        {
            return new VoiceProviderSnapshot("ProductionVoiceAdapter", true, true, false);
        }

        private static void LogResult(string gateName, HardeningGateResult result)
        {
            if (result.Passed)
            {
                Debug.Log($"[ProductHardeningPhaseOneDebugHarness] PASS {gateName}: {result.Message}");
                return;
            }

            Debug.LogError($"[ProductHardeningPhaseOneDebugHarness] FAIL {gateName}: {result.Message}");
        }
    }
}
