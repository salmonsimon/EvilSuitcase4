using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct KeyboardMouseIcons
{
    [Header("Mouse")]
    public Sprite mouse;
    public Sprite leftClick;
    public Sprite rightClick;
    public Sprite scrollWheel;

    [Header("Keyboard")]
    public Sprite escape;
    public Sprite space;
    public Sprite enter;
    public Sprite tab;
    public Sprite upArrow;
    public Sprite downArrow;
    public Sprite leftArrow;
    public Sprite rightArrow;
    public Sprite a;
    public Sprite b;
    public Sprite c;
    public Sprite d;
    public Sprite e;
    public Sprite f;
    public Sprite g;
    public Sprite h;
    public Sprite i;
    public Sprite j;
    public Sprite k;
    public Sprite l;
    public Sprite m;
    public Sprite n;
    public Sprite o;
    public Sprite p;
    public Sprite q;
    public Sprite r;
    public Sprite s;
    public Sprite t;
    public Sprite u;
    public Sprite v;
    public Sprite w;
    public Sprite x;
    public Sprite y;
    public Sprite z;
    public Sprite numberOne;
    public Sprite numberTwo;
    public Sprite numberThree;
    public Sprite numberFour;
    public Sprite numberFive;
    public Sprite numberSix;
    public Sprite numberSeven;
    public Sprite numberEight;
    public Sprite numberNine;
    public Sprite numberZero;
    public Sprite shift;
    public Sprite alt;
    public Sprite control;
    public Sprite backspace;
    public Sprite system;


    public Sprite GetSprite(string controlPath)
    {
        switch (controlPath)
        {
            case "escape": return escape;
            case "space": return space;
            case "enter": return enter;
            case "tab": return tab;
            case "upArrow": return upArrow;
            case "downArrow": return downArrow;
            case "leftArrow": return leftArrow;
            case "rightArrow": return rightArrow;
            case "a": return a;
            case "b": return b;
            case "c": return c;
            case "d": return d;
            case "e": return e;
            case "f": return f;
            case "g": return g;
            case "h": return h;
            case "i": return i;
            case "j": return j;
            case "k": return k;
            case "l": return l;
            case "m": return m;
            case "n": return n;
            case "o": return o;
            case "p": return p;
            case "q": return q;
            case "r": return r;
            case "s": return s;
            case "t": return t;
            case "u": return u;
            case "v": return v;
            case "w": return w;
            case "x": return x;
            case "y": return y;
            case "z": return z;
            case "1": return numberOne;
            case "2": return numberTwo;
            case "3": return numberThree;
            case "4": return numberFour;
            case "5": return numberFive;
            case "6": return numberSix;
            case "7": return numberSeven;
            case "8": return numberEight;
            case "9": return numberNine;
            case "0": return numberZero;
            case "shift": return shift;
            case "leftShift": return shift;
            case "rightShift": return shift;
            case "alt": return alt;
            case "leftAlt": return alt;
            case "rightAlt": return alt;
            case "control": return control;
            case "leftControl": return control;
            case "rightControl": return control;
            case "backspace": return backspace;
            case "leftSystem": return system;
            case "rightSystem": return system;
            case "look": return mouse;
            case "scroll": return scrollWheel;
            case "leftButton": return leftClick;
            case "rightButton": return rightClick;
        }

        return null;
    }
}
