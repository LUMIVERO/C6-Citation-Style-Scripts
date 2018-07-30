//TXY007

using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;

namespace SwissAcademic.Citavi.Citations
{
   public class CustomTemplateCondition
      :
      ITemplateConditionMacro
   {
      public bool IsTemplateForReference(ConditionalTemplate template, Citation citation)
      {
         
         if (citation == null) return false;
         if (citation.Reference == null) return false;
         if (citation.Reference.ReferenceType != ReferenceType.CourtDecision) return false;
         
         var organizations = citation.Reference.Organizations;
         if (organizations == null || organizations.Count == 0) return false;
         
         //note: if you do not specify the whole word, but rather its first characters, do NOT use * as a wildcard, but
         //\\w*, which means "zero or more word characters"
         var wordList = new string[] {
            "BVerfG"
         };
         var regEx = new Regex(@"\b(" + string.Join("|", wordList) + @")\b");
         return organizations.Any(item => regEx.IsMatch(item.LastName) || regEx.IsMatch(item.Abbreviation));
   
      }
   }
}