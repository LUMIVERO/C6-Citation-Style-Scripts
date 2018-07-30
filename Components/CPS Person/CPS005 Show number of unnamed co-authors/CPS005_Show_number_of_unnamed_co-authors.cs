//C6#CPS005
//C5#43115
//Description: Output number of co-authors whose names were suppressed in the reference (e.g. after et al.)
//Example: Liu M, Gingery M, Doulatov SR, et al. (17 co-authors)
//Version: 1.0  

using System.Linq;
using System.Collections.Generic;
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

			
			//return handled = true if this macro generates the output (as a IEnumerable<ITextUnit>); the standard output will be suppressed
			//return handled = false if you want Citavi to produce the standard output; 
			//you can still manipulate the component part and its elements before letting Citavi generate the output with handled = false
			handled = false;

			if (citation == null) return null;
			if (citation.Reference == null) return null;
			if (componentPart == null) return null;

			var firstPersonFieldElement = componentPart.GetFieldElements().FirstOrDefault<FieldElement>(item => item is PersonFieldElement) as PersonFieldElement;
			if (firstPersonFieldElement == null) return null;
			if (!firstPersonFieldElement.Abbreviate) return null;

			var firstPersonFieldElementIndex = componentPart.Elements.IndexOf(firstPersonFieldElement);

			if (componentPart.Scope == ComponentPartScope.ParentReference && citation.Reference.ParentReference == null) return null;
			var citationInScope = GetCitationForComponentPartScope(componentPart, citation);

			var propertyId = firstPersonFieldElement.PropertyId;

			IList<IPerson> persons;

			switch (propertyId)
			{
				case ReferencePropertyId.Authors:
					persons = citationInScope.Reference.GetAuthors();
					break;

				case ReferencePropertyId.AuthorsOrEditorsOrOrganizations:
					persons = citationInScope.Reference.GetAuthorsOrEditorsOrOrganizations();
					break;

				case ReferencePropertyId.Editors:
					persons = citationInScope.Reference.GetEditors();
					break;

				case ReferencePropertyId.Collaborators:
					persons = citationInScope.Reference.GetCollaborators();
					break;
					
				case ReferencePropertyId.Organizations:
					persons = citationInScope.Reference.GetOrganizations();
					break;
					
				case ReferencePropertyId.SeriesTitleEditors:
					persons = citationInScope.Reference.GetSeriesTitleEditors();
					break;

				default:
					//??
					return null;

			}

			if (persons == null) return null;
			var personsCount = persons.Count;
			if (personsCount == 0) return null;

			var abbreviateUpTo = firstPersonFieldElement.AbbreviateUpToPerson;
			var abbreviateIfMoreThan = firstPersonFieldElement.AbbreviateIfMoreThanPersons;

			//Wenn länger als 	- AbbreviateIfMoreThanPersons
			//Abkürzen nach 	- AbbreviateUpToPerson

			if (personsCount > abbreviateIfMoreThan)
			{

				var suppressedPersonsCount = personsCount - abbreviateUpTo;
				if (suppressedPersonsCount <= 0) return null;

				var text = suppressedPersonsCount == 1 ? " (1 co-author)" : string.Format(" ({0} co-authors)", suppressedPersonsCount);

				var literalElement = new LiteralElement(componentPart, text);
				literalElement.FontStyle = FontStyle.Neutral;
				

				componentPart.Elements.Insert(firstPersonFieldElementIndex + 1, literalElement);

			}


			return null;
				
		}
		
		Citation GetCitationForComponentPartScope(ComponentPart componentPart, Citation citation)
		{
			Citation citationInScope;
			switch (componentPart.Scope)
			{
				#region ComponentPartScope.ParentReference

				case ComponentPartScope.ParentReference:
					{
						var parentReference = citation.Reference.ParentReference;

						if (parentReference == null) return null;					
						citationInScope = new PreviewCitation(parentReference, componentPart.CitationStyle, RuleSetType.Bibliography);
					}
					break;
					
				#endregion ComponentPartScope.ParentReference

				#region ComponentPartScope.Reference

				case ComponentPartScope.Reference:
				default:
					{
						citationInScope = citation;
					}
					break;

				#endregion ComponentPartScope.Reference

			}

			return citationInScope;
		}
	}
}