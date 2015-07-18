using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

//important, this assembly must not have any depedency to the assemblies we want to check otherwise it's pointless
namespace CommonDepedency.Resiliance
{
    /// <summary>
    /// Checks if the depedency is here, if not does a force local deployment
    /// This job inherits from SPAdministrationServiceJobDefinition to have the maximum permission available
    /// </summary>
    [Serializable]
    public class CommonDepedenciesJobDefinition : SPAdministrationServiceJobDefinition
    {
        const string newtonsoftAssemblyName = "Newtonsoft.Json, Version=7.0.0.0, Culture=Neutral, PublicKeyToken=30ad4fe6b2a6aeed";
        const string solutionId = "d02cb870-239c-462f-896d-f2c7743a4437";
        public CommonDepedenciesJobDefinition() : base() { }
        public CommonDepedenciesJobDefinition(SPService service)
            : base("CommonDepedenciesJob", service, null, SPJobLockType.None)
        {

        }
        public override void Execute(Guid targetInstanceId)
        {
            try
            {
                Assembly.Load(newtonsoftAssemblyName);
            }
            catch
            {
                var solution = SPFarm.Local.Solutions[new Guid(solutionId)];
                if (solution != null && solution.Deployed && solution.DeployedServers.Select(x => x.Name).Contains(SPServer.Local.Name))
                    solution.DeployLocal(true, solution.DeployedWebApplications, true);
            }
            base.Execute(targetInstanceId);
        }
        public override string DisplayName
        {
            get
            {
                return "Depedency resolution resiliance job"; //should be localized, I didn't do it to keep the sample simple
            }
        }
        public override string Description
        {
            get
            {
                return "checks if a depedency is missing and re-deploys it if it's the case"; //same thing
            }
        }
    }
}
