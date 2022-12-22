// <copyright file="PostOperationContact_CreateEmail.cs" company="TECHXACT">
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
    using System.Text;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;

    /// <summary>
    /// Post Operation Contact Create Email
    /// </summary>
    public class PostOperationContact_CreateEmail : IPlugin
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
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Pre-Operation Contact CreateEmail Execution Started:{0}", DateTime.Now));
                        CreateEmail(service, entity, context);
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Pre-Operation Contact CreateEmail Execution Ended:{0}", DateTime.Now));
                    }
                    catch (FaultException<OrganizationServiceFault> ex)
                    {
                        throw new InvalidPluginExecutionException("An error occurred in Create Contact.", ex);
                    }
                    catch (Exception ex)
                    {
                        tracingService.Trace("Plugin: {0}", ex.ToString());
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// CreateEmail(service, entity, context)
        /// </summary>
        /// <param name="service">IOrganizationService service</param>
        /// <param name="entity">Entity entity</param>
        /// <param name="context">IPluginExecutionContext context</param>
        public static void CreateEmail(IOrganizationService service, Entity entity, IExecutionContext context)
        {
            if (service != null && entity != null && context != null)
            {
                var fullname = entity.Attributes["fullname"];
                var systemUser = service.Retrieve("systemuser", context.UserId, new Microsoft.Xrm.Sdk.Query.ColumnSet(true));
                var ownername = Convert.ToString(systemUser.Attributes["fullname"], CultureInfo.InvariantCulture);
                Entity email = new Entity("email");
                Entity fromActivityprty = new Entity("activityparty");
                Entity toActivityParty = new Entity("activityparty");
                fromActivityprty["partyid"] = new EntityReference("systemuser", context.UserId);
                toActivityParty["partyid"] = new EntityReference(entity.LogicalName, entity.Id);
                email["from"] = new Entity[] { fromActivityprty };
                email["to"] = new Entity[] { toActivityParty };
                email["regardingobjectid"] = new EntityReference(entity.LogicalName, entity.Id);
                email["subject"] = "Schedule CRM interviews with our clients.";
                StringBuilder mailBody = new StringBuilder();
                mailBody.Append("Hi, " + fullname);
                mailBody.Append("<br>");
                mailBody.Append("Greetings of the Day. We are excited to schedule CRM interviews with our clients.");
                mailBody.Append("<br>");
                mailBody.AppendFormat(CultureInfo.InvariantCulture,"Thanks,");
                mailBody.Append("<br>");
                mailBody.AppendFormat(CultureInfo.InvariantCulture, ownername);
                email["description"] = mailBody.ToString();
                Guid emailId = service.Create(email);
                SendEmailRequest sendEmailRequest = new SendEmailRequest
                {
                    EmailId = emailId,
                    TrackingToken = string.Empty,
                    IssueSend = true
                };
                service.Execute(sendEmailRequest);
            }
        }
    }
}
