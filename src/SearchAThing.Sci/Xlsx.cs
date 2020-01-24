#region SearchAThing.Sci, Copyright(C) 2016-2017 Lorenzo Delana, License under MIT
/*
* The MIT License(MIT)
* Copyright(c) 2016-2017 Lorenzo Delana, https://searchathing.com
*
* Permission is hereby granted, free of charge, to any person obtaining a
* copy of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the
* Software is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
* DEALINGS IN THE SOFTWARE.
*/
#endregion

using System.Collections.Generic;
using System.Text;
using System.Linq;
using ClosedXML.Excel;
using System.Dynamic;
using System;

namespace SearchAThing
{

    public static partial class Extensions
    {

        /// <summary>
        /// Parse xlsx sheet data into a list of dynamic objects
        /// it will search on given sheetname ( it not null ) or on any sheets ( if null )
        /// it will list for given columnNames ( if not null ) or for all columns ( if null )
        /// if columnNamesIgnoreCase result object will contains lowercase properties
        /// </summary>        
        public static IEnumerable<ImportXlsxDataSheet> ParseXlsxData(this string xlsxPathfilename,
            string _sheetName = null,
            bool sheetNameIgnoreCase = true,
            HashSet<string> columnNames = null,
            bool columnNamesIgnoreCase = true,
            string[] valid_sheetnames = null)
        {
            var wb = new XLWorkbook(xlsxPathfilename);
            string sheetName = _sheetName;

            IXLWorksheet ws = null;

            if (sheetName == null)
            {
                foreach (var _ws in wb.Worksheets)
                {
                    if (valid_sheetnames != null && !valid_sheetnames.Any(r => r.ToLower() == _ws.Name.ToLower())) continue;

                    yield return new ImportXlsxDataSheet(_ws.Name, _ws.ParseXlsxData(columnNames, columnNamesIgnoreCase));
                }
            }
            else
            {
                if (sheetNameIgnoreCase)
                {
                    sheetName = _sheetName.ToLower();
                    ws = wb.Worksheets.FirstOrDefault(w => w.Name.ToLower() == sheetName);
                }
                else
                    ws = wb.Worksheets.FirstOrDefault(w => w.Name == sheetName);

                yield return new ImportXlsxDataSheet(sheetName, ws.ParseXlsxData(columnNames, columnNamesIgnoreCase));
            }
        }

        /// <summary>
        /// Parse xlsx sheet data into a list of dynamic objects
        /// it will list for given columnNames ( if not null ) or for all columns ( if null )
        /// if columnNamesIgnoreCase result object will contains lowercase properties
        /// </summary>        
        public static List<dynamic> ParseXlsxData(this IXLWorksheet ws, HashSet<string> _columnNames = null, bool columnNamesIgnoreCase = true)
        {
            HashSet<string> columnNames = null;

            if (columnNamesIgnoreCase && _columnNames != null)
                columnNames = _columnNames.Select(w => w.ToLower()).ToHashSet();
            else
                columnNames = _columnNames;

            var res = new List<dynamic>();

            var columnDict = new Dictionary<string, int>();

            var row = ws.FirstRow();

            var lastCol = row.LastCellUsed().Address.ColumnNumber;

            for (int ci = 1; ci <= lastCol; ++ci)
            {
                var cname = (string)row.Cell(ci).Value;
                if (string.IsNullOrEmpty((string)cname)) continue;

                if (columnNamesIgnoreCase) cname = cname.ToLower();

                if (columnNames == null || columnNames.Contains(cname))
                {
                    columnDict.Add(cname, ci);
                }
            }

            var lastRow = ws.LastRowUsed().RowNumber();

            for (int ri = 2; ri <= lastRow; ++ri)
            {
                row = ws.Row(ri);

                IDictionary<string, object> eo = new ExpandoObject();

                foreach (var c in columnDict)
                {
                    var cell = row.Cell(c.Value);

                    eo.Add(c.Key, cell.Value);
                }

                res.Add(eo);
            }

            return res;
        }


    }

    public class ImportXlsxDataSheet
    {

        public string SheetName { get; private set; }
        public IEnumerable<dynamic> Rows { get; private set; }

        public ImportXlsxDataSheet(string sheetName, IEnumerable<dynamic> rows)
        {
            SheetName = sheetName;
            Rows = rows;
        }

    }

    //-------------- xlsx fixed parser

    public enum XlsxColumnDataType
    {
        type_string,
        type_number
    }

    public class XlsxColumn
    {
        public XlsxColumn(string name, XlsxColumnDataType type)
        {
            Name = name;
            DataType = type;
        }
        public string Name { get; private set; }
        public XlsxColumnDataType DataType { get; private set; }
        public int ColumnIndex { get; internal set; } = -1;
        public override string ToString()
        {
            return $"{Name} [idx:{ColumnIndex}]";
        }
    }

    public class XlsxParseData
    {
        public XlsxParseData() { }
        public XlsxParseData(string val) { data_string = val; }
        public XlsxParseData(double val) { data_number = val; }
        public string data_string { get; private set; }
        public double? data_number { get; private set; }

        public override string ToString()
        {
            if (data_string != null) return data_string;
            else if (data_number.HasValue) return data_number.Value.ToString();
            else return "(null)";
        }
    }

    public class XlsxParser
    {
        public IEnumerable<XlsxColumn> Columns { get; private set; }

        IXLWorksheet ws;

        public int RowCount { get; private set; }

        /// <summary>
        /// parse fixed type workbook ; if worksheet_name specific it will locate the matching one
        /// </summary>        
        public XlsxParser(XLWorkbook wb, IEnumerable<XlsxColumn> required_columns,
            bool column_name_match_case = false,
            string worksheet_name = null, bool worksheet_name_matchcase = false)
        {
            Columns = required_columns;
            if (worksheet_name != null)
                ws = wb.Worksheets.First(w => string.Equals(w.Name, worksheet_name, worksheet_name_matchcase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase));
            else
                ws = wb.Worksheets.First();

            var header = ws.Row(1);
            foreach (var cell in header.CellsUsed())
            {
                if (cell.Value == null || !(cell.Value is string)) continue;
                var val = cell.Value as string;
                foreach (var r in required_columns.Where(t => t.ColumnIndex == -1))
                {
                    if (string.Equals(r.Name, val, column_name_match_case ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase))
                    {
                        r.ColumnIndex = cell.WorksheetColumn().ColumnNumber();
                        continue;
                    }
                }
            }
            var q = required_columns.FirstOrDefault(w => w.ColumnIndex == -1);
            if (q != null) throw new Exception($"can't find required column [{q.Name}]");

            RowCount = ws.RangeUsed().RowCount();
        }

        /// <summary>
        /// parse data, row must greather than 1 to avoid parse header itself
        /// </summary>        
        public IReadOnlyDictionary<XlsxColumn, XlsxParseData> Parse(int row)
        {
            var dict = new Dictionary<XlsxColumn, XlsxParseData>();

            foreach (var c in Columns)
            {
                var cell = ws.Cell(row, c.ColumnIndex);
                var val = cell.Value;
                if (val == null || (val is string && string.IsNullOrEmpty((string)val)))
                {
                    dict.Add(c, new XlsxParseData());
                    continue;
                }
                switch (c.DataType)
                {
                    case XlsxColumnDataType.type_number:
                        {
                            if (!(val is double))
                                throw new Exception($"wrong data type at row={row} col={c.ColumnIndex}. found [{val.GetType()}] instead of expected number");
                            dict.Add(c, new XlsxParseData((double)val));
                        }
                        break;

                    case XlsxColumnDataType.type_string:
                        {
                            if (!(val is string))
                                throw new Exception($"wrong data type at row={row} col={c.ColumnIndex}. found [{val.GetType()}] instead of expected string");
                            dict.Add(c, new XlsxParseData((string)val));
                        }
                        break;
                }
            }

            return dict;
        }

    }

    public static partial class Extensions
    {

        public static XlsxParser Parse(this XLWorkbook wb, IEnumerable<XlsxColumn> required_columns)
        {
            return new XlsxParser(wb, required_columns);
        }

    }

}