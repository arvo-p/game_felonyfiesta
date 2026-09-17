# Raylib Port Migration Plan

Porting a WinForms + `System.Drawing` game to Raylib is a large task because all rendering and input systems need to be rewritten. Here is the step-by-step plan:

## Phase 1: Core Systems & Input (Foundation)
1. **Remove WinForms dependency:** Update `Program.cs` to remove `Application.Run(new Form1())` and initialize a Raylib window (`Raylib.InitWindow`).
2. **Update Game Loop:** Modify `Game.cs` to run a `while (!Raylib.WindowShouldClose())` loop instead of relying on a `System.Windows.Forms.Timer`.
3. **Input Handling:** Rewrite `Keyboard.cs` to wrap Raylib's input functions (e.g., `Raylib.IsKeyDown`) instead of making Win32 API calls (`GetAsyncKeyState`).

## Phase 2: Assets & Textures
1. **Sprite & Image Loading:** Modify `Sprite.cs` to use Raylib's `Texture2D` and `Raylib.LoadTexture` instead of `System.Drawing.Image` and `MemoryStream`.
2. **Resource Management:** Adjust `Resources.cs` to no longer load `PrivateFontCollection` or `System.Drawing` classes, switching to Raylib's `Font` structure.

## Phase 3: Rendering Rewrite (`Draw.cs`)
This is the most complex phase. `System.Drawing.Graphics`, `Matrix`, and `RectangleF` must be replaced:
1. **Transforms:** Replace WinForms `Matrix.RotateTransform` with Raylib's `DrawTexturePro`, which natively handles rotation and origins.
2. **Primitives:** Replace `FillPolygon`, `DrawLine`, etc. with Raylib equivalents (`DrawTriangle`, `DrawLineEx`, etc.).
3. **Camera & Parallax:** Convert the `Graphics.Transform = cameraMatrix` logic to use Raylib's `Camera2D` system (e.g., `BeginMode2D`).

## Phase 4: UI & Menus (`Form1.cs`)
1. **Main Menu & Pause UI:** Convert the WinForms Buttons and Labels in `Form1.cs` into a custom immediate-mode UI using `Raylib.DrawRectangle`, `Raylib.DrawText`, and mouse collision checks (`Raylib.CheckCollisionPointRec`).
2. **Audio:** Transition `System.Media.SoundPlayer` to Raylib's `InitAudioDevice()` and `LoadSound() / PlaySound()`.
