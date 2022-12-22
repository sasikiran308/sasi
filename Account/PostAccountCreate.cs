using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;


namespace CRM.Plugins.Practice.Account
{
    public class PostAccountCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider != null)
            {
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity entity = (Entity)context.InputParameters["Target"];
                    try
                    {
                        if (entity.LogicalName == "account")
                        {
                            // Entity acc = new Entity("contact");
                            // acc["firstname"] = "Acc-" + entity.GetAttributeValue<string>("name");
                            // acc["parentcustomerid"] = new EntityReference("account", entity.Id);
                            // service.Create(acc);
                            using (WebClient client = new WebClient())
                            {
                                var contact = new Contact();
                                contact.contactId = new Guid("93ea2fea-b33f-ed11-9db0-000d3a31c3fa");

                                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Contact));
                                MemoryStream memoryStream = new MemoryStream();
                                serializer.WriteObject(memoryStream, contact);
                                var jsonObject = Encoding.Default.GetString(memoryStream.ToArray());


                                var webClient = new WebClient();
                                webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
                                var serviceUrl = "https://sasiazure.azurewebsites.net/api/ContactRetrieve?code=kQchGp/hOf4lEYsfDLaVmnXLfvq41k34kZBOQZf21xleZeL5IWsDJQ==";

                                // upload the data using Post mehtod
                                string response = webClient.UploadString(serviceUrl, jsonObject);
                                entity.Id = entity.Id;
                                entity.Attributes["description"] = response;
                                service.Update(entity);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidPluginExecutionException(ex.Message);
                    }
                }
            }
        }
    }
    public class Contact
    {
        public Guid contactId { get; set; }
    }
}
