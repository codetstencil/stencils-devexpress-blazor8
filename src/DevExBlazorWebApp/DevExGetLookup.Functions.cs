using ZeraSystems.CodeNanite.Expansion;

namespace ZeraSystems.DevExBlazorWebApp
{
    public partial class DevExGetLookup : DevExpressBase
    {

        private void MainFunction()
        {
            var dxCode = GetCode();
            AppendText(dxCode);
        }

        string GetCode()
        {
            AppendText();
            AppendText("@code{");
            AppendText(GetLookups());
            AppendText("}");
            AppendText("");
            return ExpandedText.ToString();
        }
        string GetLookups()
        {
            var lookups = GetForeignKeysInTable(Input);
            foreach (var lookup in lookups)
            {
                AppendText("protected Task<LoadResult> Load" + lookup.TableName.Pluralize() + "(DataSourceLoadOptionsBase options, CancellationToken cancellationToken)");
                AppendText("{");
                AppendText("    return loader.GetLookupDataSource<int, " + lookup.TableName + "Model>(options, cancellationToken);");
                AppendText("}");
            }
            return ExpandedText.ToString();
        }
    }
}


