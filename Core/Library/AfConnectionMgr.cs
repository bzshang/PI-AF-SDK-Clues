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

namespace Clues.Library
{
    /// <summary>
    /// This class is responsible of connecting to PI System (AF) and to select the database to work with
    /// It also keeps handles on all relevant objects
    /// 
    /// If you want to connect to a server that is on a different domain, 
    /// 
    /// <remarks>
    /// Some interresting technical details from david hearn on PI Square: 
    /// 1) You should always create a new 'PISystems' object for each different user. The 'PISystems' object behaves like a singleton per user.
    /// 2) It is safe to use the same PISystem and related DatabaseName and Element objects in multiple threads for the same user. But you will want to be careful about iterating through collections while adding/removing objects in the collections.
    /// </remarks>
    /// <see cref="https://pisquare.osisoft.com/message/29669#29669"/>
    /// </summary>
    public class AfConnectionMgr
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(AfConnectionMgr));
        private readonly PISystems _piSystems=new PISystems();
        private readonly PISystem _piSystem;
        private AFDatabase _afDatabase;
        private string _databaseName;
        
        public string DatabaseName
        {
            get { return _databaseName; }
            set
            {
                _databaseName = value;
                ChangeDatabase();
            }
        }

        public AFDatabase GetDatabase()
        {
            return _afDatabase;
        }

        public PISystem GetPiSystem()
        {
            return _piSystem;
        }

        /// <summary>
        /// Initialize an AF Connnection object.
        /// it is required to call the Connect method before getting Database or the connected PISystem object
        /// </summary>
        /// <param name="server">Name of the PI System (AF Server) to connect to</param>
        /// <param name="databaseName">Name of the AF DatabaseName to connect to</param>
        public AfConnectionMgr(string server, string databaseName=null)
        {

            if (_piSystems.Contains(server))
                _piSystem = _piSystems[server];
            else
            {
                // in case you would like to force the auto-creation of the PI System in KST if it does not exist, uncomment the following two lines
                // and remove the exception throwing
                //
                // PISystems.DirectoryOptions = PISystems.AFDirectoryOptions.AutoAdd;
                // _piSystem = _piSystems[server];
                
                throw new KeyNotFoundException("Specified PI System does not exist");    
            }
            
            _databaseName = databaseName;
            
        }

        /// <summary>
        /// Connect to the PI system passed when creating the PiConnectionMgr Object.
        /// Database will be selected if property contains a valid database name.
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            _logger.InfoFormat("Trying to connect {0}. As {1}", _piSystem.Name, _piSystem.CurrentUserName);

            try
            {
                _piSystem.Connect(true, null);

                if (!string.IsNullOrEmpty(_databaseName))
                {
                    ChangeDatabase();    
                }

                _logger.InfoFormat("Connected to {0}. As {1}", _piSystem.Name, _piSystem.CurrentUserName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                
            }
            
            return false;


        }

        private void ChangeDatabase()
        {
            
            if (_piSystem.Databases.Contains(_databaseName))
            {
                _afDatabase = _piSystem.Databases[_databaseName];
                _logger.InfoFormat("Using af database {0}",_databaseName);
            }
            else
            {
                _afDatabase = null;
                _logger.Warn("Specified AF DatabaseName does not exist.");
            }
        }

    }
}
