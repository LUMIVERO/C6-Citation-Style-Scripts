//TXY023
//Description:	Field "TitleSupplement" ("Type of thesis", "Art der Schrift") contains "Dissertation", "Doktor" or "Master"
//Version 1.0

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
			if (string.IsNullOrEmpty(citation.Reference.TitleSupplement)) return false;
			
			var field = citation.Reference.TitleSupplement;
						
			//note: if you do not specify the whole word, but rather its first characters, do NOT use * as a wildcard, but
			//\\w*, which means "zero or more word characters"
			var wordList = new string[] {
				"Dissertation",
				"Doktor\\w*",					//z.B. auch Wörter wie "Doktorarbeit" usw.
				"Doctoral\\w*",					//z.B. auch Wörter wie "Doctoral dissertations" usw.
				"Master\\w*"					//z.B. auch Wörter wie "Masterarbeit" oder "Master's dissertations" usw. 
			};
			var regEx = new Regex(@"\b(" + string.Join("|", wordList) + @")\b", RegexOptions.IgnoreCase);
			return regEx.IsMatch(field);

		}
	}
}
