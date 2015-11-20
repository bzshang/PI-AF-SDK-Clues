using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Data;
using OSIsoft.AF.PI;

namespace Clues
{
    /// <summary>
    /// DataPipe Handler class
    /// Originally Written by Ian Gore --> thanks for you contribution Ian!
    /// <see cref="https://pisquare.osisoft.com/community/developers-club/pi-net-framework-pi-af-sdk/blog/2015/03/10/how-to-use-the-pidatapipe-or-the-afdatapipe#comment-2839"/>
    /// </summary>
    public class DataPipeHandler : IDisposable
    {
        // The pipe used depends on the constructor
        private readonly AFDataPipe _AFPipe;
        private readonly PIDataPipe _PIPipe;
        private Timer _timer;

        /// <summary>
        /// Creates a datapipe handler for an AFDataPipe
        /// </summary>
        /// <param name="observer">The observer object that will receive the data changes</param>
        public DataPipeHandler(IObserver<AFDataPipeEvent> observer)
        {
            _AFPipe = new AFDataPipe();
            _AFPipe.Subscribe(observer);
        }

        /// <summary>
        /// Creates a datapipe handler for a PIDataPipe
        /// </summary>
        /// <param name="observer">The observer object that will receive the data changes</param>
        public DataPipeHandler(IObserver<AFDataPipeEvent> observer, AFDataPipeType pipeType)
        {
            _PIPipe = new PIDataPipe(pipeType);
            _PIPipe.Subscribe(observer);
        }


        // Signup overloads
        public void AddSignups(IList<AFAttribute> attributes)
        {
            if (_AFPipe != null)
            {
                _AFPipe.AddSignups(attributes);
            }
            else
            {
                throw new NullReferenceException("Signups cannot be added to a null AFDataPipe");
            }
        }
        public void AddSignups(IList<PIPoint> piPoints)
        {
            if (_PIPipe != null)
            {
                _PIPipe.AddSignups(piPoints);
            }
            else
            {
                throw new NullReferenceException("Signups cannot be added to a null PIDataPipe");
            }
        }


        // Signup with init event overloads
        public AFListResults<AFAttribute, AFDataPipeEvent> AddSignupsWithInitEvents(IList<AFAttribute> attributes)
        {
            if (_AFPipe != null)
            {
                return _AFPipe.AddSignupsWithInitEvents(attributes);
            }
            else
            {
                throw new NullReferenceException("The AFDataPipe cannot be null");
            }
        }
        public AFListResults<PIPoint, AFDataPipeEvent> AddSignupsWithInitEvents(IList<PIPoint> piPoints)
        {

            if (_PIPipe != null)
            {
                return _PIPipe.AddSignupsWithInitEvents(piPoints);
            }
            else
            {
                throw new NullReferenceException("The PIDataPipe cannot be null");
            }
        }


        // Remove signups overloads
        public void RemoveSignups(IList<AFAttribute> attributes)
        {
            if (_AFPipe != null)
            {
                _AFPipe.RemoveSignups(attributes);
            }
            else
            {
                // Raise exception
            }
        }
        public void RemoveSignups(IList<PIPoint> piPoints)
        {
            if (_PIPipe != null)
            {
                _PIPipe.RemoveSignups(piPoints);
            }
            else
            {
                // Raise exception
            }
        }


        // Start listening by kicking off the timer
        public void StartListening(TimeSpan checkIntervall)
        {
            if (_timer == null)
                _timer = new Timer(CheckForData, null, 0, (int)checkIntervall.TotalMilliseconds);
        }


        // Stop listening by stopping the timer
        public void StopListening()
        {
            if (_timer != null)
                _timer.Dispose();
        }


        // Checks and retrieves the data from the pipe (processing is done
        // by the observer set up in the constructor)
        private void CheckForData(object o)
        {


            bool hasMoreEvents;


            // Get the events on the appropriate pipe
            if (_AFPipe != null)
            {
                _AFPipe.GetObserverEvents();
            }
            else if (_PIPipe != null)
            {
                do
                {
                    _PIPipe.GetObserverEvents(1000, out hasMoreEvents);
                }
                while (hasMoreEvents);
            }
        }


        // Free the resource
        public void Dispose()
        {
            StopListening();
            if (_AFPipe != null) _AFPipe.Dispose();
            if (_PIPipe != null) _PIPipe.Dispose();
        }
    }
}

