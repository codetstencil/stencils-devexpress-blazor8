using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ZeraSystems.CodeNanite.Expansion;
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
        "DevExpress Razor Form",
        "Generates code for a DevExpress Razor Form",
        "1.0",
        "DevExRazorForm",
        "ZeraSystems.DevExBlazorWebApp",
        "20-12-2023",
        "CS_DEVEX_RAZOR_FORM",
        "1",
        "",
        "https://www.codestencil.com"
    })]
    public partial class DevExRazorForm : ExpansionBase, ICodeStencilCodeNanite
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
