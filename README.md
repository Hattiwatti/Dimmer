# Dimmer

A very simple Beat Saber mod that adds a modifier tab for dimming the in-game environment. Useful when the light show is too intense, and you don't want to use boring static lighting.

Dimmer works by modifying the camera culling mask to first render everything except layers we don't want to dim (UI, sabers, blocks, walls etc.) After rendering, a black transparent overlay is drawn on top and then another camera is used to draw the layers that were excluded.

The modifier tab allows you to
- Turn the dimmer on or off
- Change the opacity of the dimmer overlay
- Enable rendering dimmer in Camera2 views

Dependencies:
- BeatSaberMarkupLanguage
  
<img width="1000" alt="image" src="https://github.com/user-attachments/assets/8e060fb4-fda1-4d52-bef4-1e4cc00a5c96" />

