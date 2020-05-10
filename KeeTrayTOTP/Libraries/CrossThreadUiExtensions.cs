using System;
using System.ComponentModel;

namespace KeeTrayTOTP.Libraries
{
    public static class CrossThreadUiExtensions
    {
        public static void SynchronizedInvoke(this ISynchronizeInvoke sync, Action action)
        {
            if (!sync.InvokeRequired)
            {
                action();
            }
            else
            {
                sync.Invoke(action, new object[] { });
            }
        }

        public static T SynchronizedInvoke<T>(this ISynchronizeInvoke sync, Func<T> action)
        {
            if (!sync.InvokeRequired)
            {
                return action();
            }
            else
            {
                return (T)sync.Invoke(action, new object[] { });
            }
        }
    }
}
