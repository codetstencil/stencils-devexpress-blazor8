using System;
using System.Linq;
using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.DevExBlazorWebApp
{
    public abstract class DevExpressBase : ExpansionBase
    {
        public string DxGridDataColumn(ISchemaItem item)
        {
            var text = "<DxGridDataColumn " +
                       FieldName(item.TableName + "Model.", item.ColumnName) + Caption(item.ColumnLabel) + AllowSort() +
                       SearchEnabled() + Visible() +
                       " />";

            return text.Trim();

            #region Local Functions

            string FieldName(string model, string column) =>
                "FieldName=" + ("@nameof(" + model + column + ")").AddQuotes();

            string AllowSort() => !item.IsSortColumn ? " AllowSort = " + "false".AddQuotes() : "";
            string SearchEnabled() => !item.IsSearchColumn ? " SearchEnabled = " + "false".AddQuotes() : "";
            string Visible() => item.IsNotVisible ? " Visible = " + TrueOrFalse(!item.IsNotVisible) : "";

            string DisplayFormat()
            {
                var dataType = item.ColumnType;
                var format = string.Empty;
                switch (dataType)
                {
                    case "DateTime":
                        format = "d";
                        break;
                }

                return "DisplayFormat =" + format.AddQuotes();
                //https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGridDataColumn.DisplayFormat?v=22.1
            }

            //See: https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGridDataColumn._members

            #endregion Local Functions
        }

        public string DxFormLayoutItem(ISchemaItem item)
        {
            var colSpanMd = GetExpansionString("COLSPAN_MD");
            var span = 6;
            if (!colSpanMd.IsBlank())
                span = Convert.ToInt32(colSpanMd);
            var text = Caption(item.ColumnLabel) + ColSpanMd(span);
            if (!item.IsForeignKey)
                text = Indent(8) + "<DxFormLayoutItem" + text + BindColumn(item) + "</DxFormLayoutItem>";
            else
            {
                text = Indent(8) + "<DxFormLayoutItem" + text.AddCarriage() +
                       GetCombo().AddCarriage() +
                       Indent(8) + "</DxFormLayoutItem>";
            }

            return text;

            string GetCombo()
            {
                var relatedTable = GetRelatedTable(item);
                if (string.IsNullOrEmpty(relatedTable)) return "";
                relatedTable = relatedTable.Pluralize();

                var customData = "CustomData=" + ("@Load" + relatedTable.Pluralize()).AddQuotes();
                var tData = "TData=" + (GetRelatedTable(item) + "Model").AddQuotes();
                var tValue = "TValue=" + item.ColumnType.AddQuotes();

                var textFieldName = "TextFieldName=" + ("@nameof(" + GetRelatedTable(item) + "Model." + item.LookupDisplayColumn + ")").AddQuotes();
                var valueFieldName = "ValueFieldName=" + ("@nameof(" + GetRelatedTable(item) + "Model." + GetPrimaryKey(relatedTable) + ")").AddQuotes();
                var bindValue = "@bind-Value=" + ("@item." + item.ColumnName).AddQuotes();

                var combo =
                    Indent(12) + "<DxComboBox " + customData + " " + tData + " " + tValue + " ".AddCarriage() +
                    Indent(16) + textFieldName + " ".AddCarriage() +
                    Indent(16) + valueFieldName + " ".AddCarriage() +
                    Indent(16) + bindValue + ">".AddCarriage() +
                    Indent(12) + "</DxComboBox>";
                return combo;
            }
            //See: https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxFormLayoutItem
        }

        protected string GetRelatedTable(ISchemaItem item)
        {
            var tableExists = GetTables()
                .Select(x => x.TableName).ToList().Contains(item.RelatedTable);
            return tableExists ? item.RelatedTable : item.TableName;
            //we will assume it is a self-referencing table
        }

        private string Caption(string item) => " Caption=" + (item).AddQuotes();

        private string BindColumn(ISchemaItem item)
        {
            var column = ("item." + item.ColumnName).AddQuotes();
            var bindingAttribute = "@bind-Text";
            var result = $"<DxTextBox {bindingAttribute} = {column} />";
            switch (item.ColumnType)
            {
                case "DateTime":
                    bindingAttribute = "@bind-Date";
                    result = $"<DxDateEdit {bindingAttribute} = {column} />";
                    break;

                case "bool":
                    bindingAttribute = "@bind-Checked";
                    result = $"<DxCheckBox {bindingAttribute} = {column} />";
                    break;
            }
            return result;
        }

        private string ColSpanMd(int span) => " ColSpanMd=" + (span).ToString().AddQuotes() + ">";

        private string TrueOrFalse(bool value) => value ? "true".AddQuotes() : "false".AddQuotes();
    }
}