//---------------------------------------------------------------------
// File: ActiveDirectoryHelper.cs
// 
// Summary: Gets email addresses for users and groups from Active Directory.
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
using System.DirectoryServices;
using System.Globalization;
using BtsPortal.Cache;
using BtsPortal.Services.EsbAlert.Ldap;

namespace BtsPortal.Services.EsbAlert
{
    internal static class ActiveDirectoryHelper
    {
        //private static Microsoft.Practices.EnterpriseLibrary.Caching.ICacheManager CacheMgr =
        //      Microsoft.Practices.EnterpriseLibrary.Caching.CacheFactory.GetCacheManager();

        /// <summary>
        /// Attempts to connect to Active Directory using the LDAP connection string
        /// defined in the Configuration database table.  Throws an AlertException if
        /// Active Directory cannot be reached.
        /// </summary>
        internal static void TestLDAPConnectivity(string ldapRoot)
        {
            if (!string.IsNullOrEmpty(ldapRoot) && string.Compare("LDAP://domaincontroller.domain.com/DC=domain, DC=com",
                ldapRoot, true, System.Globalization.CultureInfo.InvariantCulture) != 0)
            {
                using (DirectoryEntry root = new DirectoryEntry(ldapRoot))
                {
                    TestLDAPConnectivity(root);
                }
            }
        }

        /// <summary>
        /// Attempts to connect to Active Directory using the LDAP connection string
        /// defined in the Configuration database table.  Throws an AlertException if
        /// Active Directory cannot be reached.
        /// </summary>
        /// <param name="entryToTest">The DirectoryEntry object for which to test the connection.</param>
        private static void TestLDAPConnectivity(DirectoryEntry entryToTest)
        {
            string errorMessage =
                string.Format(CultureInfo.CurrentCulture, "A connection to Active Directory could not be established using the LDAP connection string: '{0}'. Verify that the correct LDAP connection string is configured in the Fault Settings page of the ESB Management Portal.",
               entryToTest.Path);

            try
            {
                bool entryIsEmpty = true;

                //Loop through the children to see if any children actually exist.
                foreach (DirectoryEntry child in entryToTest.Children)
                {
                    entryIsEmpty = false;
                }

                //If no children exist, then assume the LDAP connection is bad.
                if (entryIsEmpty)
                {
                    throw new LdapException(errorMessage);
                }
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                throw new LdapException(errorMessage);
            }
        }

        /// <summary>
        /// Gets the email address for a user.
        /// </summary>
        /// <param name="name">The user name.</param>
        /// <returns>A string containing the email address.</returns>
        internal static string GetEmailAddress(string name, ICacheProvider provider, string ldapRoot)
        {
            object value = provider.Get(name);
            if (value != null)
            {
                System.Diagnostics.Trace.WriteLine("Reusing email address for " + name + " from cache.");
                return (string)value;
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Retrieving from AD and caching email address for: " + name + ".");

                using (DirectoryEntry root = new DirectoryEntry(ldapRoot))
                using (DirectoryEntry u = ActiveDirectory.GetUserByLoginName(name, root))
                {
                    if (string.IsNullOrEmpty(u?.Properties["mail"].Value.ToString()))
                    {
                        throw new Exception(string.Format(CultureInfo.InvariantCulture, "Unable to retrieve information for '{0}' from AD.", name));
                    }
                    else
                    {
                        provider.Set(name, u.Properties["mail"].Value.ToString());
                        return u.Properties["mail"].Value.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves a search result collection containing the directory entry account object for
        /// each group member.
        /// </summary>
        /// <param name="groupName">The name of the security group.</param>
        /// <returns>A SearchResultCollection containing the user object for each member of the group.</returns>
        internal static SearchResultCollection GetMembersOfGroup(string groupName, ICacheProvider provider, string ldapRoot)
        {
            object value = provider.Get(groupName);
            if (value != null)
            {
                System.Diagnostics.Trace.WriteLine("Reusing group membership for " + groupName + " from cache.");
                return (SearchResultCollection)provider.Get(groupName);
            }
            else
            {
                using (DirectoryEntry root = new DirectoryEntry(ldapRoot))
                {
                    System.Diagnostics.Trace.WriteLine("Retrieving from AD and caching group membership for: " + groupName + ".");
                    SearchResultCollection usersInGroup = ActiveDirectory.GetUsersInGroup(groupName.Substring(groupName.IndexOf("\\") + 1), root);
                    provider.Set(groupName, usersInGroup);
                    return usersInGroup;
                }
            }
        }
    }
}
