using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IsObservableCollBuggy.Effects
{
    public class GradientBackgroundColorEffect : RoutingEffect
    {
        public GradientColors GradientColors { get; set; }
        public static string GradientBackgroundColorEffectId => $"{LifecycleEffect.effectResolutionGroupName}.{nameof(GradientBackgroundColorEffect)}";
        public GradientBackgroundColorEffect() : base(GradientBackgroundColorEffectId) { }
    }

    [TypeConverter(typeof(ColorsTypeConverter))]
    public class GradientColors : List<Color>
    {
        public GradientColors() : base() { }
        public GradientColors(IEnumerable<Color> colors) : base(colors) { }
    }

    /// <summary>
    /// ColorsTypeConverter
    /// </summary>
    [TypeConversion(typeof(GradientColors))]
    public class ColorsTypeConverter : TypeConverter
    {
        public override object ConvertFromInvariantString(string value)
        {
            if (value != null)
            {
                var colors = value.Split(',');
                var conv = new ColorTypeConverter();

                return new GradientColors(colors.Select(x => (Color)conv.ConvertFromInvariantString(x)));
            }

            throw new InvalidOperationException($"Cannot convert \"{value}\" into {typeof(GradientColors)}");
        }
    }

    [TypeConverter(typeof(GradientOrientationTypeConverter))]
    public enum GradientOrientation
    {
        /// <summary>
        /// Left to Right
        /// </summary>
        LeftRight = 0,
        /// <summary>
        /// Bottom Left to Top Right
        /// </summary>
        BlTr = 45,
        /// <summary>
        /// Bottom to Top
        /// </summary>
        BottomTop = 90,
        /// <summary>
        /// Bottom Right to Top Left
        /// </summary>
        BrTl = 135,
        /// <summary>
        /// Right to Left
        /// </summary>
        RightLeft = 180,
        /// <summary>
        /// Top Right to Bottom Left
        /// </summary>
        TrBl = 225,
        /// <summary>
        /// Top to Bottom
        /// </summary>
        TopBottom = 270,
        /// <summary>
        /// Top Left to Bottom Right
        /// </summary>
        TlBr = 315,
    }

    /// <summary>
    /// GradientOrientationTypeConverter
    /// </summary>
    [TypeConversion(typeof(GradientOrientation))]
    public class GradientOrientationTypeConverter : TypeConverter
    {
        public override object ConvertFromInvariantString(string value)
        {
            if (value != null)
            {
                if (Enum.TryParse(value, true, out GradientOrientation orientation))
                    return orientation;
            }
            throw new InvalidOperationException($"Cannot convert \"{value}\" into {typeof(GradientOrientation)}");
        }
    }

    public static class GradientEffect
    {
        public static BindableProperty IsEnableProperty = BindableProperty.Create("IsEnabled", typeof(bool), typeof(GradientEffect), true, propertyChanged: OnIsEnableChanged);

        static void OnIsEnableChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is View view))
            {
                return;
            }

            bool isActivated = (bool)newValue;
            if (isActivated)
            {
                view.Effects.Add(new GradientBackgroundColorEffect());
                return;
            }

            var toRemove = view.Effects.FirstOrDefault(e => e is GradientBackgroundColorEffect);
            if (toRemove != null)
            {
                view.Effects.Remove(toRemove);
            }
        }

        public static bool GetIsEnable(BindableObject view)
        {
            return (bool)view.GetValue(IsEnableProperty);
        }

        public static void SetIsEnable(BindableObject view, bool value)
        {
            view.SetValue(IsEnableProperty, value);
        }
    }
}
