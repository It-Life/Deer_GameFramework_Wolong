# PlayerPrefs Editor & Utilities

[![openupm](https://img.shields.io/npm/v/com.sabresaurus.playerprefseditor?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.sabresaurus.playerprefseditor/) [![GitHub](https://img.shields.io/github/license/sabresaurus/PlayerPrefsEditor)](https://github.com/sabresaurus/PlayerPrefsEditor/blob/master/LICENSE.md) [![PRs Welcome](https://img.shields.io/badge/PRs-welcome-blue.svg)](http://makeapullrequest.com)

PlayerPrefs Editor & Utilities provide an easy way to see what PlayerPrefs your game is using and change them at run-time. It also includes encryption support to protect your player prefs from casual hacking and has additional support for more data types.

Editor features include:
- list all active PlayerPrefs
- search for PlayerPrefs to refine results
- change PlayerPref values at run-time
- add new PlayerPrefs
- delete PlayerPrefs
- quick delete all button
- import Player Prefs from another project
- supports working with the encryption features added in the utilities

Utilities features include:
- set and get the built in PlayerPref types using an encryption layer - plain text values are transparently converted to encryption so that the PlayerPrefs are protected in the device data stores
- set and get Enum values
- set and get DateTime values
- set and get TimeSpan values
- set and get Bool values

This package was originally sold on the [Unity Asset Store](https://www.assetstore.unity3d.com/en/#!/content/26656), but as of 29th April 2017 has been open sourced under the MIT License (see LICENSE for details). It is now maintained on this [Github repository](https://github.com/sabresaurus/PlayerPrefsEditor).

For a more comprehensive quick start guide and API documentation please go to http://sabresaurus.com/docs/playerprefs-editor-utilities-documentation/


## PlayerPrefs Editor

To open the PlayersPrefs Editor go to Window -> Player Prefs Editor

This will open an editor window which you can dock like any other Unity window.


### The Player Prefs List

If you have existing saved player prefs you should see them listed in the main window. You can change the values just by changing the value text box, you can also delete one of these existing player prefs by clicking the X button on the right.


### Search

The editor supports filtering keys by enterring a keyword in the search textbox at the top. As you type the search results will refine. Search is case-insensitive and if auto-decrypt is turned on it will also work with encrypted player prefs.


### Adding A New Player Pref

At the bottom of the editor you'll see a section for adding a new player pref. There are toggle options to determine what type it is and a checkbox for whether the player pref should be encrypted. Once you've selected the right settings and filled in a key and value hit the Add button to instantly add the player pref.


### Deleting An Existing Player Pref

To delete an existing player pref, click the X button next to the player pref in the list. You can also delete all player prefs by clicking the Delete All button at the bottom of the editor.


## PlayerPrefs Utilities

IMPORTANT: If using encryption, make sure you change the key specified in SimpleEncryption.cs, this will make sure your key is unique and make the protection stronger.

In PlayerPrefsUtility.cs you'll find a set of utility methods for dealing with encryption and also new data types. There is documentation within this file explaining how each method works. There is also additional documentation at http://sabresaurus.com/docs/playerprefs-editor-utilities-documentation/
