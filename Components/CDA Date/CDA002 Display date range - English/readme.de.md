# Display date range - English

Script can be attached to both date/time field element as well as text field element.

## Voraussetzungen
Citavi 6 (oder höher)

## Beispiele
Enter date range as: `dd.MM.yyyy - dd.MM.yyyy`

Format it to `yyyy, MMMM d - d` or `MMM. dd-dd, yyyy` e.g.

## Anwendung

Die Einstellung, ob die letzten beiden Buchstaben der Ordinalzahlen bei einer englischen Datumsausgabe hochgestellt werden sollen,  findet sich in dem Bereich `GetOrdinalString` > `English`. In den Zeilen 519 und 525-531 müssen ggf. jeweils die HTML-Tags `<sup>` und `</sup>` entfernt werden, falls keine Hochstellung erwünscht ist.
- `inputNumber + "th"` = 4th
- `inputNumber + "<sup>th</sup>"` = 4<sup>th</sup>


Beispiele für eine jahresübergreifende Datumsbereichsangabe
| Formatanweisung in Zeile 60 | Ausgabeformat |
| ------------- | ------------- |
|`"{3:dd}/{2:MM}/{1:yyyy}-{6:%dd}/{5:MM}/{4:yyyy}";` | `29/12/2013-04/01/2014` (Divis) |
|`"{3:dd}/{2:MM}/{1:yyyy}–{6:%dd}/{5:MM}/{4:yyyy}";` | `29/12/2013–04/01/2014` (Gedankenstrich, bis-Strich) |
|`"{2:MMMM} {3:%do}, {1:yyyy} - {5:MMMM} {6:%do}, {4:yyyy}";`	| `December 29th, 2013 - January 4th, 2014` |
|`"{3:%do} {2:MMMM} {1:yyyy} - {6:%do} {5:MMMM} {4:yyyy}";`	| `29th December 2013 - 4th January 2014` |
|`"{1:yyyy}/{4:yyyy}{0}; {1:yyyy}, {2:MMMM} {3:%do} - {4:yyyy}, {5:MMMM} {6:%do}";`	| `2013/2014a; 2013, December 29th - 2014, January 4th` |
|`"{1:yyyy}/{4:yyyy}{0}; {3:%do} {2:MMMM} {1:yyyy} - {6:%do} {5:MMMM} {4:yyyy}";`	| `2013/2014a; 29th December 2013 - 4th January 2014` |

## Installation
Siehe Citavi Handbuch: [Using Programmable Components](https://www.citavi.com/programmable_components)

## Autor

* **Jörg Pasch** [joepasch](https://github.com/joepasch)
