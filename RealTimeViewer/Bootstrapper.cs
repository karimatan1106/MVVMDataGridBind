using Microsoft.Practices.Unity;
using Prism.Unity;
using RealTimeViewer.Views;
using System.Windows;

namespace RealTimeViewer
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }
    }
}
