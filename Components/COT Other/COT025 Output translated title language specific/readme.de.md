# Output translated title field depending on the content of the "Language" field

Im Literaturverzeichnis soll der Eintrag im Feld "Übersetzter Titel" nur bei Quellen erscheinen, die nicht auf Deutsch oder Englisch publiziert wurden.

## Voraussetzungen
Citavi 5 (oder höher)

## Beispiele

- Müller, P.: Informationskompetenz vermitteln. Berlin 2014.
- Smith, J.: Information Literacy. London 2010.
- Dubois, P.: Ceci n'est pas une pipe. [Dies ist keine Pfeife]. Paris 2019.

## Anpassung

1. Im Feld "Sprache" muss bei jedem Titel angegeben werden, in welcher Sprache der Titel publiziert wurde: de, en, fr, it etc.
2. Legen Sie im Programmcode fest, ob das Feld ausgegeben werden soll, wenn es sich um deutsche oder englische Publikationen sowie um Quellen mit abweichender bzw. fehlender Sprache handelt.

Alternatives Vorgehen
1. Neue Bedingung erstellen: Feld "Sprache" enthält eine andere Sprache als Englisch oder Deutsch.
2. Neue Vorlagen für jeden Dokumententyp erstellen, die bei dieser Bedingung zum Einsatz kommen.
3. Entsprechende Komponenten sowie zusätzlich die Komponente "Übersetzter Titel" in die neue Vorlage ziehen.

## Installation
Siehe Citavi Handbuch: [Using Programmable Components](https://www.citavi.com/programmable_components)

## Autor

* **Susanne Sitzler** [Susanne-S](https://github.com/Susanne-S)
