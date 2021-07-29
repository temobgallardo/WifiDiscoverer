﻿using Xamarin.Forms;

namespace IsObservableCollBuggy.Effects
{
    public static class EffectConfig
    {
        public static class EffectsConfig
        {
            public static bool AutoChildrenInputTransparent { get; set; } = true;

            public static readonly BindableProperty ChildrenInputTransparentProperty =
                BindableProperty.CreateAttached(
                    propertyName: "ChildrenInputTransparent",
                    returnType: typeof(bool),
                    declaringType: typeof(EffectsConfig),
                    defaultValue: false,
                    propertyChanged: (bindable, oldValue, newValue) => {
                        ConfigureChildrenInputTransparent(bindable);
                    }
                );

            public static void SetChildrenInputTransparent(BindableObject view, bool value)
            {
                view.SetValue(ChildrenInputTransparentProperty, value);
            }

            public static bool GetChildrenInputTransparent(BindableObject view)
            {
                return (bool)view.GetValue(ChildrenInputTransparentProperty);
            }

            static void ConfigureChildrenInputTransparent(BindableObject bindable)
            {
                if (!(bindable is Layout layout))
                    return;

                if (GetChildrenInputTransparent(bindable))
                {
                    foreach (var layoutChild in layout.Children)
                        AddInputTransparentToElement(layoutChild);
                    layout.ChildAdded += Layout_ChildAdded;
                }
                else
                {
                    layout.ChildAdded -= Layout_ChildAdded;
                }
            }

            static void Layout_ChildAdded(object sender, ElementEventArgs e)
            {
                AddInputTransparentToElement(e.Element);
            }

            static void AddInputTransparentToElement(BindableObject obj)
            {
                if (obj is VisualElement view && Touch.GetColor(view) == Color.Default)
                {
                    view.InputTransparent = true;
                }
            }
        }
    }
}
