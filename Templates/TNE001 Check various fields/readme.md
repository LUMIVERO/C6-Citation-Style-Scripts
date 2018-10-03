# Check various fields

Check if field is empty or not

## Prerequisites
Citavi 6 (and above)

## Use Case Example 
Sie möchten einige Titel im Literaturverzeichnis hervorheben (z.B. weil es Veröffentlichungen Ihres Instituts sind.) Sie haben vorbereitend in Ihrem Projekt die betreffenden Titel z.B. mit der blauen Marke markiert. Im Zitationsstileditor haben Sie für alle Titel eine Standardvorlage definiert. Sie erstellen eine neue Vorlage, die nun prüft, ob die blaue Marke (= Label 1) bei einem Titel gesetzt wurde. 

## Installation
See Citavi Manual: [Creating Custom Templates](http://www.citavi.com/creating_custom_templates)

## Customization
Der Programmcode enthält die generischen Feldnamen aller Dokumententypen, die keinen übergeordneten Titel haben. Die Liste der internen Feldbezeichnungen finden Sie im Handbuch. Sie können den generischen Namen auch in Citavi ermitteln, wenn Sie den Mauszeiger in das betreffende Feld stellen und die Tastenkombination CTRL+WIN+F12 drücken. Der Name des Hilfekontextes entspricht dem generischen Feldnamen.

Im Programmcode ändern Sie den Schalter **Ignore**, und zwar entweder zu **IsEmpty** (Feld ist leer) oder **IsNotEmpty** (Feld ist nicht leer). Sie können die Prüfung auch über mehrere Feldinhalte durchführen; die einzelnen Prüfungabfragen werden automatisch mit UND verknüpft.

## Author

* **Jörg Pasch** [joepasch](https://github.com/joepasch)


