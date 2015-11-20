using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clues
{
    /// <summary>
    /// This description is shown on the top of the specific applet help
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class |
                           System.AttributeTargets.Struct)
    ]
    public class AdditionalDescription : System.Attribute
    {
        public string Text { get; }

        public AdditionalDescription(string description)
        {
            Text = description;

        }
    }
}
