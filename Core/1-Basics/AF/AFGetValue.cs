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
    /// To learn more about the Path Syntax see more information here:
    /// <see cref="https://techsupport.osisoft.com/Documentation/PI-AF-SDK/html/c3241f58-2fef-4579-91cf-6f2d71180f98.htm"/>
    /// </summary>
    [Description("To get values from attribute(s) using the Path Syntax")]
    [AdditionalDescription("!!!")]
    [UsageExample("AFGetValue -s optimus -d support -p \"AFSDKExamples\\InterrestingElement | Attribute1\"")]
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
                    Console.WriteLine("{0}{1}{2}{3}{4}", GetStringValue(value), separator, value.Timestamp,separator, attribute.Name);
                }

                if (afObject is AFElement)
                {

                    var element = (AFElement)afObject;

                    // the attibute list object allows a single GetValue() call for all attributes at same time.
                    // We call this a "Bulk Call"
                    var attributes = new AFAttributeList(element.Attributes);
                    var values=attributes.GetValue();

                    // prints the results
                    values.ForEach((afvalue)=> Console.WriteLine("{0}{1}{2}{3}{4}", GetStringValue(afvalue),separator, afvalue.Timestamp,separator,afvalue.Attribute.Name));
                }
 
            }
            
            catch (Exception ex)
            {
                this.Logger.Error(ex);
            }

        }

        /// <summary>
        /// This method demonstrates how to deal with the several possible types of AFValues that can be returned.
        /// 
        /// </summary>
        /// <param name="afValue"></param>
        /// <returns></returns>
        private string GetStringValue(AFValue afValue)
        {
            string result = null;
            // Here, for each attribute we check the type and we print its value.
            //

            // we check if we have an enumeration value
            if (afValue.Value is AFEnumerationValue)
            {

                // in this case, obj.Value.ToString() give same result as obj.Value.ToString(), but in many circumstances it is 
                // better to split the treatment of this type of values because they can be processed differently

                var digValue = (AFEnumerationValue)afValue.Value;
                result = digValue.Name;
            }

            // other known types
            // Note that you may need to split these to different else if statement depending on how you are processing
            // the values.  String or bool are rarely processed the same as a single... 
            else if (afValue.Value is String || afValue.Value is Boolean || afValue.Value is double || afValue.Value is int ||
                     afValue.Value is DateTime || afValue.Value is Single)
            {
                result = afValue.Value.ToString();
            }

            // unknown types
            else
            {
                result = string.Format("{1} - Unknown type", afValue.Value.GetType().ToString());
            }

            return result;
        }


    }
}
