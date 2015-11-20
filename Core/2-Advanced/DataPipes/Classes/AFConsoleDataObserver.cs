using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIsoft.AF.Data;

namespace Clues
{
    /// <summary>  
    /// This class receives data from AF Data Pipe  
    /// </summary>  
    public class AFConsoleDataObserver : IObserver<AFDataPipeEvent>
    {
        /// <summary>  
        /// Provides the observer with new data.  
        /// </summary>  
        /// <param name="value"></param>  
        public void OnNext(AFDataPipeEvent value)
        {
            Console.WriteLine("AFDataPipe event - Attribute Name: {0}, Action Type: {1}, Value {2}, TimeStamp: {3}", value.Value.Attribute.Name, value.Action.ToString(), value.Value.Value, value.Value.Timestamp.ToString());
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
