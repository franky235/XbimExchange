﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.COBieLiteUK;
using Xbim.CobieLiteUK.Validation.Extensions;

namespace Xbim.CobieLiteUK.Validation.Reporting
{
    
    public class AssetTypeSummaryReport
    {
        private readonly IEnumerable<AssetType> _validatedAssets;

        public AssetTypeSummaryReport(IEnumerable<AssetType> vaidatedAssets)
        {
            _validatedAssets = vaidatedAssets;
        }

        public DataTable GetReport(string mainClassification = @"")
        {
            if (_validatedAssets == null || _validatedAssets.FirstOrDefault() == null)
                return null;
            if (mainClassification == @"")
            {
                var firstRequirement = _validatedAssets.FirstOrDefault();
                if (firstRequirement == null)
                    return null;

                var firstClassification = firstRequirement.Categories.FirstOrDefault();
                if (firstClassification == null)
                    return null;
                mainClassification = firstClassification.Classification;
            }

            var retTable = PrepareTable(mainClassification);

            // the progressive variable allows grouping by Maincategory and MatchingCategory values
            //
            var progressive = new Dictionary<Tuple<string, string>, ValidationSummary>();
            foreach (var reportingAsset in _validatedAssets)
            {
                var mainCatCode = "";
                var mainCat =
                    reportingAsset.Categories.FirstOrDefault(c => c.Classification == mainClassification);
                if (mainCat != null)
                {
                    mainCatCode = mainCat.Code;
                }

                var thisQuantities = new ValidationSummary()
                {
                    Passes = reportingAsset.GetValidAssetsCount(),
                    Total = reportingAsset.GetSubmittedAssetsCount(),
                    MainCatDescription = mainCat != null ? mainCat.Description : ""
                };
                    
                var matchingCat = reportingAsset.GetMatchingCategories().FirstOrDefault();
                // var sClass = (matchingCat != null) ? matchingCat.Classification : "";
                var matchCatValue = (matchingCat != null) ? matchingCat.Code : "";

                var thiItem = new Tuple<string, string>(mainCatCode, matchCatValue);

                if (!progressive.ContainsKey(thiItem))
                {
                    progressive.Add(thiItem, thisQuantities);
                }
                else
                {
                    progressive[thiItem].Add(thisQuantities);
                }
            }

            var sortedKeys = progressive.Keys.ToList();
            sortedKeys.Sort(Comparison);

            // now populates the datatable.
            foreach (var sortedKey in sortedKeys)
            {
                var value = progressive[sortedKey];
                int i = 1;
                var thisRow = retTable.NewRow();

                thisRow[i++] = sortedKey.Item1; // main classification
                thisRow[i++] = value.MainCatDescription; // main classification
                thisRow[i++] = sortedKey.Item2;
                thisRow[i++] = value.Total;
                thisRow[i++] = value.Passes;
                retTable.Rows.Add(thisRow);
            }
            return retTable;
        }

        private int Comparison(Tuple<string, string> tuple, Tuple<string, string> tuple1)
        {
            return tuple.Item1 == tuple1.Item1 
                ? System.String.Compare(tuple.Item2, tuple1.Item2, System.StringComparison.Ordinal) 
                : System.String.Compare(tuple.Item1, tuple1.Item1, System.StringComparison.Ordinal);
        }

        private class ValidationSummary
        {
            public int Total;
            public int Passes;
            public string MainCatDescription;

            public void Add(ValidationSummary addendum)
            {
                Total += addendum.Total;
                Passes += addendum.Passes;
            }
        }


        private static DataTable PrepareTable(string mainClassification)
        {
            var retTable = new DataTable("AssetTypes validation report", "DPoW");
            var workCol = retTable.Columns.Add("ID", typeof (Int32));
            workCol.AllowDBNull = false;
            workCol.Unique = true;
            workCol.AutoIncrement = true;

            retTable.Columns.Add(mainClassification, typeof (String));
            retTable.Columns.Add(mainClassification + " description", typeof (String));
            // retTable.Columns.Add("Matching classification", typeof (String));
            retTable.Columns.Add("Matching type", typeof (String));
            retTable.Columns.Add("No. Submitted", typeof (int));
            retTable.Columns.Add("No. Valid", typeof (int));
            return retTable;
        }
    }
}