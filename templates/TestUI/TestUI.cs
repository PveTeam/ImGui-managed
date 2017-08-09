﻿using ImGui;
using ImGui.Common.Primitive;
using ImGui.OSAbstraction.Graphics;

public class TestUI
{
    bool helpOn;
    bool widgetsOn = true;

    bool toggleOn = false;
    bool a = false;
    string active_id;
    double hSliderValue = 0.5f;
    double vSliderValue = 0.5f;
    ITexture image;
    string text = "ABCD\nEFGHI";


    public void OnGUI()
    {
        if(image == null)
        {
            image = GUI.CreateTexture("./Image/trees.jpg");
        }

        GUILayout.Label("ImGui says hello.");

        GUILayout.Space("Space~1", 5);
        if (GUILayout.CollapsingHeader("Help", ref helpOn))
        {
            ShowUserGuide();
        }

        if (GUILayout.CollapsingHeader("Widgets", ref widgetsOn))
        {
            text = GUILayout.Textbox("Text Box", new Size(120, 200), text);

            GUILayout.BeginHorizontal("all");
            {
                GUILayout.Space("HeadSpace",30);
                GUILayout.BeginVertical("V1");
                {
                    GUILayout.BeginHorizontal("H1");
                    {
                        if (GUILayout.Button("Button"))
                        {
                            a ^= true;
                        }
                        if (a)
                        {
                            GUILayout.Label("Thanks for clicking me!");
                        }
                    }
                    GUILayout.EndHorizontal();

                    toggleOn = GUILayout.Toggle("Toggle", toggleOn);

                    //GUILayout.BeginHorizontal("H2");
                    //{
                    //    GUILayout.Radio("Radio 0", ref active_id, "radio_b_0");
                    //    GUILayout.Radio("Radio 1", ref active_id, "radio_b_1");
                    //    GUILayout.Radio("Radio 2", ref active_id, "radio_b_2");
                    //}
                    //GUILayout.EndHorizontal();

                    hSliderValue = GUILayout.Slider("Horizontal Slider", hSliderValue, 0, 1.0);
                    GUILayout.Image("Image/trees.jpg");

                    GUILayout.BeginHorizontal("Vertical Sliders");
                    vSliderValue = GUILayout.VSlider("Vertical Slider", vSliderValue, 0, 1.0);
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

    }

    private void ShowUserGuide()
    {
        GUILayout.Label("Double-click on title bar to collapse window.");
        GUILayout.Label("Click and drag on lower right corner to resize window.");
        GUILayout.Label("Click and drag on any empty space to move window.");
        GUILayout.Label("Mouse Wheel to scroll.");
        GUILayout.Label("TAB/SHIFT+TAB to cycle thru keyboard editable fields.");
        GUILayout.Label("CTRL+Click on a slider to input text.");
        GUILayout.Label(
            @"While editing text:
- Hold SHIFT or use mouse to select text
- CTRL+Left/Right to word jump
- CTRL+A select all
- CTRL+X,CTRL+C,CTRL+V clipboard
- CTRL+Z,CTRL+Y undo/redo
- ESCAPE to revert\n
- You can apply arithmetic operators +,*,/ on numerical values.
  Use +- to subtract.");
    }
}
