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
using Clues.Library;
using CommandLine;
using OSIsoft.AF;

namespace Clues
{
    
    [Description("Connects to a PI System (AF)")]
    public class AFConnect :  AppletBase
    {
        // Command line options
        [Option('s', "server", HelpText = "Name of the AF Server (PI System) to connect to.", Required = true)]
        public string Server { get; set; }
        
        public override void Run()
        {
            try
            {
                Connect(Server);
                Logger.InfoFormat("Connected to PISystem (AF) {0}",Server);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            
        }
        
        /// <summary>
        ///Example that shows how to connect to PI System
        ///in your application, you should keep a reference to piSystems so its not garbage collected.
        /// The class  <see cref="AfConnectionMgr"/> illustrates this. (ctrl+click to navigate to the class.)
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public PISystem Connect(string server)
        {
                string serverName = server;
                PISystems piSystems = new PISystems();

                // in case you would like to force the auto-creation of the PI System in KST if it does not exist, uncomment the following line
                
                // PISystems.DirectoryOptions = PISystems.AFDirectoryOptions.AutoAdd;
                PISystem piSystem = piSystems[serverName];

                // true, null option will force a password dialog to appear.
                // you should not use this option for a service or an executable that runs outside a windows session.
                // instead use the empty constructor and make sure that the user that runs the application have an account that allows it to connect.
                piSystem.Connect(true,null);

                return piSystem;

        }

    }
}
