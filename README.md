# Volumetric Skyboxes

![Frenchinator is the GOAT](https://i.ibb.co/KxTFthzn/6e0Lv-Ma.png)

Have you ever found regular panoramic Cybergrind skyboxes a bit boring? What if you could have entire scenes unfold in the background?
Or maybe you just want a nice rain effect on your Cybergrind arena?

## Basic usage

In the blue Cybergrind terminal, open **Themes** -> **Custom** -> **Skybox**. You'll notice two extra buttons at the bottom of the screen.

![Custom skyboxes](https://i.ibb.co/bRML80Fz/m1.png)

Volumetric skybox files are located at `ULTRAKILL/Cybergrind/VolumetricSkyboxes`. This directory will be automatically created containing the default skyboxes the first time you launch the game with the mod installed.
All the custom volumetric skyboxes must also be placed inside this directory.

### Browser
The first one leads to the volumetric skyboxes browser. To close it and get back to the conventional skyboxes menu, just press it again.

![Browser](https://i.ibb.co/JR3ht7XP/m2.png)

**N.B.!** You can enable several skyboxes at once. It's a deliberate design choice to allow effect-focused skyboxes to remain enabled while another skybox is active. If multiple skyboxes have a custom fog / lighting scheme / grid textures at the same time the resulting visuals may be unpredictable.

### Settings

![Settings](https://i.ibb.co/ksMFL1MK/m3.png)

While not all skyboxes do this, some can override ambient lighting, grid textures, regular panoramic skyboxes and can emit sounds. This panel controls which features skyboxes are allowed to override.

As a bonus, even if you don't have any skybox enabled you can still tweak the lighting on Cybergrind arena.

You can also turn down the score panel if it obstructs your view.

### Installing custom volumetric skyboxes
As mentioned before, volumetric skyboxes are stored at `ULTRAKILL/Cybergrind/VolumetricSkyboxes`. Place your `.cgvsb` file there and restart ULTRAKILL for the mod to pick up all the changes.

**VERY Important note:** Volumetric skyboxes are basically Unity prefabs. They may contain sounds, flashing visuals and, most importantly, can utilize some native ULTRAKILL logic.

Even though custom scripts are _not_ supported due to security concerns and custom skyboxes cannot directly harm you, I advise you to remain careful with the skyboxes you install.

### Creating your own volumetric skyboxes
![Editor](https://i.ibb.co/yFfV4yJD/HREj9i-B.png)

It wouldn't be much fun if a handful of default skyboxes was all you were getting. Install the editor, make your own and share them with others.

The editor itself is something akin to the Rude editor (hell, 50% of its codebase _comes from_ the Rude editor) and can work within a Rude project.

You can find all the instructions here:

https://github.com/Flazhik/dev.flazhik.volumetric-skyboxes.editor

### Credits
Made by **Flazhik**. Also, join my [Discord server](https://discord.gg/Mc8RhjmbMS)!

Original idea by **Clof**

Header art by [**Frenchinator**](https://www.youtube.com/@frenchinatorer). Go check his channel out. _Now._

QA: **Frenchinator**, **Clof**, **Nicy**

Special thanks to **eternalUnion** for letting me use Rude editor exporter codebase