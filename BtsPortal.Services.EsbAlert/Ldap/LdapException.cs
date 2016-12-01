//---------------------------------------------------------------------
// File: LdapException.cs
// 
// Summary: Custom exception that is thrown in the case of an Ldap
//          access failure.
//          
//---------------------------------------------------------------------
// This file is part of the Microsoft ESB Guidance for BizTalk
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
// This source code is intended only as a supplement to Microsoft BizTalk
// Server 2006 R2 release and/or on-line documentation. See these other
// materials for detailed information regarding Microsoft code samples.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace BtsPortal.Services.EsbAlert.Ldap
{
   [Serializable]
    internal class LdapException : Exception
    {
        public LdapException() : base() { }

        public LdapException(string message) : base (message) {}

        protected LdapException(SerializationInfo info, StreamingContext context ) : base( info, context ) { }
    }
}
