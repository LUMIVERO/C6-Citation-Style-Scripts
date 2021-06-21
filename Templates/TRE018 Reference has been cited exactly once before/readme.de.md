# Titel wurde zuvor genau einmal zitiert

Bei dem aktuellen Titelnachweis handelt es sich um zweite Erwähnung dieses Titels.

Nutzen Sie dieses Skript, um die wiederholte Nennung von Titeln differenziert behandelt zu können.

Dieses Skript dient dazu,  die zweite Erwähnung des Titels eine verkürzte Form zu definieren, beispielsweise inkl. Verweis auf ein Titelstichwort. 

Das Skript kann in Fußnoten oder für Kurznachweise im Text eingesetzt werden.

## Voraussetzungen
Citavi 5 (oder höher)

## Anpassung

Aktuell behandelt dieses Skript die 2. Nennung eines Titels. 

Alternativ kombinieren Sie das Skript mit den weiteren eingebauten Standard-Wiederholungsbedingungen, um zu verhindern, dass "ebd." bereits bei der zweiten Nennung eines Titels erfolgt.
- `Gleicher Titeleintrag und gleiche Zitatseite wie vorheriger` **ODER**
- `Gleicher Titeleintrag wie vorheriger` 

Die folgenden Bedingungen können Sie bei Bedarf zusätzlich auswählen:
- `Vorheriger Titeleintrag ist NICHT Teil eines Mehrfachnachweises`
- `Vorheriger Titeleintrag steht auf derselben Seite in der Publikation` - dient dazu, dass "ebd." NICHT in der ersten Fußnote auf einer neuen Word-Seite erscheint
- `Vorheriger Titeleintrag steht für sich alleine in einer Fußnote`
- `Vorheriger Titeleintrag steht in der unmittelbar vorhergehenden Fußnote`

## Beispiele

Variante A:
1. Nennung (`Standardvorlage`): `Vollbeleg`
   - **Immanuel Kant: Kritik der reinen Vernunft. Stuttgart 1984 [1781].**
2. Nennung (Vorlage mit `diesem Skript`, muss OBERHALB von der Bedingung `Titel wurde bereits in einer vorherigen Fußnote genannt` platziert werden): `Kurznachweis` inkl. Ausgabe des vollständigen Titels sowie eines Titelstichworts (mithilfe der `Kurzbeleg`-Funktion)
   - **Kant: Kritik der reinen Vernunft (=KrV).**
3. und weitere Nennungen (Vorlage mit der Bedingung `Titel wurde bereits in einer vorherigen Fußnote genannt`): `Kurznachweis` inkl. Ausgabe des Titelstichworts
   - **Kant: KrV.**

Variante B, wenn das Skript mit den Wiederholungsbedingungen kombiniert wird:
1. Nennung (`Standardvorlage`): `Vollbeleg`
   - **Immanuel Kant: Kritik der reinen Vernunft (=KrV). Stuttgart 1984 [1781].**
2. Nennung (Vorlage mit `diesem Skript`, muss OBERHALB von der Bedingung `Titel wurde bereits in einer vorherigen Fußnote genannt` platziert werden): `Kurznachweis` (ggf. inkl. Ausgabe eines Titelstichworts mithilfe der `Kurzbeleg`-Funktion) - hier soll noch KEIN ebd. verwendet werden
   - **Kant: KrV.**
3. und weitere Nennungen, wenn identisch mit dem vorherigen Titeleintrag (Vorlage mit der Bedingung `Titel wurde bereits in einer vorherigen Fußnote genannt` & weiteren `Wdh.-Bedingungen`):
   - **ebd.**

## Installation
Siehe Citavi Handbuch: [Creating Custom Templates](http://www.citavi.com/creating_custom_templates)

## Autor
* **Jörg Pasch** [joepasch](https://github.com/joepasch)
