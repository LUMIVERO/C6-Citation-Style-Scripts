//TXY005

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
			if (citation.Reference.ParentReference == null) return false;
			if (string.IsNullOrEmpty(citation.Reference.ParentReference.CustomField1)) return false;
			
			string test = citation.Reference.ParentReference.CustomField1;
						
			//note: if you do not specify the whole word, but rather its first characters, do NOT use * as a wildcard, but
			//\\w*, which means "zero or more word characters"
			var wordList = new string[] {
				"Lexikon\\w*",			//z.B. auch Wörter wie "Lexikonartikel" usw. 
				"Enzyklopädie\\w*",
				"Encyclopedia", 
				"encyclopédie" 
			};
			var regEx = new Regex(@"\b(" + string.Join("|", wordList) + @")\b", RegexOptions.IgnoreCase);
			return regEx.IsMatch(test);

		}
	}
}
