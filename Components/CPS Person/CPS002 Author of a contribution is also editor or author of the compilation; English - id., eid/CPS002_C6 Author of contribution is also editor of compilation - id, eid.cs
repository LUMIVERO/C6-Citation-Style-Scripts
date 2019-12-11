//C6#CPS002
//Description: Author of contribution is also editor of compilation - ders., dies.
//Version: 4.0
//Version: 4.0 Completely re-written for Citavi 6
//Version: 3.2 See note under 3.0 + jumps over 'empty' person fields
//Version: 3.1 Corrects error that "ders./dies./ibid." was always displayed
//Version: 3.0 Compares Persons by checking surrounding template (complete revision)
//Version: 2.0 Now works with GroupPrefix and GroupSuffix as well as additional LiteralElements or FieldElements inside the component part
//Version: 1.5 In case of "Contribution in Collected Works", we have to check for "Author (of contribution ) = Author (of parent reference)" 
//            and not for "Author (of contribution) = Editor (of parent reference). See code line 40.
//Version: 1.4 New variable "outputInSmallCaps", "outputInBold", "outputUnderlined"
//Version: 1.3 Single N oder unknown author yields "ders." instead of "dies."
//added outputInItalics variable on top

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
			//return handled = true if this macro generates the output (as an IEnumerable<ITextUnit>); the standard output will be suppressed
			//return handled = false if you want Citavi to produce the standard output; 

			handled = false;

			if (citation == null || citation.Reference == null) return null;
			if (componentPart == null || componentPart.Elements == null || !componentPart.Elements.Any()) return null;
			
			Reference reference = citation.Reference;
			if (reference == null) return null;
			
			#region ThisPersonFieldElement		
			
			PersonFieldElement thisPersonFieldElement = GetFirstPersonFieldElement(componentPart);
			if (thisPersonFieldElement == null || thisPersonFieldElement.SuppressOutput) return null;
			
			#endregion
			
			#region ThesePersons
			
			IEnumerable<Person> thesePersons = thisPersonFieldElement.GetPersons(citation);
			if (thesePersons == null || !thesePersons.Any()) return null;
			
			#endregion
			
			#region PreviousPersonFieldElement
		
			PersonFieldElement previousPersonFieldElement = GetPreviousPersonFieldElement(thisPersonFieldElement, template, citation);
			if (previousPersonFieldElement == null) return null;
			
			#endregion
			
			#region PreviousPersons
			
			IEnumerable<Person> previousPersons = previousPersonFieldElement.GetPersons(citation);
			if (previousPersons == null || !previousPersons.Any()) return null;
			
			#endregion
			
			PersonTeamsCompareResult compareResult = GetPersonTeamsIdentityAndGenderStructure(thesePersons, previousPersons);
			if (compareResult.Identity == PersonTeamsIdentity.None) return null;
			
            bool handleAlsoPartialIdentity = true;  //if true, both of the following cases (1) and (2) will be treated. Otherwise, 
                                        			//if false, only case (1) will be treated 
			//(1) Watson, Mary / Smith, Emma, »An interesting contribution«, in: iidem (Eds.), Edited book with 2 editors, London 2012.
            //(2) Watson, Mary / Smith, Emma, »A rather boring contribution«, in: iidem / Green, Peter (Eds.), Edited book with 3 editors, London 2012.
            if (compareResult.Identity == PersonTeamsIdentity.Partial && !handleAlsoPartialIdentity) return null;

			//if you want "ders."/"dies." written in italics or other font styles, choose the desired option, for example FontStyle.Italic, FontStyle.Bold or FontStyle.SmallCaps
			LiteralTextUnit idemSingularMaleLiteral = new LiteralTextUnit("id.", FontStyle.Neutral);
         	LiteralTextUnit idemSingularFemaleLiteral = new LiteralTextUnit("ead.", FontStyle.Neutral);
         	LiteralTextUnit idemSingularNeutralLiteral = new LiteralTextUnit("id.", FontStyle.Neutral);
         	LiteralTextUnit idemPluralLiteral = new LiteralTextUnit("eid.", FontStyle.Neutral);
			
			//NOTE: If you want a prefix such as "In: " and a suffix " (Hrsg.)", you can define them as group prefix and suffix on the field element inside the component part editor
			
			
			//CAUTION: This script will get more complex, if the separators between persons differ from group to group of for the last person
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
						afp.TextUnits.Clear();
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
						afp.TextUnits.Clear();
					}
					else
					{						
						afp.TextUnits.Insert(0, personSeparator);
					}
				}
				
				#endregion
				
            };
		

			return null;
		}
		
		#region GetFirstPersonFieldElement
		
		private static PersonFieldElement GetFirstPersonFieldElement(ComponentPart componentPart)
		{
			if (componentPart == null) return null;
			if (componentPart.Elements == null || !componentPart.Elements.Any()) return null;
			
			return componentPart.Elements.OfType<PersonFieldElement>().FirstOrDefault() as PersonFieldElement;
		}
		
		#endregion
		
		#region GetPreviousPersonFieldElement
		
		private static PersonFieldElement GetPreviousPersonFieldElement(PersonFieldElement thisPersonFieldElement, Template template, Citation citation)
		{
			if (thisPersonFieldElement == null) return null;
            if (template == null || template.ComponentParts == null || !template.ComponentParts.Any()) return null;

            IEnumerable<PersonFieldElement> allPersonFieldElements = template.ComponentParts.SelectMany(part => part.Elements).OfType<PersonFieldElement>().ToList();
            if (allPersonFieldElements == null || !allPersonFieldElements.Any()) return null;
		
			int thisIndex = allPersonFieldElements.FindIndex<PersonFieldElement>(item => item.Id == thisPersonFieldElement.Id);
			if (thisIndex == -1 || thisIndex == 0) return null;
			
			for (int i = thisIndex - 1; i >= 0; i--)
			{
				PersonFieldElement previousPersonFieldElement = allPersonFieldElements.ElementAtOrDefault<PersonFieldElement>(i) as PersonFieldElement;
				if (previousPersonFieldElement == null) continue;
				
				IEnumerable<Person> previousPersons = previousPersonFieldElement.GetPersons(citation);
				if (previousPersons == null || !previousPersons.Any()) continue;
				
				return previousPersonFieldElement;
			}
			
			return null;
		}
		
		#endregion
		
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