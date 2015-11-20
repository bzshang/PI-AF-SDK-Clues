using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF.Data;

namespace Clues { 
    /// <summary>  
    /// This class receives data from a PI Data Pipe  
    /// </summary>  
    public class PIConsoleDataObserver : IObserver<AFDataPipeEvent>
    {

        private AFDataPipeType _dataPipeType;

        /// <summary>
        /// Creates a new instance of console data observer.
        /// It prints data it receives to the console
        /// </summary>
        /// <param name="afDataPipeType">The type of DataPipe, will be printed on the commandline, so its easier to understand what type of data is coming.</param>
        public PIConsoleDataObserver(AFDataPipeType afDataPipeType)
        {
            _dataPipeType = afDataPipeType;
        }

        /// <summary>  
        /// Provides the observer with new data.  
        /// </summary>  
        /// <param name="dataPipeEvent"></param>  
        public void OnNext(AFDataPipeEvent dataPipeEvent)
        {
            Console.WriteLine("PIDataPipe event - {4} - Tag Name: {0}, Action Type: {1}, Value {2}, TimeStamp: {3}", dataPipeEvent.Value.PIPoint.Name, dataPipeEvent.Action.ToString(), dataPipeEvent.Value.Value, dataPipeEvent.Value.Timestamp.ToString(), _dataPipeType);
        }

        /// <summary>  
        /// An error has occured  
        /// </summary>  
        /// <param name="error"></param>  
        public void OnError(Exception error)
        {
            Console.WriteLine("Provider has sent an error");
            Console.WriteLine(error.Message);
            Console.WriteLine(error.StackTrace);
        }

        /// <summary>  
        /// Notifies the observer that the provider has finished sending push-based notifications.  
        /// </summary>  
        public void OnCompleted()
        {
            Console.WriteLine("Provider has terminated sending data");
        }
    }
}
