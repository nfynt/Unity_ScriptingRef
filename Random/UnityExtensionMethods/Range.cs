using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Range<T>
{
    public T Min;
    public T Max;

    public abstract T RandomInRange();
}

[Serializable]
public class RangeInt : Range<int> {
    public override int RandomInRange()
    {
        if (Min <= Max) return UnityEngine.Random.Range(Min, Max);
        return UnityEngine.Random.Range(Max, Min);
    }
}

[Serializable]
public class RangeFloat : Range<float> {
    public RangeFloat()
    {
        Min = 0;
        Max = 1;
    }

    public override float RandomInRange()
    {
        if (Min <= Max) return UnityEngine.Random.Range(Min, Max);
        return UnityEngine.Random.Range(Max, Min);
    }
}

[Serializable]
public class RangeColor : Range<Color> {
    public RangeColor()
    {
        Min = Color.HSVToRGB(0f, 0f, 0f);
        Max = Color.HSVToRGB(0.999f, 1f, 1f);
    }

    public override Color RandomInRange()
    {
        float minH, minS, minV, maxH, maxS, maxV;
        Color.RGBToHSV(Min, out minH, out minS, out minV);
        Color.RGBToHSV(Max, out maxH, out maxS, out maxV);

        return UnityEngine.Random.ColorHSV(minH, maxH, minS, maxS, minV, maxV);
    }
}