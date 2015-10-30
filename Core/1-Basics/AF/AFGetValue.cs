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
using System.Globalization;
using Clues.Library;
using CommandLine;
using OSIsoft.AF;
using OSIsoft.AF.Asset;

namespace Clues
{
    /// <summary>
    /// Usage example:
    /// ... AFGetValue -s optimus -d support -p "AFSDKExamples\InterrestingElement|Attribute1"
    /// </summary>
    [Description("To get values from attribute(s) using the Path Syntax")]
    public class AFGetValue : AppletBase
    {
        // Command line options

        [Option('s', "server", HelpText = "Name of the AF Server (PI System) to connect to", Required = true)]
        public string Server { get; set; }

        [Option('d', "database", HelpText = "Name of the AF database to connect to. Will use default database if not specified.", Required = false)]
        public string Database { get; set; }

        [Option('p', "path", HelpText = "Element or attribute path to read the value from. If an element is passed all attribute values are returned.  Examples: Element1|attribute,Element1,", Required = false)]
        public string AttributePath { get; set; }

        public override void Run()
        {

            try
            {

                var connection = new AfConnectionMgr(Server, Database);
                connection.Connect();

                AFDatabase database = connection.GetDatabase();

                var afObject = AFObject.FindObject(AttributePath, database);

                var separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;

                if (afObject is AFAttribute)
                {
                    var attribute = (AFAttribute)afObject;
                    AFValue value = attribute.GetValue();
                    Console.WriteLine("{0}{1}{2}{3}{4}", value.Value,separator, value.Timestamp,separator, attribute.Name);
                }

                if (afObject is AFElement)
                {
                    var element = (AFElement)afObject;
                    var attributes = new AFAttributeList(element.Attributes);
                    var values=attributes.GetValue();

                    values.ForEach((afvalue)=> Console.WriteLine("{0}{1}{2}{3}{4}", afvalue.Value,separator, afvalue.Timestamp,separator,afvalue.Attribute.Name));
                }
 
            }
            
            catch (Exception ex)
            {
                this.Logger.Error(ex);
            }

        }


    }
}
