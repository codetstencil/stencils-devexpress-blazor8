using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using ZeraSystems.CodeNanite.Expansion;
using ZeraSystems.CodeStencil.Contracts;
namespace ZeraSystems.DevExBlazorWebApp
{
    public partial class DevExSetTableViewDataStore : ExpansionBase
    {
        List<ISchemaItem> _columns;
        private string _tableName;
        //private string _tableLower;
        //private List<ISchemaItem> _lookupColumns;
        private bool _isTableNotView;
        private void MainFunction()
        {
            _tableName = Input;
            _isTableNotView = GetTableObject(Input).Schema == "TABLE";
            _columns = GetColumns(Input);
            AppendText();
            AppendText(GenerateCode());
        }
        private string GenerateCode()
        {
            var getOverrides = GetOverrides();
            BuildSnippet();
            BuildSnippet("");
            //BuildSnippet("public void DevExSetTableViewDataStore()", 8);
            //BuildSnippet("{", 8);
            BuildSnippet(getOverrides, 12);
            //BuildSnippet("}", 8);
            return BuildSnippet();
        }

        private string GetOverrides()
        {
            BuildSnippet();
            BuildSnippet("");
            var pkType = string.Empty;
            var primaryKey = string.Empty;
            var getHashCode = string.Empty;
            var keyToString = string.Empty;
            if (_isTableNotView) //TABLE
            {
                pkType = GetPrimaryKeyType(_tableName);
                primaryKey = GetPrimaryKey(_tableName);
            }
            else if (!_isTableNotView)  //VIEW
            {
                var row = _columns.FirstOrDefault(x => (x.TableName == _tableName && x.ParentId !=0));
                if (row != null)
                {
                    pkType = row.ColumnType;
                    primaryKey = row.ColumnName;
                    if (pkType is "string")
                    {
                        getHashCode = ".GetHashCode()";
                        keyToString = ".ToString()";
                    }
                }

                //BuildSnippet("// No overrides necessary for read-only operations. ");
                //BuildSnippet("// Updateable Views not yet supported in this version. See: https://chatgpt.com/share/6767d1bc-f888-8008-a96f-f8ebfabe57a5 ");
            }
            BuildSnippet("public override string KeyField =>nameof(" + _tableName + "Model." + primaryKey + ");");
            BuildSnippet("public override int ModelKey(" + _tableName + "Model model) => model." + primaryKey + getHashCode + ";");
            BuildSnippet("public override void SetModelKey(" + _tableName + "Model model, int key) => model." + primaryKey + " = key"+ keyToString + ";");
            BuildSnippet("protected override int DBModelKey(" + _tableName + " model) => model." + primaryKey + getHashCode + ";");



            //if (_isTableNotView)
            //{
            //    pkType = GetPrimaryKeyType(_tableName);
            //    primaryKey = GetPrimaryKey(_tableName);
            //    BuildSnippet("public override string KeyField =>nameof("+_tableName+"Model."+ primaryKey + ");");
            //    BuildSnippet("public override int ModelKey(" + _tableName + "Model model) => model." + primaryKey + ";");
            //    BuildSnippet("public override void SetModelKey(" + _tableName + "Model model, "+ pkType + " key) => model." + primaryKey + " = key;");
            //    BuildSnippet("protected override "+ pkType+ " DBModelKey("+_tableName+" model) => model." + primaryKey + ";");
            //}
            //else if (!_isTableNotView)
            //{
            //    BuildSnippet("// No overrides necessary for read-only operations. ");
            //    BuildSnippet("// Updateable Views not yet supported in this version. See: https://chatgpt.com/share/6767d1bc-f888-8008-a96f-f8ebfabe57a5 ");
            //}

            return BuildSnippet();
        }

    }
}

