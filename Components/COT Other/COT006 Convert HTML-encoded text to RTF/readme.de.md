# Convert HTML-encoded text to RTF

Sie möchten innerhalb von Text-Feldern, für die Citavi keine Formatierungsmöglichkeit vorsieht (z.B. `Zeitschrift`), einen Teil des Textes bzw. Namens <i>kursiv</i> stellen, <b>fett</b> formatieren, <u>unterstreichen</u>, <sup>höherstellen</sup> oder <sub>tieferstellen</sub>.

Wichtig: 
1) Wenn der gesamte Feldinhalt bei den entsprechenden Dokumententypen immer in der besonderen Weise (z.B. kursiv) ausgegeben werden muss, sollte die Formatierung über die Einstellungsmöglichkeiten der Felder im Zitationsstil vorgenommen werden.

2) Auch bei reinen Textfeldern (wie `Übersetzter Titel`, `Kurzbeleg` oder den `Freitext`-Feldern) wird dieses Skript nicht benötigt. Die unten aufgeführten HMTL-Tags werden hier auch ohne Skript umgesetzt. Dies gilt allerdings nicht für die Anzeige der Feldinhalte im Projekt im Reiter `TItel`, sondern nur in der Titelanzeige oberhalb der Titelliste (`Zahnrad`-Icon > `Aktuellen Titel im Zitationsstil anzeigen`) sowie im Word-Dokument.

Der Programmcode wird also nur ggf. bei den Feldern vom Typ "PersonFieldElement" (`Autor`, `Herausgeber`, `Institution` etc.), "PeriodicalFieldElement" (`Zeitschrift`), "SeriesTitleFieldElement" (`Reihentitel`) oder "NumericFieldElement" (`Auflage`, `Bandnummer der Reihe` etc.) benötigt.

Die folgenden HTML-Tags können verwendet werden:

- `<b>fett</b>` (bold)
- `<i>kursiv</i>` (italic)
- `<u>unterstrichen</u>` (underline)
- `<sup>hochgestellt</sup>` (superscript)
- `<sub>tiefgestellt</sub>` (subscript)

## Voraussetzungen
Citavi 5 (oder höher)

## Beispiele

- Smith, John (1918): Das kurze Leben der Eintagsfliege. (Studien zur *Ephemeroptera*, 12). London. 

## Installation
Siehe Citavi Handbuch: [Using Programmable Components](https://www.citavi.com/programmable_components)

## Autor

* **Jörg Pasch** [joepasch](https://github.com/joepasch)
