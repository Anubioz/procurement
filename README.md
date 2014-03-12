Procurement-mod
===========

Faster fork of the [official Procurement](https://code.google.com/p/procurement/), aimed to work with large amount of tabs/items

## Features of the modified version:

* Removed some fancy stuff that slowed everything down (see the tests section below)
* Instant forum thread generation - no need to wait hours when selecting all items
* Total currency displayed in chaos instead of GCP (with Invasionish rates - modify them in Settings.xml
* Uses up to 2x less RAM (when switching tabs, searching for items, etc)
* Various bugfixes*Contains latest code from the original procurement source-tree
* Checks for updates at start-up (they are more frequent, then original)
* Various bug-fixes ([check the commits in github for a full list](https://github.com/Anubioz/procurement/commits/master))

## Tests:

1. Starting up in offline mode and searching for an item with 188 tabs

    * _Original procurement_: **00:02:48.61**
    * _Procurement-mod_: **00:00:59.17**

2. Creating a forum thread, containing every item from 137 tabs (by pressing "Select All" in the Trading tab)

    * _Original procurement_: **00:19:06.14**
    * _Procurement-mod_: **00:00:26.43**

3. RAM usage after searching for 5 items & switching through several tabs

    * _Original procurement_: **852MB**
    * _Procurement-mod_: **455MB**

### Latest changes log:

* 1.3.0.01: First version with update check feature
* Mar 13, 2014: Added update check feature, which requires version numbering
* Mar 12, 2014: Merged official tree r200 changes - added itemhover display of corrupted state
* Mar 12, 2014: Total currency now is displayed in chaos orbs - change ratios from default in Settings.xml (using Invasion ratios as default)
* Mar 12, 2014: Greatly improved memory consumption by killing leaks + loading speed improvement
* Mar 11, 2014: Merged changes from r199
* Mar 10, 2014: Fixed Vaal Orbs
* Mar 03, 2014: Initial Release

#### [Download procurement-mod binaries for Windows](https://github.com/Anubioz/procurement/raw/master/WindowsBinaries/procurement-mod-binaries-1.3.0.01.zip)