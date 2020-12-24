# Capturing ETS2 and ATS maps
This is going to be a rough guide on how to capture maps from ETS2 and ATS. Please bear in mind that this is not an easy process by any means, and you're likely to encounter some issue(s). If you've no idea what you're doing, it may be a difficult process to follow.

### The basics
First of all, you need: A map (that thing you drive on in ETS2 or ATS). You can use any map mod you like, as long as they work in ETS2 or ATS. Besides that, you will need ETS2 or ATS and a few tools.. (These are Windows-only, sorry).

### The capturing
Open up ETS2, enable the map mod and capture mod in your profile. Then go to the map, all the roads should be coloured magenta.  Zoom to the zoomlevel that no city names are visible anymore, and go to the upper-left corner of the map. Open Funbit's capturing utility, and follow his instructions. 

**Possible problems:**
*ETS2 crashes:*
Try editing the mod to work with the latest version, most likely the game_data.sii is outdated. (Though I cannot look into the future)
*ATS has no black background:*
Please drive, go to the free roam camera (press 0), go under the map until the background is completely black. Now the map's background should be black as well.
*It shows some sort of floating engine in the middle:*
Correct, no idea how to get around that, apart from using triple monitors

### After the map has been captured
You should have a map.png in the folder you selected. Check whether the capturing went right, and edit the map using Photoshop if you wish (will use a lot of memory, 16GB is recommended). When done, please proceed:

Install OSGeo4W and replace the gdal2tiles.py in the install directory with the gdal2tiles-leaflet version. You should now be able to use the -l switch. (You might be able to use gdal2tiles-leaflet standalone. I have not tried it but it will probably work)

Go to OSGeoW, and use `gdal2tiles -p raster -z 0-7 -l map.png X:\export\folder`, this will create a raster map with eight zoomlevels, using the leaflet structure (inverting y) from map.png to X:\export\folder

This folder will be the tiles folder in your map pack. If your map uses a different ratio than the normal map, please recalculate the pointPerPixel constant used in the map.js. Calculating this can be done by teleporting to a point in the game, and looking up what its pixel value is. The calculation:

    ((coordinates2 - coordinates1) / (pixels2 - pixels1)) / 2

Example used for ATS:
Redding crossing: 
Pixels:
X: 2244
Y:1278
Ingame coordinates:
X: -61235.7
Y: 2328.74

San Francisco crossing:
Pixels:
X: 2250
Y: 3666
Ingame coordinates:
X: -61215.9
Y: 13385.4

((13385.4 - 2328.74) / (3666 - 1278)) / 2

Please adjust the offset values to match the proper location, the best method is trial and error.

## Capturing the cities
See the README for ETS2 City Coordinate Retriever. 
Replace the variable content in cities.js with the data inside the citiesList from the export. 
If your map contains countries that were not in the default map, please adjust the `COUNTRY_NAME_TO_CODE` in map.js to include the new countries.

## Tools used:

 - ETS2/ATS
 - Funbit's MapExtractor (http://forum.scssoft.com/viewtopic.php?p=405122#p405122)
 - Map capture mod adjusted for ETS2 1.23 (http://www.mediafire.com/download/lij06ya5dts0t18/Map_Capture_Settings.zip) or ATS 1.2 (http://www.mediafire.com/download/w5lj4n9pp3pzs64/material.zip)
 - OSGeo4W (https://trac.osgeo.org/osgeo4w/)
 - gdal2tiles-leaflet (https://github.com/commenthol/gdal2tiles-leaflet)
 - ETS2-City-Coordinate-Retriever (https://github.com/Koenvh1/ETS2-City-Coordinate-Retriever)
 - ets2-mobile-route-advisor (https://github.com/mike-koch/ets2-mobile-route-advisor)
 - A calculator
