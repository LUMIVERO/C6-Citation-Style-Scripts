# Zeitschriftenabkürzungen anhand anderer Abkürzung-Felder ermitteln

Der Zeitschriftenname wird gemäß der hinterlegten Einstellung (`Abkürzung 1` bzw. `2` oder `3`) der Komponente abgekürzt.

Wenn in der Komponente das Feld `Abkürzung 1` oder `2` ausgewählt ist, sorgt das Skript darüberhinaus dafür, dass auch der Inhalt des anderen passenden Abkürzungsfelds `2` bzw. `1` herangezogen wird und die Interpunktion entsprechend korrigiert wird, wenn das eigentlich ausgewählte Abkürzungsfeld leer ist.

Das Skript liegt in 2 Varianten vor:
- Das Feld `Abkürzung 3` wird nur ausgewertet, wenn das Skript `CPE003-A Vary points in abbreviated journal names - with extended fallback option` verwendet und dort der Schalter `extendedFallback` auf `true` gesetzt ist.
- Das Skript `CPE003-B Vary points in abbreviated journal names - without extended fallback option` verzichtet auf die Nutzung der Option `Automatisch zurückfallen auf den nächsten verfügbaren Namen`.

Die Abkürzungsfelder von Zeitschriftennamen lässt sich anzeigen und anpassen, wenn Sie die betreffende Zeitschrift über das Menü `Listen` > `Zeitschriften und Zeitungen` bearbeiten.

## Voraussetzungen
Citavi 5 (oder höher)

## Beispiele

In Citavi gibt es neben dem Namensfeld für das Journal noch 3 Abkürzungsfelder für Zeitschriftennamen:

Feld in Citavi | Zweck | Ausgabe
---|---|---
Name | vollständiger Name der Zeischrift | New England Journal of Medicine
Abkürzung 1 | mit Punkten abgekürzte Namen | N. England J. M.
Abkürzung 2 | ohne Punkte abgekürzte Namen | New Eng J Med
Abkürzung 3 | vom Verlag festgesetzte Abkürzungen | NEJM  - spielt bei beiden Skripte außer als Fallback bei Skript A keine Rolle

Ergänzender Hinweis: Wenn Sie bei PubMed recherchieren, erhalten Sie immer die Abkürzungen automatisch mitgeliefert, aber nur für das Feld `Abkürzung 2`.  

## Customizing
Erstellen Sie eine Kopie der Komponente **Zeitschrift**. Fügen Sie den Programmcode dieser Komponente hinzu.

## Installation
Siehe Citavi Handbuch: [Using Programmable Components](https://www.citavi.com/programmable_components)

## Autor

* **Jörg Pasch** [joepasch](https://github.com/joepasch)
