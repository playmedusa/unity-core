# unity-core

# Events

# Input
It's build of two base componentes: The InputSystem and the InputDeviceComponent.
## System
## IDC
## Scriptables
## Recording

# Menu
The Menu system requires the Input system.
A basic hierarchy to work with may be:
 
 - Menu (Canvas + StateViewManager)
	- Ingame view (StateView)
	- Pause view (StateView)
	- GameOver view (StateView)

Then, we can set up the initial view in the StateViewManager public field.
By inheriting from the StateView class we can customize the scene-in and scene-out for a certain view.

## Menu cursor
The menu cursor requires the Input system, a StateViewManager and a StateView to properly work.
Each StateView will set if the MenuCursor renders or not.

## The static view

# FX

# Tweening

# Other tools
## Extensions
#### InstanceFromPool
Depends on the Object pooling script. Can be used with an array or a single gameobject.
It will create a poll for that gameObject with the desired length (if it doesn't exists already) and will return the next entry.

#### RouletteWheel
From any given weights float array, launches a roulette and returns the winner's index.

## Object pooling
## FSM
### FSMObject

# Other licenses
The i18n folder is forked from https://github.com/MoonGateLabs/i18n-unity-csharp/ with a few customizations.
