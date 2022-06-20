# Marker-basierter_AR

![Photo](https://livinglab-essigfabrik.ids-research.de/wp-content/uploads/2022/02/200211_Living_Lab_web-1044.jpg)

## About

### USE CASE 1: STADTMODELL ÜBERLAGERT MIT VIDEOINHALTEN MIT HILFE VON MARKER-BASIERTER AR

Die grundsätzliche Idee dieser Entwicklung besteht darin, mediale Inhalte, wie Videos
oder Grafiken mit einem räumlichen Stadtmodell zu überlagern um eine zusätzliche,
darüber liegende Informationsebene zu schaffen. So können – neben den
baulichen Strukturen und Dimensionen auch zusätzliche Daten und Informationen,
die den räumlichen Kontext betreffen, abgelesen werden.
Im hier beschriebenen Anwendungsfall konnten an einem eigens gebauten Stadtmodell
des Deutzer Hafens im Maßstab 1:50 Details über die zukünftige Gebäudenutzung,
die nicht unmittelbar aus dem Modell ablesbar waren, anschaulich abgerufen
und interaktiv erlebbar gemacht werden. Diese Idee wurde mit Hilfe von QR-Codes
umgesetzt. Die QR-Codes wurden als Marker gedruckt und an entsprechende Stellen
auf einem Stadtmodell platziert. Per Smartphone-Kamera können diese dann
eingescannt werden um einen Link im Browser zu öffnen, der wiederum die räumlich-
relevanten Inhalte darstellte. Vorteilhaft an dieser Methode ist der besonders
niederschwellige Zugang zu virtuellen Inhalten, da nutzerseitig ein Smartphone mit
Internetanschluss genügt und keine bestimmte Applikation installiert werden muss.

![Diagram Use Case 1](https://livinglab-essigfabrik.ids-research.de/wp-content/uploads/2022/06/Flowcharts-fuer-Publikation-2.1.-Stadtmodell-ueberlagert-mit-Videoinhalten-mit-Hilfe-von-QR-Codes.jpg)

### USE CASE 2: STADTRUNDGANG ÜBERLAGERT MIT VIDEOINHALTEN MIT HILFE VON MARKER-BASIERTEN AR

Ähnlich wie im zuvor beschriebenen Prototyp, wurden bei dieser Umsetzung mediale
Inhalte als überlagerter Layer im räumlichen Kontext eingesetzt. Allerdings
wurden die QR-Codes in diesem Szenario im städtischen Außenraum aufgehängt,
um während eines Stadtspaziergangs an vorab definierten Orten zusätzliche, themenspezifische
Inhalte abrufen zu können. Diese Methode der Überlagerung von
realen und virtuellen Ortsinformationen ist vermutlich am ehesten bekannt aus touristischen,
künstlerischen oder marketing-bezogenen Umsetzungen.
Im hier beschriebenen Anwendungsfall der städtebaulichen Partizipationsveranstaltungen
konnten mit Hilfe der Marker während Ortsbegehungen nun auch animierte
Renderings zukünftiger Entwürfe gezeigt oder auch inhaltliche Nutzerbefragungen
realisiert werden. Anders, als bei der Realisierung des vorherigen Prototyps, wurde
hierfür allerdings eine eigene Smartphone-Applikation entwickelt. Die Funktion der
QR-Codes stellen in dieser Applikation keine Links zu einer Online-Videoplattform
dar, sondern dienen als Marker (Image-Tracking) mit der dann eine holographische
Videodarstellung eingeblendet wird. Als Grundgerüst der Applikation diente die
Laufzeit- und Entwicklungsumgebung Unity3D. Daneben wurde die Computer Vision
Bibliothek OpenCV für die Bildverarbeitung genutzt. Die App nutzt die interne
Kamera des jeweiligen Smartphones zum Abfilmen der realen Umgebung. Beim Filmen
bzw. Scannen des Markers wurden dieser dann der entsprechende in der App
gespeicherte Inhalt (z.B. Video-Inhalte) überlagert.
Während des ersten Meilenstein-Events konnte der Prototyp mit einer Gruppe von
rund 25 Teilnehmenden getestet werden. Im Rahmen eines Rundgangs durch das
zukünftige Quartier Deutzer Hafen war zu beobachten, dass die meisten Teilnehmenden
die Interaktion mit den QR-Codes sehr einfach bedienen konnten und mit
großem Interesse die Video-Inhalte hinter den QR-Codes nutzen. Die technische Realisierung
mit Hilfe von Markern hatte bei der Anzahl an Teilnehmenden allerdings
zur Folge, dass nicht jeder bzw. jede die Gelegenheit bekommen konnte, sich alle
Videos anzuschauen, da es bei dieser Applikation nötig war permanent den QR-Code
zu filmen, um das Video anschauen zu können. Bei der Anzahl an Teilnehmenden,
hätte dieser Vorgang den zeitlichen Rahmen der Veranstaltung überschritten. Die
Nutzung eigener Smartphones konnte hier jedoch etwas Abhilfe schaffen. Dennoch
gab es bei der Distribution der Applikation auf die privaten Geräte einige Hürden:
Zum einen konnte die Applikation nur für Android-Geräte mit einer gewissen Versionsnummer realisiert werden und zum anderen musste die App per USB-Verbindung
im Vorfeld von Laptops bezogen werden. Daraus resultierte ein kleiner zeitlicher
Verzug im Ablauf des Programms.
Generell lässt sich sagen, dass die Nutzung privater Smartphones, anstelle von Tablets
aus dem Bestand des Projekts, im Szenario des Stadtspaziergangs aufgrund
der Teilnehmer:innenzahl sinnvoll war, aber die technischen Hürden zu viele Komplikationen
hervorgerufen haben. Zur Gewährleistung einer reibungslosen Funktion
zukünftiger, eigens entwickelter Applikationen wurde daher hieraus der Rückschluss
gezogen, dass die internen Tablets des Living-Labs ein bessere Nutzererfahrung liefern
können.

![Diagram Use Case 2](https://livinglab-essigfabrik.ids-research.de/wp-content/uploads/2022/06/Flowcharts-fuer-Publikation-2.2.-Stadtrundgang-ueberlagert-mit-Videoinhalten-mit-Hilfe-von-Marker-basierten-AR.jpg)

