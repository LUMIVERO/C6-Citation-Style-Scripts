# Titel wurde zuvor mindestens zweimal in Fußnoten zitiert

Bei dem aktuellen Titelnachweis handelt es sich um mindestens die dritte Erwähnung dieses Titels in den Fußnoten.

Nutzen Sie dieses Skript, um die wiederholte Nennung von Titeln differenziert behandelt zu können. In der Standardvorlage für die ersten Nennung wurde der Vollbeleg definiert. Eine weitere Vorlage mit der Bedingung `Titel wurde bereits in einer vorherigen Fußnote genannt` enthält eine verkürzte Form inkl. Verweis auf ein Titelstichwort, das für die weiteren Zitierungen verwendet wird.

Dieses Skript dient dazu, alle weiteren Erwähnungen des Titels mit einer noch kürzeren Zitierweise zu definieren. 

Alternativ kombinieren Sie das Skript mit den weiteren eingebauten Standard-Wiederholungsbedingungen, um zu verhindern, dass "ebd." bereits bei der zweiten Nennung eines Titels erfolgt.
- `Gleicher Titeleintrag und gleiche Zitatseite wie vorheriger` **ODER**
- `Gleicher Titeleintrag wie vorheriger` 

Die folgenden Bedingungen können Sie bei Bedarf zusätzlich auswählen:
- `Vorheriger Titeleintrag ist NICHT Teil eines Mehrfachnachweises`
- `Vorheriger Titeleintrag steht auf derselben Seite in der Publikation` - dient dazu, dass "ebd." NICHT in der ersten Fußnote auf einer neuen Word-Seite erscheint
- `Vorheriger Titeleintrag steht für sich alleine in einer Fußnote`
- `Vorheriger Titeleintrag steht in der unmittelbar vorhergehenden Fußnote`

## Voraussetzungen
Citavi 5 (oder höher)

## Beispiele

Variante A:
1. Nennung (`Standardvorlage`): `Vollbeleg`
   - **Immanuel Kant: Kritik der reinen Vernunft. Stuttgart 1984 [1781].**
2. Nennung (Vorlage mit der Bedingung `Titel wurde bereits in einer vorherigen Fußnote genannt`): `Kurznachweis` inkl. Ausgabe des vollständigen Titels sowie eines Titelstichworts (mithilfe der `Kurzbeleg`-Funktion)
   - **Kant: Kritik der reinen Vernunft (=KrV).**
3. und weitere Nennungen (Vorlage mit diesem Skript): `Kurznachweis` inkl. Ausgabe des Titelstichworts
   - **Kant: KrV.**

Variante B, wenn das Skript mit den Wiederholungsbedingungen kombiniert wird:
1. Nennung (`Standardvorlage`): `Vollbeleg`
   - **Immanuel Kant: Kritik der reinen Vernunft (=KrV). Stuttgart 1984 [1781].**
2. Nennung (Vorlage mit der Bedingung `Titel wurde bereits in einer vorherigen Fußnote genannt`): `Kurznachweis` (ggf. inkl. Ausgabe eines Titelstichworts mithilfe der `Kurzbeleg`-Funktion) - hier soll noch KEIN ebd. verwendet werden
   - **Kant: KrV.**
3. und weitere Nennungen, wenn identisch mit dem vorherigen Titeleintrag (Vorlage mit diesem Skript & weiteren Wdh.-Bedingungen):
   - **ebd.**

## Installation
Siehe Citavi Handbuch: [Creating Custom Templates](http://www.citavi.com/creating_custom_templates)

## Autor
* **Jörg Pasch** [joepasch](https://github.com/joepasch)
