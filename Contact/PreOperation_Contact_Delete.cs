// <copyright file="PreOperation_Contact_Delete.cs" company="TECHXACT">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Post Operation SettlementParty Create DuplicateReocrd Check Plugin.</summary>
namespace CRM.Plugins.Practice
{
    using System;
    using System.Globalization;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Pre Operation Contact Delete
    /// </summary>
    public class PreOperation_Contact_Delete : IPlugin
    {
        /// <summary>
        /// Execute(IServiceProvider serviceProvider)
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider serviceProvider</param>
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider != null)
            {
                ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference)
                {
                    EntityReference entity = (EntityReference)context.InputParameters["Target"];
                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                    try
                    {
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, " Pre Operation Contact Delete Execution started: {0} ", DateTime.Now));
                        bool hasRole = IsRoleThere(service);
                        if (entity.LogicalName != "contact")
                        {
                            return;
                        }

                        if (entity.LogicalName == "contact")
                        {
                            Guid contactId = entity.Id;
                            Entity contact = new Entity("contact");
                            ColumnSet isContactDeleteAllowed = new ColumnSet(new string[] { "cdi_iscontactdeleteallowed" });
                            contact = service.Retrieve(contact.LogicalName, contactId, isContactDeleteAllowed);

                            bool iscontactdelete = contact.Attributes.Contains("cdi_iscontactdeleteallowed") ? (bool)contact.Attributes["cdi_iscontactdeleteallowed"] : false;
                            if (!iscontactdelete && !hasRole)
                            {
                                throw new InvalidPluginExecutionException("You don't have permission or value of is allowed delete is no");
                            }
                        }

                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, " Pre Operation Contact Delete Execution Ended: {0} ", DateTime.Now));
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidPluginExecutionException(ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Is Role there(service)
        /// </summary>
        /// <param name="service">IOrganizationService service</param>
        /// <returns>is User Has Role</returns>
        public static bool IsRoleThere(IOrganizationService service)
        {
            bool isUserHasRole = false;
            if (service != null)
            {
                string fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>" +
                                     "<entity name='role'>" +
                                      "<attribute name='name' />" +
                                      "<attribute name='businessunitid' />" +
                                      "<attribute name='roleid' />" +
                                      "<order attribute='name' descending='false' />" +
                                      "<link-entity name='systemuserroles' from='roleid' to='roleid' visible='false' intersect='true'>" +
                                       "<link-entity name='systemuser' from='systemuserid' to='systemuserid' alias='ab'>" +
                                        "<filter type='and'>" +
                                         "<condition attribute='systemuserid' operator='eq-userid' />" +
                                        "</filter>" +
                                       "</link-entity>" +
                                      "</link-entity>" +
                                     "</entity>" +
                                  "</fetch>";
                EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(fetchXml));
                if (entityCollection != null && entityCollection.Entities.Count > 0)
                {
                    string rolename = string.Empty;
                    foreach (var role in entityCollection.Entities)
                    {
                        rolename = role.Attributes.Contains("name") ? Convert.ToString(role.Attributes["name"], CultureInfo.InvariantCulture) : string.Empty;
                        if (rolename.ToUpperInvariant() == "SYSTEM ADMINISTRATOR")
                        {
                            isUserHasRole = true;
                            break;
                        }
                    }
                }
            }
            return isUserHasRole;
        }
    }
}