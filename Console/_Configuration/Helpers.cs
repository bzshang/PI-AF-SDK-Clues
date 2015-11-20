using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Clues
{
    static class Helpers
    {

        
        public static string GetAttributeValue(this Type type,Type attributeClassType, string attributeName)
        {

            if(type==null) return string.Empty;

            var attributeValue = "";
            var att = type.GetCustomAttributes(attributeClassType, true).FirstOrDefault();
            if (att != null)
            {

                PropertyInfo property = att.GetType().GetProperty(attributeName);
                attributeValue=property.GetValue(att).ToString();

            }

            return attributeValue;

        }
    }

}
