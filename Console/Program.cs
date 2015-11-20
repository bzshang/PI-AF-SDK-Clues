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
using CommandLine;
using log4net;


namespace Clues
{
    class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

        [STAThread]
        private static void Main(string[] args)
        {
            // creates the options class
            var options = new ConsoleOptions();
            
            // Parses the arguments, if the parse fails, it displays Usage information
            Parser.Default.ParseArguments(args, options, OnVerbCommand);


            
        }

        private static void OnVerbCommand(string appletName, object appletObject)
        {
            if (appletObject != null)
                ((AppletBase) appletObject).Run();
        }
    }
}
