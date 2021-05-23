using System;
using Xamarin.Forms;

namespace WifiPage.Effects
{
    public class LifecycleEffect : RoutingEffect
    {
        public const string effectResolutionGroupName = "WifiPage.Effects";
        public static string LifeCycleEffectId => $"{effectResolutionGroupName}.{nameof(LifecycleEffect)}";
        public event EventHandler Loaded;
        public event EventHandler Unloaded;

        public LifecycleEffect() : base(LifeCycleEffectId)
        {
#if __ANDROID__
			if (System.DateTime.Now.Ticks < 0)
				_ = new Xamarin.CommunityToolkit.Android.Effects.LifeCycleEffectRouter();
#elif __IOS__
			if (System.DateTime.Now.Ticks < 0)
				_ = new Xamarin.CommunityToolkit.iOS.Effects.LifeCycleEffectRouter();
#elif UWP
			if (System.DateTime.Now.Ticks < 0)
				_ = new Xamarin.CommunityToolkit.UWP.Effects.LifeCycleEffectRouter();
#endif
        }

        public void RaiseLoadedEvent(Element element) => Loaded?.Invoke(element, EventArgs.Empty);
        public void RaiseUnloadedEvent(Element element) => Unloaded?.Invoke(element, EventArgs.Empty);
    }
}
