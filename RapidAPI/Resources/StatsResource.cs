using RapidAPI.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RapidAPI.Resources
{
    public class StatsResource
    {
        //Static values to store highest/most data - stores the last data standing after validation
        public int mostCasesValue = 0;
        public string mostCasesCountry = null;
        public float highestPercentValue = 0.0f;
        public string highestPercentCountry = null;

        public List<string> GetListOfIsoCodes()
        {
            List<string> isoCodes = new List<string>();
            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            //Simplified LinQ statement to derive all ISO codes associated with the filtered cultures
            isoCodes.AddRange(from culture in cultures
                              where !culture.Equals(CultureInfo.InvariantCulture)
                              where !culture.IsNeutralCulture                       
                              let regionInfo = new RegionInfo(culture.Name)
                              select regionInfo.Name);   
            return isoCodes;   //Returns 500+ ISO country codes. 
        }

        public void FindMostCases(string condition, Model model)
        {
            //Compares most cases, highest percentage depending on the condition
            float totalCases = float.Parse(model.confirmed) + float.Parse(model.recovered) + float.Parse(model.critical) + float.Parse(model.deaths);
            switch (condition)
            {
                case "confirmed":
                    CompareCases(Int32.Parse(model.confirmed), model.country);
                    break;
                case "recovered":
                    CompareCases(Int32.Parse(model.recovered), model.country);
                    ComparePercentage(float.Parse(model.recovered), totalCases, model.country);
                    break;
                case "critical":
                    CompareCases(Int32.Parse(model.critical), model.country);
                    ComparePercentage(float.Parse(model.critical), totalCases, model.country);
                    break;
                case "deaths":
                    CompareCases(Int32.Parse(model.deaths), model.country);
                    ComparePercentage(float.Parse(model.deaths), totalCases, model.country);
                    break;
                case "defualt":
                    break;
            }
        }

        public void CompareCases(int compareValue, string countryName)
        {
            if (mostCasesValue < compareValue)
            {
                mostCasesValue = compareValue;
                mostCasesCountry = countryName;
            }
        }

        public void ComparePercentage(float compareValue, float totalValue, string country)
        {
            if (highestPercentValue < (float)(compareValue / totalValue) * 100)
            {
                highestPercentValue = (compareValue / totalValue) * 100;
                highestPercentCountry = country;
            }
        }

        public void ResetMaxValues()
        {
            //Avoids storing same data for each condition - useful if its handled in the same test case
            mostCasesValue = 0;
            mostCasesCountry = null;
            highestPercentValue = 0;
            highestPercentCountry = null;
        }
    }
}
