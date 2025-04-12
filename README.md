# Dimmer

A very simple Beat Saber mod that adds a modifier tab for dimming the in-game lights. Useful when the light show is too intense, and you don't want to just use boring static lighting.

The dimmer can be applied both to the brightness (alpha channel) and the actual color (RGB channels) of the light. The default setting of only dimming the alpha seems to work fine, but dimming the color of the light results to a somewhat of a different look.

Modes:
- Multiplier multiplies all values with whatever value "Dimmer Multiplier" is set to. Simple!
- Range mode allows you to use Range Minimum and Range Maximum to define a range to which any values above the minimum are scaled to.
  For example with a minimum of 0.5 and a maximum of 0.6 a value of 1.0 would get scaled to 0.6, 0.75 would get scaled to 0.55 and values at or below 0.5 would remain the same.
  Max component is used for the RGB channel to scale all the color channels. The idea was to allow for finer control so that lights that are already dark would not get dimmed more.

![image](https://github.com/user-attachments/assets/6fecad57-e652-43c2-a38f-342f2086409f)
