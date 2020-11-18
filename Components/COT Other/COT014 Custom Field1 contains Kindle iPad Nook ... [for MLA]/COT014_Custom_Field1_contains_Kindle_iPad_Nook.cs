//C6#COT014
//C5#431523
//Description: Custom Field 1 contains special data
//Version: 2.1
//V 2.1: works on both stand-alone references and child references; detects scope (i.e. "parent" or "child") of the component part it is attached to
//Purpose: output publication media type from custom field - some words must be in italics, if they denote a commercial product or company, see list below
//Should be customized for Citavi 6 new field "medium"  

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;
using SwissAcademic.Drawing;

namespace SwissAcademic.Citavi.Citations
{
	public class ComponentPartFilter
		:
		IComponentPartFilter
	{
		public IEnumerable<ITextUnit> GetTextUnits(ComponentPart componentPart, Template template, Citation citation, out bool handled)
		{

			handled = false;
			
			if (componentPart == null) return null;
			if (citation == null) return null;
			if (citation.Reference == null) return null;
			
			if (componentPart.Scope == ComponentPartScope.ParentReference && citation.Reference.ParentReference == null) return null;
			var referenceInScopeOfComponentPart = componentPart.Scope == ComponentPartScope.ParentReference ? citation.Reference.ParentReference : citation.Reference; 
			
			
			
			//check for the first custom field in the componentPart ... we expect this to be the field where the publication media type ist stored
			ReferencePropertyId[] customFieldProperties = new ReferencePropertyId[] 
			{ 
				ReferencePropertyId.CustomField1, 
				ReferencePropertyId.CustomField2,
				ReferencePropertyId.CustomField3,
				ReferencePropertyId.CustomField4,
				ReferencePropertyId.CustomField5,
				ReferencePropertyId.CustomField6,
				ReferencePropertyId.CustomField7,
				ReferencePropertyId.CustomField8,
				ReferencePropertyId.CustomField9
			};
			TextFieldElement customFieldElement = componentPart.GetFieldElements().OfType<TextFieldElement>().FirstOrDefault(fieldElement => customFieldProperties.Contains(fieldElement.PropertyId));
			if (customFieldElement == null) return null;
			
			//check if corresponding reference field contains data
			var mediaType = referenceInScopeOfComponentPart.GetValue(customFieldElement.PropertyId) as string;
			if (string.IsNullOrEmpty(mediaType)) return null;
			
			//the following words should be output in italics
			var specialMediaTypes = new string[] {
				"iPad", 
				"Kindle", 
				"Microsoft",
				"Word",
				"PowerPoint",		
				"Excel",
				"Nook",
				"Sony",
				"Adobe"
			};
			
			var regEx = new Regex(@"\b(" + string.Join("|", specialMediaTypes) + @")\b", RegexOptions.IgnoreCase);
			if (regEx.IsMatch(mediaType))
			{
				var words = mediaType.Split(' ');
				var newLiteralElements = new List<LiteralElement>();
				
			
				for(int i = 0; i < words.Count(); i++)
				{
					var word = words[i];
					if (string.IsNullOrWhiteSpace(word)) continue;
					
					LiteralElement newLiteralElement = null;
					if (i < words.Count() - 1) newLiteralElement = new LiteralElement(componentPart, word + " ");
					else newLiteralElement = new LiteralElement(componentPart, word);
					
					if (specialMediaTypes.Contains(word)) 
					{
						newLiteralElement.FontStyle = SwissAcademic.Drawing.FontStyle.Italic;
					}
					else
					{
						newLiteralElement.FontStyle = SwissAcademic.Drawing.FontStyle.Neutral;
					}
					
					newLiteralElements.Add(newLiteralElement);
				}
				
				
				var index = componentPart.Elements.IndexOf(customFieldElement);
				componentPart.Elements.RemoveAt(index);
				componentPart.Elements.InsertElements(index, newLiteralElements);
				foreach (var element in componentPart.Elements.OfType<LiteralElement>())
				{
					element.ApplyCondition = ElementApplyCondition.Always;
				}
				//componentPart.Elements.ReplaceItem(customFieldElement, newLiteralElements);
			}
			
			return null;


		}
	}
}