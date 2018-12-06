using System;

using TrackTracker.BLL.Model.Base;



namespace TrackTracker.BLL.Model
{
    public class MetaTagDisplayable : ModelObjectBase
    {
        public string TagName { get; set; }

        public string CurrentValue { get; set; }

        public string NewValue { get; set; }
    }
}
