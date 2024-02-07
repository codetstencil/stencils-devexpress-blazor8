using System.Collections.Generic;
using System.ComponentModel.Composition;
using ZeraSystems.CodeStencil.Contracts;

namespace ZeraSystems.DevExBlazorWebApp
{
    /// <summary>
    /// There are 11 elements in the String Array used by the CodeStencil Nanite engine.
    ///  0 - Publisher : This is the name of the publisher
    ///  1 - Title : This is the title of the Code Nanite
    ///  2 - Details : This is the
    ///  3 - Version Number
    ///  4 - Label : Label of the Code Nanite
    ///  5 - Namespace
    ///  6 - Release Date
    ///  7 - Name to use for Expander Label
    ///  8 - Indicates that the Nanite is Schema Dependent
    ///  9 - RESERVED
    /// 10 - Url of Help for the Nanite
    /// </summary>
    [Export(typeof(ICodeStencilCodeNanite))]
    [CodeStencilCodeNanite(new[]
    {
        "ZERA Systems Inc.",
        "DevExpress Grid Columns",
        "Generates code for DevExpress Grid Columns",
        "1.2",
        "DevExGridColumns",
        "ZeraSystems.DevExBlazorWebApp",
        "05-02-2024",
        "CS_DEVEX_GRID_COLUMNS",
        "1",
        "",
        "https://help.codestencil.com/custom-stencils/devexpress-blazor-web-app"
    })]
    public partial class DevExGridColumns : DevExpressBase, ICodeStencilCodeNanite
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public int Counter { get; set; }
        public List<string> OutputList { get; set; }
        public List<ISchemaItem> SchemaItem { get; set; }
        public List<IExpander> Expander { get; set; }
        public List<string> InputList { get; set; }

        public void ExecutePlugin()
        {
            Initializer(SchemaItem, Expander);
            MainFunction();
            Output = ExpandedText.ToString();
        }

    }
}

