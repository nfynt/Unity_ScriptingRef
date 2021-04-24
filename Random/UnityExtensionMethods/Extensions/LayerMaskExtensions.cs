using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskExtensions
{
    public static bool MaskContains(this LayerMask mask, int layerNumber)
    {
        return mask == (mask | (1 << layerNumber));
    }
}