﻿using NPOI.SS.UserModel;
using Xbim.COBieLiteUK;

namespace Xbim.CobieLiteUK.Validation.Reporting
{
    internal class ExcelCellVisualValue
    {
        public ExcelCellVisualValue()
        { }

        public ExcelCellVisualValue(BorderStyle borderStyle)
        {
            BorderLeft = borderStyle;
            BorderRight = borderStyle;
            BorderTop = borderStyle;
            BorderBottom = borderStyle;
        }

        public BorderStyle? BorderLeft { get; set; }
        public BorderStyle? BorderRight { get; set; }
        public BorderStyle? BorderTop { get; set; }
        public BorderStyle? BorderBottom { get; set; }

        internal void SetCell(ICell excelCell, IVisualValue visualValue )
        {
            var attribute = visualValue.VisualValue;

            if (attribute is StringAttributeValue)
            {
                excelCell.SetCellType(CellType.String);
                excelCell.SetCellValue(((StringAttributeValue) (attribute)).Value);
            }
            else if (attribute is IntegerAttributeValue)
            {
                excelCell.SetCellType(CellType.Numeric);
                var v = ((IntegerAttributeValue) (attribute)).Value;
                if (v.HasValue)
                {
                    // ReSharper disable once RedundantCast
                    excelCell.SetCellValue((double) v.Value);
                }
            }
            else if (attribute is DecimalAttributeValue)
            {
                excelCell.SetCellType(CellType.Numeric);
                var v = ((DecimalAttributeValue) (attribute)).Value;
                if (v.HasValue)
                {
                    // ReSharper disable once RedundantCast
                    excelCell.SetCellValue((double) v.Value);
                }
            }
            else if (attribute is BooleanAttributeValue)
            {
                excelCell.SetCellType(CellType.Boolean);
                var v = ((BooleanAttributeValue) (attribute)).Value;
                if (v.HasValue)
                {
                    excelCell.SetCellValue(v.Value);
                }
            }
            else if (attribute is DateTimeAttributeValue)
            {
                excelCell.SetCellType(CellType.Numeric);
                var v = ((DateTimeAttributeValue) (attribute)).Value;
                if (v.HasValue)
                {
                    excelCell.SetCellValue(v.Value);
                }
            }

            if (visualValue.AttentionStyle == VisualAttentionStyle.None)
                return;

            var cellStyle = excelCell.Sheet.Workbook.CreateCellStyle();
            if (BorderLeft.HasValue)
                cellStyle.BorderLeft = BorderLeft.Value;
            if (BorderRight.HasValue)
                cellStyle.BorderRight = BorderRight.Value;
            if (BorderTop.HasValue)
                cellStyle.BorderTop = BorderTop.Value;
            if (BorderBottom.HasValue)
                cellStyle.BorderBottom = BorderBottom.Value;
            
            cellStyle.FillPattern = FillPattern.SolidForeground;
            switch (visualValue.AttentionStyle)
            {
                case VisualAttentionStyle.Amber:
                    cellStyle.FillForegroundColor = IndexedColors.Orange.Index;
                    break;
                case VisualAttentionStyle.Green:
                    cellStyle.FillForegroundColor = IndexedColors.Green.Index;
                    break;
                case VisualAttentionStyle.Red:
                    cellStyle.FillForegroundColor = IndexedColors.Red.Index;
                    break;
            }
            excelCell.CellStyle = cellStyle;
        }
    }
}