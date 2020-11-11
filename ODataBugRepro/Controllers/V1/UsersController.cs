using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace ODataBugRepro.Controllers.V1
{
    [Authorize]
    [ApiVersion("1")]
    [ODataRoutePrefix("Users")]
    [Produces("application/json")]
    public class UsersController : ODataController
    {
        [EnableQuery]
        public IQueryable<User> Get() => GetUsers();

        [EnableQuery]
        [ODataRoute("({id})")]
        public IQueryable<User> Get(int id) => GetUsers().Where(f => f.Id == id);

        private static IQueryable<User> GetUsers() => new List<User>
        {
            new User
            {
                Id = 1,
                Firstname = "Bob",
                Lastname = "Marley",
            },
            new User
            {
                Id = 2,
                Firstname = "Mahatma",
                Lastname = "Gandhi",
            },
        }.AsQueryable();
    }
}
