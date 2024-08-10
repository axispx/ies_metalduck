using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace metalduck.Authorization
{

    public static class DocumentOperations
    {
        public static OperationAuthorizationRequirement Create =
            new OperationAuthorizationRequirement { Name = Constants.CreateOperationName };
        public static OperationAuthorizationRequirement Delete =
            new OperationAuthorizationRequirement { Name = Constants.DeleteOperationName };
        public static OperationAuthorizationRequirement Approve =
            new OperationAuthorizationRequirement { Name = Constants.ApproveOperationName };
        public static OperationAuthorizationRequirement Reject =
            new OperationAuthorizationRequirement { Name = Constants.RejectOperationName };
        public static OperationAuthorizationRequirement Share =
            new OperationAuthorizationRequirement { Name = Constants.ShareOperationName };
    }

    public class Constants
    {
        public static readonly string CreateOperationName = "Create";
        public static readonly string ShareOperationName = "Share";
        public static readonly string DeleteOperationName = "Delete";
        public static readonly string ApproveOperationName = "Approve";
        public static readonly string RejectOperationName = "Reject";

        public static readonly string DocumentAdministratorsRole ="Admin";
        public static readonly string DocumentManagersRole = "Manager";
    }

}

