using System.Numerics;
using Raylib_cs;
using System;
using System.Collections.Generic;
using Raylib_cs;

public class Keyboard {
    List<KeyboardKey> registeredkeys = new List<KeyboardKey>();
    List<KeyboardKey> keysPressed = new List<KeyboardKey>();
    List<KeyboardKey> keysPressedOnce = new List<KeyboardKey>();
    List<KeyboardKey> keysReleased = new List<KeyboardKey>();

    public Keyboard(KeyboardKey[] keys) {
        foreach(var key in keys) {
            registeredkeys.Add(key);
        }
    }

    public void ReadKeys() {
        keysReleased.Clear();
        
        // Raylib handles window focus internally in most cases for input,
        // but we can optionally check Window focus if needed.
        if (!Raylib.IsWindowFocused()) {
            keysPressed.Clear();
            keysPressedOnce.Clear();
            return;
        }

        foreach (KeyboardKey k in registeredkeys) {
            if (Raylib.IsKeyDown(k)) {
                if (keysPressed.Contains(k) == false) {
                    keysPressed.Add(k);
                    keysPressedOnce.Add(k);
                }
            } else {
                if (keysPressed.Contains(k)) {
                    keysReleased.Add(k);
                }
                keysPressed.Remove(k);
                keysPressedOnce.Remove(k);
            }
        }
    }

    public bool GetKey(KeyboardKey k) {
        return keysPressed.Contains(k);
    }

    public bool GetKeyOnce(KeyboardKey k) {
        return keysPressedOnce.Remove(k);
    }

    public bool GetKeyUp(KeyboardKey k) {
        return keysReleased.Contains(k);
    }
}




