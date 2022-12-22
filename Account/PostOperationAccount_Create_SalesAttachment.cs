// <copyright file="PostOperationAccount_Create_SalesAttachment.cs" company="TECHXACT">
// Copyright (c) 2022 All Rights Reserved
// </copyright>
// <author></author>
// <date>20/10/2022 1:09:36 PM</date>
// <summary>Implements the Post Operation Account Create SalesAttachment Plugin.</summary>
namespace CRM.Plugins.Practice.Account
{
    using System;
    using System.ServiceModel;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System.Globalization;

    /// <summary>
    /// Post Operation Account Create SalesAttachment
    /// </summary>
    public class PostOperationAccount_Create_SalesAttachment : IPlugin
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
                        SalesAttachments(service, context, entity);
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
        /// Implement the Sales Attachments
        /// </summary>
        /// <param name="serviceProvider">IOrganizationService serviceProvider</param>
        /// <param name="context">IPluginExecutionContext context</param>
        /// <param name="entity">Entity entity</param>
        private static void SalesAttachments(IOrganizationService serviceProvider, IPluginExecutionContext context, Entity entity)
        {
            Entity email = new Entity("email");
            Entity fromActivityprty = new Entity("activityparty");
            Entity toActivityParty = new Entity("activityparty");
            fromActivityprty["partyid"] = new EntityReference("systemuser", context.UserId);
            toActivityParty["partyid"] = new EntityReference("account", entity.Id);
            email["from"] = new Entity[] { fromActivityprty };
            email["to"] = new Entity[] { toActivityParty };
            email["subject"] = "Creating Email.";
            email["description"] = "Creating Email";
            email["directioncode"] = true;
            Guid emailId = serviceProvider.Create(email);

            QueryExpression queryExpression = new QueryExpression("cdi_salesattachmentt");
            queryExpression.ColumnSet = new ColumnSet(true);

            EntityCollection salesCollection = serviceProvider.RetrieveMultiple(queryExpression);
            if (salesCollection != null && salesCollection.Entities.Count > 0)
            {
                foreach (var salesAttachements in salesCollection.Entities)
                {
                    QueryExpression getAllAnnotations = new QueryExpression("annotation");
                    getAllAnnotations.ColumnSet = new ColumnSet(true);
                    getAllAnnotations.Criteria.AddCondition("objectid", ConditionOperator.Equal, salesAttachements.Id);

                    EntityCollection annotationCollection = serviceProvider.RetrieveMultiple(getAllAnnotations);
                    if (annotationCollection != null && annotationCollection.Entities.Count > 0)
                    {
                        foreach (var oldNotes in annotationCollection.Entities)
                        {
                            Entity attachment = new Entity("activitymimeattachment");
                            attachment["subject"] = oldNotes.Contains("subject") ? Convert.ToString(oldNotes["subject"], CultureInfo.InvariantCulture) : string.Empty;
                            if (oldNotes.Contains("isdocument") && (Convert.ToBoolean(oldNotes.Attributes["isdocument"], CultureInfo.InvariantCulture) == true))
                            {
                                attachment["body"] = oldNotes["documentbody"];
                                attachment["filename"] = oldNotes.Contains("filename") ? Convert.ToString(oldNotes["filename"], CultureInfo.InvariantCulture) : string.Empty;
                                attachment["mimetype"] = oldNotes["mimetype"];
                            }

                            attachment["objectid"] = new EntityReference("email", emailId);
                            attachment["attachmentnumber"] = 1;
                            attachment["objecttypecode"] = "email";
                            serviceProvider.Create(attachment);
                        }
                    }
                }
            }
        }
    }
}
