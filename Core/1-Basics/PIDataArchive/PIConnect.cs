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
using OSIsoft.AF.PI;

namespace Clues
{

    [Description("Connects to a PI Data Archive Server")]
    public class PIConnect :  AppletBase
    {
        // Command line options
        [Option('s', "server", HelpText = "Name of the PI Data Archive Server (PI Server) to connect to", Required = true)]
        public string Server { get; set; }

        public override void Run()
        {
            try
            {
                Connect(Server);
                Logger.InfoFormat("Connected to PI Data Archive {0}",Server);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            
        }


        public PIServer Connect(string server)
        {
                string serverName = server;
                PIServers piServers = new PIServers();
                PIServer piServer = piServers[serverName];
                
                // true, null option will force a password dialog to appear.
                // you should not use this option for a service or an executable that runs outside a windows session.
                // instead use the empty constructor and make sure that the user that runs the application have an account that allows it to connect.
                piServer.Connect(true,null);
                return piServer;

        }

    }
}
