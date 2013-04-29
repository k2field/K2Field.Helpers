using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;

namespace K2Field.Helpers.Core.Code
{
    /// <summary>
    /// Delegate for raised/lowered methods
    /// </summary>
    public delegate void CodeToRunElevated();
    public delegate void WaitCallback(object codeDelegate);

    public static class WindowsPrincipalHelper
    {
        public static WindowsImpersonationContext RevertToServiceAccount()
        {
            //revert to the user context of the service account for this part
            return WindowsIdentity.Impersonate(IntPtr.Zero);
        }

        public static void RevertToImpersonatedUser(WindowsImpersonationContext _serviceAccountContext)
        {
            if (_serviceAccountContext != null)
            {
                _serviceAccountContext.Undo();
            }
        }

        public static void RunWithServiceAccount(CodeToRunElevated secureCode)
        {
            RunWithServiceAccountPrivileges(new WaitCallback(CodeToRunElevatedWrapper), secureCode);
        }

        internal static void RunWithServiceAccountPrivileges(WaitCallback secureCode, object param)
        {
            var _serviceAccountContext = RevertToServiceAccount();
            try
            {
                secureCode(param);
            }
            finally
            {
                RevertToImpersonatedUser(_serviceAccountContext);
            }
        }

        private static void CodeToRunElevatedWrapper(object codeDelegate)
        {
            // Runs the codeDelegate (which is the elevated code) so that both the wrapper
            // and the codeDelegate will be executed after the usercontext has changed. 
            ((CodeToRunElevated)codeDelegate)();
        }



    }
}
