using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;

namespace K2Field.Helpers.Core
{
    public static class ExceptionHelper
    {
        /// <summary>
        /// http://vasters.com/clemensv/2012/09/06/Are+You+Catching+Falling+Knives.aspx
        /// We should not really be catching these fatal errors
        /// You can use this on any exception as an extension. And whenever you want to do a "catch all", you do this:
        ///try
        ///{
        ///    DoWork();
        ///}
        ///catch (Exception e)
        ///{
        ///    if (e.IsFatal())
        ///    {
        ///        throw;
        ///    }
        ///    Trace.TraceError(..., e);
        ///}
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static bool IsFatal(this Exception exception)
        {
            while (exception != null)
            {
                if (exception as OutOfMemoryException != null && exception as InsufficientMemoryException == null || exception as ThreadAbortException != null ||
                    exception as AccessViolationException != null || exception as SEHException != null || exception as StackOverflowException != null)
                {
                    return true;
                }
                else
                {
                    if (exception as TypeInitializationException == null && exception as TargetInvocationException == null)
                    {
                        break;
                    }
                    exception = exception.InnerException;
                }
            }
            return false;
        }
    }
}
