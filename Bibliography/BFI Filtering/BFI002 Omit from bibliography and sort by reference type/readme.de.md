# Im Literaturverzeichnis sollen bestimmte (z.B. juristische) Dokumententypen unterdrückt und/oder gruppiert werden

Sie möchten bestimmte Dokumententypen im Text oder in den Fußnoten zitieren, aber diese aus dem Literaturverzeichnis ausschließen. Im Literaturverzeichnis möchten Sie die verbliebene Literatur entsprechend der zugrundeliegenden Dokumententypen gruppieren.

## Anpassung
Führen Sie einen Doppelklick auf den Bereich `EDIT Sections for each Reference Type` aus (beginnt in Code-Zeile 22). Hier können Sie für jeden Dokumententyp festlegen, ob darauf basierende Titel im Literaturverzeichnis erscheinen und ggf. in Gruppen zusammengefasst werden sollen.

Verwenden Sie die Ziffern wie folgt:

Ziffer | Bedeutung
-------- | --------
-1 | soll nicht im Literaturverzeichnis erscheinen
0 | soll am Anfang (oder am Ende) einsortiert werden
1 | soll in die erste Gruppe einsortiert werden
2 | soll in die zweite Gruppe einsortiert werden
3 | soll in die dritte Gruppe einsortiert werden
 
Wenn Sie beim Befehl `MakeSectionZeroFirstSection` die Option true wählen, werden alle Dokumententypen, denen Sie die Sortierziffer `0` zuordnen, am Anfang des Literaturverzeichnisses erscheinen.

## Voraussetzung
Citavi 5 (oder höher)

## Installation
Siehe Citavi Handbuch: [Sortierreihenfolge festlegen](https://www1.citavi.com/sub/manual6/de/index.html?cse_sorting_the_bibliography.html)

## Autor

* **Jörg Pasch** [joepasch](https://github.com/joepasch)
