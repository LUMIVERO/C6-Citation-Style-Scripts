# Datumsbereich ausgeben (Format: deutsch)

Das Skript kann zu Datums- sowie zu Textfeldern hinzugefügt werden.

## Voraussetzungen
Citavi 6 (oder höher)

## Beispiele
Datum eingeben in der Form: `dd.MM.yyyy - dd.MM.yyyy`

Umformen zu (z.B.): `yyyy, d. - d. MMMM` oder auch `dd.-dd. MMMM yyyy`

## Anwendung

Die Einstellung, ob die letzten beiden Buchstaben der Ordinalzahlen bei einer englischen Datumsausgabe hochgestellt werden sollen,  findet sich in dem Bereich `GetOrdinalString` > `English`. In den Zeilen 519 und 525-531 müssen ggf. jeweils die HTML-Tags `<sup>` und `</sup>` entfernt werden, falls keine Hochstellung erwünscht ist.
- `inputNumber + "th"` = 4th
- `inputNumber + "<sup>th</sup>"` = 4<sup>th</sup>


Beispiele für eine jahresübergreifende Datumsbereichsangabe
| Formatanweisung in Zeile 60 | Ausgabeformat |
| ------------- | ------------- |
|`"{3:dd}.{2:MM}.{1:yyyy}-{6:%dd}.{5:MM}.{4:yyyy}";` | `29.12.2013-04.01.2014` (Divis) |
|`"{3:dd}.{2:MM}.{1:yyyy}–{6:%dd}.{5:MM}.{4:yyyy}";` | `29.12.2013–04.01.2014` (Gedankenstrich, bis-Strich) |
|`"{3:%d}. {2:MMMM} {1:yyyy} – {6:%d}. {5:MMMM} {4:yyyy}";` | `29. Dezember 2013 – 4. Januar 2014` |
|`"{1:yyyy}/{4:yyyy}{0}, {3:%d}. {2:MMMM} {1:yyyy} – {6:%d}. {5:MMMM} {4:yyyy}";` | `2013/2014a, 29. Dezember 2013 – 4. Januar 2014` |
|`"{1:yyyy}/{4:yyyy}{0}, {2:MMMM} {1:yyyy} – {5:MMMM} {4:yyyy}";` | `2013/2014a, Dezember 2013 – Januar 2014` |

## Installation
Siehe Citavi Handbuch: [Using Programmable Components](https://www.citavi.com/programmable_components)

## Autor

* **Jörg Pasch** [joepasch](https://github.com/joepasch)
