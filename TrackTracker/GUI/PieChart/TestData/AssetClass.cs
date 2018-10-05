using System;
using System.Collections.Generic;
using System.ComponentModel;



namespace WPFPieChart
{
    public class AssetClass : INotifyPropertyChanged
    {
        private string category;

        public string Category
        {
            get { return category; }
            set
            {
                category = value;
                RaisePropertyChangeEvent("Category");
            }
        }

        private uint count;

        public uint Count
        {
            get { return count; }
            set
            {
                count = value;
                RaisePropertyChangeEvent("Count");
            }
        }



        public static List<AssetClass> ConstructTestData()
        {
            List<AssetClass> assetClasses = new List<AssetClass>();

            assetClasses.Add(new AssetClass() { Category = "Rock", Count = 12 });
            assetClasses.Add(new AssetClass() { Category = "Pop", Count = 25 });
            assetClasses.Add(new AssetClass() { Category = "Rap", Count = 50 });

            return assetClasses;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangeEvent(String propertyName)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }

        #endregion
    }
}
