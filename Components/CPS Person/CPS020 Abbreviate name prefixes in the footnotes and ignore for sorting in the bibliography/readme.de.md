# Namenspräfix in Fußnoten auch bei 'non-dropping particle' abkürzen, hinsichtlich der Sortierung im Literaturverzeichnis aber ignorieren 

Sie zitieren Autoren nur mit Nachnamen in den Fußnoten. Wenn Sie aber Autoren mit Präfixen im Namen zitieren, soll das Präfix auch erscheinen und zwar abgekürzt vor dem Nachnamen (z. B. v. Münch). Im Literaturverzeichnis soll das Präfix ausgeschrieben werden und vor dem Nachnamen stehen, hinsichtlich der Sortierung aber ignoriert werden. Die Sortierung soll also nur den Nachnamen berücksichtigen. 

## Voraussetzungen
Citavi 6 (oder höher)

## Beispiele
In der Fußnote:
- v. Münch, Legal education and the legal profession in Germany, 2002

Im Literaturverzeichnis: 
- von Münch, Ingo, Legal education and the legal profession in Germany, Baden-Baden 2002.

Der Beispiel-Autor soll im Literaturverzeichnis unter "M" erscheinen.

## Anpassung
Sie müssen den Code in Ihrem Zitationsstil bei allen Personen-Komponenten (z.B. **Autor, Herausgeber oder Institution**) im **Fußnoten**-Regelset bei allen Dokumententypen einbauen. (Beachten Sie bitte, dass in dieser Hinsicht eine Abweichung zum Skript CPS015 "_Präfixe in Literaturverzeichnis und Fußnoten unterschiedlich ausgeben_" besteht, das bei den Personen-Komponenten im **Literaturverzeichnis**-Regelset einzufügen ist.)

Zusätzlich müssen in den Eigenschaften der Personen-Komponente folgende Einstellungen getroffen werden:
1. Rufen Sie die Personen-Komponente mit einem Doppelklick auf.
2. Setzen Sie im Abschnitt "**Namenspräfixe wie folgt formatieren**" ein Häkchen unter "**Namenspräfixe wie folgt abkürzen (Lang- und Kurzformen getrennt mit '|'**".
3. Tragen Sie im Feld "**Abkürzungsliste**" die benötigten Paare aus Lang- und Kurzform jeweils mit dem Pipe-Zeichen separiert ein, also beispielsweise: `von|v.|von der|v. d.`

![Eigenschaften der Personen-Komponente - Namenspräfixe wie folgt formatieren](CSE%20Namenspräfixe%20wie%20folgt%20formatieren%20%2B%20abkürzen%20-%20Abkürzungsliste.png)

In Ihrem Citavi-Projekt erfassen Sie unter **Listen** > **Personen und Institutionen** den Namen mit dem Präfix im Feld "**Name**", z. B. `von Münch`. Im Feld **Sortieren nach** geben Sie den Namen so ein, wie er im Literaturverzeichnis für die Sortierung berücksichtigt werden soll (z. B. `Münch, Ingo von`)

## Installation
Siehe Citavi Handbuch: [Using Programmable Components](https://www.citavi.com/programmable_components)

## Autor

* **Jörg Pasch** [joepasch](https://github.com/joepasch)
