# Seitenbereich durch ff. abkürzen, wenn dieser aus genau 3 Seiten (oder ggf. 3 ODER 4 Seiten) besteht

Citavi unterscheidet bei Seitenzahlbereichen nur die Fälle "alle Seiten", "genau 1 Seite", "genau zwei Seiten".
Dieses Skript deckt den Fall ab, dass bei genau drei Seiten (oder ggf. drei oder vier Seiten) nur die erste Seitenzahl genannt und um " ff." ergänzt werden soll.

## Voraussetzungen
Citavi 5 (oder höher)

## Anwendung
In Zeile 31 kann der Schalter auf `false` gestellt werden, wenn auch ein Bereich von 3 ODER 4 Seiten mit " ff." verkürzt werden soll. 
Wenn `true` gewählt wurde, gilt das Skript nur für Seitenbereiche mit genau 3 Seiten.

## Beispiele

- Statt `S. 1-3` wird `S. 1 ff.` ausgegeben

## Installation
Siehe Citavi Handbuch: [Using Programmable Components](https://www.citavi.com/programmable_components)

## Autor

* **Jörg Pasch** [joepasch](https://github.com/joepasch)
