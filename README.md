# Dimmer

A very simple Beat Saber mod that adds a modifier tab for dimming the in-game lights. Useful when the light show is too intense, and you don't want to use boring static lighting.

The dimmer can be applied both to the brightness (alpha channel) and the actual color (RGB channels) of the light. The default setting of only dimming the alpha seems to work fine, but dimming the color of the light results to a somewhat of a different look.

The color and brightness multipliers simply multiply the RGB and Alpha channels with the given values. Dimming RGB has a mutening effect to the color, while dimming alpha makes the colors look less bright. There are also options to limit both the RGB and alpha channels. This can be useful if you just want to get rid of very bright lights  without completely killing off lights taht are already dim. There is also the option to override alpha values for Chroma-colored walls.

Dependencies:
- BeatSaberMarkupLanguage
- SiraUtil
  
<img width="1000" alt="image" src="https://github.com/user-attachments/assets/4952252f-2e65-40ab-8fe2-780c7dc7f404" />

