//CPS022 
//Description: In the bibliography - Replace names after first mention by Ders., Dies.

using System;
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
            handled = false;

            if (citation == null || citation.Reference == null || citation.CitationManager == null) return null;
            if (componentPart == null || componentPart.Elements == null || !componentPart.Elements.Any()) return null;
            if (template == null) return null;

            //filter deactivates itself, if the bibliography is NOT YET completely sorted
            //this is necessary to avoid that this filter in turn changes the sort order, that it depends upon
            if (citation.CitationManager.BibliographyCitations.IsSorted == false) return null;

            //make sure the current componentPart is the FIRST inside the template
            if (template.ComponentParts == null || template.ComponentParts.Count == 0) return null;
            if (template.ComponentParts[0].Id != componentPart.Id) return null;

            #region ThisBibliographyCitation

            var thisBibliographyCitation = citation as BibliographyCitation;
            if (thisBibliographyCitation == null) return null;


            #endregion ThisBibliographyCitation

            #region PreviousBibliographyCitation

            var previousBibliographyCitation = GetPreviousVisibleBibliographyCitation(thisBibliographyCitation);
            if (previousBibliographyCitation == null) return null;
            if (previousBibliographyCitation.Reference == null) return null;
            if (previousBibliographyCitation.NoBib == true) return null;

            #endregion PreviousBibliographyCitation

            #region ThisTemplate

            var thisTemplate = thisBibliographyCitation.Template;
            if (thisTemplate == null) return null;

            #endregion ThisTemplate

            #region PreviousTemplate

            var previousTemplate = previousBibliographyCitation.Template;
            if (previousTemplate == null) return null;

            #endregion PreviousTemplate

            #region ThisPersonFieldElement

            var thisPersonFieldElement =
            (
                componentPart.Elements != null &&
                componentPart.Elements.Count > 0 ?
                componentPart.Elements[0] :
                null
            ) as PersonFieldElement;
            if (thisPersonFieldElement == null) return null;

            #endregion ThisPersonFieldElement

            #region PreviousPersonFieldElement

            var previousPersonFieldElement =
            (
                previousTemplate.ComponentParts != null &&
                previousTemplate.ComponentParts.Count > 0 &&
                previousTemplate.ComponentParts[0].Elements != null &&
                previousTemplate.ComponentParts[0].Elements.Count > 0 ?
                previousTemplate.ComponentParts[0].Elements[0] :
                null
            ) as PersonFieldElement;
            if (previousPersonFieldElement == null) return null;

            #endregion PreviousPersonFieldElement

            #region ThesePersons

            //we DO have a valid citation/reference a previous citation/reference, so we can compare their persons
            IEnumerable<Person> thesePersons = thisBibliographyCitation.Reference.GetValue(thisPersonFieldElement.PropertyId) as IEnumerable<Person>;
            if (thesePersons == null || thesePersons.Count() == 0) return null;

            #endregion ThesePersons

            #region PreviousPersons

            IEnumerable<Person> previousPersons = previousBibliographyCitation.Reference.GetValue(previousPersonFieldElement.PropertyId) as IEnumerable<Person>;
            if (previousPersons == null || previousPersons.Count() == 0) return null;

            bool failedOnce = false;

            #endregion PreviousPersons

            //since we are still here, we DO have persons in both cases to compare
			
			PersonTeamsCompareResult compareResult = GetPersonTeamsIdentityAndGenderStructure(thesePersons, previousPersons);
			if (compareResult.Identity == PersonTeamsIdentity.None) return null;
			
			bool handleAlsoPartialIdentity = true;  //if true, both of the following cases (1) and (2) will be treated. Otherwise, 
                                                    //if false, only case (1) will be treated 
            //(1) Watson, Mary / Smith, Emma, »An interesting contribution«, in: iidem (Eds.), Edited book with 2 editors, London 2012.
            //(2) Watson, Mary / Smith, Emma, »A rather boring contribution«, in: iidem / Green, Peter (Eds.), Edited book with 3 editors, London 2012.
			if (compareResult.Identity == PersonTeamsIdentity.Partial && !handleAlsoPartialIdentity) return null;
			

			//if you want "ders."/"dies." written in italics or other font styles, choose the desired option, for example FontStyle.Italic, FontStyle.Bold or FontStyle.SmallCaps			
			LiteralTextUnit idemSingularMaleLiteral = new LiteralTextUnit("Ders.", FontStyle.Neutral);
         	LiteralTextUnit idemSingularFemaleLiteral = new LiteralTextUnit("Dies.", FontStyle.Neutral);
         	LiteralTextUnit idemSingularNeutralLiteral = new LiteralTextUnit("Dass.", FontStyle.Neutral);
         	LiteralTextUnit idemPluralLiteral = new LiteralTextUnit("Dies.", FontStyle.Neutral);	

            //CAUTION: This script will get more complex, if the separators between persons differ from group to group os for the last person
            LiteralTextUnit personSeparator = new LiteralTextUnit(thisPersonFieldElement.FirstGroupPersonSeparator.Text, thisPersonFieldElement.FirstGroupPersonSeparator.FontStyle);
			
			//we remove all separators, because some person names will have to be suppressed and we want to avoid excess separators such as the /'s in idem///John Smith
			thisPersonFieldElement.FirstGroupPersonSeparator.Text = "";
			thisPersonFieldElement.FirstGroupLastPersonSeparator.Text = "";
			thisPersonFieldElement.FirstGroupToSecondGroupSeparator.Text = "";
			thisPersonFieldElement.SecondGroupPersonSeparator.Text = "";
			thisPersonFieldElement.SecondGroupLastPersonSeparator.Text = "";
			
			
			
			AfterFormatPersonEventArgs afp;
            thisPersonFieldElement.PersonFormatter.AfterFormatPerson +=
            (sender, e) =>
            {
                afp = (AfterFormatPersonEventArgs)e;
				
				#region Full Identity
				
				if (compareResult.Identity == PersonTeamsIdentity.Full)
				{
					if (afp.Index == 0)
					{
						afp.TextUnits.Clear();
						switch (compareResult.GenderStructure)
						{
							case PersonTeamGenderStructure.SingleMale:
								{
									afp.TextUnits.Add(idemSingularMaleLiteral);
								}
								break;
							case PersonTeamGenderStructure.SingleFemale:
								{
									afp.TextUnits.Add(idemSingularFemaleLiteral);
								}
								break;
							case PersonTeamGenderStructure.SingleNeuter:
								{
									afp.TextUnits.Add(idemSingularNeutralLiteral);
								}
								break;
							default:
								{
									afp.TextUnits.Add(idemPluralLiteral);
								}
								break;
						}
					}
					else
					{
						afp.TextUnits.Clear(); //we suppress the output of the person's name
					}
				}
				
				#endregion 
				
				#region Partial Identity
				
				else
				{
					if (afp.Index == 0)
					{
						afp.TextUnits.Clear();
						switch (compareResult.GenderStructure)
						{
							case PersonTeamGenderStructure.SingleMale:
								{
									afp.TextUnits.Add(idemSingularMaleLiteral);
								}
								break;
							case PersonTeamGenderStructure.SingleFemale:
								{
									afp.TextUnits.Add(idemSingularFemaleLiteral);
								}
								break;
							case PersonTeamGenderStructure.SingleNeuter:
								{
									afp.TextUnits.Add(idemSingularNeutralLiteral);
								}
								break;
							default:
								{
									afp.TextUnits.Add(idemPluralLiteral);
								}
								break;
						}
					}
					else if (afp.Index < compareResult.IdenticalPersonsCount)
					{
						afp.TextUnits.Clear(); //we suppress the output of the person's name
					}
					else
					{						
						afp.TextUnits.Insert(0, personSeparator); //we allow output of the person's name but inject the separator again
					}
				}
				
				#endregion
				
            };

            return null;
        }

        #region GetPreviousVisibleCitation

        private static BibliographyCitation GetPreviousVisibleBibliographyCitation(BibliographyCitation bibliographyCitation)
        {
            if (bibliographyCitation == null) return null;
            BibliographyCitation previousBibliographyCitation = bibliographyCitation;

            //consider nobib
            do
            {
                previousBibliographyCitation = previousBibliographyCitation.PreviousBibliographyCitation;
                if (previousBibliographyCitation == null) return null;

            } while (previousBibliographyCitation.NoBib == true);

            //still here? found one!
            return previousBibliographyCitation;
        }

        #endregion GetPreviousCitation

		#region GetPersonTeamsIdentityAndGenderStructure

        public static PersonTeamsCompareResult GetPersonTeamsIdentityAndGenderStructure(IEnumerable<Person> personsA, IEnumerable<Person> personsB)
        {
            PersonTeamsIdentity identity = PersonTeamsIdentity.None;
            PersonTeamGenderStructure genderStructure = PersonTeamGenderStructure.Unknown;
            int identicalPersonsCount = 0;

            var personPairs = GetPersonPairsByIndexPosition(personsA, personsB);
            var overallCounter = personPairs.Count();
            var identityCounter = 0;
            var maleCounter = 0;
            var femaleCounter = 0;
            var neutralCounter = 0;

            for (int i = 0; i < personPairs.Count(); i++)
            {
                var personA = personPairs.ElementAt(i).Item1;
                var personB = personPairs.ElementAt(i).Item2;

                if (personA == null || personB == null) break; 
                if (!personA.Id.Equals(personB.Id)) break;

                //identical!
                identityCounter++;
                //determine sex (just need to look at one of them, because they are identical)
                if (personA.Sex == Sex.Male)
                {
                    maleCounter++;
                    continue;
                }
                if (personA.Sex == Sex.Female)
                {
                    femaleCounter++;
                    continue;
                }
                if (personA.Sex == Sex.Neutral || personA.Sex == Sex.Unknown)
                {
                    neutralCounter++;
                    continue;
                }
            }

            #region PersonTeamsIdentity

            if (identityCounter == 0)
            {
                identity = PersonTeamsIdentity.None;
            }
            else if (identityCounter == overallCounter)
            {
                identity = PersonTeamsIdentity.Full;
            }
            else if (identityCounter < overallCounter)
            {
                identity = PersonTeamsIdentity.Partial;
            }

            #endregion

            #region IdenticalPersonsCount

            identicalPersonsCount = identityCounter;

            #endregion

            #region PersonTeamGenderStructure

            if (identityCounter == 1 && maleCounter == 1) genderStructure = PersonTeamGenderStructure.SingleMale;
            else if (identityCounter == 1 && femaleCounter == 1) genderStructure = PersonTeamGenderStructure.SingleFemale;
            else if (identityCounter == 1 && neutralCounter == 1) genderStructure = PersonTeamGenderStructure.SingleNeuter;

            else if (identityCounter > 1 && maleCounter == identityCounter) genderStructure = PersonTeamGenderStructure.TeamOfMalesOnly;
            else if (identityCounter > 1 && femaleCounter == identityCounter) genderStructure = PersonTeamGenderStructure.TeamOfFemalesOnly;
            else if (identityCounter > 1 && neutralCounter == identityCounter) genderStructure = PersonTeamGenderStructure.TeamOfNeutersOnly;

            else if (identityCounter > 1 && maleCounter + femaleCounter == identityCounter) genderStructure = PersonTeamGenderStructure.TeamOfMalesAndFemales;
            else if (identityCounter > 1 && femaleCounter + neutralCounter == identityCounter) genderStructure = PersonTeamGenderStructure.TeamOfFemalesAndNeuters;
            else if (identityCounter > 1 && maleCounter + neutralCounter == identityCounter) genderStructure = PersonTeamGenderStructure.TeamOfMalesAndNeuters;

            else if (identityCounter >= 3
                && maleCounter >= 1 && femaleCounter >= 1 && neutralCounter >= 1
                && maleCounter + femaleCounter + neutralCounter == identityCounter) genderStructure = PersonTeamGenderStructure.TeamOfMalesFemalesAndNeuters;

            else genderStructure = PersonTeamGenderStructure.Unknown;

            #endregion

            return new PersonTeamsCompareResult(identity, genderStructure, identicalPersonsCount);
        }

        private static IEnumerable<Tuple<Person, Person>> GetPersonPairsByIndexPosition(IEnumerable<Person> personsX, IEnumerable<Person> personsY)
        {
            int countX = personsX == null || !personsX.Any() ? 0 : personsX.Count();
            int countY = personsY == null || !personsY.Any() ? 0 : personsY.Count();

            int maxCount = Math.Max(countX, countY);
            if (maxCount == 0) yield break;

            for (int i = 0; i < maxCount; i++)
            {
                Person personX = personsX == null ? null : personsX.ElementAtOrDefault(i);
                Person personY = personsY == null ? null : personsY.ElementAtOrDefault(i);

                yield return new Tuple<Person, Person>(personX, personY);
            }
        }

        #endregion
				
		#region PersonTeamsIdentity

	    public enum PersonTeamsIdentity
	    { 
	        None,
	        Partial,
	        Full
	    }

	    #endregion

	    #region PersonTeamGenderStructure

	    public enum PersonTeamGenderStructure
	    {
	        Unknown,

	        /// <summary>
	        /// Identical person, a single female (Latin: eadem), F
	        /// </summary>
	        SingleFemale,
	        
	        /// <summary>
	        /// Identical person, a single male (Latin: idem), M
	        /// </summary>
	        SingleMale,
	 
	        /// <summary>
	        /// Identical persons, a single (neuter) organization (Latin: idem), N
	        /// </summary>
	        SingleNeuter,

	        /// <summary>
	        /// Identical persons, only females, 2 or more (Latin: eaedem), FF
	        /// </summary>
	        TeamOfFemalesOnly,
	        
	        /// <summary>
	        /// Identical persons, only males, 2 or more (Latin: eidem), MM
	        /// </summary>
	        TeamOfMalesOnly,
	        
	        /// <summary>
	        /// Identical persons, only (neuter) organizations, 2 or more (Latin: eadem), NN
	        /// </summary>
	        TeamOfNeutersOnly,

	        /// <summary>
	        /// Identical persons, mixed group of females and males only, MF
	        /// </summary>
	        TeamOfMalesAndFemales,

	        /// <summary>
	        /// Identical persons, mixed group of females and neuters only, FN
	        /// </summary>
	        TeamOfFemalesAndNeuters,
	        
	        /// <summary>
	        /// /// Identical persons, mixed group of males and neuters only, MN
	        /// </summary>
	        TeamOfMalesAndNeuters,

	        /// <summary>
	        /// Identical persons, mixed group of females, males and neuters, MFN
	        /// </summary>
	        TeamOfMalesFemalesAndNeuters
	    }
		
		#endregion
		
		#region PersonTeamsCompareResult

	    public class PersonTeamsCompareResult
	    {
	        #region Constructor

	        public PersonTeamsCompareResult(PersonTeamsIdentity identity, PersonTeamGenderStructure genderStructure, int identicalPersonsCount)
	        {
	            Identity = identity;
	            GenderStructure = genderStructure;
	            IdenticalPersonsCount = identicalPersonsCount;
	        }

	        #endregion

	        #region Properties

	        #region Identity

	        /// <summary>
	        /// Identity.None       Müller, Meier, Schultze versus Schmidt, Weber, Fischer:
	        /// Identity.Partial    Müller, Meier, Schultze versus Müller, Meier, Weber
	        /// Identity.Full       Müller, Meier, Schultze versus Müller, Meier, Schultze
	        /// </summary>
	        public PersonTeamsIdentity Identity
	        {
	            get; private set;
	        }

	        #endregion

	        #region GenderStructure

	        public PersonTeamGenderStructure GenderStructure   
	        {
	            get; private set;
	        }

	        #endregion

	        #region IdenticalPersonsCount

	        public int IdenticalPersonsCount
	        {
	            get; private set;
	        }

	        #endregion

	        #endregion
	    }

	    #endregion
	}
}
