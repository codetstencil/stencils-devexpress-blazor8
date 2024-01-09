using System;
using System.Linq;

namespace ZeraSystems.DevExBlazorWebApp
{
    public partial class DevExInjectRelated
    {
        private void MainFunction()
        {
            OutputList = SchemaItem
                .Where(x => (x.TableName == Input && 
                             x.IsForeignKey && 
                             !string.IsNullOrEmpty(x.RelatedTable)))
                .Select(x=>x.RelatedTable).Distinct().ToList();

        }
    }
}