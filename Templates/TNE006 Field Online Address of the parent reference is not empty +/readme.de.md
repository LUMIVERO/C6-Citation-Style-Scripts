# Feld "Online-Adresse" des übergeordneten Titels ist nicht leer

## Voraussetzungen
Citavi 5 (oder höher)

## Beispiele
Sie möchten für Beiträge in Sammelwerken eine Internetadresse ausgeben. Diese tragen Sie gewöhnlich beim Beitrag im Sammelwerk in das Feld `Online-Adresse` ein. 
Wenn dieses Feld aber leer ist, soll geprüft werden ob möglicherweise das entsprechende Feld beim übergeordneten Werk einen Eintrag enthält.

- Schmidt, Markus (2020): Beitragstitel, in: Meyer, Silvia: Sammelwerkstitel, S. 14-16. [URL des übergeordneten Werks]

## Anpassung
Erstellen Sie eine neue programmierte Vorlagenbedingung und tragen Sie eine erklärende Bezeichnung in das Feld `Name` ein.

In die neue leere Vorlage ziehen Sie aus der Komponentenpalette auf der linken Seite die gewünschten Komponenten.

Statt der Komponente `Online-Adresse`, die sich auf den Beitrag selbst bezieht, nutzen Sie aus dem unteren Bereich der Komponentenpalette die Komponente <img src="https://www1.citavi.com/sub/manual6/de/icon_cse.png">`Online-Adresse` mit dem orange-farbenen Pfeilsymbol, welche das entsprechende Feld des übergeordneten Werks ausgibt, s. [Handbuch](https://www1.citavi.com/sub/manual6/de/index.html?cse_contributions.html).

## Installation
Siehe Citavi Handbuch: [Creating Custom Templates](http://www.citavi.com/creating_custom_templates)

## Autor

* **Jörg Pasch** [joepasch](https://github.com/joepasch)
