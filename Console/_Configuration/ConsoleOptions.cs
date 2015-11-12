/*
* THIS FILE IS AUTO GENERATED
* TO MODIFY IT - YOU NEED TO CHANGE THE T4 TEMPLATE --> ExampleCommandLineOptions.tt
*
*
*/
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace Clues
{
    /// <summary>
    ///     For configuration details please see:
    ///     <see cref="http://commandline.codeplex.com/"/>
    ///     <see cref="https://github.com/gsscoder/commandline/wiki"></see>
    /// </summary>
    public class ConsoleOptions
    {

	
		[VerbOption("AFConnect", HelpText = "Connects to a PI System (AF)")]
        public AFConnect AFConnect { get; set; }
      
	
		[VerbOption("AFCreateAttribute", HelpText = "Creates an AF attribute on the specified element. Supports all standard attributes.")]
        public AFCreateAttribute AFCreateAttribute { get; set; }
      
	
		[VerbOption("AFElements", HelpText = "List and create elements")]
        public AFElements AFElements { get; set; }
      
	
		[VerbOption("AFGetValue", HelpText = "To get values from attribute(s) using the Path Syntax")]
        public AFGetValue AFGetValue { get; set; }
      
	
		[VerbOption("PIConnect", HelpText = "Connects to a PI Data Archive Server")]
        public PIConnect PIConnect { get; set; }
      
	
		[VerbOption("PIConnectSettings", HelpText = "This applet allows to change the timeouts of a PI Data Archive Connection.")]
        public PIConnectSettings PIConnectSettings { get; set; }
      
	
		[VerbOption("PIFindPoints", HelpText = "Finds PIPoints based on tag name filter and optionally from point source.")]
        public PIFindPoints PIFindPoints { get; set; }
      
	
		[VerbOption("PIGetCurrentValue", HelpText = "Reads the most recent value for the specified tag.")]
        public PIGetCurrentValue PIGetCurrentValue { get; set; }
      
	
		[VerbOption("PIGetCurrentValueBulk", HelpText = "Reads the most recent value for multiple tags that match the search mask.")]
        public PIGetCurrentValueBulk PIGetCurrentValueBulk { get; set; }
      
       

        [HelpVerbOption("help")]
        public string GetUsage(string verb)
        {

            HelpText helpText;
            
            if (verb == null)
            {
                helpText=HelpText.AutoBuild(this,(HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current),true);
                helpText = UsageHeading(helpText);
                // usage
                helpText.AddPreOptionsLine(string.Format("Usage: \n {0} [-?] [applet [-?|options]]", Path.GetFileName(System.Reflection.Assembly.GetEntryAssembly().Location)));    
            }
            else
            {
                helpText=HelpText.AutoBuild(this, verb);
                helpText = UsageHeading(helpText);
                // usage
                helpText.AddPreOptionsLine("\nUsage for " + verb + ":");
            }

            return helpText;
        }

        private HelpText UsageHeading(HelpText helpText)
        {
            
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = fvi.FileVersion; // or fvi.ProductVersion
            
            helpText.Heading = "\n------------------------------------------------------ \n";
            helpText.Heading += "PI-AF-SDK: Command Line Utility and ExampleS (CLUES) \n";
         //   helpText.Heading += "Version: " + version + " \n";
            helpText.Heading += "Copyright 2015 OSIsoft - PI Developers Club\n";
            helpText.Heading += "Source code: github.com/osisoft/PI-AF-SDK-clues\n";
            helpText.Heading += "Licensed under the Apache License, Version 2.0";
            helpText.Copyright = "------------------------------------------------------ ";
            return helpText;
        }
    }

}
