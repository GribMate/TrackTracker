using System;
using System.Collections.Generic;
using System.ComponentModel;



namespace PieChart
{
    public class TESTCLASS : INotifyPropertyChanged
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



        public static List<TESTCLASS> ConstructTestData()
        {
            List<TESTCLASS> assetClasses = new List<TESTCLASS>();

            assetClasses.Add(new TESTCLASS() { Category = "Rock", Count = 12 });
            assetClasses.Add(new TESTCLASS() { Category = "Pop", Count = 25 });
            assetClasses.Add(new TESTCLASS() { Category = "Rap", Count = 50 });

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
