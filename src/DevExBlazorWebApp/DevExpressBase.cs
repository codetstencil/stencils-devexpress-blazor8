using System.Linq;
using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

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

            //text = text.AddTag("DxGridDataColumn", 0);
            return text.Trim();

            #region Local Functions

            string FieldName(string model, string column) =>
                "FieldName=" + ("@nameof(" + model + column + ")").AddQuotes();

            string AllowSort() =>
                item.IsSortColumn ? " AllowSort = " + TrueOrFalse(item.IsSortColumn) : "";

            string SearchEnabled() =>
                item.IsSearchColumn ? " SearchEnabled = " + TrueOrFalse(item.IsSearchColumn) : "";

            string Visible() =>
                item.IsNotVisible ? " Visible = " + TrueOrFalse(!item.IsNotVisible) : "";

            //string TrueOrFalse(bool value) => value ? "true".AddQuotes() : "false".AddQuotes();

            string CellDisplayTemplate()
            {
                if (item.IsForeignKey)
                {

                }

                return "";
            }

            //See: https://docs.devexpress.com/Blazor/DevExpress.Blazor.DxGridDataColumn._members
            #endregion

        }

        public string DxFormLayoutItem(ISchemaItem item)
        {
            var text = Caption(item.ColumnLabel) + ColSpanMd();

            //text = text.AddTag("DxFormLayoutItem", 4);
            if (!item.IsForeignKey)
                text = Indent(8) + "<DxFormLayoutItem" + text + "<DxTextBox" + BindColumn(item) + " />" + "</DxFormLayoutItem>";
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
            GetRelatedTable(item);
        }

        protected string GetRelatedTable(ISchemaItem item)
        {
            var tableExists = GetTables()
                .Select(x => x.TableName).ToList().Contains(item.RelatedTable);
            return tableExists ? item.RelatedTable : item.TableName;
            //we will assume it is a self-referencing table

        }

        /*
            <DxComboBox CustomData="@LoadEmployees" TData="EmployeeModel" TValue="int"
               TextFieldName="@nameof(EmployeeModel.FullName)"
               ValueFieldName="@nameof(EmployeeModel.EmployeeId)"
               @bind-Value="@item.SupportRepId">
           </DxComboBox>
        */



        private string Caption(string item) => " Caption=" + (item).AddQuotes();
        private string BindColumn(ISchemaItem item)
        {
            var column = ("item." + item.ColumnName).AddQuotes();
            if (item.ColumnType == "DateTime") 
                return " @bind-Date=" + column;
            return " @bind-Text=" + column;
        }

        private string ColSpanMd(int span = 6) => " ColSpanMd=" + (span).ToString().AddQuotes() + ">";
        private string TrueOrFalse(bool value) => value ? "true".AddQuotes() : "false".AddQuotes();
    }


}

