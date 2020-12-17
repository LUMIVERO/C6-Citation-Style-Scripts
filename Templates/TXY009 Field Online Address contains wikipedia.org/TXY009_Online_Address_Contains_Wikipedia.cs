//TXY009

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
			if (citation.Reference.ReferenceType != ReferenceType.InternetDocument) return false;
		
			var onlineAddress = citation.Reference.OnlineAddress;
			if (string.IsNullOrEmpty(onlineAddress)) return false;
		
			//note: if you do not specify the whole word, but rather its first characters, do NOT use * as a wildcard, but
			//\\w*, which means "zero or more word characters"
			var wordList = new string[] {
				"wikipedia.org"
			};
			var regEx = new Regex(@"\b(" + string.Join("|", wordList) + @")\b");
			return regEx.IsMatch(onlineAddress);

		}
	}
}
