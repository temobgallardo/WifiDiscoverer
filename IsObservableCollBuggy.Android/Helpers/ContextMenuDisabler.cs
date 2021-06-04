using Android.Views;

namespace IsObservableCollBuggy.Droid.Helpers
{
    class ContextMenuDisabler : Java.Lang.Object, ActionMode.ICallback
    {
        public bool OnActionItemClicked(ActionMode mode, IMenuItem item) => false;
        public bool OnCreateActionMode(ActionMode mode, IMenu menu) => false;
        public void OnDestroyActionMode(ActionMode mode) { }
        public bool OnPrepareActionMode(ActionMode mode, IMenu menu) => false;
    }
}