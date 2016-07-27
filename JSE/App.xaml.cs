using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace JSE
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ExceptionWindow ew = new ExceptionWindow();
            ew.exception = e.Exception;
            ew.ShowDialog();
            e.Handled = true;
            Environment.Exit(0);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ExceptionWindow ew = new ExceptionWindow();
            ew.exception = (Exception)e.ExceptionObject;
            ew.ShowDialog();
            Environment.Exit(0);
        }
    }
}
