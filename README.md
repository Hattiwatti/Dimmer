# Dimmer

A very simple Beat Saber mod that adds a modifier tab for dimming the in-game environment. Useful when the light show is too intense, and you don't want to use boring static lighting.

Dimmer works by modifying the camera culling mask to first render everything except layers we don't want to dim (UI, sabers, blocks, walls etc.) After rendering, a black transparent overlay is drawn on top and then another camera is used to draw the layers that were excluded.

The modifier tab allows you to
- Turn the dimmer on or off
- Change the opacity of the dimmer overlay
- Enable rendering dimmer in Camera2 views

There is a small performance hit from having to use an extra camera to render the different layers, possibly due to the extra BloomPrePass component that is running. This cost seems to be lower when running with legacy multi-pass rendering enabled (can be toggled with AssetBundleLoadingTools)

Dependencies:
- BeatSaberMarkupLanguage
  
<img width="1000" alt="image" src="https://github.com/user-attachments/assets/8e060fb4-fda1-4d52-bef4-1e4cc00a5c96" />

## Comparison between 0, 0.5, 0.75, 0.90 and 1.0 opacity

<img width="1000" alt="0" src="https://github.com/user-attachments/assets/898cece7-f127-4ee6-9bf0-a12f87e2903f" />
<img width="1000" alt="50" src="https://github.com/user-attachments/assets/c8b241fa-26d5-41af-9500-0c8d69f46474" />
<img width="1000" alt="75" src="https://github.com/user-attachments/assets/4a94ab62-2ee7-4b77-8097-b6ffd4372ed8" />
<img width="1000" alt="90" src="https://github.com/user-attachments/assets/70dfd2c2-cf33-4479-92fb-38323340fa4a" />
<img width="1000" alt="100" src="https://github.com/user-attachments/assets/adad0e79-1232-4fca-8449-5a0917ed1d01" />
