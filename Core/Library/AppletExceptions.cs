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


namespace Clues.Library
{

    /// <summary>
    /// This files contains classes that inherits from the base .NET exception.
    /// It is an elegant way to create more useful exceptions.
    /// </summary>

    public class AFAttributeNotFoundException : Exception
    {
        public AFAttributeNotFoundException() : base("AF Attribute not found") { }
    }
    
    public class AFObjectIsNotAnAttribute : Exception
    {
        public AFObjectIsNotAnAttribute() : base("AF object resolved is not an AF attribute.") { }
    }

    public class AFElementTemplateNotFound : Exception
    {
        public AFElementTemplateNotFound() : base("The AF Element template was not present in the database.") { }
    }

    public class AFTableNotFoundException : Exception
    {
        public AFTableNotFoundException() : base("AF table was not found"){}
    }


    public class InvalidAFObjectException : Exception
    {
        public InvalidAFObjectException() : base("The parsed AF object was not of the expected type.") { }
    }


}
