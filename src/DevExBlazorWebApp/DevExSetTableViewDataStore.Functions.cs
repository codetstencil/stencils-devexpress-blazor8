using System;
using System.Collections.Generic;
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
            //_tableLower = _tableName.ToLower();
            //var tableObject = SchemaItem
            //    .FirstOrDefault(x=>x.TableName == _tableName && x.ColumnName == _tableName);
            //if (tableObject != null)
            //{
            //    _isTableNotView = tableObject.Schema == "TABLE";
            //}
            _isTableNotView = GetTableObject(Input).Schema == "TABLE";
            _columns = GetColumns(Input);
            //_columns = GetTheColumns(SchemaItem, _tableName);
            //_lookupColumns = _columns
            //    .Where(x => x.IsForeignKey && !string.IsNullOrEmpty(x.LookupDisplayColumn))
            //    .ToList();
            AppendText();
            AppendText(GenerateCode());
        }
        private string GenerateCode()
        {
            var getOverrides = GetOverrides();
            BuildSnippet();
            BuildSnippet("");
            BuildSnippet("public void DevExSetTableViewDataStore()", 8);
            BuildSnippet("{", 8);
            BuildSnippet(getOverrides, 12);
            BuildSnippet("}", 8);
            return BuildSnippet();
        }

        private string GetOverrides()
        {
            BuildSnippet();
            BuildSnippet("");

            if (_isTableNotView)
            {
                var pkType = GetPrimaryKeyType(_tableName);
                var primaryKey = GetPrimaryKey(_tableName);
                BuildSnippet("public override string KeyField =>nameof("+_tableName+"Model."+ primaryKey + ");");
                BuildSnippet("public override int ModelKey(" + _tableName + "Model model) ==> model." + primaryKey + ");");
                BuildSnippet("public override void SetModelKey(" + _tableName + "Model model, "+ pkType + " key) => model." + primaryKey + " = key;");
                BuildSnippet("protected override "+ pkType+ " DBModelKey("+_tableName+" model) => model.KeyField =>nameof(" + _tableName + "Model." + primaryKey + ";");
            }
            else if (!_isTableNotView)
            {
                BuildSnippet("// No overrides necessary for read-only operations. ");
                BuildSnippet("// Updateable Views not yet supported in this version. See: https://chatgpt.com/share/6767d1bc-f888-8008-a96f-f8ebfabe57a5 ");
            }
            return BuildSnippet();
        }

        public List<ISchemaItem> GetTheColumns(
            List<ISchemaItem> schemaItem,
            string table,
            bool onlyIsChecked = true,
            string excludeColumn = null,
            bool noCalculated = false)
        {
            if (onlyIsChecked)
                return schemaItem.Where<ISchemaItem>((Func<ISchemaItem, bool>)(e => !string.IsNullOrEmpty(e.ColumnType)))
                    .Where<ISchemaItem>((Func<ISchemaItem, bool>)(e => e.TableName == table && e.IsChecked == onlyIsChecked))
                    .Where<ISchemaItem>((Func<ISchemaItem, bool>)(e => e.ColumnName != excludeColumn))
                    .OrderBy<ISchemaItem, int>((Func<ISchemaItem, int>)(e => e.ColumnSequence))
                    .ToList<ISchemaItem>();
            return noCalculated ? schemaItem
                .Where<ISchemaItem>((Func<ISchemaItem, bool>)(e => !string.IsNullOrEmpty(e.ColumnType)))
                .Where<ISchemaItem>((Func<ISchemaItem, bool>)(e => e.TableName == table))
                .Where<ISchemaItem>((Func<ISchemaItem, bool>)(e => e.ColumnName != excludeColumn))
                .Where<ISchemaItem>((Func<ISchemaItem, bool>)(e => !e.IsCalculatedColumn))
                .OrderBy<ISchemaItem, int>((Func<ISchemaItem, int>)(e => e.ColumnSequence))
                .ToList<ISchemaItem>() 
                : schemaItem
                .Where<ISchemaItem>((Func<ISchemaItem, bool>)(e => !string.IsNullOrEmpty(e.ColumnType)))
                .Where<ISchemaItem>((Func<ISchemaItem, bool>)(e => e.TableName == table))
                .Where<ISchemaItem>((Func<ISchemaItem, bool>)(e => e.ColumnName != excludeColumn))
                .OrderBy<ISchemaItem, int>((Func<ISchemaItem, int>)(e => e.ColumnSequence))
                .ToList<ISchemaItem>();
        }


    }
}
/*
        public override string KeyField =>nameof([%CS_CURRENT_TABLE%]Model.[%CS_PRIMARY_KEY%]);
        public override int ModelKey([%CS_CURRENT_TABLE%]Model model) => model.[%CS_PRIMARY_KEY%];
        public override void SetModelKey([%CS_CURRENT_TABLE%]Model model, int key) => model.[%CS_PRIMARY_KEY%] = key;
        protected override int DBModelKey([%CS_CURRENT_TABLE%] model) => model.[%CS_PRIMARY_KEY%];
*/
