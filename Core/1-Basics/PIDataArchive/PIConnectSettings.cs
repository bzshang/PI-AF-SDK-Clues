#region Copyright
//  Copyright 2015 OSIsoft, LLC
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
#endregion
using System;
using System.ComponentModel;
using CommandLine;
using OSIsoft.AF;
using Clues.Library;
using OSIsoft.AF.PI;

namespace Clues
{

    [Description("This applet allows to change the timeouts of a PI Data Archive Connection.")]
    public class PIConnectSettings : AppletBase
    {
        // Command line options
        [Option('c', "connection", HelpText = "Name of the PI Data Archive connection to modify", Required = true)]
        public string Server { get; set; }

        [Option("operationTimeout", HelpText = "Specifies the time to wait, in seconds, for an AF SDK operation (Data Access) to complete. CAUTION: Changing this setting will persist for ALL applications after it is changed. Default value is 60s.", Required = false)]
        public int OperationTimeout { get; set; }

        [Option("connectionTimeout", HelpText = "Specifies the time to wait, in seconds, for the AF SDK to Connect to a PI Server. CAUTION: This setting will persist for ALL applications after it is changed. Default value is 10s.", Required = false)]
        public int ConnectionTimeout { get; set; }
        
        public override void Run()
        {
            try
            {
                PIServers piServers = new PIServers();
                PIServer piServer = piServers[Server];

                if (piServer == null)
                    throw new PIServerNotFoundException();

                Logger.InfoFormat("Found server connection: {0}",piServer.Name);
                
                if (OperationTimeout > 0)
                {
                    ChangeOperationTimeOut(piServer,OperationTimeout);
                }

                if (ConnectionTimeout > 0)
                {
                    ChangeConnectionTimeOut(piServer, ConnectionTimeout);
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }


        private void ChangeOperationTimeOut(PIServer server,int timeout)
        {

            if (server.Collective != null)
            {
                
                PICollective collective = server.Collective;

                // to change connection properties, in case of a collective
                foreach (PICollectiveMember member in collective.Members)
                {
                    Logger.InfoFormat("Changing collective member {0} {1} from {2}s to {3}s",member.Name,"operation timeout", member.OperationTimeOut.TotalSeconds,timeout);
                    member.OperationTimeOut = new TimeSpan(0, 0, timeout);
                }
            }
            else
            {
                Logger.InfoFormat("Changing server {0} {1} from {2}s to {3}s", server.Name, "operation timeout", server.ConnectionInfo.OperationTimeOut.TotalSeconds, timeout);
                server.ConnectionInfo.OperationTimeOut = new TimeSpan(0, 0, timeout);
            }

        }

        private void ChangeConnectionTimeOut(PIServer server,int timeout)
        {

            if (server.Collective != null)
            {

                PICollective collective = server.Collective;

                // to change connection properties, in case of a collective
                foreach (PICollectiveMember member in collective.Members)
                {
                    Logger.InfoFormat("Changing collective member {0} {1} from {2}s to {3}s", member.Name, "connection timeout", member.ConnectionTimeOut.TotalSeconds, timeout);
                    member.ConnectionTimeOut = new TimeSpan(0, 0, timeout);
                }
            }
            else
            {
                Logger.InfoFormat("Changing server {0} {1} from {2}s to {3}s", server.Name, "connection timeout", server.ConnectionInfo.ConnectionTimeOut.TotalSeconds, timeout);
                server.ConnectionInfo.ConnectionTimeOut = new TimeSpan(0, 0, timeout);
            }

        }



    }
}
