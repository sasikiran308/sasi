// <copyright file="PostOperationEmail_Create_AddTxtAttachmentToEmail.cs" company="TECHXACT">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Post Operation Email Create Add Txt Attachment To Email Plugin.</summary>
namespace CRM.Plugins.Practice.Email
{
    using System;
    using System.Globalization;
    using System.ServiceModel;
    using System.Text;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Post Operation Email Create Add Txt Attachment To Email
    /// </summary>
    public class PostOperationEmail_Create_AddTxtAttachmentToEmail : IPlugin
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
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Post-Operation Account Execution Started:{0}", DateTime.Now));
                        AddTxtAttachmentToEmail(service, entity);
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Post-Operation Account Execution Ended:{0}", DateTime.Now));
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
        /// Implement the Add Txt Attachment To Email
        /// </summary>
        /// <param name="service">IOrganizationService service</param>
        /// <param name="entity">Entity entity</param>
        private static void AddTxtAttachmentToEmail(IOrganizationService service, Entity entity)
        {
            if (entity.Contains("regardingobjectid") && (entity.Attributes["regardingobjectid"] as EntityReference).LogicalName == "contact")
            {
                Entity contact = service.Retrieve("contact", (entity.Attributes["regardingobjectid"] as EntityReference).Id, new ColumnSet("description"));
                if (contact != null && contact.Contains("description"))
                {
                    string description = contact.Attributes.Contains("description") ? Convert.ToString(contact.Attributes["description"], CultureInfo.InvariantCulture) : string.Empty;
                    Entity attachment = new Entity("activitymimeattachment");
                    attachment["subject"] = "Attachment";
                    string filename = "Sasi.txt";
                    attachment["filename"] = filename;
                    byte[] fileStream = Encoding.ASCII.GetBytes(description);

                    attachment["body"] = Convert.ToBase64String(fileStream);
                    attachment["mimetype"] = "text/plain";
                    attachment["objectid"] = new EntityReference("email", entity.Id);
                    attachment["objecttypecode"] = "email";
                    service.Create(attachment);
                }
            }
        }
    }
}
