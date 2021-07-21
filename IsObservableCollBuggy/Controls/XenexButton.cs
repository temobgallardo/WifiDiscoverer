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

        /// <summary>
        /// Duration of the animation in ms
        /// </summary>
        public string Duration { get; set; } = "140";
        /// <summary>
        /// The element event which triggers the animation
        /// </summary>
        public string TriggeringEvent { get; set; } = "Clicked";
        /// <summary>
        /// The scale value of the animation
        /// </summary>
        public float ClickScale { get; set; } = 1.05f;
        
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
            _eventTrigger.Event = TriggeringEvent;
            Triggers.Add(_eventTrigger);
        }
    }
}
