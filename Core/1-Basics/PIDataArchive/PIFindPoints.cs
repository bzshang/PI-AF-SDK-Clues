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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Clues.Library;
using CommandLine;
using OSIsoft.AF;
using OSIsoft.AF.PI;

namespace Clues
{
    /// <summary>
    /// Demonstrates how to get PI Points using the FindPIPoints Method
    /// Note that there are several static methods for FindPIPoints, please see documentation for further details.
    /// <see cref="https://techsupport.osisoft.com/Documentation/PI-AF-SDK/html/Overload_OSIsoft_AF_PI_PIPoint_FindPIPoints.htm"/>
    /// Usage example, u:
    ///  PIFindPoints -s server -f sinsus*  --sourceFilter R
    /// </summary>
    [Description("Finds PIPoints based on tag name filter and optionally from point source.")]
    public class PIFindPoints :  AppletBase
    {
        // Command line options
        [Option('s', "server", HelpText = "PI Data Archive Server to connect to",Required = true)]
        public string Server { get; set; }

        [Option('f', "nameFilter", HelpText = "TagName filter, ex: sinus* ",Required = true)]
        public string NameFilter { get; set; }

        [Option("sourceFilter", HelpText = "Filter for tag point source. ex: R  ", Required = false)]
        public string SourceFilter { get; set; }

        public override void Run()
        {
            try
            {
                PiConnectionMgr piConnectionMgr = new PiConnectionMgr(Server);
                piConnectionMgr.Connect();
                
                PIServer pi = piConnectionMgr.GetPiServer();

                List<PIPoint> points = PIPoint.FindPIPoints(pi, NameFilter,SourceFilter).ToList();
                
                Logger.InfoFormat("{0} Tags Found",points.Count());
                Logger.Info(string.Join(",",points.Select(p=>p.Name)));
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            
        }

        
    }
}
