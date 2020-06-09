# Der Nachweis hat das Format "Autor (Jahr)" - gilt für den 2. Teil des Nachweises mit der Jahreszahl und den Zitat-Seiten (YearOnly)

Wenn Sie in Citavis Word Add-in die Option `Person (Jahr)` nutzen, werden immer zwei separate Nachweise eingefügt:
- **Autorname** = Nachweis mit der Option `Nur Person` & `Keine Klammern`
- **(Jahr)** = Nachweis mit der Option `Nur Jahr`

Diese programmierte Vorlagenbedingung gilt für den zweiten Teil des Nachweises. Nutzen Sie dieses Skript, um den Part innerhalb der Klammern abweichend von der Standardvorlage oder einer der Wiederholungsregeln zu definieren.

Wenn Sie beispielsweise bei "Autor (Jahr)"-Nachweisen auf die ebd.-Verwendung verzichten möchten, müssten Sie für beide Teile des Nachweises mithilfe des jeweiligen Skripts eine eigene Vorlage hinterlegen und dort die benötigten Komponenten einfügen.

## Voraussetzungen
Citavi 5 (oder höher)

## Beispiele
Die Formatierung des Nachweises soll sich immer dann von der Standardvorlage unterscheiden, wenn Sie in Citavis Word Add-in die Option `Person (Jahr)` nutzen.
- (Autor Jahr): (Hinz & Kunz 2014, S. 18) - gleicher Titeleintrag wie vorheriger: (ebd.)
- Autor (Jahr): Hinz und Kunz (2014, S. 18) - gleicher Titeleintrag wie vorheriger: Hinz und Kunz (2014, S. 18), also keine ebd.-Verwendung.

## Anpassung
Erstellen Sie eine neue programmierte Vorlagenbedingung und tragen Sie eine erklärende Bezeichnung in das Feld `Name` ein, z.B.: `Autor (Jahr) Bedingung _ Nur Jahr`.

In die neue leere Vorlage ziehen Sie aus der Komponentenpalette auf der linken Seite die Komponenten `Jahr ermittelt` und `Zitat-Seiten` (die Komponente `Autor, Herausgeber oder Institution [last name]` wird nicht benötigt, weil über diese Vorlage nur der Part innerhalb der Klammern ausgegeben wird).

Falls Sie für eine der Komponente nverschiedene Formatierungen oder Trennzeichen verwenden möchten, müssten Sie die Komponente markieren und über den Befehl `Komponente` > `Duplizieren` eine Kopie erzeugen, die Sie entsprechend anpassen.

Setzen Sie unter `Datei` > `Eigenschaften des Zitationsstils` > `Mehrdeutige Nachweis`e > ein Häkchen vor `Zusatzoptionen bei Vorlagen anzeigen` und bestätigen Sie die Änderung mit `OK`:

<img src="https://github.com/Citavi/C6-Citation-Style-Scripts/blob/master/Templates/TXY021%20Citation%20has%20the%20format%20Author%20(Year)%20-%20second%20part%20of%20the%20citation%20(YearOnly)/Zitationsstil-Eigenschaften%20-%20Mehrdeutige%20Nachweise%20-%20Zusatzoptionen%20bei%20Vorlagen%20anzeigen.png" width="600">

Rufen Sie im Stil den **Unklaren Dokumententyp** auf, klicken im Regelset **Kurznachweis im Text** auf die neue Vorlage **"Autor (Jahr) Bedingung _ Nur Jahr"** und setzen ein Häkchen vor `alle Zitationen, die nach diesem Template ausgegeben werden, nicht auf Mehrdeutigkeit prüfen`.

Wenn die übrigen Dokumententypen auf dem Unklaren Dokumententyp basieren, genügt dies, ansonsten müssten die Schritte noch für die weiteren Dokumententypen wiederholt werden - nur dass Sie die programmierte Vorlagenbedingung nicht neu einfügen und kompilieren müssen, sondern diese nun auch bei den übrigen Dokumententypen und Regelsets zur Auswahl steht.

## Installation
Siehe Citavi Handbuch: [Creating Custom Templates](http://www.citavi.com/creating_custom_templates)

## Autor
* **Jörg Pasch** [joepasch](https://github.com/joepasch)
