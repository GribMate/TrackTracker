using System;

using TrackTracker.BLL.Model.Base;



namespace TrackTracker.BLL.Model
{
    public class StatisticsChartableData : ModelObjectBase
    {
        private string category;
        public string Category
        {
            get => category;
            set { SetProperty(ref category, value); }
        }

        private int count;
        public int Count
        {
            get => count;
            set { SetProperty(ref count, value); }
        }
    }
}
