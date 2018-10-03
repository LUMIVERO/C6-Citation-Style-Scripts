# Check various fields

Check if field is empty or not

## Prerequisites
Citavi 6 (and above)

## Use Case Example
Sie möchten einige Titel im Literaturverzeichnis hervorheben (z.B. weil es Veröffentlichungen Ihres Instituts sind.) Sie haben vorbereitend in Ihrem Projekt die betreffenden Titel z.B. mit der blauen Marke markiert. Im Zitationsstileditor haben Sie für alle Titel eine Standardvorlage definiert. Sie erstellen eine neue Vorlage, die nun prüft, ob die blaue Marke (= Label 1) bei einem Titel gesetzt wurde. [→ Translate this with DeepL](https://www.deepl.com/translator#de/en/Sie%20m%C3%B6chten%20einige%20Titel%20im%20Literaturverzeichnis%20hervorheben%20(z.B.%20weil%20es%20Ver%C3%B6ffentlichungen%20Ihres%20Instituts%20sind.)%20Sie%20haben%20vorbereitend%20in%20Ihrem%20Projekt%20die%20betreffenden%20Titel%20z.B.%20mit%20der%20blauen%20Marke%20markiert.%20Im%20Zitationsstileditor%20haben%20Sie%20f%C3%BCr%20alle%20Titel%20eine%20Standardvorlage%20definiert.%20Sie%20erstellen%20eine%20neue%20Vorlage%2C%20die%20nun%20pr%C3%BCft%2C%20ob%20die%20blaue%20Marke%20(%3D%20Label%201)%20bei%20einem%20Titel%20gesetzt%20wurde.)

## Installing
[Creating Custom Templates](http://www.citavi.com/creating_custom_templates)

## Customizing
Der Programmcode enthält die generischen Feldnamen aller Dokumententypen, die keinen übergeordneten Titel haben. Die Liste der internen Feldbezeichnungen finden Sie im Handbuch. Sie können den generischen Namen auch in Citavi ermitteln, wenn Sie den Mauszeiger in das betreffende Feld stellen und die Tastenkombination CTRL+WIN+F12 drücken. Der Name des Hilfekontextes entspricht dem generischen Feldnamen.
Im Programmcode ändern Sie den Schalter **Ignore**, und zwar entweder zu **IsEmpty** (Feld ist leer) oder **IsNotEmpty** (Feld ist nicht leer). Sie können die Prüfung auch über mehrere Feldinhalte durchführen; die einzelnen Prüfungabfragen werden automatisch mit UND verknüpft.[→ Translate this with DeepL](https://www.deepl.com/translator#de/en/Der%20Programmcode%20enth%C3%A4lt%20die%20generischen%20Feldnamen%20aller%20Dokumententypen%2C%20die%20keinen%20%C3%BCbergeordneten%20Titel%20haben.%20Die%20Liste%20der%20internen%20Feldbezeichnungen%20finden%20Sie%20im%20Handbuch.%20Sie%20k%C3%B6nnen%20den%20generischen%20Namen%20auch%20in%20Citavi%20ermitteln%2C%20wenn%20Sie%20den%20Mauszeiger%20in%20das%20betreffende%20Feld%20stellen%20und%20die%20Tastenkombination%20CTRL%2BWIN%2BF12%20dr%C3%BCcken.%20Der%20Name%20des%20Hilfekontextes%20entspricht%20dem%20generischen%20Feldnamen.%0AIm%20Programmcode%20%C3%A4ndern%20Sie%20den%20Schalter%20**Ignore**%2C%20und%20zwar%20entweder%20zu%20**IsEmpty**%20(Feld%20ist%20leer)%20oder%20**IsNotEmpty**%20(Feld%20ist%20nicht%20leer).%20Sie%20k%C3%B6nnen%20die%20Pr%C3%BCfung%20auch%20%C3%BCber%20mehrere%20Feldinhalte%20durchf%C3%BChren%3B%20die%20einzelnen%20Pr%C3%BCfungabfragen%20werden%20automatisch%20mit%20UND%20verkn%C3%BCpft.)

## Author

* **Jörg Pasch** [joepasch](https://github.com/joepasch)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details


