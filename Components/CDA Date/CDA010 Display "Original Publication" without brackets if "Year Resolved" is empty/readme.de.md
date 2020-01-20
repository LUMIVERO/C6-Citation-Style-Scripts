# Display "Original Publication" without brackets if "Year Resolved" is empty
Ausgabe der eckigen Klammern vor und nach der Komponente "Erstveröffentlichung" unterdrücken, wenn die Komponente "Jahr ermittelt" leer ist.

## Voraussetzungen
Citavi 6 (oder höher)

## Beispiele

- Müller, P.: Informationskompetenz vermitteln. Berlin 2014 [1999].
- Smith, J.: Information Literacy. London <del>[</del>1998<del>]</del>.

## Installation
Erstellen Sie eine neue [kombinierte Komponente](https://www.citavi.com/combining_components). Fügen Sie dort die beiden folgenden Elemente ein:
- "Jahr ermittelt" (YearResolved)
- "Erstveröffentlichung" (OriginalPublication)

Fügen Sie die eckigen Klammern vor und nach dem Element "Erstveröffentlichung" hinzu, und zwar als [Textelemente](https://www.citavi.com/text_elements).
Der neuen kombinierten Komponente fügen Sie das hier bereitgestellte Skript bei.

Siehe Citavi Handbuch: [Using Programmable Components](https://www.citavi.com/programmable_components)

## Autor

* **Jörg Pasch** [joepasch](https://github.com/joepasch)
