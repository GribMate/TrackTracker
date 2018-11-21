using System;



namespace TrackTracker.BLL.Model
{
    /*
     * A base class, that represents a selectable model entity.
     * It can signal the view whether its selectable and can be bound to be selected.
    */
    public class SelectableObject : ModelObjectBase
    {
        public SelectableObject(bool selectable = true)
        {
            IsSelectable = selectable;
            IsSelected = false;
        }

        private bool isSelectable;
        public virtual bool IsSelectable
        {
            get => isSelectable;
            protected set { SetProperty(ref isSelectable, value); }
        }

        private bool isSelected;
        public virtual bool IsSelected
        {
            get => isSelected;
            set { SetProperty(ref isSelected, value); }
        }
    }
}