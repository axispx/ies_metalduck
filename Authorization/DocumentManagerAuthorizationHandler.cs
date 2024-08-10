using System.Threading.Tasks;
using metalduck.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace metalduck.Authorization
{
    public class DocumentManagerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Document>
    {
        protected override Task
            HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Document document)
        {
            if (context.User == null || document == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for approval/reject, return.
            if (requirement.Name != Constants.ApproveOperationName &&
                requirement.Name != Constants.RejectOperationName &&
                requirement.Name != Constants.ShareOperationName)
            {
                return Task.CompletedTask;
            }

            // Managers can approve, reject or download.
            if (context.User.IsInRole(Constants.DocumentManagersRole))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}