using System;

using TrackTracker.BLL.Model.Base;



namespace TrackTracker.BLL.Model
{
    public class PieChartableData : ModelObjectBase
    {
        private string name;
        public string Name
        {
            get => name;
            set { SetProperty(ref name, value); }
        }

        private double count;
        public double Count
        {
            get => count;
            set { SetProperty(ref count, value); }
        }
    }
}