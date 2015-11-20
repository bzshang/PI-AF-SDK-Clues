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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using OSIsoft.AF;
using Clues.Library;
using Clues.Library.Workers;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Data;
using OSIsoft.AF.PI;
using OSIsoft.AF.Time;

namespace Clues
{
    /// <summary>
    /// <example>
    /// PIDeleteUtil -s SRV-PI -t simulator.random.1,simulator.random.2, simulator.random.3 --st *-1Y --et *
    /// </example>
    /// </summary>
    [Description("Deletes data in archive for specified tag(s) and for a specific time range."),
     AdditionalDescription(" WARNING: This applet will delete data on the sepcified PI Data Archive and this operation is un-reversible." +
                           "\n The deletion will occur only when the EnableDelete option is passed, it is recommended to run a test first (without the EnableDelete) option to verify what will be deleted. " +
                           "\n The -p option may help to see what is the data that will be deleted.")]

    public class PIDelete : AppletBase
    {
        // 
        //Command line options
        [Option('s', "server", HelpText = "PI Data Archive Server to connect to", Required = true)]
        public string Server { get; set; }

        [OptionList('t', "tags", HelpText = "list of tags to delete data from.  Delimited by ',' ", Separator = ',', Required = true)]
        public List<string> TagList { get; set; }

        [Option("st", HelpText = "Inclusive Start Time to start deleting data.", Required = true)]
        public string StartTime { get; set; }

        [Option("et", HelpText = "Inclusive End Time to stop deleting data.", Required = true)]
        public string EndTime { get; set; }

        [Option("EnableDelete", HelpText = "Enables the deletion. CAUTION: when using this option the data on the PI Data Archive will be unrecoverable!", Required = false)]
        public bool EnableDelete { get; set; }

        [Option('d', "days", HelpText = "Specifies the number of days to delete at once.  This parameters serves to chunk the data into several intervals to avoid too big data calls.  This parameter must be defined according to your data density.", DefaultValue = 15, Required = false)]
        public int Days { get; set; }

        [Option('p',"PrintEvents", HelpText = "Outputs deleted events to the screen. Caution: this option will increase the time it takes to perform the deletion.", Required = false)]
        public bool PrintEvents { get; set; }



        /// <summary>
        /// Class variables
        /// </summary>
        PiConnectionMgr piConnectionMgr = null;
        DataProcessor dataEraser;


        private void ValidateParameters()
        {
            if (Days <= 0 || Days > 30)
                throw new PIDeleteUtilInvalidParameterException("Days must be included between 1 and 30");
        }


        public override void Run()
        {
            DeleteData();
        }

        /// <summary>
        /// This method deletes the data stored in specified tags of the PI Data Archive
        /// To delete data, it is required to first read the values that you want to delete, and then
        /// Call the update values method with the AFUpdateOption.Remove option
        /// <remarks>
        /// </remarks>
        /// </summary>
        private void DeleteData()
        {
            try
            {
                ValidateParameters();

                piConnectionMgr = new PiConnectionMgr(Server);
                piConnectionMgr.Connect();
                PIServer server = piConnectionMgr.GetPiServer();

                var timer = Stopwatch.StartNew();

                // Gets the tags and creates a point list with the tags, to prepare for bulk read call
                var points = PIPoint.FindPIPoints(server, TagList);
                var pointList = new PIPointList(points);
                Logger.InfoFormat("Initialized PI Points for deletion: {0}", string.Join(", ", points.Select(p => p.Name)));

                // converts strings to AFTime objects this will throw an error if invalid
                var startTime = new AFTime(StartTime);
                var endTime = new AFTime(EndTime);

                if (startTime > endTime)
                    throw new PIDeleteUtilInvalidParameterException("Start Time must be smaller than End Time");

                // defines the data eraser task that will work in parallel as the data querying task
                dataEraser = new DataProcessor(EraseData);
                var eraseTask = Task.Run(() => dataEraser.Run());
                
                // splits iterates the period, over 
                foreach (var period in Library.Helpers.EachNDay(startTime, endTime, Days))
                {
                    Logger.InfoFormat("Getting tags information for period {0} to {1} ({2} Days chunk)", startTime, endTime,
                        Days);

                    // makes the first data call
                    var data = pointList.RecordedValues(period, AFBoundaryType.Inside, null, false,
                        new PIPagingConfiguration(PIPageType.TagCount, 100));

                    Logger.InfoFormat("Adding the data to the queue for deletion. ({0} to {1})", startTime, endTime);
                    // we push this data into the data processor queue so we can continue to query for the rest of the data.
                    dataEraser.DataQueue.Add(data);
                }

              
                dataEraser.DataQueue.CompleteAdding();
                    // // this will tell the data eraser that no more data will be added and allow it to complete

                eraseTask.Wait(); // waiting for the data processor to complete

                Logger.InfoFormat(
                    "Deletion process completed in {0} seconds.  With {1} events deleted (assuming there was no errors).",
                    Math.Round(timer.Elapsed.TotalSeconds, 0), dataEraser.TotalEventProcessed);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }


        /// <summary>
        /// This is the method used by the data processor to work with the data
        /// </summary>
        /// <param name="data"></param>
        private void EraseData(IEnumerable<AFValues> data)
        {
            // puts all AFvalue objects in a single list
            var values = data.SelectMany(afValues => afValues).ToList();

            Logger.InfoFormat("Processing delete action for {0} values.  Deletion is {1}", values.Count, EnableDelete==true ? "Enabled" : "Disabled - Data will not be deleted.");
            
            if(PrintEvents)
                values.ForEach(val=>Logger.InfoFormat("{0},{1},{2}",val.PIPoint.Name,val.Timestamp,val.Value));

            if (EnableDelete && values.Count>0)
            {
                // gets the PI Data Archive server instance
                var server = values[0].PIPoint.Server;

                // sends the values to the server for deletion
                var errors= server.UpdateValues(values, AFUpdateOption.Remove,AFBufferOption.BufferIfPossible);

                // shows errors if there was any
                if (errors!=null && errors.HasErrors)
                    errors.Errors.ToList().ForEach(e=>Logger.Error(e));

                Logger.InfoFormat("Deleted {0} values", values.Count);
            }
        }






    }

    /// <summary>
    /// Exceptions
    /// </summary>

    public class PIDeleteUtilInvalidParameterException : Exception
    {
        public PIDeleteUtilInvalidParameterException(string message = "") : base("The passed parameter is not valid. " + message) { }
    }
}
