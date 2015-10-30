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
using System.Runtime.CompilerServices;
using Clues.Library;
using CommandLine;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.PI;

namespace Clues
{

    [Description("Reads the most recent value for the specified tag.")]
    public class PIGetCurrentValue :  AppletBase
    {
        // Command line options
        [Option('s', "server", HelpText = "PI Data Archive Server to connect to",Required = true)]
        public string Server { get; set; }

        [Option('t', "tag", HelpText = "tag to read snapshot value from",Required = true)]
        public string Tag { get; set; }

        public override void Run()
        {
            try
            {
                PiConnectionMgr piConnectionMgr = new PiConnectionMgr(Server);
                piConnectionMgr.Connect();

                PIServer pi = piConnectionMgr.GetPiServer();

                PIPoint point = PIPoint.FindPIPoint(pi, Tag);
                AFValue value=point.CurrentValue();

                Logger.InfoFormat("The current value for PI Point {0} is : {1} - {2}", Tag,value.Timestamp,value.Value);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            
        }
    }
}
