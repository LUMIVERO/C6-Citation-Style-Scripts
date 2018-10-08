# If you select 'None (as entered)' for input, force output of the prefix
Bei "Zitat-Seiten" Präfix "S." oder Suffix "ff." anzeigen, auch wenn Sie keine Seitenzahlbereiche verwenden.

## Prerequisites
Citavi 5 (and above)

## Use Case Example 
Sie geben über Citavis Word Add-In oder bei Zitaten direkt in Citavi Seiteninformationen ein, die Citavi nicht mehr als Seitenbereiche erkennt, z.B.
- 228, Rn. 14f
- 140, 220, 405-406
Damit Citavi nicht versucht, Regeln zur Darstellung von Seiten anzuwenden (z.B. zeige nur erste Seite an, zeige nur ganze arabische Zahlen an), wählen Sie unter den Optionen im Word Add-In die Möglichkeit **Zahlensystem** > **Unverändert (wie Eingabe)**.
 
In der Konsequenz wendet Citavi aber auch nicht mehr die Regeln an, um den Seiten das gewünschte Präfix voranzustellen: 
- S. für Seiten
- Sp. für Spalten
- Rn. für Randnummer, 
entsprechend den Einstellungen, die Sie im Zitationsstil getroffen haben. 

Gleiches gilt für mögliche Suffixe: f, ff.
 
Das hier bereitgestellte Skript sorgt dafür, dass Sie die Option "Unverändert (wie Eingabe)" nutzen können und trotzdem die im Zitationsstil definierten Präfixe und Suffixe erscheinen.

## Installation
See Citavi Manual: [Using Programmable Components](https://www.citavi.com/programmable_components)

## Author

* **Jörg Pasch** [joepasch](https://github.com/joepasch)
