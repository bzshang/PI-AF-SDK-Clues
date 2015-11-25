# PI-AF-SDK-Clues - BETA
## **C**ommand **L**ine **U**tility & **E**xample**S**

Clues is a community project that provides a code-base to work with the PI-AF-SDK.  It has three main goals: 
* To provide code examples
* To make the code examples useful and re-useable in a form of a command line application
* To create a container for quick application prototyping

The project is built into a C# Visual Studio Solution.  

Most of the content it includes is inspired from real development support cases and PI Square questions. You should see more content added to this project in the near future as we continue to import our existing snippets into this project.  It is currently in a very early state and you should consider it beta for now.  We would really like your feedbacks as well as your contributions.
___  

# How it works?

To use clues, you need to compile it. So you will need Visual Studio and PI AF SDK installed to be able to compile the solution.
Once the solution is compiled, the folder PI-AF-SDK-Clues\Build will contain all you need to run the command line application. 
You can open a command line in PI-AF-SDK-Clues\Build and run the examples from there.  Each time you build, this folder gets updated with new compiled version.  

In the solution:
* Each applet has its own class
* Each applet has its own command line parameters

See section "How to add an new applet in the Visual Studio Solution?" below to get started and create your own applet.

___  

# Command line

### To get help :  
`clues.exe -?`  
### Subset of available commands:  
	Usage:
	 clues.exe [-?] [applet [-?|options]]

	  AFConnect                Connects to a PI System (AF)

	  AFCreateAttribute        Creates an AF attribute on the specified element.
							   Supports all standard attributes.

	  AFDataPipeListener       Illustrates the functionning of the AF Data Pipe, to
							   get changes from AFAttributes as changes occurs

	  AFElements               List and create elements

	  AFGetValue               To get values from attribute(s) using the Path
							   Syntax

	  PIConnect                Connects to a PI Data Archive Server

	  PIConnectSettings        This applet allows to change the timeouts of a PI
							   Data Archive Connection.

	  PIDataPipeListener       Illustrates the functionning of the PI Data Pipe, to
							   get changes from PI Tags as changes occurs

	  PIDelete                 Deletes data in archive for specified tag(s) and for
							   a specific time range.

	  PIFindPoints             Finds PIPoints based on tag name filter and
							   optionally from point source.

	  PIGetCurrentValue        Reads the most recent value for the specified tag.

	  PIGetCurrentValueBulk    Reads the most recent value for multiple tags that
							   match the search mask.


### Get Help for a Specific Applet
`clues.exe AFConnect -?`

	Usage for AFConnect:

	  -s, --server    Required. Name of the AF Server (PI System) to connect to

# How to add an new applet in the Visual Studio Solution?

* Navigate to the Core Project

* Create a new class in the Core Project structure. Structure is as follow:

	    1-Basics : Basic functions
	        +AF
	        +PI
	    2-Advanced : Most advanced topics such as performance, impersonation, Data Pipes, etc.
	        +AF
	        +PI
	    3-Utility : For applets that require more code and a file structure.  In this case make sure to give a different namespace to your supporting files, ex: Clues.Applet1.  Only the main applet file should sit into the Clues Namespace
	        +ApppletName
	            +Folder1
	            +Folder2
	            +Applet.cs

* Copy-Paste the content of AppletTemplate.cs.txt into a newly created class. 

* Rename the class and the class file name and also give a description: this description will be exposed when running the command line usage: clues.exe -?  and clues AppletX -?
    > [Description("Applet General Description")]  
    > [AdditionalDescription("Applet Specific Description")]  
    > [UsageExample("Applet -s SRV01")]  
    > public class **AppletX** : AppletBase

* Add the command line options your applet needs.  You may refer to existing applet for the logic.  For more details please refer to the **Command Line Paser Library** Help located here: https://github.com/gsscoder/commandline/wiki.

* Create your applet logic

* Compile

* Compilation generated a **Build** folder in the solution folder, you can open a command line from there to test your applet.  Each time you build, it gets updated.

___  

# Notes

* If you create a class that is generic enough to be re-used, create it in Library folder.  Namespace for it is Clues.Library

* _Configuration folders are the plumbing for the application, so you should not need to refer to them.

* ExampleCommandLineOptions.cs is auto-generated. Don't modify it. In the case it is needed, you should modify ExampleCommandLineOptions.tt.  The build event that triggers the file autogeneration is in the file T4.TransformOnBuild.targets, which is not exposed in the solution.

* Be careful to not use same options switches twice (especially when copy pasting).  If you do so you end up with a null parameter value and you wont understand why!

* Be careful to not create a class constructor with parameters, otherwise the automatic configuration may break and give you odd errors like: Error	1	Running transformation: System.MissingMethodException: No parameterless constructor defined for this object.
___  

# How to contribute?  
*You need a GitHub account to contribute*

A quick and easy explanation here:  
[GitHub Guides: Forking 4 minutes read](https://guides.github.com/activities/forking/)  

In summary:
* Fork the repository
* Clone this fork on your dev machine
* Make the changes
* Push the changes back to your GitHub forked repository
* Make pull request
___  

# License

    Copyright 2015 OSIsoft, LLC
     
    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at
     
    http://www.apache.org/licenses/LICENSE-2.0
     
    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
