using System.Numerics;
using Raylib_cs;
using System;
using Raylib_cs;

namespace game;

static class Program
{
    static void Main()
    {
        int windowWidth = 1024 + 256;
        int windowHeight = 512 + 256;
        
        Raylib.InitWindow(windowWidth, windowHeight, "Petty Goober - Raylib");
        Raylib.SetTargetFPS(60);
        Raylib.InitAudioDevice();
        
        // Form1 is not used for Game loop now, but Game.Init handles setup.
        // We need to refactor Game.cs heavily so we'll just initialize directly here.
        Game.Init(windowWidth, windowHeight);
        
        while (!Raylib.WindowShouldClose())
        {
            Game.Loop();
        }
        
        Game.End();
        Raylib.CloseAudioDevice();
        Raylib.CloseWindow();
    }    
}

