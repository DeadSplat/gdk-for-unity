using System.Collections.Generic;
using CommandLine;

namespace Improbable.Gdk.DeploymentLauncher
{
    public class Options
    {
        public class Common
        {
            [Option("environment", Required = false, HelpText = "The SpatialOS environment name")]
            public string Environment { get; set; }
        }

        [Verb("create", HelpText = "Create a deployment.")]
        public class Create : Common
        {
            [Option("project_name", Required = true, HelpText = "The SpatialOS project name")]
            public string ProjectName { get; set; }

            [Option("assembly_name", Required = true, HelpText = "The assembly to use in the deployment.")]
            public string AssemblyName { get; set; }

            [Option("deployment_name", Required = true, HelpText = "The name of the deployment to create.")]
            public string DeploymentName { get; set; }

            [Option("launch_json_path", Required = true, HelpText = "The path to the launch json file.")]
            public string LaunchJsonPath { get; set; }

            [Option("snapshot_path", Required = false, HelpText = "The path to the snapshot.")]
            public string SnapshotPath { get; set; }

            [Option("region", Required = false, HelpText = "The region to launch the deployment in.")]
            public DeploymentRegionCode Region { get; set; }

            [Option("cluster", Required = false, HelpText = "The specific cluster to launch a deployment in.")]
            public string Cluster { get; set; }

            [Option("tags", Required = false, HelpText = "Tags to add to this deployment. Comma separated",
                Separator = ',')]
            public IEnumerable<string> Tags { get; set; }

            [Option("runtime_version", Required = true, HelpText = "The runtime version to use for this deployment.")]
            public string RuntimeVersion { get; set; }
        }

        [Verb("create-sim", HelpText = "Create a simulated player deployment")]
        public class CreateSimulated : Create
        {
            [Option("target_deployment", Required = true,
                HelpText = "The deployment to connect the simulated players to.")]
            public string TargetDeployment { get; set; }

            [Option("flag_prefix", Required = true, HelpText = "The prefix used in setting the worker flags.")]
            public string FlagPrefix { get; set; }

            [Option("simulated_coordinator_worker_type", Required = true, HelpText = "The worker type for the simulated player coordinator.")]
            public string SimulatedCoordinatorWorkerType { get; set; }
        }

        [Verb("stop", HelpText = "Stop a running deployment.")]
        public class Stop : Common
        {
            [Option("project_name", Required = true, HelpText = "The SpatialOS project name")]
            public string ProjectName { get; set; }

            [Option("deployment_id", Required = true, HelpText = "The ID of the deployment to stop.")]
            public string DeploymentId { get; set; }
        }

        [Verb("list", HelpText = "List deployments for a given project.")]
        public class List : Common
        {
            [Option("project_name", Required = true, HelpText = "The SpatialOS project name")]
            public string ProjectName { get; set; }
        }

        public enum DeploymentRegionCode
        {
            None,
            US,
            EU,
            CN
        }
    }
}
