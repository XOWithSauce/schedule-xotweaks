# SCHEDULE I XOTWEAKS MOD

**NEEDS MELON LOADER**
> Previously known as LessShadows and / or LessTrash mod, both discontinued and integrated to this mod

## Features

- Remove Shadow Casting from the Sun / Moon / Street Lights to improve performance
- Configure Sun Light / Moon Light intensity
- Configure Optimized Light Distance to improve performance
- Configure Fog ending distance and Camera rendering distance
- Automatic Trash Cleaning
- Change the Time progression multiplier (Slow or speed up time ingame)
- Allows for disabling the Bushes objects or the Sea
- Configure Flash Light Intensity and Range
- Configure the Contrast to change how the game looks
- Apply any changes to config.json file by pressing LEFT CTRL + R to reload changes while in game

## Important!

- **"alternate" or "alternate-beta" branch users**: Download the `XOTweaks` version.
- **"default" or "beta" branch users**: Download the `XOTweaks-IL2CPP` version.

## Installation Steps

1. Install Melon Loader from a trusted source like [MelonWiki](https://melonwiki.xyz/).
2. Copy the DLL file and the `XOTweaks` folder (with `config.json`) into the `Mods` folder.
3. You are good to go!

## Configuration

1. Open the `XOTweaks` folder and locate the file called `config.json`.
2. The default contents of the `config.json` file are as follows:
   
```json
{
  "sunShadowDisabled": true,
  "sunLightIntensityMult": 0.3,
  "moonShadowDisabled": true,
  "moonLightIntensityMult": 9.5,
  "streetLightShadowDisabled": true,
  "optimizedLightApply": true,
  "optimizedLightDistance": 50.0,
  "fogEndDistanceMult": 180.0,
  "trashClearThreshold": 300,
  "timeProgressMult": 1.0,
  "disableBushes": true,
  "disableSea": false,
  "flashLightIntensity": 3.5,
  "flashLightRange": 30.0,
  "contrastMult": 2.0,
  "hotReloadEnabled": true,
  "camFarClipPlane": 230.0
}
```

- sunShadowDisabled:
	- true: Toggles off Shadow Casting from the Sun, improving performance
	- false: Toggles on Shadow Casting from the Sun, improving performance

- sunLightIntensityMult:
    - Controls how bright the sun light is
    - Default: 1.0 (Note: Must have decimal point in value)

- moonShadowDisabled:
	- true: Toggles off Shadow Casting from the Moon, improving performance
	- false: Toggles on Shadow Casting from the Moon, improving performance

- moonLightIntensityMult:
    - Controls how bright the moon light is
    - Default: 1.0 (Note: Must have decimal point in value)

- streetLightShadowDisabled:
    - true: Toggles off Shadow Casting from the Street Lights, improving performance
	- false: Toggles on Shadow Casting from the Street Lights, improving performance

- optimizedLightApply:
    - true: Sets the *optimizedLightDistance* on every Optimized Light to cull lights earlier, improving performance
    - false: Skips setting the *optimizedLightDistance* on Optimized Light instances

- optimizedLightDistance:
    - Controls how far Optimized Light can be, before its culled (not rendered). Lower value = better performance.
    - Default: Varies by Instance; 50.0, 80.0 or 100.0 (Note: Must have decimal point in value)

- fogEndDistanceMult:
    - Controls how far from the player Fog is rendered. Adjust alongside *camFarClipPlane* value.
    - Default: 250.0 (Note: Must have decimal point in value)

- trashClearThreshold:
    - Controls how often Trash is cleared from the world. If the value is over 9000 then trash is not cleared.
    - Default: 300 (Note: seconds, must be a whole number without decimal point)

- timeProgressMult:
    - Controls how fast the ingame days progress. 0.5 = Day length is doubled, 2.0 = day length is half of normal. Used to speed up time while in game.
    - Default: 1.0

- disableBushes:
    - true: Toggles off Bushes in the game
	- false: Toggles on Bushes in the game

- disableSea:
    - true: Toggles off Sea in the game
    - false: Toggles on Sea in the game

- flashLightIntensity:
    - Controls the player Flashlight intensity
    - Default: 2.7 (Note: Must have decimal point in value)

- flashLightRange:
    - Controls the player Flashlight range
    - Default: 8.0 (Note: Must have decimal point in value)

- contrastMult:
    - Controls the contrast for post processing effects
    - Default: 1.0 (Note: Must have decimal point in value)

- hotReloadEnabled:
    - true: While pressing LEFT CTRL + R keys the configuration will reload and apply any changes.
    - false: Hot Reload feature is disabled and configuration changes will only be reloaded if the save file is reloaded from Main Menu.

- camFarClipPlane:
    - Controls how far the player camera will render objects. Lower values = better performance. Adjust alongside *fogEndDistanceMult* value.
    - Default 3000.0 (Note: Must have decimal point in value)

> **Note**: The `config.json` and file will be automatically created in the `Mods/XOTweaks/` directory if it's missing.
---

<details>
  <summary>Configuration file that has Game Defaults</summary>
  Using the configuration preset provided here you can disable the mod settings to Game Defaults. This means that no change should be applied to the game. This is also a template which can be modified to tweak any values you want.

___

```json
{
  "sunShadowDisabled": false,
  "sunLightIntensityMult": 1.0,
  "moonShadowDisabled": false,
  "moonLightIntensityMult": 1.0,
  "streetLightShadowDisabled": false,
  "optimizedLightApply": false,
  "optimizedLightDistance": 50.0,
  "fogEndDistanceMult": 250.0,
  "trashClearThreshold": 9001,
  "timeProgressMult": 1.0,
  "disableBushes": false,
  "disableSea": false,
  "flashLightIntensity": 2.7,
  "flashLightRange": 8.0,
  "contrastMult": 1.0,
  "hotReloadEnabled": false,
  "camFarClipPlane": 3000.0
}
```
___
</details>
