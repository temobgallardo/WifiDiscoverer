using IsObservableCollBuggy.Animations;
using Xamarin.Forms;

namespace IsObservableCollBuggy.Triggers
{
    [ContentProperty("Animation")]
    public class BeginAnimation : TriggerAction<VisualElement>
    {
        public BaseAnimation Animation { get; set; }

        protected override async void Invoke(VisualElement sender)
        {
            if (Animation != null)
                await Animation.Begin();
        }
    }
}
