using System;
using System.Collections.Generic;
using System.Linq;
using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.DevExBlazorWebApp
{
    public abstract class DevExpressBase : ExpansionBase
    {
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

        public string Caption(string item) => " Caption=" + (item).AddQuotes();

        private string BindColumn(ISchemaItem item)
        {
            var attributes = GetAttributes(item);
            var column = ("item." + item.ColumnName).AddQuotes();
            var bindingAttribute = "@bind-Text";

            string result;
            switch (item.ColumnType)
            {
                case "DateTime":
                    bindingAttribute = "@bind-Date";
                    result = $"<DxDateEdit {bindingAttribute} = {column} " + GetDateTimeMask() + "/>";
                    break;
                case "bool":
                    bindingAttribute = "@bind-Checked";
                    result = $"<DxCheckBox {bindingAttribute} = {column} />";
                    break;
                case "int":
                    bindingAttribute = "@bind-Value";
                    result = $"<DxSpinEdit {bindingAttribute} = {column} />";
                    break;
                case "decimal":
                    bindingAttribute = "@bind-Value";
                    result = $"<DxSpinEdit {bindingAttribute} = {column} " + GetDecimalMask() + "/>";
                    break;
                default:
                    result = $"<DxTextBox {bindingAttribute} = {column} />";
                    break;
            }

            return GetDefaultMask(result);

            string GetDecimalMask()
            {
                if (attributes.Contains("Currency"))
                    return " Mask = " + "@NumericMask.Currency".AddQuotes();
                return " Mask = " + "#,##0.00".AddQuotes();
            }
            string GetDateTimeMask()
            {
                return " Mask = " + "@DateTimeMask.ShortDate".AddQuotes();
            }
            string GetDefaultMask(string mask)
            {
                if (!item.MaskPattern.IsBlank())
                    return " Mask = " + item.MaskPattern.AddQuotes();
                return mask;
            }

        }

        private string ColSpanMd(int span) => " ColSpanMd=" + (span).ToString().AddQuotes() + ">";
        //private string ColSpanMd()
        //{
        //    var colSpanMd = GetExpansionString("COLSPAN_MD");
        //    if (!colSpanMd.IsBlank())
        //        _colSpanMd = Convert.ToInt32(colSpanMd);


        //    return " ColSpanMd=" + (_colSpanMd).ToString().AddQuotes() + ">";
        //}

        public string TrueOrFalse(bool value) => value ? "true".AddQuotes() : "false".AddQuotes();
        List<string> GetAttributes(ISchemaItem item)
        {
            var input = item.ColumnAttribute;

            // Split the string into lines
            var lines = input.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            for (var i = 0; i < lines.Count; i++)
            {
                lines[i] = lines[i]
                    .Replace("[DataType(DataType.", "")
                    .Replace(")]", "");
            }

            return lines;
        }
    }
}