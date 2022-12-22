using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Globalization;

namespace CRM.Common
{
    public class Common
    {
        /// <summary>
        /// Implement the create task method.
        /// </summary>
        /// <param name="service">IOrganizationService service</param>
        /// <param name="entity">Entity entity</param>
        public static void CreateTask(IOrganizationService service, Entity entity)
        {
            if (service != null && entity != null)
            {
                Entity task = new Entity(Task.EntityName);
                task[Task.PrimaryName] = "Task created from test unit.";
                task[Task.Description] = "Task created from test unit. ";
                task[Task.Regarding] = new EntityReference(entity.LogicalName, entity.Id);
                service.Create(task);
            }
        }

        /// <summary>
        /// DuplicateRecord(IOrganizationService organizationService, Entity entity,ITracingService tracingService)
        /// </summary>
        /// <param name="organizationService">IOrganizationService organizationService</param>
        /// <param name="entity">Entity entity</param>
        /// <param name="tracingService">ITracingService tracingService</param>
        public static void DuplicateRecord(IOrganizationService organizationService, Entity entity)
        {
            if (organizationService != null && entity != null)
            {
                string firstname = entity.Attributes.Contains("firstname") ? Convert.ToString(entity.Attributes["firstname"], CultureInfo.InvariantCulture) : string.Empty;
                string lastname = entity.Attributes.Contains("lastname") ? Convert.ToString(entity.Attributes["lastname"], CultureInfo.InvariantCulture) : string.Empty;
                string emailaddress1 = entity.Attributes.Contains("emailaddress1") ? Convert.ToString(entity.Attributes["emailaddress1"], CultureInfo.InvariantCulture) : string.Empty;

                QueryExpression queryExpression = new QueryExpression("contact");
                queryExpression.ColumnSet.AddColumns(new string[] { "firstname", "lastname", "emailaddress1" });
                queryExpression.Criteria.AddCondition("firstname", ConditionOperator.Equal, firstname);
                queryExpression.Criteria.AddCondition("lastname", ConditionOperator.Equal, lastname);
                queryExpression.Criteria.AddCondition("emailaddress1", ConditionOperator.Equal, emailaddress1);

                EntityCollection entityCollection = organizationService.RetrieveMultiple(queryExpression);
                if (entityCollection != null && entityCollection.Entities != null && entityCollection.Entities.Count > 0)
                {
                    throw new InvalidPluginExecutionException("Duplicate record with name" + " " + firstname + " " + lastname + " and with email address" + emailaddress1);
                }
            }
        }
        public static void RetrieveAccountRecord(IOrganizationService service, Entity entity)
        {
            if (service != null && entity != null)
            {
                EntityReference accountReference = entity.Contains("parentcustomerid") ? entity.Attributes["parentcustomerid"] as EntityReference : null;
                if (accountReference != null && accountReference.Id != Guid.Empty)
                {
                    Entity account = service.Retrieve("account", accountReference.Id, new ColumnSet("name"));

                    string name = account.Contains("name") ? Convert.ToString(account.Attributes["name"], CultureInfo.InvariantCulture) : string.Empty;

                    Entity contact = new Entity("contact");
                    contact.Id = entity.Id;
                    contact["firstname"] = name;
                    service.Update(contact);

                    service.Delete(account.LogicalName, account.Id);
                }
            }
        }
    }
}
