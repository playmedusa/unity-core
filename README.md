# unity-core
## Setting this project as git submodule
Example to add this repository as a submodule to an existing unity project:
```
git submodule add https://github.com/playmedusa/unity-core.git  Assets/unity-core
```

If you are clonning the unity project you need to init the existing submodules:
```
git submodule update --init --recursive
```

Finally, to pull new commits:
```
git pull --recurse-submodules
```

Note: This repo requires Scripting Runtime Version set to .NET 4.x in Unity Player Settings.

# Events

# Input
It's build of two base componentes: The InputSystem and the InputDeviceComponent.
## System
## IDC
## Scriptables
## Recording

# Menu
The menu system includes Menu states and UI view handlind and supports custom animated buttons.

The ```StateViewManager``` and ```StateView``` compose the skeleton for any menu.
On **Awake**, all ```StateViews``` will reset the anchored position, so the views can be scattered to easily work in edit mode.

A basic hierarchy to work with may be:
 
 - Menu root (**Canvas** + ```StateViewManager```)
	- Ingame view (```StateView```)
	- Pause view (```StateView```)
	- GameOver view (```StateView```)

Then, we can set up the initial view in the ```StateViewManager``` public field.
By inheriting from the ```StateView``` class we can customize the scene-in and scene-out for a certain view.
Changing from one view to another may be easily done with the ```ShowStateView``` method found in the ```StateViewManager``` class.

By default, the ```StateView``` class will try to fade in and out views looking for a CanvasGroup.
To change this behaviour, just use inheritance and override the ```Open``` and ```Close``` coroutines.

## AnimatedButton and ButtonAnimation
The ```AnimatedButton``` class is a customized Unity button that extends the base behaviour to work with the ```ButtonAnimation``` and ```MenuCursor``` clases.
To create a new custom animation we can inherit from ```AnimatedButton``` and override the **Idle, Select, Press, Release and Click** coroutines. ```BubbleButtonAnimation```, ```SpriteSwapButtonAnimation``` or ```TintButtonAnimation``` are some examples ready to be used.

## Menu cursor
This is a helper object that will sync the ```AnimatedButton``` states with the ```EventSystem``` while using a controller. Thus, the menu cursor requires the Input system, a ```StateViewManager``` and a ```StateView``` to properly work.
The menu cursor will be rendered only if the current ```StateView``` allows it.

## The static view
This ```StateView``` can be set up in the ```StateViewManager```. This view will be shown simultaneously with any other view if the ```Show Static``` bool is on.

## Locking views

# FX

# Tweening

# Other tools
## Extensions
#### InstanceFromPool
Depends on the Object pooling script. Can be used with an array or a single gameobject.
It will create a poll for that gameObject with the desired length (if it doesn't exists already) and will return the next entry.

#### RouletteWheel
From any given weights float array, launches a roulette and returns the winner's index.

#### TaskExtensions

## Object pooling
## FSM
### FSMObject

## Preprocessor

# Other licenses
The i18n folder is forked from https://github.com/MoonGateLabs/i18n-unity-csharp/ with a few customizations.
