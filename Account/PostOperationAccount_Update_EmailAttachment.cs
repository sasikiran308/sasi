// <copyright file="PostOperationAccount_Update_EmailAttachment.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Post Operation Account Update Email Attachment Plugin.</summary>
namespace CRM.Plugins.Practice.Account
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Post Operation Account Update Email Attachment
    /// </summary>
    public class PostOperationAccount_Update_EmailAttachment : IPlugin
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
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Post Operation Account Update Email Attachment Execution Started:{0}", DateTime.Now));
                        RetrieveNotesAttachment(service, context, entity);
                        tracingService.Trace(string.Format(CultureInfo.InvariantCulture, "Post Operation Account Update Email Attachment Execution Ended:{0}", DateTime.Now));
                    }
                    catch (FaultException<OrganizationServiceFault> ex)
                    {
                        throw new InvalidPluginExecutionException("An error occurred in Post Operation Account Update Email Attachment.", ex);
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
        /// RetrieveNotesAttachment(organizationService, context, entity)
        /// </summary>
        /// <param name="organizationService">IOrganizationService organizationService</param>
        /// <param name="context"> IPluginExecutionContext context </param>
        /// <param name="entity"> Entity entity </param>
        private static void RetrieveNotesAttachment(IOrganizationService organizationService, IPluginExecutionContext context, Entity entity)
        {
            // Create Email
            Entity email = new Entity("email");
            Entity fromActivityprty = new Entity("activityparty");
            Entity toActivityParty = new Entity("activityparty");
            fromActivityprty["partyid"] = new EntityReference("systemuser", context.UserId);
            toActivityParty["partyid"] = new EntityReference(entity.LogicalName, entity.Id);
            email["from"] = new Entity[] { fromActivityprty };
            email["to"] = new Entity[] { toActivityParty };
            email["regardingobjectid"] = new EntityReference(entity.LogicalName, entity.Id);
            email["subject"] = "Schedule CRM interviews with our clients.";
            Guid emailId = organizationService.Create(email);

            QueryExpression queryNotes = new QueryExpression("annotation");
            queryNotes.ColumnSet = new ColumnSet(new string[] { "subject", "mimetype", "filename", "documentbody" });
            queryNotes.Criteria = new FilterExpression();
            queryNotes.Criteria.FilterOperator = LogicalOperator.And;
            queryNotes.Criteria.AddCondition(new ConditionExpression("objectid", ConditionOperator.Equal, new Guid("49f1a375-b33f-ed11-9db0-000d3a31c3fa")));

            EntityCollection mimeCollection = organizationService.RetrieveMultiple(queryNotes);
            if (mimeCollection.Entities.Count > 0)
            {
                Entity notesAttachment = mimeCollection.Entities.First();

                // Create email attachment
                Entity emailAttachment = new Entity("activitymimeattachment");
                emailAttachment["subject"] = notesAttachment.GetAttributeValue<string>("subject");
                emailAttachment["objectid"] = new EntityReference("email", emailId);
                emailAttachment["objecttypecode"] = "email";
                emailAttachment["filename"] = notesAttachment.GetAttributeValue<string>("filename");
                emailAttachment["body"] = notesAttachment.GetAttributeValue<string>("documentbody");
                emailAttachment["mimetype"] = notesAttachment.GetAttributeValue<string>("mimetype");
                organizationService.Create(emailAttachment);
            }
        }
    }
}
