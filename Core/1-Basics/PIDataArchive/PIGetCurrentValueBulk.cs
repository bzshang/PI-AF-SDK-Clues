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
using OSIsoft.AF.Asset;
using OSIsoft.AF.PI;
using OSIsoft.AF;

namespace Clues
{
    [Description("Reads the most recent value for multiple tags that match the search mask.")]
    public class PIGetCurrentValueBulk : AppletBase
    {
        // Command line options
        [Option('s', "server", HelpText = "PI Data Archive Server to connect to", Required = true)]
        public string Server { get; set; }

        [Option('t', "tag mask", HelpText = "tag mask specifying tags to read snapshot values from", Required = true)]
        public string TagMask { get; set; }

        public override void Run()
        {
            try
            {
                PiConnectionMgr piConnectionMgr = new PiConnectionMgr(Server);
                piConnectionMgr.Connect();

                PIServer pi = piConnectionMgr.GetPiServer();

                PIPointList pointList = new PIPointList(PIPoint.FindPIPoints(pi, TagMask));

                AFListResults<PIPoint, AFValue> values = pointList.CurrentValue();

                foreach (AFValue val in values)
                {
                    Logger.InfoFormat("The current value for PI Point {0} is : {1} - {2}", val.PIPoint, val.Timestamp, val.Value);
                }              

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }


    }
}
