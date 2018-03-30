using System;
using System.Collections.Generic;
using UnityEngine;
using MediaToolkit.Model;
using MediaToolkit.Options;
using MediaToolkit;

public class MediaInfo : MonoBehaviour {

    int x;

    void Start()
    {
        x = 0;
        var inputFile = new MediaFile { Filename = @"C:\Users\shubh\Videos\VidTest\mitti.mp4" };
        var outputFile = new MediaFile { Filename = @"C:\Users\shubh\Videos\VidTest\mitti.jpg" };

        using (var engine = new Engine(@"C:\Users\shubh\Documents\Unity_Projects\GDC\VidCodec\Assets\packages\MediaToolkit.1.1.0.1\build\ffmpeg.exe"))
        {
            engine.GetMetadata(inputFile);

            // Saves the frame located on the 15th second of the video.
            var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(15) };
            engine.GetThumbnail(inputFile, outputFile, options);
        }
    }

    void OnDisable()
    {
       // writer.Close();
        Debug.Log(x);
    }
}
