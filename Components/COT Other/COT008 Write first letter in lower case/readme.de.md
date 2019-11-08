# Write first letter in lower case
Alle Wörter im Titelfeld sollen mit einem kleinen Buchstaben beginnen. Ausnahme: erstes Wort im Titel.

## Voraussetzungen
Citavi 6 (oder höher)

## Beispiele

- Doe, J. (2012): Living with dogs today. Köln.

## Konfiguration
Wenn Sie nur englischsprachige Literatur zitieren, wählen Sie in Zeile 37 die Option `ensureEnglishIsReferenceLanguage = false`. 

Wenn Sie englisch- und anderssprachige Literatur zitieren, wählen Sie in Zeile 37 die Option `ensureEnglishIsReferenceLanguage = true`. Tragen Sie in Ihrem Projekt bei nicht-englischsprachiger Literatur die jeweilige [Sprache](https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes) der Publikation in das Feld **Sprache** ein, z.B. *de* für Deutsch, *fr* für Französisch, *it* für Italienisch etc.

## Installation
Siehe Citavi Handbuch: [Using Programmable Components](https://www.citavi.com/programmable_components)

Sie können Wörter von der automatischen Kleinschreibung ausschließen: Tragen Sie diese im Code in Zeile 86 bis 90 in die Liste der "printAsStatedExpressions" ein.

## Autor
* **Jörg Pasch** [joepasch](https://github.com/joepasch)
