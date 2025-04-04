﻿namespace FEntwumS.NetlistViewer.Types;

public class ElementClickedEventArgs : EventArgs
{
    public string? NodePath { get; }
    
    public ElementClickedEventArgs(string? nodePath) { NodePath = nodePath; }
}