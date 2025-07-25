# TODO

## Scenes
* [ ] 2/3 more islands / levels
  * [ ] Different climates -> different microbes?
  * [ ] Have to discover microbes?

## NPCs
* [ ] More NPCs
  * [ ] More minigames

## QOL
* [ ] More music / sfx - looking like midi tracks arent easy to find royalty free
  * [ ] If adding more music, make sure that the tracks may be split for different sound fonts
  * [ ] Does this exist online?
* [ ] Control rebinding

## Functionality
* Further optimization (scripts, mesh merging)
  * Shadow quality
  * Target frame rate
  * Anti-Aliasing
  * Post-processing

## Game Demo TODO
* Get video made
  * Show off complete game
* [x] Add ending to first island
* [x] Controls images
  * Keyboard
    * sprint
    * Pause
  * Gameapd
    * sprint
    * Pause
* Playing notes
  * Playtime for me - 32 minutes and 22 seconds
  * Beach
    * [x] Player not doing research - adjust dialogue
    * [x] After talking to Carla first time - Make dialgoue more natural
    * [x] Pylons capable of iewing environment and changing it - Change dialogue
  * Cave
    * [x] "Well you don't need me to tell ou" - Typo
    * [x] When being asked to look at the encyclopedia, remind player of tablet
    * [x] "You can feel free to take your next steps" - Might be a typo
  * Mountain
    * [x] First scientist dialogue - "You've picking up on it so fast"
    * [x] Only give first set of microbes in obj desc. Not everything
    * [x] Final scientist dialogue not switched
  * General
    * [x] Colors on dialogue progress indicator
    * [x] NPCs slide around still
    * Objective tracker doesnt always start on top
    * Player merges into drone
    * Camera doesnt follow player smoothly during drone
    * [x] Step up should only go when player has movement vector applied
    * Drone can get stuck when given several conflicting instructions
    * Minigame cam doesnt work
    * [x] Objective descriptions need to autosize
    * In general, add filler dialogue after story dialogue given

## Playtest Feedback
* Carla compressed dialogue (I like it as is, maybe change context to make more sense?)
* [x] Resource for help always (Tablet)
* More balanced approach for bug minigame
* Start graphs at maxed out state?
  * Would make more sense, but also doesn't show motion
* Hats not implemented
* [x] Ladder snapping not perfect (WORKIN ON IT)
* [x] Still able to reach unstable state in mountain pylon (POTENTIALLY FIXED)
* Swipe scrolling messes with click UI (UNABLE TO REPRODUCE)
* [x] Add encyclopedia access globally (Tablet)
* [x] Lots of movement between island and boat
  * [x] Got rid of "return to the scientist" missions at the starts of the sections. Potentially re-add
