using metalduck.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace metalduck.Authorization
{
    public class DocumentIsOwnerAuthorizationHandler
                : AuthorizationHandler<OperationAuthorizationRequirement, Document>
    {
        UserManager<IdentityUser> _userManager;

        public DocumentIsOwnerAuthorizationHandler(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            Document document)
        {
            if (context.User == null || document == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for CRUD permission, return.
            if (requirement.Name != Constants.CreateOperationName &&
                requirement.Name != Constants.DeleteOperationName &&
                requirement.Name != Constants.ShareOperationName)
            {
                return Task.CompletedTask;
            }

            if (document.OwnerId == _userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}