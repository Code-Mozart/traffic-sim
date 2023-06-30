# Verkehrssimulation
## Mitglieder:
Luis Gutzeit, 209054

Markus Marewitz, 206633

Selim Özdemir, 209094
## Besonderheiten des Projekts:
Das Projekt besitzt keine direkte Interaktionsmöglichkeit. Um seine eigenen Straßen erstellen zu können, muss man jedoch einige Schritte beachten. 
Die einzelnen Schritte zur Erstellung eines Verkehrssystems:
1. Erstellen eines leeren Objekts namens "Road Network" mit dem Road Network Script.
2. Platzieren der MeshTiles unter den erstellten Prefabs. Straßen verbinden automatisch mit nahegeliegenden Straßen-Tiles, sowohl im Editor als auch in der Runtime. Bitte beachte, dass das Löschen von verbundenen Straßen eventuell zu Fehlern führen kann.
3. Platzieren von Traffic Agents aus dem Prefab Ordner.
4. In jedem Traffic Agent das erstellte Road Network setzen.
## Herausforderungen:
Die größte Herausforderung war es die Auto den Nodes folgen zu lassen. Wir wollten zuerst nicht, dass die Autos den Spuren einfach geradlinig folgen, sondern dass die Autos in gewisser Weise selbständig fahren und die Nodes nur anpeilen. Das bedeutet wir haben den Autos verschiedene Parameter um das Fahrverhalten in Kurven anpassen zu können. Leider hatte dies nicht gepasst, weswegen wir wieder zum einfacheren Fahrverhalten gewechselt sind. Dies hatte uns dementsprechend viel Zeit gekostet, konnten aber simultan die Straßensysteme aufbauen.
## Gesammelte Erfahrungen:
Wir haben Erfahrungen in Metaprogramming sammeln können über die Nutzung von Gizmo-Methoden. Außerdem konnten wir auch Erfahrung in der Zusammenarbeit sammeln und wie man als Team in Unity ein Projekt entwickelt. Dadurch dass das Projekt auch sehr offen von den Anforderungen war, konnten wir auch leicht unsere eigenen Ideen in das Projekt einbringen.
## Verwendete Assets, Codefragmente und Inspirition:
Für das ganze Projekt haben wir keinerlei Tutorials oder Codefragmente genutzt. Der gesamte Code wurde von uns selbst geschrieben. Das Automodell haben wir in Blender erstellt gehabt, während wir die Straßentiles in kleineren Fragmenten aus einem Polygon City Package genommen haben (https://assetstore.unity.com/packages/3d/environments/urban/city-package-107224).
## Video:
Link:   
