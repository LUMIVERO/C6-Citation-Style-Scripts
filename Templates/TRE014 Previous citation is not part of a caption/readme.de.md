# Vorheriger Titeleintrag ist NICHT Teil einer Bild- oder Tabellen-Legende

Bei dem aktuellen Titelnachweis handelt es sich um den unmittelbaren Nachfolger einer zuvor erfolgten Erwähnung dieses Titels.

Nutzen Sie dieses Skript, um die wiederholte Nennung von Titeln differenziert behandelt zu können. Nur wenn der unmittelbar vorherige Titeleintrag nicht in einer Tabellen- oder Bild-Unterschrift steht, soll der hier behandelte Nachfolge-Nachweis durch "ebd." ersetzt werden.

## Voraussetzungen
Citavi 6 (oder höher)

## Beispiele
<img src="https://github.com/Citavi/C6-Citation-Style-Scripts/blob/master/Templates/TRE014%20Previous%20citation%20is%20not%20part%20of%20a%20caption/ebd.%20-%20Vorheriger%20Titeleintrag%20ist%20NICHT%20Teil%20einer%20Bild-%20oder%20Tabellen-Legende.png" width="500"/>

## Anwendung

Wichtig: Bis einschließlich Citavi 6.7 übergibt das Word Add-In die korrekte Information über die Eigenschaft eines Nachweises, Teil einer Bild-/Tabellen-Unterschrift zu sein, nur bei Bedarf.

Damit diese Information verfügbar ist und von der vorliegenden programmierten Vorlagenbedingung genutzt werden kann, muss im Zitationsstil daher bei einer beliebigen Vorlage auch eine der beiden folgenden eingebauten Bedingungen ausgewählt sein (es muss also nicht zwingend die Vorlage sein, die Sie mit diesem Skript anlegen, auch wenn das empfehlenswert ist):
- `Aktueller Titel ist Teil einer Bild-oder Tabellen-Legende` **ODER**
- `Aktueller Titel ist NICHT Teil einer Bild- oder Tabellen-Legende`

Da in der Regel auch in Bild-/Tabellen-Unterschriften selbst keine ebd.-Ausgabe erwünscht ist, sollte am einfachsten bei der Vorlage mit dieser programmierten Bedingung zusätzlich die folgende eingebaute Bedingung selektiert werden:
- `Aktueller Titel ist NICHT Teil einer Bild- oder Tabellen-Legende`

Darüberhinaus kombinieren Sie das Skript zusätzlich mit einer der beiden weiteren eingebauten Standard-Wiederholungsbedingungen:
- `Gleicher Titeleintrag und gleiche Zitatseite wie vorheriger` **ODER**
- `Gleicher Titeleintrag wie vorheriger`

Die folgenden Bedingungen können Sie bei Bedarf zusätzlich auswählen:
- `Vorheriger Titeleintrag ist NICHT Teil eines Mehrfachnachweises`
- `Vorheriger Titeleintrag steht auf derselben Seite in der Publikation` - dient dazu, dass "ebd." NICHT in der ersten Fußnote auf einer neuen Word-Seite erscheint
- `Vorheriger Titeleintrag steht für sich alleine in einer Fußnote`
- `Vorheriger Titeleintrag steht in der unmittelbar vorhergehenden Fußnote`

## Installation
Siehe Citavi Handbuch: [Creating Custom Templates](http://www.citavi.com/creating_custom_templates)

## Autor
* **Jörg Pasch** [joepasch](https://github.com/joepasch)
