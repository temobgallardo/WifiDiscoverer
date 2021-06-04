using Android.App;
using Android.Content;
using Android.InputMethodServices;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Android.InputMethodServices.KeyboardView;
using _androidLogger = Android.Util.Log;
using Android.Text.Method;
using Android.Views.InputMethods;
using Color = Android.Graphics.Color;
using IsObservableCollBuggy.Controls;
using IsObservableCollBuggy.Droid.Helpers;
using IsObservableCollBuggy.Droid.Renderers;

[assembly: ExportRenderer(typeof(EntryWithCustomKeyboard), typeof(EntryWithCustomKeyboardRenderer))]
namespace IsObservableCollBuggy.Droid.Renderers
{
    public class EntryWithCustomKeyboardRenderer : EntryRenderer, IOnKeyboardActionListener
    {
        EntryWithCustomKeyboard entryWithCustomKeyboard;

        KeyboardView mKeyboardView;
        Android.InputMethodServices.Keyboard mKeyboard;

        InputTypes inputTypeToUse;

        bool keyPressed;
        bool _capsOn;
        bool _moreSymbols = true;
        bool _isAlphanumericKeyboard = true;

        public EntryWithCustomKeyboard EntryWithCustomKeyboard { get => entryWithCustomKeyboard; set => entryWithCustomKeyboard = value; }

        public EntryWithCustomKeyboardRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            var newCustomEntryKeyboard = e.NewElement as EntryWithCustomKeyboard;
            var oldCustomEntryKeyboard = e.OldElement as EntryWithCustomKeyboard;

            if (newCustomEntryKeyboard == null && oldCustomEntryKeyboard == null)
                return;

            if (e.NewElement != null)
            {
                EntryWithCustomKeyboard = newCustomEntryKeyboard;
                CreateCustomKeyboard();

                inputTypeToUse = EntryWithCustomKeyboard.Keyboard.ToInputType() | InputTypes.TextFlagNoSuggestions/* | InputTypes.TextFlagCapCharacters*/;

                EditText.FocusChange += Control_FocusChange;
                EditText.TextChanged += EditText_TextChanged;
                EditText.Click += EditText_Click;
                EditText.Touch += EditText_Touch;
                EditText.InputType = inputTypeToUse;
            }

            // Dispose control
            if (e.OldElement != null)
            {
                EditText.FocusChange -= Control_FocusChange;
                EditText.TextChanged -= EditText_TextChanged;
                EditText.Click -= EditText_Click;
                EditText.Touch -= EditText_Touch;
            }
        }

        protected override void OnFocusChangeRequested(object sender, VisualElement.FocusRequestArgs e)
        {
            e.Result = true;

            if (e.Focus)
                Control.RequestFocus();
            else
            {
                EditText.SetCompoundDrawablesRelativeWithIntrinsicBounds(0, 0, 0, 0);
                Control.ClearFocus();
            }
        }

        // Event handlers
        private void Control_FocusChange(object sender, FocusChangeEventArgs e)
        {
            // Workaround to avoid null reference exceptions in runtime
            if (this.EditText.Text == null)
                this.EditText.Text = string.Empty;

            if (e.HasFocus)
            {
                mKeyboardView.OnKeyboardActionListener = this;

                if (this.Element.Keyboard == Xamarin.Forms.Keyboard.Text)
                    this.CreateCustomKeyboard();
                if (!string.IsNullOrWhiteSpace(this.EditText.Text) && this.EditText.Text.Length > 0)
                {
                    ShowClearButton();
                }
                else
                {
                    HideClearButton();
                }

                this.ShowKeyboardWithAnimation();
            }
            else
            {
                // When the control looses focus, we set an empty listener to avoid crashes
                HideClearButton();
                this.mKeyboardView.OnKeyboardActionListener = new NullListener();
                this.HideKeyboardView();
            }
        }

        void ShowClearButton()
        {
            this.EditText.SetCompoundDrawablesRelativeWithIntrinsicBounds(0, 0, Resource.Drawable.ClearTextBtn, 0);
        }

        void HideClearButton()
        {
            this.EditText.SetCompoundDrawablesRelativeWithIntrinsicBounds(0, 0, 0, 0);
        }

        private void EditText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            // Ensure no key is pressed to clear focus
            if (this.EditText.Text.Length != 0 && !this.keyPressed)
            {
                ShowClearButton();
            }
            else
            {
                HideClearButton();
            }
            this.EditText.SetSelection(this.EditText.Text.Length);
        }

        private void EditText_Click(object sender, System.EventArgs e)
        {
            ShowKeyboardWithAnimation();
        }

        private void EditText_Touch(object sender, TouchEventArgs e)
        {
            this.EditText.InputType = InputTypes.Null;

            this.EditText.OnTouchEvent(e.Event);
            if (!string.IsNullOrWhiteSpace(this.EditText.Text) && this.EditText.Text.Length > 0)
                ShowClearButton();
            else
                HideClearButton();

            if (this.EditText.GetCompoundDrawables()[2] != null)
            {
                var loc = new int[2];
                this.EditText.GetLocationOnScreen(loc);
                if (e.Event.RawX >= ((loc[0] + this.EditText.Width) - this.EditText.GetCompoundDrawables()[2].Bounds.Width()) &&
                    e.Event.RawX <= ((loc[0] + this.EditText.Width) + this.EditText.GetCompoundDrawables()[2].Bounds.Width()))
                {
                    this.EditText.Text = "";
                    this.EditText.SetTextColor(Color.Black);
                    this.EditText.SetFocusable(ViewFocusability.Focusable);
                    this.EditText.SetSelection(this.EditText.Text.Length);
                }
            }
            this.EditText.InputType = this.inputTypeToUse;

            e.Handled = true;
        }

        void HideNativeKeyboard()
        {
            // Ensure native keyboard is hidden
            var imm = (InputMethodManager)Context.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(EditText.WindowToken, 0);
        }

        // Keyboard related section

        // Method to create our custom keyboard view
        private void CreateCustomKeyboard()
        {
            var activity = (Activity)Context;
            var rootView = activity.Window.DecorView.FindViewById(Android.Resource.Id.Content);
            var activityRootView = (ViewGroup)((ViewGroup)rootView).GetChildAt(0);

            this.mKeyboardView = activityRootView.FindViewById<KeyboardView>(Resource.Id.customKeyboard);
            EditText.CustomSelectionActionModeCallback = new ContextMenuDisabler();

            // If the previous line fails, it means the keyboard needs to be created and added
            if (this.mKeyboardView == null)
            {
                this.mKeyboardView = (KeyboardView)activity.LayoutInflater.Inflate(Resource.Layout.custom_keyboard, null);
                this.mKeyboardView.Id = Resource.Id.customKeyboard;
                this.mKeyboardView.Focusable = true;
                this.mKeyboardView.FocusableInTouchMode = true;

                // TODO: Comment and test
                this.mKeyboardView.Release += (sender, e) => { };

                var layoutParams = new Android.Widget.RelativeLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);
                layoutParams.AddRule(LayoutRules.AlignParentBottom);
                activityRootView.AddView(this.mKeyboardView, layoutParams);
            }

            this.HideKeyboardView();

            this.mKeyboardView.Keyboard = GetKeyboardById(Resource.Xml.alphanumeric_keyboard);
        }

        // Method to show our custom keyboard
        private void ShowKeyboardWithAnimation()
        {
            // First we must ensure that keyboard is hidden to
            // prevent showing it multiple times
            if (this.mKeyboardView.Visibility == ViewStates.Gone)
            {
                // Ensure native keyboard is hidden
                HideNativeKeyboard();

                this.EditText.InputType = InputTypes.Null;
                this.mKeyboardView.Enabled = true;

                // Show custom keyboard with animation
                this.mKeyboardView.Visibility = ViewStates.Visible;
            }
        }

        // Method to hide our custom keyboard
        private void HideKeyboardView()
        {
            mKeyboardView.Visibility = ViewStates.Gone;
            mKeyboardView.Enabled = false;
            EditText.InputType = InputTypes.Null;
        }

        // Implementing IOnKeyboardActionListener interface
        public void OnKey([GeneratedEnum] Android.Views.Keycode primaryCode, [GeneratedEnum] Android.Views.Keycode[] keyCodes)
        {
            var view = FindViewById<Android.Views.View>(
                Android.Resource.Id.Content);
            //view.PlaySoundEffect(SoundEffects.Click);

            if (!EditText.IsFocused)
                return;

            // Ensure key is pressed to avoid removing focus
            this.keyPressed = true;

            // Create event for key press
            long eventTime = JavaSystem.CurrentTimeMillis();

            var ev = new KeyEvent(eventTime, eventTime, KeyEventActions.Down | KeyEventActions.Multiple, primaryCode, 0, 0, 0, 0,
                                  KeyEventFlags.SoftKeyboard | KeyEventFlags.KeepTouchMode);

            // Ensure native keyboard is hidden
            HideNativeKeyboard();

            switch (ev.KeyCode)
            {
                case Android.Views.Keycode.Enter:
                    // Sometimes EditText takes long to update the HasFocus status
                    if (EditText.HasFocus)
                    {
                        // Close the keyboard, remove focus and launch command associated action
                        HideKeyboardView();

                        ClearFocus();

                        EntryWithCustomKeyboard.EnterCommand?.Execute(null);
                    }
                    break;
                case Android.Views.Keycode.Tab:
                    if (_isAlphanumericKeyboard)
                    {
                        SwitchCaps(ref _capsOn);
                    }
                    else
                    {
                        SwitchSymbolsKeyboard(ref _moreSymbols);
                    }
                    break;
                case Android.Views.Keycode.CtrlLeft:
                    if (_isAlphanumericKeyboard)
                    {
                        mKeyboardView.Keyboard = GetKeyboardById(Resource.Xml.symbols_keyboard);
                        _isAlphanumericKeyboard = false;
                        _moreSymbols = true;
                    }
                    else
                    {
                        ToCaps(_capsOn);
                        _isAlphanumericKeyboard = true;
                    }
                    break;
            }

            // Set the cursor at the end of the text
            this.EditText.SetSelection(this.EditText.Text.Length);

            if (EditText.HasFocus)
            {
                DispatchKeyEvent(ev);

                keyPressed = false;
            }
        }

        void SwitchCaps(ref bool capsOn)
        {
            if (capsOn)
            {
                mKeyboardView.Keyboard = GetKeyboardById(Resource.Xml.alphanumeric_keyboard);
                capsOn = false;
            }
            else
            {
                mKeyboardView.Keyboard = GetKeyboardById(Resource.Xml.alphanumeric_caps_keyboard);
                capsOn = true;
            }
        }

        void ToCaps(bool capsOn)
        {
            if (capsOn)
            {
                mKeyboardView.Keyboard = GetKeyboardById(Resource.Xml.alphanumeric_caps_keyboard);
            }
            else
            {
                mKeyboardView.Keyboard = GetKeyboardById(Resource.Xml.alphanumeric_keyboard);
            }
        }

        void SwitchSymbolsKeyboard(ref bool moreSymbols)
        {
            if (moreSymbols)
            {
                mKeyboardView.Keyboard = GetKeyboardById(Resource.Xml.more_symbols_keyboard);
                moreSymbols = false;
            }
            else
            {
                mKeyboardView.Keyboard = GetKeyboardById(Resource.Xml.symbols_keyboard);
                moreSymbols = true;
            }
        }

        Android.InputMethodServices.Keyboard GetKeyboardById(int id) => new Android.InputMethodServices.Keyboard(Context, id);

        public void OnPress([GeneratedEnum] Android.Views.Keycode primaryCode)
        {
        }

        public void OnRelease([GeneratedEnum] Android.Views.Keycode primaryCode)
        {
            //enteredPassword = (Element as EntryWithCustomKeyboard)?.PasswordEntered;
            if ((Element as EntryWithCustomKeyboard)?.ControlName == "PasswordEntry")
            {
                this.EditText.TransformationMethod = PasswordTransformationMethod.Instance;
            }
            this.EditText.SetText(this.EditText.Text?.ToUpper(), TextView.BufferType.Editable);

            this.mKeyboardView.Visibility = ViewStates.Visible;

            this.EditText.RequestFocus();
        }

        public void OnText(ICharSequence text)
        {
        }

        public void SwipeDown()
        {
        }

        public void SwipeLeft()
        {
        }

        public void SwipeRight()
        {
        }

        public void SwipeUp()
        {
        }

        private class NullListener : Java.Lang.Object, IOnKeyboardActionListener
        {
            public void OnKey([GeneratedEnum] Android.Views.Keycode primaryCode, [GeneratedEnum] Android.Views.Keycode[] keyCodes)
            {
            }

            public void OnPress([GeneratedEnum] Android.Views.Keycode primaryCode)
            {
            }

            public void OnRelease([GeneratedEnum] Android.Views.Keycode primaryCode)
            {
            }

            public void OnText(ICharSequence text)
            {
            }

            public void SwipeDown()
            {
            }

            public void SwipeLeft()
            {
            }

            public void SwipeRight()
            {
            }

            public void SwipeUp()
            {
            }
        }
    }
}