using System.Linq;
using Xamarin.Forms;
using static IsObservableCollBuggy.Effects.EffectConfig;

namespace IsObservableCollBuggy.Effects
{
    public class TouchRoutingEffect : RoutingEffect
    {
        public TouchRoutingEffect() : base($"WifiPage.Effects.{nameof(TouchRoutingEffect)}")
        {
        }
    }

    public static class Touch
    {
        public static readonly BindableProperty ColorProperty = BindableProperty.CreateAttached(
               propertyName: "Color",
               returnType: typeof(Color),
               declaringType: typeof(TouchRoutingEffect),
               defaultValue: Color.Default,
               propertyChanged: TryGenerateEffect
           );

        public static void SetColor(BindableObject view, Color value)
        {
            view.SetValue(ColorProperty, value);
        }

        public static Color GetColor(BindableObject view)
        {
            return (Color)view.GetValue(ColorProperty);
        }

        static void TryGenerateEffect(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is VisualElement element))
                return;

            var eff = element.Effects.FirstOrDefault(e => e is TouchRoutingEffect);
            if (GetColor(bindable) != Color.Default || GetColor(bindable) != null)
            {
                element.InputTransparent = false;

                if (eff != null) return;

                element.Effects.Add(new TouchRoutingEffect());

                if (EffectsConfig.AutoChildrenInputTransparent && bindable is Layout &&
                    !EffectsConfig.GetChildrenInputTransparent(element))
                {
                    EffectsConfig.SetChildrenInputTransparent(element, true);
                }
            }
            else
            {
                if (eff == null || element.BindingContext == null) return;

                element.Effects.Remove(eff);

                if (EffectsConfig.AutoChildrenInputTransparent && bindable is Layout &&
                    EffectsConfig.GetChildrenInputTransparent(element))
                {
                    EffectsConfig.SetChildrenInputTransparent(element, false);
                }
            }
        }
    }
}
