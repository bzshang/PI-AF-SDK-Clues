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
using log4net;
using OSIsoft.AF;
using OSIsoft.AF.PI;


namespace Clues.Library
{
    /// <summary>
    /// Helps managing connection to PI Server
    /// </summary>
    public class PiConnectionMgr
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(PiConnectionMgr));
        private readonly PIServers _piServers=new PIServers();
        private readonly PIServer _piServer;

        public static PiConnectionMgr ConnectAndGetServer(string server, out PIServer piServer)
        {
            var manager=new PiConnectionMgr(server);
            manager.Connect();
            piServer = manager.GetPiServer();

            return manager;

        }
        
        
        public PIServer GetPiServer()
        {
            return _piServer;
        }

        /// <summary>
        /// Initialize a PI Server Connnection object.
        /// You should call the Connect method before access data with the PIServer property
        /// </summary>
        /// <param name="server">Name of the PI System (AF Server) to connect to</param>
        public PiConnectionMgr(string server)
        {
            // if the server does not exist in the local KST we throw an exception.  This is a choice made, you could decide to connect anyway.
            // to do so you'll need to look at the PIServers.DirectoryOptions Property: https://techsupport.osisoft.com/Documentation/PI-AF-SDK/html/P_OSIsoft_AF_PI_PIServers_DirectoryOptions.htm
            // ex: 
            //PIServers.DirectoryOptions=PISystems.AFDirectoryOptions.AutoAdd;
            
            if (_piServers.Contains(server))
                _piServer = _piServers[server];
            else
            {
                throw new KeyNotFoundException("Specified PI System does not exist");    
            }
            
            
        }

        public bool Connect()
        {
            _logger.InfoFormat("Trying to connect to PI Data Archive {0}. As {1}", _piServer.Name, _piServer.CurrentUserName);

            try
            {
                _piServer.Connect(true, null);

                _logger.InfoFormat("Connected to {0}. As {1}", _piServer.Name, _piServer.CurrentUserName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                
            }
            
            return false;


        }

    }
}
