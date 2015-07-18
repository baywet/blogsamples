using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using CommonDepedency.Resiliance;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Linq;
using System.Linq;

namespace CommonDepedency.Features.CommonDepedency.Resiliance
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("a8fb76db-1d3f-4108-b674-481b1cdb1495")]
    public class CommonDepedencyEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            RegisterCommonDepedenciesJob();
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            RemoveCommonDepedenciesJob();
        }
        private void RemoveCommonDepedenciesJob()
        {
            var commonDepedenciesJobs = SPFarm.Local.TimerService.JobDefinitions.OfType<CommonDepedenciesJobDefinition>().ToList();
            for (var i = commonDepedenciesJobs.Count() - 1; i > -1; i--)
                commonDepedenciesJobs.ElementAt(i).Delete();
        }
        private void RegisterCommonDepedenciesJob()
        {
            try
            {
                var timerService = SPFarm.Local.TimerService;//this way we ensure it's registered to all servers of the farm
                var resilianceJob = new CommonDepedenciesJobDefinition(timerService)
                {//schedule is up to you
                    Schedule = new SPHourlySchedule()
                    {
                        BeginMinute = 0,
                        EndMinute = 59
                    }
                };
                resilianceJob.Update();
            }
            catch
            {
                //here you should do some logging
            }
        }

        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
