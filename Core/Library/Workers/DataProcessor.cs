using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using OSIsoft.AF.Asset;

namespace Clues.Library.Workers
{


    /// <summary>
    /// This class makes data processing.
    /// It also keeps the count of the number of events processed
    /// </summary>
    public class DataProcessor
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(DataProcessor));
        private readonly BlockingCollection<IEnumerable<AFValues>> _dataQueue = new BlockingCollection<IEnumerable<AFValues>>();
        private Action<IEnumerable<AFValues>> _dataProcessingAction;
        private Action _finalAction;
        private Task task;

        private bool _forceDataEnumeration = true;

        public int TotalEventProcessed { get; private set; }


         /// <summary>
        /// Creates a new instance of a DataProcessor
        /// </summary>
        public BlockingCollection<IEnumerable<AFValues>> DataQueue
        {
            get { return _dataQueue; }
        }

        /// <summary>
        /// Creates a new data processor
        /// </summary>
        /// <param name="dataProcessingAction"> This method will receive the data as a List of AFValues. This method must implement what you need to do with the data.</param>
        /// <param name="finalAction">optional, if there is someting to do when after the data processing is completed this action can serve this purpose. </param>
        /// /// <param name="forceDataenumeration">Default true.  If true, the data will be converted into a list, forcing the enumeration of the AFValues.  This is useful if you want the result from a bulk call, that are paged, to get enumerated before the data processing method occurs.</param>
        public DataProcessor(Action<IEnumerable<AFValues>> dataProcessingAction, Action finalAction = null, bool forceDataenumeration=true)
        {
            _dataProcessingAction = dataProcessingAction;
            _finalAction = finalAction;
        }

        public void Run()
        {
            ProcessData();
        }

        public void RunAsync()
        {
            task = Task.Run(() => ProcessData());
        }

        /// <summary>
        /// Waits for the async task to complete
        /// DataQueue.CompleteAdding() must be called for the ProcessData method to complete.
        /// </summary>
        public void WaitAsyncCompleted()
        {
            task.Wait();
        }



        /// <summary>
        /// Processes the data in the DataQueue
        /// </summary>
        private void ProcessData()
        {
            try
            {
                _logger.Info("Starting the process data task");

                // GetConsuming Enumerable will iterate over the data as it comes in the data queue, when no data it will just wait indefinitely.
                // DataQueue.CompleteAdding() will terminate to process remaining data in the queue and terminate the enumeration.
                foreach (IEnumerable<AFValues> dataFromQueue in _dataQueue.GetConsumingEnumerable())
                {
                    List<AFValues> dataList = null;

                    // this will force enumeration of all the values, and may proceed with remaining data calls that were not yet completed
                    if (_forceDataEnumeration)
                    {
                        dataList = dataFromQueue.ToList();

                        var count = dataList.Sum(v => v.Count);

                        TotalEventProcessed += count;
                    }
                        
                    

                    try
                    {

                        if (_dataProcessingAction != null)
                            _dataProcessingAction(dataList ?? dataFromQueue);

                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }
            }

            catch (Exception ex)
            {
                if (!(ex is OperationCanceledException))
                    _logger.Error(ex);
            }
            finally
            {
                if (_finalAction != null) _finalAction?.Invoke();
            }
        }

    }
}
