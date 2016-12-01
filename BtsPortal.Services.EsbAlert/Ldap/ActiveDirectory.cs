//---------------------------------------------------------------------
// File: ActiveDirectory.cs
// 
// Summary: Provides access to Active Directory structures such as users
//          and security groups.
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

using System.DirectoryServices;

namespace BtsPortal.Services.EsbAlert.Ldap
{
    internal static class ActiveDirectory
    {
        /// <summary>
        /// Gets all of the users within the specified group.
        /// </summary>
        /// <param name="rootContainer">The DirectoryEntry from which to begin the recursive search.</param>
        /// <returns>A SearchResultCollection containing all directory entries that match the search criteria.</returns>
        /// <exception cref="LdapException">When no user under group supplied is encountered</exception>
        internal static SearchResultCollection GetUsersInGroup(string groupName, DirectoryEntry rootContainer)
		  {
            //Get distinguishedName for the group
            string distinguishedName = GetDistinguishedName(groupName, rootContainer);

            //create instance of the direcory searcher
            using (DirectorySearcher deSearch = new DirectorySearcher(rootContainer))
            {
               //set the search filter
               deSearch.Filter = "(&(objectClass=user)(memberOf=" + distinguishedName + "))";

               //Set the search scope
               deSearch.SearchScope = SearchScope.Subtree;

               //find all instances
               SearchResultCollection results = deSearch.FindAll();

               if (results.Count == 0)
               {
                  throw new LdapException("No users entry found in Active Directory with group name '" + groupName + "under '" + distinguishedName);
               }
               else
               {
                  return results;
               }
            }
		 }


        /// <summary>
        /// Gets all of the user groups under the specified root.
        /// </summary>
        /// <param name="rootContainer"></param>
        /// <returns></returns>
        internal static string GetDistinguishedName(string groupName, DirectoryEntry rootContainer)
        {
           //create instance of the direcory searcher
           using (DirectorySearcher deSearch = new DirectorySearcher(rootContainer))
           {
              //set the search filter
              deSearch.Filter = "(&(objectClass=group)(Name=" + groupName + "))";

              //Set the search scope
              deSearch.SearchScope = SearchScope.Subtree;

              //find all instances
              SearchResultCollection results = deSearch.FindAll();

              if (results.Count == 1)
              {

                 return results[0].GetDirectoryEntry().Properties["distinguishedName"].Value.ToString();
              }
              else
              {
                 throw new LdapException("Unable to retrieve distinguished name for group '" + groupName + " under '" + rootContainer + "'");
              }
           }
        }

        /// <summary>
        /// Gets the user with the specified login name.
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="parentContainer"></param>
        /// <returns></returns>
        internal static DirectoryEntry GetUserByLoginName(string loginName, DirectoryEntry parentContainer)
        {
            //create instance of the direcory searcher
           using (DirectorySearcher deSearch = new DirectorySearcher(parentContainer))
           {
              deSearch.Filter = "(&(objectClass=user)(samAccountName=" + loginName.Substring(loginName.IndexOf("\\") + 1) + "))";
              
              //find the first instance
              SearchResult result = deSearch.FindOne();

              if (result == null)
              {
                 throw new LdapException("Unable to retrieve user for login name '" + loginName + "'");
              }
              else
              {
                 return result.GetDirectoryEntry();
              }
           }
        }

        /// <summary>
        /// This will return a DirectoryEntry object if the user does exist, otherwise it will return null.
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        internal static DirectoryEntry GetUserByCanonicalName(string canonicalName, DirectoryEntry parentContainer)
        {
            //create instance of the direcory searcher
           using (DirectorySearcher deSearch = new DirectorySearcher(parentContainer))
           {
              //set the search filter
              deSearch.Filter = "(&(objectClass=user)(cn=" + canonicalName + "))";
              deSearch.SearchScope = SearchScope.Subtree;

              //find the first instance
              SearchResult result = deSearch.FindOne();

              if (result == null)
              {
                 throw new LdapException("Unable to retrieve user for canonical name '" + canonicalName + " under '" + parentContainer + "'");
              }
              else
              {
                 return result.GetDirectoryEntry();
              }
           }
        }
    }
}
