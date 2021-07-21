using System;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace IsObservableCollBuggy.xUnit.Tests.Extensions
{
    public static class ButtonExtensions
    {
        public static void PerformClick(this Button self)
        {
            // Check parameters
            if (self == null)
                throw new ArgumentNullException(nameof(self));

            self.RaiseEventViaReflection(nameof(self.Clicked), EventArgs.Empty);

            // 2.) Execute the command, if bound and can be executed
            ICommand boundCommand = self.Command;
            if (boundCommand != null)
            {
                object parameter = self.CommandParameter;
                if (boundCommand.CanExecute(parameter) == true)
                    boundCommand.Execute(parameter);
            }
        }
        private static void RaiseEventViaReflection<TEventArgs>(this object self, string eventName, TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            try
            {
                var type = self.GetType();
                typeof(Button).InvokeMember(eventName, BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, self, new object[] { EventArgs.Empty });
                var eventDelegate2 = self.GetType().GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic);
                var eventDelegate = (MulticastDelegate)eventDelegate2.GetValue(self);

                if (eventDelegate == null) return;

                foreach (var handler in eventDelegate.GetInvocationList())
                {
                    handler.GetMethodInfo()?.Invoke(handler.Target, new object[] { self, eventArgs });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
                
           
        }
    }
}