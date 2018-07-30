//C6#COT019
//C5#431527
//Description: Different prefix or suffix depending on language of reference
//Version: 1.1
//Version 1.1 Recognize component part scope (parent or child)
//currently implemented for: bold & italics  

using System.Linq;
using System.Collections.Generic;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Citations;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;
using SwissAcademic.Drawing;
using System.Text.RegularExpressions;

namespace SwissAcademic.Citavi.Citations
{
	public class ComponentPartFilter
		:
		IComponentPartFilter
	{

        public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
		{

			handled = false;
			if (citation == null) return null;
			
			Reference reference = citation.Reference;
			if (reference == null) return null;
			
			if (componentPart == null) return null;
			if (componentPart.Elements == null || !componentPart.Elements.Any() || componentPart.Elements.Count != 1) return null;
			
			TextFieldElement firstTextFieldElement = componentPart.Elements.OfType<TextFieldElement>().FirstOrDefault() as TextFieldElement;
			if (firstTextFieldElement == null) return null;

           	ComponentPartScope scope = componentPart.Scope;
			Reference referenceInScope = null;
			if (scope == ComponentPartScope.Reference)
			{
				referenceInScope = reference;
			}
			else
			{
				referenceInScope = reference.ParentReference;
			}
			if (referenceInScope == null) return null;
			
			
			ReferencePropertyId propertyTagged = ReferencePropertyId.None;
			switch (firstTextFieldElement.PropertyId)
			{
				case ReferencePropertyId.Title:
				{
					propertyTagged = ReferencePropertyId.TitleTagged;
				}
				break;
				
				case ReferencePropertyId.Subtitle:
				{
					propertyTagged = ReferencePropertyId.SubtitleTagged;
				}
				break;
				
				case ReferencePropertyId.TitleSupplement:
				{
					propertyTagged = ReferencePropertyId.TitleSupplementTagged;
				}
				break;
			}
			if (propertyTagged == ReferencePropertyId.None) return null;	
			
			string stringTagged = referenceInScope.GetValue(propertyTagged) as string;
			
			if (string.IsNullOrEmpty(stringTagged)) return null;
			if (!HasTags(stringTagged)) return null;
			

			bool italicFound = false;
			
			TextUnitCollection textUnitCollection = TextUnitCollectionUtility.TaggedTextToTextUnits(firstTextFieldElement, stringTagged);		
			foreach(ITextUnit textUnit in textUnitCollection)
			{
				//Flip bits where required
				if (firstTextFieldElement.FontStyle.HasFlag(FontStyle.Italic)) textUnit.TemporaryFontStyle ^= FontStyle.Italic;
				if (firstTextFieldElement.FontStyle.HasFlag(FontStyle.Bold)) textUnit.TemporaryFontStyle ^= FontStyle.Bold;
			}		
			
			handled = true;
			return textUnitCollection;
		}
		
		static Regex TaggedTextRegex3 = new Regex("<\\s*?span\\s+?style\\s*?=\\s*?\"[\\w\\s-:;]+?\"\\s*?>|<\\s*?b\\s*?>|<\\s*?i\\s*?>|<\\s*?u\\s*?>|<\\s*?sub\\s*?>|<\\s*?sup\\s*?>", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public static bool HasTags(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            return TaggedTextRegex3.Match(text).Success;
        }
	}
}