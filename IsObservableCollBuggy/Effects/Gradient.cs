using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IsObservableCollBuggy.Effects
{
    public class GradientRoutingEffect : RoutingEffect
    {
        public static string GradientRoutingEffectId => $"WifiPage.Effects.{nameof(GradientRoutingEffect)}";
        public GradientRoutingEffect() : base(GradientRoutingEffectId) { }
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

    public static class Gradient
    {
        public static BindableProperty IsEnableProperty = BindableProperty.Create(
            propertyName: "IsEnabled",
            returnType: typeof(bool),
            declaringType: typeof(Gradient),
            defaultValue: true,
            propertyChanged: TryGenerateEffect);

        public static bool GetIsEnable(BindableObject view) => (bool)view.GetValue(IsEnableProperty);

        public static void SetIsEnable(BindableObject view, bool value) => view.SetValue(IsEnableProperty, value);

        public static BindableProperty OrientationProperty = BindableProperty.Create(
            propertyName: "Orientation",
            returnType: typeof(GradientOrientation),
            declaringType: typeof(Gradient),
            defaultValue: GradientOrientation.LeftRight,
            propertyChanged: TryGenerateEffect);

        public static GradientOrientation GetOrientation(BindableObject view) => (GradientOrientation)view?.GetValue(OrientationProperty);

        public static void SetOrientation(BindableObject view, GradientOrientation value) => view?.SetValue(OrientationProperty, value);

        public static BindableProperty ColorsProperty = BindableProperty.Create(
            propertyName: "Colors",
            returnType: typeof(GradientColors),
            declaringType: typeof(Gradient),
            defaultValue: default,
            propertyChanged: TryGenerateEffect);

        public static GradientColors GetColors(BindableObject view) => (GradientColors)view?.GetValue(ColorsProperty);

        public static void SetColors(BindableObject view, GradientColors colors) => view?.SetValue(ColorsProperty, colors);

        public static BindableProperty CornerRadiusProperty = BindableProperty.Create(
            propertyName: "CornerRadius",
            returnType: typeof(float),
            declaringType: typeof(Gradient),
            defaultValue: 0f,
            propertyChanged: TryGenerateEffect);

        public static float GetCornerRadius(BindableObject view) => (float)view?.GetValue(CornerRadiusProperty);

        public static void SetCornerRadius(BindableObject view, float radius) => view?.SetValue(CornerRadiusProperty, radius);

        /// <summary>
        /// The color of the touch effect. If not set there won't be any animation.
        /// </summary>
        public static BindableProperty TouchColorProperty = BindableProperty.Create(
            propertyName: "TouchColor",
            returnType: typeof(Color),
            declaringType: typeof(Gradient),
            defaultValue: default(Color),
            propertyChanged: TryGenerateEffect);

        public static Color GetTouchColor(BindableObject view) => (Color)view?.GetValue(TouchColorProperty);

        public static void SetTouchColor(BindableObject view, Color color) => view?.SetValue(TouchColorProperty, color);

        public static void TryGenerateEffect(BindableObject view, object oldValue, object newValue)
        {
            System.Diagnostics.Debug.WriteLine($"Gradient - {nameof(TryGenerateEffect)}");

            if (!(view is VisualElement element)) return;

            var gradientsEffects = element.Effects.OfType<GradientRoutingEffect>();

            if (GetColors(element) is null)
            {
                foreach (var gc in gradientsEffects.ToArray())
                {
                    element.Effects.Remove(gc);
                }

                return;
            }

            if (!gradientsEffects.Any())
            {
                element.Effects.Add(new GradientRoutingEffect());
            }
        }
    }
}
