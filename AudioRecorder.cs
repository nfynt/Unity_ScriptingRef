// Records speaker and microphone audio into two individual wav files.
// Check WaveMixer.cs(https://github.com/nfynt/ScriptsCollection/blob/master/WavMixer.cs) to merge wav file and get mp3 as final output.

using System;
using System.Collections.Generic;
using System.Text;
using CSCore;
using CSCore.Codecs.WAV;
using CSCore.SoundIn;
using System.Threading;
using System.IO;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using NAudio.MediaFoundation;
using System.Linq;

namespace Shubham.AudRecLib
{
    public class AudioRecorder
    {
        public static CSCore.SoundIn.WasapiCapture spkAud;
        public static NAudio.Wave.WaveInEvent micAud;
        public static WaveFileWriter wfw;
        private static string _micLoc;
        private static string _spkLoc;
        private static string _finLoc;

        private static bool stopRec;

        private static Thread recT;

        public static void StartRecording(string spkLoc, string micLoc)
        {
            stopRec = false;

            _spkLoc = spkLoc;
            _micLoc = micLoc;
            //_finLoc = finalLoc;

            recT = new Thread(new ThreadStart(RecThread));
            recT.Start();
        }

        public static void RecThread()
        {
            micAud = new NAudio.Wave.WaveInEvent();
            //micAud.WaveFormat = new NAudio.Wave.WaveFormat(44100, 1);
            //micAud.DataAvailable += MicAud_DataAvailable;
            //micAud.RecordingStopped += MicAud_RecordingStopped;
            //// micAud.DataAvailable += (s, capData) => wfw.Write(capData.Buffer, 0, capData.BytesRecorded);
            //wfw = new WaveFileWriter(_micLoc, micAud.WaveFormat);
            //micAud.StartRecording();

            using (spkAud = new CSCore.SoundIn.WasapiLoopbackCapture())
            {
                spkAud.Initialize();

                micAud.WaveFormat = new NAudio.Wave.WaveFormat(spkAud.WaveFormat.SampleRate, spkAud.WaveFormat.Channels);
                micAud.DataAvailable += MicAud_DataAvailable;
                micAud.RecordingStopped += MicAud_RecordingStopped;
                // micAud.DataAvailable += (s, capData) => wfw.Write(capData.Buffer, 0, capData.BytesRecorded);
                wfw = new WaveFileWriter(_micLoc, micAud.WaveFormat);
                micAud.StartRecording();

                using (var w = new WaveWriter(_spkLoc, spkAud.WaveFormat))
                {
                    spkAud.DataAvailable += (s, capData) => w.Write(capData.Data, capData.Offset, capData.ByteCount);
                    spkAud.Start();

                    while (!stopRec) ;

                    spkAud.Stop();
                    micAud.StopRecording();                    
                }
            }
        }

        public static void StopRecording()
        {
            stopRec = true;
           // MergeWav(_spkLoc, _micLoc, _finLoc);
        }

        private static void MicAud_RecordingStopped(object sender, NAudio.Wave.StoppedEventArgs e)
        {
            if (micAud != null)
            {
                micAud.Dispose();
                micAud = null;
            }

            if (wfw != null)
            {
                wfw.Dispose();
                wfw = null;
            }
        }

        private static void MicAud_DataAvailable(object sender, WaveInEventArgs e)
        {
            wfw.Write(e.Buffer, 0, e.BytesRecorded);
            wfw.Flush();
        }

        public static void MergeWav()
        {
            string inp1, inp2, output;
            inp1 = inp2 = output = "NA";

            if (inp1 == "NA")
            {
                inp1 = _spkLoc;
                inp2 = _micLoc;
                output = _finLoc;
            }
            var paths = new[] {
                            inp1,
                            inp2
                        };

            // open all the input files
            var readers = paths.Select(f => new NAudio.Wave.WaveFileReader(f)).ToArray();

            // choose the sample rate we will mix at
            var maxSampleRate = readers.Max(r => r.WaveFormat.SampleRate);

            // create the mixer inputs, resampling if necessary
            var mixerInputs = readers.Select(r => r.WaveFormat.SampleRate == maxSampleRate ?
                r.ToSampleProvider() :
                new MediaFoundationResampler(r, NAudio.Wave.WaveFormat.CreateIeeeFloatWaveFormat(maxSampleRate, r.WaveFormat.Channels)).ToSampleProvider());

            // create the mixer
            var mixer = new NAudio.Wave.SampleProviders.MixingSampleProvider(mixerInputs);

            // write the mixed audio to a 16 bit WAV file
            WaveFileWriter.CreateWaveFile16(output, mixer);

            // clean up the readers
            foreach (var reader in readers)
            {
                reader.Dispose();
            }
        }
    }

}
