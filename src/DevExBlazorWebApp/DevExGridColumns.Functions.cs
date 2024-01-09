using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using ZeraSystems.CodeNanite.Expansion;

namespace ZeraSystems.DevExBlazorWebApp
{
    public partial class DevExGridColumns : DevExpressBase
    {
        private string _model;

        private void MainFunction()
        {
            _model = Input + "Model";
            AppendText();

            var dxGrid = GetDxGridString();
            var dxFormLayout = GetDxEditFormLayoutString();
            var dxCode = GetCode();

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
            var columns = GetColumns(Input);
            if (!columns.Any()) return "";

            AppendText(Indent(4) + "<GridColumns>");
            foreach (var column in columns)
            {
                AppendText(Indent(8) + DxGridDataColumn(column));
            }
            AppendText(Indent(4) + "</GridColumns>");
            return ReturnResult(ExpandedText.ToString());
        }

        private string GetDxEditFormLayoutString()
        {
            var columns = GetColumns(Input)
                .Where(x => x.IsCalculatedColumn is false).ToList();

            if (!columns.Any()) return "";

            var text = Indent(4) + "@{".AddCarriage() +
                       Indent(12) + "var item = (" + _model + ")ctx.EditModel;".AddCarriage() +
                       Indent(8) + "}";

            AppendText(Indent(4) + "<EditFormLayoutItems Context=" + "ctx".AddQuotes() + ">");
            AppendText(Indent(4) + text);

            foreach (var column in columns)
            {
                AppendText(DxFormLayoutItem(column));
            }
            AppendText(Indent(4) + "</EditFormLayoutItems>");
            return ReturnResult();
        }

        private string GetCode()
        {
            var lookups = GetLookups();
            AppendText("@code{");
            AppendText(lookups);
            AppendText("}");
            return ReturnResult();
        }

        private string GetLookups()
        {
            var lookups = GetForeignKeysInTable(Input).ToList();
            var createdLookups = new List<string>();
            foreach (var lookup in lookups)
            {
                if (createdLookups.Contains(lookup.RelatedTable)) continue;
                var table = GetRelatedTable(lookup); //  lookup.RelatedTable;
                AppendText(Indent(4) + "protected Task<LoadResult> Load" + table.Pluralize() + "(DataSourceLoadOptionsBase options, CancellationToken cancellationToken)");
                AppendText(Indent(4) + "{");
                AppendText(Indent(8) + "return loader.GetLookupDataSource<int, " + table + "Model" + ">(options, cancellationToken);");
                AppendText(Indent(4) + "}");
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
}