// <copyright file="PreOperationAccount_Update_AllContactsCurrentStatus.cs" company="TECHXACT">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Post Operation SettlementParty Create DuplicateReocrd Check Plugin.</summary>
namespace CRM.Plugins.Practice
{
    using System;
    using System.Globalization;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Pre Operation Account Update All Contacts Current Status
    /// </summary>
    public class PreOperationAccount_Update_AllContactsCurrentStatus : IPlugin
    {
        /// <summary>
        ///  Execute(service Provider)
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider serviceProvider</param>
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider != null)
            {
                // Obtain the tracing service
                ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                // Obtain the execution context from the service provider.  
                IPluginExecutionContext context = (IPluginExecutionContext)
                    serviceProvider.GetService(typeof(IPluginExecutionContext));

                // The InputParameters collection contains all the data passed in the message request.  
                if (context.InputParameters.Contains("Target") &&
                    context.InputParameters["Target"] is Entity)
                {
                    // Obtain the target entity from the input parameters.  
                    Entity entity = (Entity)context.InputParameters["Target"];

                    // Obtain the organization service reference which you will need for  
                    // web service calls.  
                    IOrganizationServiceFactory serviceFactory =
                        (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                    try
                    {
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Pre Operation Account Create StatusUpdate Execution Started:{0}", DateTime.Now));

                        //QueryExpression queryExpression = new QueryExpression("contact");
                        //queryExpression.ColumnSet.AddColumn("parentcustomerid");
                        //queryExpression.Criteria.AddCondition("parentcustomerid", ConditionOperator.Equal, entity.Id);
                        string fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                                            "  <entity name='contact'>" +
                                            "    <attribute name='fullname' />" +
                                            "    <attribute name='telephone1' />" +
                                            "    <attribute name='contactid' />" +
                                            "    <order attribute='fullname' descending='false' />" +
                                            "    <filter type='and'>" +
                                            "      <condition attribute='parentcustomerid' operator='eq' uiname='' uitype='account' value='" + entity.Id + "' />" +
                                            "    </filter>" +
                                            "  </entity>" +
                                            "</fetch>";

                        EntityCollection entityCollection = service.RetrieveMultiple(new FetchExpression(fetchXml));
                        if (entityCollection.Entities.Count > 0)
                        {
                            foreach (var account in entityCollection.Entities)
                            {
                                Entity contact = new Entity("contact");
                                contact.Id = account.Id;
                                contact["cdi_contactcurrentstatus"] = (OptionSetValue)entity.Attributes["cdi_accountcurrentstatus"];
                                service.Update(contact);
                            }
                        }

                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Pre Operation Account Create StatusUpdate Execution Ended:{0}", DateTime.Now));
                    }
                    catch (FaultException<OrganizationServiceFault> ex)
                    {
                        throw new InvalidPluginExecutionException("An error occurred in Post Operation Account Create Status Update.", ex);
                    }
                    catch (Exception ex)
                    {
                        tracingService.Trace("Plugin: {0}", ex.ToString());
                        throw;
                    }
                }
            }
        }
    }
}
