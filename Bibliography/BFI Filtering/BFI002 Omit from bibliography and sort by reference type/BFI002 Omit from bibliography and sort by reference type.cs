//#C5_BFI002
//#C5_43334
//Description: 	Omit from bibliography and sort by reference type
//Version 1.2:	Sorts also Volume and Number
//Version 1.1:	Allows for excluding a certain reference type from the bibliography > Set section number to -1 under "EDIT Sections for each Reference Type"

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Citations;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Collections;

namespace SwissAcademic.Citavi.Comparers
{
	public class CustomCitationComparer
		:
		ICustomCitationComparerMacro
	{


		#region EDIT Sections for each Reference Type

		bool MakeSectionZeroFirstSection = true;

		Dictionary<Tuple<ReferenceType, ReferenceType>, int> FullReferenceTypeSections = new Dictionary<Tuple<ReferenceType, ReferenceType>, int>()
		{
			//0 means "Other" or "Default"; these references will be sorted to the end of the list or - if MakeSectionZeroFirstSection - at the beginning
			//for other sections use numbers 1, 2, 3 etc. and the references will be grouped and ordered in an ascending fashion
			//if you want to exclude full reference type from the bibliography, give it section -1

			{FullReferenceType.ArchiveMaterial, 0},							//Archivgut
			{FullReferenceType.ArchiveMaterialInBookEdited, 0},				//Archivgut im Sammelwerk

			{FullReferenceType.AudioBook, 0},								//Hörbuch
			{FullReferenceType.AudioOrVideoDocument, 0},					//Ton- oder Filmdokument
			{FullReferenceType.Book, 0},									//Monographie
			{FullReferenceType.BookEdited, 0},								//Sammelwerk
			{FullReferenceType.Broadcast, 0},								//Radio- oder Fernsehsendung [Radio or TV Broadcast]
			{FullReferenceType.CollectedWorks, 0},							//Schriften eines Autors
			{FullReferenceType.ComputerProgram, 0},							//Software
			{FullReferenceType.ConferenceProceedings, 0},					//Tagungsband

			{FullReferenceType.ContributionInBookEdited, 0},				//Beitrag im Sammelwerk
			{FullReferenceType.ContributionInCollectedWorks, 0},			//Beitrag in Schriften eines Autors
			{FullReferenceType.ContributionInConferenceProceedings, 0},		//Beitrag im Tagungsband
			{FullReferenceType.ContributionInSpecialIssue, 0},				//Beitrag in Sonderheft
			{FullReferenceType.ContributionInUnpublishedWork, 0},			//Beitrag in Grauer Literatur / Bericht / Report

			{FullReferenceType.ContributionInLegalCommentary, 0},			//Beitrag im Gesetzeskommentar - wird bei juristischen Stilen vom Literaturverzeichnis ausgeschlossen
			{FullReferenceType.CourtDecision, 0},							//Gerichtsentscheidung - wird bei juristischen Stilen vom Literaturverzeichnis ausgeschlossen
			{FullReferenceType.File, 0},									//Akte
			{FullReferenceType.InternetDocument, 0},						//Internetdokument
			{FullReferenceType.InterviewMaterial, 0},						//Interviewmaterial
			{FullReferenceType.JournalArticle, 0},							//Zeitschriftenaufsatz
			{FullReferenceType.Lecture, 0},									//Vortrag [Presentation]
			{FullReferenceType.LegalCommentary, 0},							//Gesetzeskommentar - wird bei juristischen Stilen vom Literaturverzeichnis ausgeschlossen
			{FullReferenceType.Manuscript, 0},								//Manuskript
			{FullReferenceType.Map, 0},										//Geographische Karte
			{FullReferenceType.Movie, 0},									//Spielfilm
			{FullReferenceType.MusicAlbum, 0},								//Musikwerk / Musikalbum

			{FullReferenceType.MusicTrack, 0},								//Musiktitel
			{FullReferenceType.MusicTrackInMusicAlbum, 0},					//Musiktitel im Album
			{FullReferenceType.NewsAgencyReport, 0},						//Agenturmeldung
			{FullReferenceType.NewspaperArticle, 0},						//Zeitungsartikel
			{FullReferenceType.Patent, 0},									//Patentschrift
			{FullReferenceType.PersonalCommunication, 0},					//Persönliche Mitteilung
			{FullReferenceType.PressRelease, 0},							//Pressemitteilung
			{FullReferenceType.RadioPlay, 0},								//Hörspiel
			{FullReferenceType.SpecialIssue, 0},							//Sonderheft, Beiheft
			{FullReferenceType.Standard, 0},								//Norm

			{FullReferenceType.StatuteOrRegulation, 0},						//Gesetz / Verordnung in Zeitschrift - wird bei juristischen Stilen vom Literaturverzeichnis ausgeschlossen
			{FullReferenceType.StatuteOrRegulationInBookEdited, 0},			//Gesetz / Verordnung im Sammelwerk - wird bei juristischen Stilen vom Literaturverzeichnis ausgeschlossen

			{FullReferenceType.Thesis, 0},									//Hochschulschrift
			{FullReferenceType.Unknown, 0},									//Unklarer Dokumententyp
			{FullReferenceType.UnpublishedWork, 0}							//Graue Literatur / Bericht / Report
		};

		#endregion EDIT Sections for each Reference Type

		public int Compare(Citation x, Citation y)
		{

			//First we make sure we are comparing BibliographyCitations only
			var xBibliographyCitation = x as BibliographyCitation;
			var yBibliographyCitation = y as BibliographyCitation;

			if (xBibliographyCitation == null || yBibliographyCitation == null) return 0;
			var xReference = xBibliographyCitation.Reference;
			var yReference = yBibliographyCitation.Reference;
			if (xReference == null || yReference == null) return 0;

			//1st Comparer
			var defaultCitationComparer = CitationComparer.AuthorYearTitleOrNoAuthorThenTitleYearAscending;


			//2nd Comparer, if first yields 0
			var yearTitleVolumeSortDescriptors = new List<PropertySortDescriptor<Reference>>();
			yearTitleVolumeSortDescriptors.Add(new PropertySortDescriptor<Reference>(ReferencePropertyDescriptor.YearResolved));
			yearTitleVolumeSortDescriptors.Add(new PropertySortDescriptor<Reference>(ReferencePropertyDescriptor.Title));
			yearTitleVolumeSortDescriptors.Add(new PropertySortDescriptor<Reference>(ReferencePropertyDescriptor.Volume));
			yearTitleVolumeSortDescriptors.Add(new PropertySortDescriptor<Reference>(ReferencePropertyDescriptor.Number));
			var yearTitleVolumeComparer = new CitationComparer(yearTitleVolumeSortDescriptors);


			var xSection = GetBibliographySection(xReference);
			var ySection = GetBibliographySection(yReference);

			if (xSection == -1) xBibliographyCitation.NoBib = true;
			if (ySection == -1) yBibliographyCitation.NoBib = true;

			var sectionComparison = xSection.CompareTo(ySection);

			if (sectionComparison == 0)
			{
				var defaultCompareResult = defaultCitationComparer.Compare(x, y);
			if (defaultCompareResult != 0) return defaultCompareResult;
			
			return yearTitleVolumeComparer.Compare(x, y);
			}
			else
			{
				return sectionComparison;
			}
		}

		#region GetBibliographySection

		int GetBibliographySection(Reference reference)
		{
			if (reference.ReferenceType == null) return MakeSectionZeroFirstSection ? int.MinValue : int.MaxValue; ;

			Tuple<ReferenceType, ReferenceType> fullReferenceType = FullReferenceType.GetFullReferenceType(reference);
			if (fullReferenceType == null) return MakeSectionZeroFirstSection ? int.MinValue : int.MaxValue;

			int section;
			if (FullReferenceTypeSections.TryGetValue(fullReferenceType, out section))
			{
				if (section < 0) return -1;
				if (section == 0) return MakeSectionZeroFirstSection ? int.MinValue : int.MaxValue;
				return section;
			}

			return MakeSectionZeroFirstSection ? int.MinValue : int.MaxValue;
		}

		#endregion GetBibliographySection
	}

	public static class FullReferenceType
	{
		#region  Constructor

		static FullReferenceType()
		{
			ArchiveMaterial
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.ArchiveMaterial, null);			//can stand alone
			ArchiveMaterialInBookEdited
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.ArchiveMaterial, ReferenceType.BookEdited);

			AudioBook
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.AudioBook, null);
			AudioOrVideoDocument
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.AudioOrVideoDocument, null);
			Book
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.Book, null);
			BookEdited
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.BookEdited, null);
			Broadcast
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.Broadcast, null);
			CollectedWorks
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.CollectedWorks, null);
			ComputerProgram
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.ComputerProgram, null);
			ConferenceProceedings
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.ConferenceProceedings, null);

			//Contribution
			//	= new Tuple<ReferenceType, ReferenceType>(ReferenceType.Contribution, null);				
			//can NOT stand alone, Citavi will interpret it as ContributionInBookEdited
			ContributionInBookEdited
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.Contribution, ReferenceType.BookEdited);
			ContributionInCollectedWorks
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.Contribution, ReferenceType.CollectedWorks);
			ContributionInConferenceProceedings
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.Contribution, ReferenceType.ConferenceProceedings);
			ContributionInSpecialIssue
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.Contribution, ReferenceType.SpecialIssue);
			ContributionInUnpublishedWork
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.Contribution, ReferenceType.UnpublishedWork);

			//ContributionInLegalCommentary
			//	= new Tuple<ReferenceType, ReferenceType>(ReferenceType.ContributionInLegalCommentary, null);
			//can NOT stand alone, Citavi will interpret it as ContributionInLegalCommentary
			ContributionInLegalCommentary
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.ContributionInLegalCommentary, ReferenceType.LegalCommentary);

			CourtDecision
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.CourtDecision, null);
			File
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.File, null);
			InternetDocument
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.InternetDocument, null);
			InterviewMaterial
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.InterviewMaterial, null);
			JournalArticle
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.JournalArticle, null);
			Lecture
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.Lecture, null);
			LegalCommentary
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.LegalCommentary, null);
			Manuscript
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.Manuscript, null);
			Map
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.Map, null);
			Movie
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.Movie, null);
			MusicAlbum
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.MusicAlbum, null);

			MusicTrack
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.MusicTrack, null);
			MusicTrackInMusicAlbum
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.MusicTrack, ReferenceType.MusicAlbum);

			NewsAgencyReport
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.NewsAgencyReport, null);
			NewspaperArticle
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.NewspaperArticle, null);
			Patent
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.Patent, null);
			PersonalCommunication
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.PersonalCommunication, null);
			PressRelease
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.PressRelease, null);
			RadioPlay
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.RadioPlay, null);
			SpecialIssue
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.SpecialIssue, null);
			Standard
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.Standard, null);

			StatuteOrRegulation
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.StatuteOrRegulation, null);
			//can stand alone, but is then more or less a StatuteOrRegulationInPeriodical
			StatuteOrRegulationInBookEdited
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.StatuteOrRegulation, ReferenceType.BookEdited);

			Thesis
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.Thesis, null);
			Unknown
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.Unknown, null);
			UnpublishedWork
				= new Tuple<ReferenceType, ReferenceType>(ReferenceType.UnpublishedWork, null);
		}

		#endregion Constructor

		#region Properties

		public static Tuple<ReferenceType, ReferenceType> ArchiveMaterial { get; private set; }					//can stand alone
		public static Tuple<ReferenceType, ReferenceType> ArchiveMaterialInBookEdited { get; private set; }

		public static Tuple<ReferenceType, ReferenceType> AudioBook { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> AudioOrVideoDocument { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> Book { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> BookEdited { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> Broadcast { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> CollectedWorks { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> ComputerProgram { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> ConferenceProceedings { get; private set; }

		//can NOT stand alone
		//public static Tuple<ReferenceType, ReferenceType> Contribution {get; private set;}						
		public static Tuple<ReferenceType, ReferenceType> ContributionInBookEdited { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> ContributionInCollectedWorks { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> ContributionInConferenceProceedings { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> ContributionInSpecialIssue { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> ContributionInUnpublishedWork { get; private set; }

		//can NOT stand alone
		public static Tuple<ReferenceType, ReferenceType> ContributionInLegalCommentary { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> CourtDecision { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> File { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> InternetDocument { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> InterviewMaterial { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> JournalArticle { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> Lecture { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> LegalCommentary { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> Manuscript { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> Map { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> Movie { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> MusicAlbum { get; private set; }

		public static Tuple<ReferenceType, ReferenceType> MusicTrack { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> MusicTrackInMusicAlbum { get; private set; }

		public static Tuple<ReferenceType, ReferenceType> NewsAgencyReport { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> NewspaperArticle { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> Patent { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> PersonalCommunication { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> PressRelease { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> RadioPlay { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> SpecialIssue { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> Standard { get; private set; }

		//can stand alone, but is then more or less a StatuteOrRegulationInPeriodical
		public static Tuple<ReferenceType, ReferenceType> StatuteOrRegulation { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> StatuteOrRegulationInBookEdited { get; private set; }

		public static Tuple<ReferenceType, ReferenceType> Thesis { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> Unknown { get; private set; }
		public static Tuple<ReferenceType, ReferenceType> UnpublishedWork { get; private set; }

		#endregion Properties

		#region Methods

		#region GetFullReferenceTypes

		public static IEnumerable<Tuple<ReferenceType, ReferenceType>> GetFullReferenceTypes()
		{
			yield return FullReferenceType.ArchiveMaterial;
			yield return FullReferenceType.AudioBook;
			yield return FullReferenceType.AudioOrVideoDocument;
			yield return FullReferenceType.Book;
			yield return FullReferenceType.BookEdited;
			yield return FullReferenceType.Broadcast;
			yield return FullReferenceType.CollectedWorks;

			yield return FullReferenceType.ComputerProgram;
			yield return FullReferenceType.ConferenceProceedings;

			yield return FullReferenceType.ContributionInBookEdited;
			yield return FullReferenceType.ContributionInCollectedWorks;
			yield return FullReferenceType.ContributionInConferenceProceedings;

			yield return FullReferenceType.ContributionInSpecialIssue;
			yield return FullReferenceType.ContributionInUnpublishedWork;

			yield return FullReferenceType.ContributionInLegalCommentary;
			yield return FullReferenceType.CourtDecision;
			yield return FullReferenceType.File;
			yield return FullReferenceType.InternetDocument;
			yield return FullReferenceType.InterviewMaterial;
			yield return FullReferenceType.JournalArticle;
			yield return FullReferenceType.Lecture;
			yield return FullReferenceType.LegalCommentary;

			yield return FullReferenceType.Manuscript;
			yield return FullReferenceType.Map;
			yield return FullReferenceType.Movie;
			yield return FullReferenceType.MusicAlbum;
			yield return FullReferenceType.MusicTrack;
			yield return FullReferenceType.MusicTrackInMusicAlbum;

			yield return FullReferenceType.NewsAgencyReport;
			yield return FullReferenceType.NewspaperArticle;
			yield return FullReferenceType.Patent;
			yield return FullReferenceType.PersonalCommunication;
			yield return FullReferenceType.PressRelease;
			yield return FullReferenceType.RadioPlay;
			yield return FullReferenceType.SpecialIssue;
			yield return FullReferenceType.Standard;

			yield return FullReferenceType.StatuteOrRegulation;
			yield return FullReferenceType.StatuteOrRegulationInBookEdited;

			yield return FullReferenceType.Thesis;
			yield return FullReferenceType.Unknown;
			yield return FullReferenceType.UnpublishedWork;
		}

		#endregion GetFullReferenceTypes

		#region GetFullReferenceType

		public static Tuple<ReferenceType, ReferenceType> GetFullReferenceType(Reference reference)
		{
			if (reference == null) return null;
			if (reference.ReferenceType == null) return null;

			ReferenceType referenceType = reference.ReferenceType;
			ReferenceType parentReferenceType;

			if (reference.ParentReference == null || reference.ParentReference.ReferenceType == null)
			{
				parentReferenceType = null;
			}
			else
			{
				parentReferenceType = reference.ParentReference.ReferenceType;
			}

			//Special cases: reference types that can NOT stand alone with regards to citation style engine
			if (parentReferenceType == null)
			{
				if (referenceType == ReferenceType.Contribution)
				{
					//Citavi interprets contributions without parent as "ContributionInBookEdited"
					parentReferenceType = ReferenceType.BookEdited;
				}
				else if (referenceType == ReferenceType.ContributionInLegalCommentary)
				{
					//Citavi interprets ContributionsInLegalCommentary without parent als "ContributionInLegalCommentary"
					parentReferenceType = ReferenceType.LegalCommentary;
				}
			}

			return new Tuple<ReferenceType, ReferenceType>(referenceType, parentReferenceType);

		}

		#endregion GetFullReferenceType

		#endregion Methods
	}
}
