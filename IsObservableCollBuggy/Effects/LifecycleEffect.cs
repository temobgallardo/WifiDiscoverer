using System;
using Xamarin.Forms;

namespace IsObservableCollBuggy.Effects
{
    public class LifecycleEffect : RoutingEffect
    {
        public const string effectResolutionGroupName = "WifiPage.Effects";
        public static string LifeCycleEffectId => $"{effectResolutionGroupName}.{nameof(LifecycleEffect)}";
        public event EventHandler Loaded;
        public event EventHandler Unloaded;

        public LifecycleEffect() : base(LifeCycleEffectId)
        {
            // Don't remove, for linking porposes
#if __ANDROID__
			if (System.DateTime.Now.Ticks < 0)
				_ = new LifecycleEffect();
#elif __IOS__
			if (System.DateTime.Now.Ticks < 0)
				_ = new LifecycleEffect();
#elif UWP
			if (System.DateTime.Now.Ticks < 0)
				_ = new LifecycleEffect();
#endif
        }

        public void RaiseLoadedEvent(Element element) => Loaded?.Invoke(element, EventArgs.Empty);
        public void RaiseUnloadedEvent(Element element) => Unloaded?.Invoke(element, EventArgs.Empty);
    }
}
