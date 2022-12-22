// <copyright file="CreateContact.cs" company="TECHXACT">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Post Operation SettlementParty Create DuplicateReocrd Check Plugin.</summary>
namespace CRM.Plugins.Practice
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using CRM.Common;
    using System.Globalization;

    /// <summary>
    /// Create Contact
    /// </summary>
    public class CreateContact : IPlugin
    {
        /// <summary>
        /// Execute(IServiceProvider serviceProvider)
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
                        // Task Activity
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "PostContact Create Plugin Execution Started:{0}", DateTime.Now));
                        Common.CreateTask(service, entity);

                        // Email Activity
                        //Entity email = new Entity("email");
                        //Entity fromActivityparty = new Entity("activityparty");
                        //Entity toActivityParty = new Entity("activityparty");
                        //fromActivityparty["partyid"] = new EntityReference("systemuser", new Guid("b9a20cd3-ec25-ed11-9db1-000d3a5c30a2"));
                        //toActivityParty["partyid"] = new EntityReference(entity.LogicalName, entity.Id);
                        //email["from"] = new Entity[] { fromActivityparty };
                        //email["to"] = new Entity[] { toActivityParty };
                        //email["regardingobjectid"] = new EntityReference(entity.LogicalName, entity.Id);
                        //email["subject"] = "Creating email Activity from Plugin.";
                        //email["description"] = "Creating email Activity from Plugin.";
                        //email["directioncode"] = true;
                        //Guid emailId = service.Create(email);

                        //// PhoneCall Activity
                        //Entity phonecall = new Entity("phonecall");
                        //phonecall["to"] = new Entity[] { toActivityParty };
                        //phonecall["from"] = new Entity[] { fromActivityparty };
                        //phonecall["regardingobjectid"] = new EntityReference(entity.LogicalName, entity.Id);
                        //phonecall["subject"] = "Making Phone Call Activity from plugin.";
                        //phonecall["description"] = "Making Phone Call Activity from plugin.";
                        //Guid phonecallId = service.Create(phonecall);

                        //// Letter Activity
                        //Entity letter = new Entity("letter");
                        //letter["from"] = new Entity[] { fromActivityparty };
                        //letter["to"] = new Entity[] { toActivityParty };
                        //letter["regardingobjectid"] = new EntityReference(entity.LogicalName, entity.Id);
                        //letter["subject"] = "Creating Letter from plugin.";
                        //letter["description"] = "Creating Letter from plugin.";
                        //Guid letterID = service.Create(letter);

                        //// Fax
                        //Entity fax = new Entity("fax");
                        //fax["from"] = new Entity[] { fromActivityparty };
                        //fax["to"] = new Entity[] { toActivityParty };
                        //fax["regardingobjectid"] = new EntityReference(entity.LogicalName, entity.Id);
                        //fax["subject"] = "Sending fax from plugin.";
                        //fax["description"] = "Sending fax from plugin.";
                        //Guid faxID = service.Create(fax);

                        //tracingService.Trace($"Task Created Succefully Task id={taskid} and email created successfully with guid {emailId}");
                        //tracingService.Trace($"phone call Created Succefully Task id={phonecallId} and letter created successfully with guid {letterID}");
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "PostContact Create Plugin Execution Ended:{0}", DateTime.Now));
                    }
                    catch (FaultException<OrganizationServiceFault> ex)
                    {
                        throw new InvalidPluginExecutionException("An error occurred in Create Contact.", ex);
                    }
                    catch (Exception ex)
                    {
                        tracingService.Trace("CreateContact: {0}", ex.ToString());
                        throw;
                    }
                }
            }
        }
    }
}
