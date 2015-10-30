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
using OSIsoft.AF.Asset;

namespace Clues
{
    /// <summary>
    /// Usage Example:
    /// AFCreateAttribute -e \\tst-srv\db1\AFSDKExamples\CreateAttributes -n Attribute1 -v \\tst-srv\sinusoid -t AttPIPoint
    /// AFCreateAttribute -e \\tst-srv\db1\AFSDKExamples\CreateAttributes -n Attribute2 -v "SELECT Name FROM Info WHERE Version = 1.5" -t AttTableLookup
    /// \\optimus\CDT158|sinusoid
    /// B=double;[B*10]
    ///
    /// For details on the path syntax:
    /// <see cref="https://techsupport.osisoft.com/Documentation/PI-AF-SDK/html/c3241f58-2fef-4579-91cf-6f2d71180f98.htm"/>
    /// </summary>
    [Description("Creates an AF attribute on the specified element. Supports all standard attributes.")]
    public class AFCreateAttribute : AppletBase
    {
        //Command line Options
        //[Option('s', "server", HelpText = "AF Server name to connect to",Required = true)]
        //public string Server { get; set; }

        [Option('e', "elementPath", HelpText = @"Element path, must uses the path syntax starting with \\ and including database name. i.e: \\TST-SRV\db1\Element1 ", Required = true)]
        public string elementPath { get; set; }

        [Option('n', "name", HelpText = "Name of the new attribute to create", Required = true)]
        public string AttributeName { get; set; }

        [Option('v', "value", HelpText = "Value or config string of the new attribute to create", Required = true)]
        public string AttributeValue { get; set; }

        [Option('t', "attributeType", HelpText = "Type of attribute to create, allowed entries: AttDouble,AttPIPoint,AttTableLookup,AttPIPointArray,AttFormula,AttStringBuilder", Required = true)]
        public AttributeTypeEnum AttributeType { get; set; }

        public enum AttributeTypeEnum
        {
            AttDouble,
            AttPIPoint,
            AttTableLookup,
            AttPIPointArray,
            AttFormula,
            AttStringBuilder
        }

        public override void Run()
        {
            try
            {
                // in this case the, FindObject static method will make all the connection work, and return us the element we need.
                var element = (AFElement) PISystem.FindObject(elementPath, null);

                if (element == null)
                    throw new AFAttributeNotFoundException();

                
                Logger.InfoFormat("Found Element {0}", (element.Name));

                
                CreateAttribute(element, AttributeType, AttributeName, AttributeValue);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }

        
        /// <summary>
        /// Method that creates an AF Attribute on an AF Element
        /// </summary>
        /// <param name="element">AFElement object</param>
        /// <param name="type">Type of attribute to create</param>
        /// <param name="name">name of the new attribute that will be created</param>
        /// <param name="value">Depending on the type of attribute created, the value can be either a scalar value or the ConfigString Value</param>
        public void CreateAttribute(AFElement element, AttributeTypeEnum type, string name, string value)
        {
            var piSystem = element.PISystem;
            var attribute = element.Attributes.Add(name);
            Logger.InfoFormat("Creating attribute {0}", (name));

            switch (type)
            {

                // AFCreateAttribute -e \\tst-srv\db1\AFSDKExamples\CreateAttributes -n Attribute1 -v 10 -t AttDouble
                case AttributeTypeEnum.AttDouble:        
                    attribute.Type = typeof(double);
                    attribute.SetValue(new AFValue(double.Parse(value)));
                    break;

                // AFCreateAttribute -e \\tst-srv\db1\AFSDKExamples\CreateAttributes -n Attribute2 -v \\tst-srv\sinusoid -t AttPIPoint
                case AttributeTypeEnum.AttPIPoint:
                    attribute.DataReferencePlugIn = AFDataReference.GetPIPointDataReference(piSystem);
                    attribute.ConfigString = value;
                    break;

                // AFCreateAttribute -e \\tst-srv\db1\AFSDKExamples\CreateAttributes -n Attribute2 -v "SELECT Name FROM Info WHERE Version = 1.5" -t AttTableLookup
                case AttributeTypeEnum.AttTableLookup:
                    attribute.DataReferencePlugIn = piSystem.DataReferencePlugIns["Table Lookup"];
                    attribute.Type = typeof (string); // if a specific type is needed, you'll need to pass it and set it here.
                    attribute.ConfigString = value;
                    break;

                // AFCreateAttribute -e \\tst-srv\db1\AFSDKExamples\CreateAttributes -n Attribute2 -v "\\optimus\CDT158|sinusoid" -t AttPIPointArray
                case AttributeTypeEnum.AttPIPointArray:
                    attribute.DataReferencePlugIn = piSystem.DataReferencePlugIns["PI Point Array"];
                    attribute.ConfigString = value;
                    break;

                // AFCreateAttribute -e \\tst-srv\db1\AFSDKExamples\CreateAttributes -n Attribute2 -v "B=double;[B*10]" -t AttFormula
                case AttributeTypeEnum.AttFormula:
                    attribute.DataReferencePlugIn = piSystem.DataReferencePlugIns["Formula"];
                    attribute.ConfigString = value;
                    break;



            }

            Logger.Info("Checking in the changes");
            
            element.CheckIn();

            Logger.Info("Attibute Created");


        }
    }
}
