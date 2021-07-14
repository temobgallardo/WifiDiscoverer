﻿
using IsObservableCollBuggy.Effects;
using IsObservableCollBuggy.Models;
using Models.Interfaces;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IsObservableCollBuggy.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WifiView : ContentView
    {
        private readonly WifiConnection _wifiConnection;
        public WifiView()
        {
            InitializeComponent();
            _wifiConnection = new WifiConnection(DependencyService.Get<IWifiConnectionReceiver>(), DependencyService.Get<IToastMessage>(), DependencyService.Get<IBrodcastSevice>());
            BindingContext = _wifiConnection;
        }

        protected void ViewLifecycleEffect_OnLoaded(object sender, EventArgs e)
        {
            _wifiConnection.OnAttached();
        }

        protected void ViewLifecycleEffect_OnUnloaded(object sender, EventArgs e)
        {
            _wifiConnection.OnDettached();
        }
    }
}