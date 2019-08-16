# Präfixe in Literaturverzeichnis und Fußnoten unterschiedlich ausgeben

Sie zitieren Autoren nur mit Nachnamen in den Fußnoten. Wenn Sie aber Autoren mit Präfixen im Namen zitieren, soll das Präfix auch erscheinen und zwar vor dem Nachnamen (z. B. Ingo von Münch). Im Literaturverzeichnis soll das Präfix nach dem Vornamen stehen. 

## Voraussetzungen
Citavi 6 (oder höher)

## Beispiele
In der Fußnote:
- von Münch, Legal education and the legal profession in Germany, 2002

Oder:
- v. Münch, Legal education and the legal profession in Germany, 2002

Im Literaturverzeichnis: 
- Münch, Ingo von, Legal education and the legal profession in Germany, Baden-Baden 2002.

## Anpassung
Sie müssen den Code in Ihrem Zitationsstil bei allen Personen-Komponenten (z.B. **Autor, Herausgeber oder Institution**) im **Literaturverzeichnis**-Regelset bei allen Dokumententypen einbauen. (Beachten Sie bitte, dass in dieser Hinsicht eine Abweichung zum Skript CPS020 "_Namenspräfix in Fußnoten auch bei 'non-dropping particle' abkürzen, hinsichtlich der Sortierung im Literaturverzeichnis aber ignorieren_" besteht, das bei den Personen-Komponenten im **Fußnoten**-Regelset einzufügen ist.)

Zusätzlich müssen in den Eigenschaften der Personen-Komponente folgende Einstellungen getroffen werden:
1. Rufen Sie die Personen-Komponente mit einem Doppelklick auf.
2. Setzen Sie im Abschnitt "**Namenspräfixe wie folgt formatieren**" ein Häkchen unter "**Namenspräfixe wie folgt abkürzen (Lang- und Kurzformen getrennt mit '|'**".
3. Tragen Sie im Feld "**Abkürzungsliste**" die benötigten Paare aus Lang- und Kurzform jeweils mit dem Pipe-Zeichen separiert ein, also beispielsweise: `von|v.|von der|v. d.`

![Eigenschaften der Personen-Komponente - Namenspräfixe wie folgt formatieren](CSE%20Namenspräfixe%20wie%20folgt%20formatieren%20%2B%20abkürzen%20-%20Abkürzungsliste.png)

In Ihrem Citavi-Projekt erfassen Sie unter **Listen** > **Personen und Institutionen** den Namen mit dem (ggf. abgekürzten) Präfix im Feld "**Name**", z. B. `von Münch` oder `v. Münch`. Im Feld **Sortieren nach** geben Sie den Namen so ein, wie er im Literaturverzeichnis erscheinen soll (z. B. `Münch, Ingo von`)

## Installation
Siehe Citavi Handbuch: [Using Programmable Components](https://www.citavi.com/programmable_components)

## Autor

* **Jörg Pasch** [joepasch](https://github.com/joepasch)
