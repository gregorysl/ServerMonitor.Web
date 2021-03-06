﻿using System.Collections.Generic;
using System.Linq;
using ServerMonitor.Helpers;

namespace ServerMonitor.Oracle
{
    public class OracleInstanceBl
    {
        protected readonly OracleInstanceRepository OracleInstanceRepository;

        public OracleInstanceBl(SettingsHandler settings)
        {
            var instanceSettings = settings.Data.InstanceManager;
            OracleInstanceRepository = new OracleInstanceRepository(new OracleModuleDbContext(instanceSettings.Server,
                instanceSettings.Database, instanceSettings.Username, instanceSettings.Password));
        }

        public virtual OracleInstanceConnectionDetails GetAvailableInstance(string buildServerName)
        {
            var instance = OracleInstanceRepository.GetOldestOracleInstance(buildServerName);
            if (instance == null)
            {
                return null;
            }

            return new OracleInstanceConnectionDetails
            {
                Id = instance.Id,
                Host = instance.ConnectionDetails.Host,
                Protocol = instance.ConnectionDetails.Protocol,
                Port = instance.ConnectionDetails.Port,
                Sid = instance.Sid,
                Service = instance.Service,
                Username = instance.ConnectionDetails.Username,
                Password = instance.ConnectionDetails.Password,
                Name = instance.Name
            };
        }
        public IEnumerable<OracleInstanceDetails> GetAllInstances()
        {
            var instances = OracleInstanceRepository.GetAllInstances();

            return instances.Select(i => new OracleInstanceDetails
            {
                Id = i.Id,
                IsReserved = i.IsReserved,
                CurrentBuildName = i.CurrentBuildName,
                CurrentBuildDate = i.CurrentBuildDate,
                Name = i.Name,
                DisplayName = i.DisplayName,
                Sid = i.Sid,
                Service = i.Service,
                IsDeploying = i.IsDeployInProgress
            });
        }

        public void ChangeInstanceReservation(int instanceId, bool reserve)
        {
            if (reserve)
            {
                OracleInstanceRepository.ReserveInstance(instanceId);
            }
            else
            {
                OracleInstanceRepository.UnreserveInstance(instanceId);
            }
        }

        public bool ChangeInstanceDeployInProgress(int instanceId, bool isDeploying, string buildName)
        {
            if (isDeploying)
            {
                return OracleInstanceRepository.SetDeployInProgress(instanceId);
            }

            OracleInstanceRepository.UnsetDeployInProgress(instanceId);
            OracleInstanceRepository.SetCurrentBuildDate(instanceId);
            OracleInstanceRepository.SetCurrentBuildName(instanceId, buildName);
            return true;
        }
    }
}
