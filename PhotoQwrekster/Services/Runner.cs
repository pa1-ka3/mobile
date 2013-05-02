using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Phone.Controls;

namespace PhotoTreqwister.Services
{
    public static class Runner
    {
        public static Dispatcher Dispatcher;
        public static void Invoke(Action action)
        {
            if (Dispatcher.CheckAccess())
                action.Invoke();
            else
                Dispatcher.BeginInvoke(action);
        }
    }
}
