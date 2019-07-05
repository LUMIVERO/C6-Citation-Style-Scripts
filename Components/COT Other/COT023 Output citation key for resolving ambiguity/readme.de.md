# Ausgabe des Felds "Kurzbeleg" zur Vermeidung von mehrdeutigen Nachweisen
Für alle Titel wurden automatisch [Kurzbelege generiert](https://www1.citavi.com/sub/manual6/de/index.html?customizing_citation_keys.html), beispielsweise in Form von Titelstichworten. Der Eintrag im Feld "**Kurzbeleg**" soll nur dann ausgegeben werden, wenn ansonsten mehrdeutige Nachweise auftreten würden.
Die Verwendung von Mehrdeutigkeitsbuchstaben ist im Zitationsstil NICHT vorgesehen.

## Voraussetzungen
Citavi 6.3.6 (oder höher)

## Beispiele
- Müller 2014, Kurzbeleg Buch A, S. 17.
- Müller 2014, Kurzbeleg Buch B, S. 34.
- Doe 2015<del>, Kurzbeleg soll nicht erscheinen, da kein mehrdeutiger Nachweis vorliegt</del>, S. 24.

## Anpassung
Sie müssen den Code in Ihrem Zitationsstil bei der **Kurzbeleg**-Komponente im Regelset **Fußnote** bzw. **Kurznachweis im Text** einbauen.
Falls gewünscht, kann das Skript zusätzlich auch bei der **Kurzbeleg**-Komponente im Regelset **Literaturverzeichnis** eingesetzt werden, um nur die zuvor im Text bzw. den Fußnoten ausgegebenen Kurzbelege auch beim korrespondierenden Literaturverzeichniseintrag aufzuführen.

Entfernen Sie im Zitationsstil-Editor unter **Datei** > **Eigenschaften des Zitationsstils** > **Mehrdeutige Nachweise** das Häkchen vor "**Einen eindeutigen Buchstaben anfügen**". Die weiteren Optionen zur Differenzierung von mehrdeutigen Nachweisen können bei Bedarf ausgewählt werden.

Wichtig: Bis zum Erscheinen von Citavi 6.4 kann bei Verwendung des Skripts in großen Publikationen mit sehr vielen Nachweisen in Text und/oder Fußnoten ein Performance-Verlust auftreten. Wenn dadurch ein flüssiges Arbeiten behindert wird, deaktivieren Sie in Citavis Word Add-in unter "**Optionen**" die Einstellung "**Formatierungsanweisung des Zitationsstils sofort anwenden**". Klicken Sie stattdessen von Zeit zu Zeit auf die Schaltfläche **Aktualisieren**, um die Formatierungsanweisungen des Zitationsstils anzuwenden.

## Installation
Siehe Citavi Handbuch: [Using Programmable Components](https://www.citavi.com/programmable_components)

## Autor
* **Jörg Pasch** [joepasch](https://github.com/joepasch)
