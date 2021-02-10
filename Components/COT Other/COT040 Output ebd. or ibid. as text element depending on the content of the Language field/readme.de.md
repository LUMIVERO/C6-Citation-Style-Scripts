# Ebd. und ibid. sprachabhängig ausgeben

Nutzen Sie dieses Skript, um die wiederholte Nennung von Titeln je nach Sprache der jeweiligen Quelle mit "ebd." oder "ibid." differenziert behandelt zu können.

Erstellen Sie wie gewohnt eine neue Vorlage mit einer der beiden eingebauten Standard-Wiederholungsbedingungen, s. www.citavi.com/ebenda:
- `Gleicher Titeleintrag und gleiche Zitatseite wie vorheriger` **ODER**
- `Gleicher Titeleintrag wie vorheriger` 

Die folgenden Bedingungen können Sie bei Bedarf zusätzlich auswählen:
- `Vorheriger Titeleintrag ist NICHT Teil eines Mehrfachnachweises`
- `Vorheriger Titeleintrag steht auf derselben Seite in der Publikation` - dient dazu, dass "ebd." NICHT in der ersten Fußnote auf einer neuen Word-Seite erscheint
- `Vorheriger Titeleintrag steht für sich alleine in einer Fußnote`
- `Vorheriger Titeleintrag steht in der unmittelbar vorhergehenden Fußnote`

Erstellen Sie ein neues Textelement [(s. Handbuch)](https://www1.citavi.com/sub/manual6/de/index.html?cse_text_elements.html) mit beliebigem Inhalt, an das Sie dieses Skript hängen. 

Falls Sie auch "a.a.O." bzw. "Op. cit." mehrsprachig ausgeben möchten, erstellen Sie ein zweites Textelement und fügen dort das entsprechend angepasste Skript hinzu.

## Voraussetzungen
Citavi 5 (oder höher)

## Beispiele

1. Müller 2014, S. 14-15. Siehe auch ihre stimmigen Thesen zu XY, ebd. S. 125.
2. Smith 2010, pp. 22-28. Siehe auch seine kontroversen Auslassungen zu XY, ibid. p. 125.

## Anpassung

1. Im Feld "Sprache" muss bei jedem Titel angegeben werden, in welcher Sprache der Titel publiziert wurde: de, en, fr, it etc.
2. Legen Sie im Programmcode den auszugebeneden Text für deutsche und englische Quellen sowie solche mit abweichender bzw. fehlender Sprache fest.

Alternatives Vorgehen
1. Neue Bedingung erstellen: Feld "Sprache" enthält "en".
2. Neue Vorlagen für jeden Dokumententyp erstellen, die bei dieser Bedingung zum Einsatz kommen.
3. Entsprechende Komponenten (Jahrgang, Seiten von-bis, Heftnummer, Herausgeber, ebd.-Textelement etc.) duplizieren und dort die Präfix- bzw. Suffix-Felder bzw. das Text-Feld entsprechend der Sprache füllen (Vol., pp., No., eds., ibid.).

## Installation
Siehe Citavi Handbuch: [Using Programmable Components](https://www.citavi.com/programmable_components)

## Autor

* **Susanne Sitzler** [Susanne-S](https://github.com/Susanne-S)
