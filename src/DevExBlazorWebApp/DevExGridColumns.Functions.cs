using System;
using System.Collections.Generic;
using System.Linq;
using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.DevExBlazorWebApp
{
    public partial class DevExGridColumns
    {
        private string _model;
        private List<ISchemaItem> _lookups;
        private List<ISchemaItem> _editColumns;
        private List<ISchemaItem> _gridColumns;
        private List<string> _tablesList;

        private int _colSpanMd = 12;
        private void MainFunction()
        {
            _model = Input + "Model";
            _lookups = GetForeignKeysInTable(Input).Distinct(new SchemaItemRelatedTableComparer()).ToList();
            _gridColumns = GetColumns(Input);
            _editColumns = GetColumns(Input)
                .Where(x => x.IsCalculatedColumn is false).ToList();
            _tablesList = GetTables().Select(x => x.TableName).ToList();
            var colSpanMd = GetExpansionString("COLSPAN_MD");
            if (!colSpanMd.IsBlank())
                _colSpanMd = Convert.ToInt32(colSpanMd);

            AppendText();

            var dxGrid = GetDxGridString();
            //var dxFormLayout = GetDxEditFormLayoutString();
            var dxFormLayout = GetEditFormLayoutItem();
            var dxCode = GetCodeBehind();

            AppendText();
            AppendText("<BrowseAndEditCtrl TKey=" + "int".AddQuotes() + " TModel=" + _model.AddQuotes() + ">");
            AppendText(dxGrid);
            AppendText(dxFormLayout);
            AppendText("</BrowseAndEditCtrl>");
            AppendText("");
            AppendText(dxCode);
        }

        private string GetDxGridString()
        {

            if (!_gridColumns.Any()) return "";
            var gridString = Indent(4) + "<GridColumns>".AddCarriage();
            foreach (var column in _gridColumns)
            {
                gridString += DxGridDataColumn(column).AddCarriage();
            }
            gridString += Indent(4) + "</GridColumns>".AddCarriage();
            //return ReturnResult(gridString);
            return gridString;
        }


        public string DxGridDataColumn(ISchemaItem item)
        {
            var text = "<DxGridDataColumn " +
                       FieldName(item.TableName + "Model.", item.ColumnName) + Caption(item.ColumnLabel) + AllowSort() +
                       SearchEnabled() + Visible();
            if (!item.IsForeignKey)
            {
                //text += " />";
                //return text.Trim();
                return Indent(8) + text + " />";
            }

            return EditSettingsString();


            #region Local Functions
            //string Caption(string label) => " Caption=" + (label).AddQuotes();

            string FieldName(string model, string column) =>
                "FieldName=" + ("@nameof(" + model + column + ")").AddQuotes();

            string AllowSort() => !item.IsSortColumn ? " AllowSort = " + "false".AddQuotes() : "";
            string SearchEnabled() => !item.IsSearchColumn ? " SearchEnabled = " + "false".AddQuotes() : "";
            string Visible() => item.IsNotVisible ? " Visible = " + TrueOrFalse(!item.IsNotVisible) : "";

            //string DisplayFormat()
            //{
            //    var dataType = item.ColumnType;
            //    var format = string.Empty;
            //    switch (dataType)
            //    {
            //        case "DateTime":
            //            format = "d";
            //            break;
            //    }

            //    return "DisplayFormat =" + format.AddQuotes();
            //    //https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGridDataColumn.DisplayFormat?v=22.1
            //}


            string EditSettingsString()
            {
                AppendText();
                AppendText(Indent(8) + "<DxGridDataColumn " +
                            FieldName(item.TableName + "Model.", item.ColumnName) + Caption(item.ColumnLabel) + AllowSort() +
                            SearchEnabled() + Visible() + ">");
                AppendText(Indent(12) + "<EditSettings>");
                AppendText(Indent(16) + "<DxComboBoxSettings Data=" + ("@" + (item.RelatedTable.Pluralize() + ".data")).AddQuotes());
                AppendText(Indent(36) + "ValueFieldName=" + item.ColumnName.AddQuotes());
                AppendText(Indent(36) + "TextFieldName=" + item.LookupDisplayColumn.AddQuotes());
                AppendText(Indent(36) + "FilteringMode=" + "DataGridFilteringMode.Contains".AddQuotes());
                AppendText(Indent(36) + "ClearButtonDisplayMode=" + "DataEditorClearButtonDisplayMode.Auto".AddQuotes() + " />");
                AppendText(Indent(12) + "</EditSettings>");
                AppendText(Indent(8) + "</DxGridDataColumn>");
                return ReturnResult();
            }

            //See: https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGridDataColumn._members

            #endregion Local Functions


        }

        private string GetEditFormLayoutItem()
        {
            AppendText(Indent(4) + "<EditFormLayoutItems Context=" + "ctx".AddQuotes() + ">");
            AppendText(Indent(8) + "@{");
            AppendText(Indent(12) + "var item = (" + _model + ")ctx.EditModel;");
            AppendText(Indent(8) + "}");
            AppendText("");

            foreach (var column in _editColumns)
            {
                AppendText(Indent(8) + "<DxFormLayoutItem Caption=" + column.ColumnLabel.AddQuotes() + " ColSpanMd=" + "12".AddQuotes() + ">");
                AppendText(Indent(12) + "@ctx.GetEditor(nameof(item." + column.ColumnName + "))");
                AppendText(Indent(8) + "</DxFormLayoutItem>");
            }

            AppendText(Indent(4) + "</EditFormLayoutItems>");
            return ReturnResult();

        }


        private string GetCodeBehind()
        {
            var loaderMethods = CreateLoaderMethods();
            AppendText("@code{");
            foreach (var lookup in _lookups)
            {
                AppendText(Indent(4) + "public LoadResult " + lookup.RelatedTable.Pluralize() + " { get; set; } = new();");
            }
            AppendText("");
            AppendText(Indent(4) + "readonly DataSourceLoadOptionsBase _options = new();");
            AppendText(Indent(4) + "readonly CancellationToken _cancellationToken = new();");
            AppendText(Indent(4) + "protected override async Task OnInitializedAsync() => await LoadData();");
            AppendText("");
            AppendText(Indent(4) + "async Task LoadData()");
            AppendText(Indent(4) + "{");
            foreach (var lookup in _lookups)
            {
                //AppendText(Indent(8) + lookup.RelatedTable.Pluralize() + " = await Load" + lookup.RelatedTable.Pluralize() + "(_options, _cancellationToken);");
                AppendText(Indent(8) + lookup.RelatedTable.Pluralize() + AwaitMethod(lookup) + "(_options, _cancellationToken);");
            }
            AppendText(Indent(4) + "}");
            AppendText("");
            AppendText(loaderMethods);
            AppendText("}");
            return ReturnResult();

            string AwaitMethod(ISchemaItem lookupRow)
            {
                if (_tablesList.Contains(lookupRow.RelatedTable) || _tablesList.Contains(lookupRow.RelatedTable.Pluralize()))
                    return " = await Load" + lookupRow.RelatedTable.Pluralize();
                else
                    return " = await Load" + lookupRow.TableName.Pluralize();
            }
        }

        private string CreateLoaderMethods()
        {
            //var lookups = GetForeignKeysInTable(Input).ToList();
            var createdLookups = new List<string>();
            foreach (var lookup in _lookups)
            {
                if (createdLookups.Contains(lookup.RelatedTable)) continue;
                var table = GetRelatedTable(lookup); //  lookup.RelatedTable;
                AppendText("");
                AppendText(Indent(4) + "protected Task<LoadResult> Load" + table.Pluralize() + "(DataSourceLoadOptionsBase options, CancellationToken cancellationToken) =>");
                AppendText(Indent(8) + "Loader.GetLookupDataSource<int, " + table + "Model" + ">(options, cancellationToken);");
                createdLookups.Add(lookup.RelatedTable);
            }
            return ReturnResult();
        }

        private string ReturnResult(string text = null)
        {
            var result = ExpandedText.ToString();
            if (text != null)
                result = text.Trim();

            ExpandedText.Clear();
            return result;
        }
    }

    public class SchemaItemRelatedTableComparer : IEqualityComparer<ISchemaItem>
    {
        public bool Equals(ISchemaItem x, ISchemaItem y)
        {
            // Check whether the compared objects reference the same data.
            if (ReferenceEquals(x, y)) return true;

            // Check whether any of the compared objects is null.
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            // Check whether the ISchemaItem's RelatedTable properties are equal.
            return x.RelatedTable == y.RelatedTable;
        }

        public int GetHashCode(ISchemaItem obj)
        {
            // Check whether the object is null.
            if (ReferenceEquals(obj, null)) return 0;

            // Get hash code for the RelatedTable field if it is not null.
            return obj.RelatedTable == null ? 0 : obj.RelatedTable.GetHashCode();
        }
    }

}