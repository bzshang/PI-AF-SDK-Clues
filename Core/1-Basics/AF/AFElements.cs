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
using System.Data;
using Clues.Library;
using CommandLine;
using CommandLine.Text;
using OSIsoft.AF;
using OSIsoft.AF.Asset;


namespace Clues
{
    /// <summary>
    /// <example>
    /// Listing the hierarchy
    /// AFElements -s optimus -l
    /// AFElements -s optimus -e Element1
    /// 
    /// Creating an element
    /// AFElements -s optimus -d support -l
    /// AFElements -s optimus -d support -c Element1::
    /// AFElements -s optimus -d support -c Child1:Element1:
    /// AFElements -s optimus -d support -c reactor:"\\OPTIMUS\Support\Element1":Reactor
    /// </example>
    /// </summary>
    [Description("List and create elements")]
    public class AFElements : AppletBase
    {
        // Command line options
        [Option('s', "server", HelpText = "Name of the AF Server (PI System) to connect to", Required = true)]
        public string Server { get; set; }

        [Option('d', "database", HelpText = "Name of the AF Database to operate on", Required = true)]
        public string DatabaseName { get; set; }

        [OptionList('c', "create",HelpText = "Create an element. Syntax: -c ElementName:<ParentElementPath>:<ElementTemplate>.  Note that putting two : separator is mandatory. And beware it could conflict with your element name or parent path if it contains :",Separator = ':')]
        public IList<string> NewElementParams { get; set; }
   
        [Option('l', "list", HelpText = "Lists all child elements of the specified Element Name.", MutuallyExclusiveSet = "list")]
        public bool OptionList { get; set; }
        
        [Option('e', "ElementPath", HelpText = "Specifies an Element Name to list data from.  If not specifies, the entire database will be listed", MutuallyExclusiveSet = "list")]
        public string ElementPath { get; set; }
        

        public override void Run()
        {
            try
            {
                
                var connection = new AfConnectionMgr(Server, DatabaseName);
                connection.Connect();
                var afDatabase = connection.GetDatabase();

                // Parsing options
                if (OptionList)
                {
                    ListElements(afDatabase,ElementPath);
                }

                if (NewElementParams.Count>0)
                {
                    var elementName = NewElementParams[0];
                    var elementPath=NewElementParams.Count >= 2 ? NewElementParams[1]:null;
                    var elementTemplate=NewElementParams.Count >= 3 ? NewElementParams[2]:null;
                    CreateElement(afDatabase, elementName, elementPath, elementTemplate);
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }



        private void CreateElement(AFDatabase afDatabase, string name,string path,string template)
        {
            // check if we have a parent element, if yes we get it, if not we set parent to database level
            Logger.InfoFormat("Parsing parameters");
            dynamic parent = !string.IsNullOrEmpty(path)?(object)AFElement.FindObject(path, afDatabase):afDatabase;
            var elementTemplate = !string.IsNullOrEmpty(template) ? GetTemplate(afDatabase, template):null;

            //if parent is null, the passed string did not resolve to a parent
            if(parent==null)
                throw new NoNullAllowedException(string.Format("The parent path passed {0} could not resolve to an AF object in the Database",path));

            // here we check if the FindObject method has returned a Element, if not an exception will be thrown
            if(!(parent is AFElement || parent is AFDatabase))
                throw new InvalidAFObjectException();
            
            // create the element
            Logger.InfoFormat("Creating the element");
            parent.Elements.Add(name,elementTemplate);
            parent.CheckIn();

            Logger.InfoFormat("Element {0} created.",name);

        }

       
 

        /// <summary>
        /// List the entire hierarchy of an element or a database
        /// </summary>
        /// <param name="afDatabase"></param>
        /// <param name="elementPath"></param>
        private void ListElements(AFDatabase afDatabase,string elementPath)
        {

            AFElement root;
            if (!string.IsNullOrEmpty(elementPath))
            {
                // start from a single element
                root = afDatabase.Elements[elementPath];
                PrintHierarchy(root);
            }
            else
            {
                // if starting at the database level, there can be several Elements at the first level
                foreach (var element in afDatabase.Elements)
                {
                    PrintHierarchy(element);
                }
            }
        }

        private void PrintHierarchy(AFElement node)
        {
            
            // this call loads all the hierarchy on the client.  This saves network calls.
            // you should use the full load option if you will be using attributes. this example does not.
            // you can also use partial loading instead, by using AFElement.Loadattributes after this call to
            // load only the attributes you need.  This will be the fastest.
            AFElement.LoadElementsToDepth(new []{node},false, 10, 10000);
            
            PrintNode(node, 0);
        }

        private void PrintNode(AFElement node, int level)
        {
            Console.WriteLine(string.Format("{0}{1}", "".PadRight(level, '\t'), node.Name));

            foreach (AFElement child in node.Elements)
            {
                PrintNode(child, level + 1); //<-- recursive
            }
        }

        private AFElementTemplate GetTemplate(AFDatabase afDatabase, string template)
        {
            if (!afDatabase.ElementTemplates.Contains(template))
                throw new AFElementTemplateNotFound();

            return afDatabase.ElementTemplates[template];

        }





    }
}
