using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;

namespace ODataBugRepro
{
    public class User : IModelConfiguration
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string routePrefix)
        {
            builder.EntitySet<User>("Users");
        }
    }
}
