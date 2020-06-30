# Format letter for resolving ambiguity
Alle Mehrdeutigkeitsbuchstaben frei plazieren und formatieren.

Wichtig: Möglicherweise soll der Mehrdeutigkeitsbuchstabe nur bei Quellen bzw. Internetdokumenten mit fehlendem Erscheinungsjahr abweichend mit einem zusätzlichen Leerzeichen platziert werden: `(Miller 2010a)` vs. `(Smith o. J. a)` oder `(Smith no date b)`. In dem Fall ist stattdessen das Skript [CDA006 Display o. J. if year of publication is unknown](https://github.com/Citavi/C6-Citation-Style-Scripts/tree/master/Components/CDA%20Date/CDA006%20Display%20o.%20J.%20if%20year%20of%20publication%20is%20unknown) zu verwenden. Das zusätzliche Leerzeichen ist dort in Zeile 23 einzutragen.

## Voraussetzungen
Citavi 5 (oder höher)

## Beispiele
Citavi hängt bei Mehrdeutigkeiten, also für den Fall, dass ein Autor mit mehreren Publikationen aus demselben Jahr im Text zitiert wird, einen Buchstaben an das Jahr (Miller 2010a, 2010b); einzustellen über das Menü **Eigenschaften des Zitationsstils** > **Mehrdeutige Nachweise**. Die Formatierung des Buchstabens ist nicht anpassbar.
Dieses Skript ist für den seltenen Fall gedacht, dass der Buchstabe besondere Formatierungen benötigt (Miller 2010<sup>a</sup>) bzw. frei im Text platziert werden muss.

## Anpassung
In den Eigenschaften des Zitationsstils ist einzustellen, dass der Buchstabe für die Mehrdeutigkeitsunterscheidung an das Feld **Kurzbeleg** angehängt werden soll (auch wenn das im Stil nicht verwendet wird). 

Hinter das Feld **Jahr ermittelt** im Literaturverzeichnis sowie im Kurznachweis im Text ist ein zusätzliches Textelement anzuhängen, das keinen Inhalt hat. An dieses Textelement ist der hier bereitgestellte Programmcode zu hängen.

Das leere Textelement ist entsprechend zu formatieren, z.B. "Hochgestellt" oder "Kursiv".

## Installation
Siehe Citavi Handbuch: [Using Programmable Components](https://www.citavi.com/programmable_components)

## Autor

* **Jörg Pasch** [joepasch](https://github.com/joepasch)
