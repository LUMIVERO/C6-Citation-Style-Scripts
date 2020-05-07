//#C6_TNE001
//#C5_43211
//Version 1.1
//Check if field is empty or not

using System.Linq;
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
		#region EDIT Fields to check

		Dictionary<ReferencePropertyId, Ensure> FieldsToCheck = new Dictionary<ReferencePropertyId, Ensure>()
		{
			//														EDIT THIS COLUMN ONLY
			{ReferencePropertyId.Abstract, 							Ensure.Ignore},
			{ReferencePropertyId.AccessDate, 						Ensure.Ignore},
			{ReferencePropertyId.Authors, 							Ensure.Ignore},
			{ReferencePropertyId.AuthorsOrEditorsOrOrganizations, 	Ensure.Ignore},
			{ReferencePropertyId.BibTeXKey, 						Ensure.Ignore},
			{ReferencePropertyId.Categories,						Ensure.Ignore},
			{ReferencePropertyId.ChildReferences,					Ensure.Ignore},
			{ReferencePropertyId.CitationKey,						Ensure.Ignore},
			{ReferencePropertyId.Collaborators,						Ensure.Ignore},
			{ReferencePropertyId.CoverPath,							Ensure.Ignore},
			{ReferencePropertyId.CreatedBy,							Ensure.Ignore},
			{ReferencePropertyId.CreatedOn,							Ensure.Ignore},
			{ReferencePropertyId.CustomField1,						Ensure.IsNotEmpty},
			{ReferencePropertyId.CustomField2,						Ensure.IsEmpty},
			{ReferencePropertyId.CustomField3,						Ensure.Ignore},
			{ReferencePropertyId.CustomField4,						Ensure.Ignore},
			{ReferencePropertyId.CustomField5,						Ensure.Ignore},
			{ReferencePropertyId.CustomField6,						Ensure.Ignore},
			{ReferencePropertyId.CustomField7,						Ensure.Ignore},
			{ReferencePropertyId.CustomField8,						Ensure.Ignore},
			{ReferencePropertyId.CustomField9, 						Ensure.Ignore},
			{ReferencePropertyId.Date,								Ensure.Ignore},
			{ReferencePropertyId.Date2,								Ensure.Ignore},
			{ReferencePropertyId.Doi,								Ensure.Ignore},
			{ReferencePropertyId.Edition,							Ensure.Ignore},
			{ReferencePropertyId.EditionNumberResolved,				Ensure.Ignore},
			{ReferencePropertyId.Editors,							Ensure.Ignore},
			{ReferencePropertyId.Evaluation,						Ensure.Ignore},
			{ReferencePropertyId.HasLabel1,							Ensure.Ignore},
			{ReferencePropertyId.HasLabel2,							Ensure.Ignore},
			{ReferencePropertyId.Isbn,								Ensure.Ignore},
			{ReferencePropertyId.Keywords,							Ensure.Ignore},
			{ReferencePropertyId.Language,							Ensure.Ignore},
			{ReferencePropertyId.Locations,							Ensure.Ignore},
			{ReferencePropertyId.ModifiedBy,						Ensure.Ignore},
			{ReferencePropertyId.ModifiedOn,						Ensure.Ignore},
			{ReferencePropertyId.Notes,								Ensure.Ignore},
			{ReferencePropertyId.Number,							Ensure.Ignore},
			{ReferencePropertyId.NumberOfVolumes,					Ensure.Ignore},
			{ReferencePropertyId.OnlineAddress,						Ensure.Ignore},
			{ReferencePropertyId.Organizations,						Ensure.Ignore},
			{ReferencePropertyId.OriginalCheckedBy,					Ensure.Ignore},
			{ReferencePropertyId.OriginalPublication,				Ensure.Ignore},
			{ReferencePropertyId.OthersInvolved,					Ensure.Ignore},
			{ReferencePropertyId.PageCount,							Ensure.Ignore},
			{ReferencePropertyId.PageRange,							Ensure.Ignore},
			{ReferencePropertyId.ParallelTitle,						Ensure.Ignore},
			{ReferencePropertyId.ParentReference,					Ensure.Ignore},
			{ReferencePropertyId.Periodical,						Ensure.Ignore},
			{ReferencePropertyId.PlaceOfPublication,				Ensure.Ignore},
			{ReferencePropertyId.Price,								Ensure.Ignore},
			{ReferencePropertyId.Publishers,						Ensure.Ignore},
			{ReferencePropertyId.PubMedId,							Ensure.Ignore},
			{ReferencePropertyId.Quotations,						Ensure.Ignore},
			{ReferencePropertyId.Rating,							Ensure.Ignore},
			{ReferencePropertyId.SeriesTitle,						Ensure.Ignore},
			{ReferencePropertyId.SeriesTitleEditors,				Ensure.Ignore},
			{ReferencePropertyId.ShortTitle,						Ensure.Ignore},
			{ReferencePropertyId.SourceOfBibliographicInformation, 	Ensure.Ignore},
			{ReferencePropertyId.SpecificField1,					Ensure.Ignore},
			{ReferencePropertyId.SpecificField2,					Ensure.Ignore},
			{ReferencePropertyId.SpecificField3,					Ensure.Ignore},
			{ReferencePropertyId.SpecificField4,					Ensure.Ignore},
			{ReferencePropertyId.SpecificField5,					Ensure.Ignore},
			{ReferencePropertyId.SpecificField6,					Ensure.Ignore},
			{ReferencePropertyId.SpecificField7,					Ensure.Ignore},
			{ReferencePropertyId.StorageMedium,						Ensure.Ignore},
			{ReferencePropertyId.Subtitle,							Ensure.Ignore},
			{ReferencePropertyId.TableOfContents,					Ensure.Ignore},
			{ReferencePropertyId.Tasks,								Ensure.Ignore},
			{ReferencePropertyId.TextLinks,							Ensure.Ignore},
			{ReferencePropertyId.Title,								Ensure.Ignore},
			{ReferencePropertyId.TitleInOtherLanguages,				Ensure.Ignore},
			{ReferencePropertyId.TitleSupplement,					Ensure.Ignore},
			{ReferencePropertyId.TranslatedTitle,					Ensure.Ignore},
			{ReferencePropertyId.UniformTitle,						Ensure.Ignore},
			{ReferencePropertyId.Volume,							Ensure.Ignore},
			{ReferencePropertyId.Year, 								Ensure.Ignore}
		};


		#endregion EDIT Fields to check

		private enum Ensure
		{
			Ignore,
			IsEmpty,
			IsNotEmpty
		}

		public bool IsTemplateForReference(ConditionalTemplate template, Citation citation)
		{
			if (citation == null) return false;

			var reference = citation.Reference as Reference;
			if (reference == null) return false;

			IEnumerable<ReferencePropertyId> fieldsToCheck =
											(from kvp in FieldsToCheck
											 where kvp.Value == Ensure.IsEmpty || kvp.Value == Ensure.IsNotEmpty
											 select kvp.Key).ToList();

			if (!fieldsToCheck.Any()) return false;

			foreach (ReferencePropertyId propertyId in fieldsToCheck)
			{
				ReferencePropertyDescriptor property = ReferencePropertyDescriptor.GetPropertyDescriptor(propertyId);
				if (property == null) continue;

				if (!IsConditionMet(reference, property, FieldsToCheck[propertyId])) return false;
			}

			return true;
		}

		private bool IsConditionMet(Reference reference, ReferencePropertyDescriptor property, Ensure ensure)
		{
			if (ensure == Ensure.Ignore) return true;
			if (reference == null) return true;
			if (property == null) return true;

			#region String

			if (property.DataType == typeof(string))
			{
				string content = (string)reference.GetValue(property.PropertyId);
				if (string.IsNullOrEmpty(content)) return ensure == Ensure.IsEmpty;
				else return ensure == Ensure.IsNotEmpty;
			}

			#endregion String

			#region ReferencePersonCollection or NotificationCollection<Person>

			if (property.DataType == typeof(ReferencePersonCollection))
			{
				ReferencePersonCollection persons = (ReferencePersonCollection)reference.GetValue(property.PropertyId);
				if (persons == null || persons.Count == 0) return ensure == Ensure.IsEmpty;
				else return ensure == Ensure.IsNotEmpty;
			}

			if (property.DataType == typeof(NotificationCollection<Person>))
			{
				NotificationCollection<Person> persons = (NotificationCollection<Person>)reference.GetValue(property.PropertyId);
				if (persons == null || persons.Count == 0) return ensure == Ensure.IsEmpty;
				else return ensure == Ensure.IsNotEmpty;
			}

			#endregion ReferencePersonCollection or NotificationCollection<Person>

			#region ReferenceCategoryCollection

			if (property.DataType == typeof(ReferenceCategoryCollection))
			{
				if (reference.Categories == null || reference.Categories.Count == 0) return ensure == Ensure.IsEmpty;
				else return ensure == Ensure.IsNotEmpty;
			}

			#endregion ReferenceCategoryCollection

			#region ReferenceChildReferenceCollection

			if (property.DataType == typeof(ReferenceChildReferenceCollection))
			{
				if (reference.ChildReferences == null || reference.ChildReferences.Count == 0)
				{
					return ensure == Ensure.IsEmpty;
				}
				else
				{
					return ensure == Ensure.IsNotEmpty;
				}
			}

			#endregion ReferenceChildReferenceCollection

			#region bool

			if (property.DataType == typeof(bool))
			{
				bool? boolValue = (bool)reference.GetValue(property.PropertyId);
				if (!boolValue.GetValueOrDefault(false)) return ensure == Ensure.IsEmpty;
				else return ensure == Ensure.IsNotEmpty;
			}

			#endregion bool

			#region Isbn

			if (property.DataType == typeof(Isbn))
			{
				Isbn isbn = (Isbn)reference.GetValue(property.PropertyId);
				if (string.IsNullOrWhiteSpace(isbn.InputString)) return ensure == Ensure.IsEmpty;
				else return ensure == Ensure.IsNotEmpty;
			}

			#endregion Isbn

			#region ReferenceKeywordCollection

			if (property.DataType == typeof(ReferenceKeywordCollection))
			{
				if (reference.Keywords == null || reference.Keywords.Count == 0) return ensure == Ensure.IsEmpty;
				else return ensure == Ensure.IsNotEmpty;
			}

			#endregion ReferenceKeywordCollection

			#region ReferenceLocationCollection

			if (property.DataType == typeof(ReferenceLocationCollection))
			{
				if (reference.Locations == null || reference.Locations.Count == 0) return ensure == Ensure.IsEmpty;
				else return ensure == Ensure.IsNotEmpty;
			}

			#endregion ReferenceLocationCollection

			#region ParentReference

			if (property.DataType == typeof(Reference))
			{
				if (reference.ParentReference == null) return ensure == Ensure.IsEmpty;
				else return ensure == Ensure.IsNotEmpty;
			}

			#endregion ParentReference

			#region Periodical

			if (property.DataType == typeof(Periodical))
			{
				if (reference.Periodical == null) return ensure == Ensure.IsEmpty;
				else return ensure == Ensure.IsNotEmpty;
			}

			#endregion Periodical

			#region ReferencePublisherCollection

			if (property.DataType == typeof(ReferencePublisherCollection))
			{
				if (reference.Publishers == null || reference.Publishers.Count == 0) return ensure == Ensure.IsEmpty;
				else return ensure == Ensure.IsNotEmpty;
			}

			#endregion ReferencePublisherCollection

			#region RefernceQuotationCollection

			if (property.DataType == typeof(ReferenceQuotationCollection))
			{
				if (reference.Quotations == null || reference.Quotations.Count == 0) return ensure == Ensure.IsEmpty;
				else return ensure == Ensure.IsNotEmpty;
			}

			#endregion ReferenceQuotationCollection

			#region SeriesTitle

			if (property.DataType == typeof(SeriesTitle))
			{
				if (reference.SeriesTitle == null) return ensure == Ensure.IsEmpty;
				else return ensure == Ensure.IsNotEmpty;
			}

			#endregion SeriesTitle

			#region Editors

			if (property.DataType == SeriesTitlePropertyDescriptor.Editors.DataType)
			{
				if (reference.Editors == null || !reference.Editors.Any()) return ensure == Ensure.IsEmpty;
				else return ensure == Ensure.IsNotEmpty;
			}

			#endregion Editors

			#region ReferenceReferenceTaskCollection

			if (property.DataType == typeof(ReferenceReferenceTaskCollection))
			{
				if (reference.Tasks == null || reference.Tasks.Count == 0) return ensure == Ensure.IsEmpty;
				else return ensure == Ensure.IsNotEmpty;
			}

			#endregion Reference

			return true;
		}
	}
}
