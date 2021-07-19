using IsObservableCollBuggy.Animations;
using IsObservableCollBuggy.Triggers;
using Xamarin.Forms;

namespace IsObservableCollBuggy.Controls
{
    public class XenexButton : Button
    {
        private readonly ClickAnimation _clickAnimation = new ClickAnimation();
        private readonly BeginAnimation _beginAnimation = new BeginAnimation();
        private readonly EventTrigger _eventTrigger = new EventTrigger();

        public string Duration { get; set; }
        public string Event { get; set; }
        public float ClickScale{ get; set; }
        
        public XenexButton()
        {
            _clickAnimation.Target = this;
            _beginAnimation.Animation = _clickAnimation;
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();

            _clickAnimation.Duration = Duration;
            _clickAnimation.Scale = ClickScale;

            _eventTrigger.Actions.Add(_beginAnimation);
            _eventTrigger.Event = Event;
            Triggers.Add(_eventTrigger);
        }
    }
}
