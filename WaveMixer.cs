//Mixes two wav file to out single mp3 file.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using CSCore;
//using CSCore.Codecs.WAV;
//using CSCore.SoundIn;
using System.Threading;
using System.IO;
using NAudio.Wave;
using System.Diagnostics;

namespace AudRec
{
    class WavMixer
    {
       // public static CSCore.SoundIn.WasapiCapture spkAud;
        //public static NAudio.Wave.WaveInEvent micAud;
        //public static WaveFileWriter wfw;
        private string _micLoc;
        private string _spkLoc;
        private string _finLoc;
        private string _lameLoc;
        private string _destLoc;

        //args[0]: spk file location
        //args[1]: mic file location
        //args[2]: final file location

        static void Main(string[] args)
        {
            Program pp = new Program();
            if (args.Length == 5 && args[0] == "merge2wav")
            {
                pp._spkLoc = args[1];
                pp._micLoc = args[2];
                pp._finLoc = args[3];
                pp._lameLoc = args[4];

                pp.MergeWav(pp._spkLoc, pp._micLoc, pp._finLoc, pp._lameLoc);

            }
            else if (args.Length == 4 && args[0] == "convert2wav")
            {
                //Console.WriteLine("Number of parameters: " + args.Length);
                pp._finLoc = args[1];
                pp._destLoc = args[2];
                pp._lameLoc = args[3];
                //string wavOutput = Path.Combine(Path.GetDirectoryName(pp._finLoc), Path.GetFileNameWithoutExtension(pp._finLoc)) + ".wav";
                pp.LameMp3ToWav(pp._finLoc, pp._destLoc, pp._lameLoc);
            }
            else if (args.Length == 4 && args[0] == "convert2mp3")
            {
                pp._finLoc = args[1];
                pp._destLoc = args[2];
                pp._lameLoc = args[3];
                pp.LameWavToMp3(pp._finLoc, pp._destLoc, pp._lameLoc);
            }
            else
            {
                Console.WriteLine("Invalid parameters: " + args.Length);
                foreach (string s in args)
                    Console.WriteLine(s);
                return;
                pp._spkLoc = "spk.wav";
                pp._micLoc = "mic.wav";
                pp._finLoc = "fin.wav";
            }

            //Console.WriteLine("Starting the mic and speaker recording");

            //StartRecording();

           // while (true) ;
        }
        /*
        public static void StartRecording()
        {
            //AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            micAud = new NAudio.Wave.WaveInEvent();
            micAud.WaveFormat = new NAudio.Wave.WaveFormat(44100, 2);
            micAud.DataAvailable += MicAud_DataAvailable;
            micAud.RecordingStopped += MicAud_RecordingStopped;
            // micAud.DataAvailable += (s, capData) => wfw.Write(capData.Buffer, 0, capData.BytesRecorded);
            wfw = new WaveFileWriter(_micLoc, micAud.WaveFormat);
            micAud.StartRecording();

            using (spkAud = new CSCore.SoundIn.WasapiLoopbackCapture())
            {
                spkAud.Initialize();
                using (var w = new WaveWriter(_spkLoc, spkAud.WaveFormat))
                {
                    spkAud.DataAvailable += (s, capData) => w.Write(capData.Data, capData.Offset, capData.ByteCount);
                    spkAud.Start();

                    Debug.WriteLine("Sleeping thread");
                    Thread.Sleep(3000);

                    //spkAud.Dispose();
                    spkAud.Stop();
                    micAud.StopRecording();
                }
            }
        }

        /*
        static void OnProcessExit(object sender, EventArgs e)
        {
            Debug.WriteLine("I'm out of here");

            Debug.WriteLine("Stopping the mic recording");
            MergeWav(_spkLoc, _micLoc, _finLoc);
        }
        //

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
        */

        public void MergeWav(string inp1, string inp2, string output, string lamLoc)
        {
            if (!File.Exists(inp1))
                return;
            Console.WriteLine(inp1);
            Console.WriteLine(inp2);
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


            string mp3output = Path.Combine(Path.GetDirectoryName(output), Path.GetFileNameWithoutExtension(output)) + ".mp3";
            //Console.WriteLine("output loc: " + mp3output);
            LameWavToMp3(output, mp3output, lamLoc);

            //????????????????????????????Uncomment when taking the final build
            //File.Delete(inp1);
            //File.Delete(inp2);

            //Console.WriteLine("Done converting!");

            //Console.ReadLine();
        }

        public void LameWavToMp3(string wavFile, string outmp3File, string lameLoc)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = lameLoc;
                psi.Arguments = "-V2 " + wavFile + " " + outmp3File;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                Process p = Process.Start(psi);
                p.WaitForExit();

               // Console.WriteLine("Finished converting to mp3...");
                File.Delete(wavFile);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to execute lame\n:"+lameLoc);
                Console.WriteLine(wavFile);
                Console.WriteLine(outmp3File);
                Console.WriteLine(e.ToString());
            }
        }

        public void LameMp3ToWav(string mp3File, string outWavFile, string lameLoc)
        {
            Console.WriteLine(mp3File);
            Console.WriteLine(outWavFile);
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = lameLoc;
                psi.Arguments = "-V2 " + mp3File + " " + outWavFile;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                Process p = Process.Start(psi);
                p.WaitForExit();

                //Console.WriteLine("Finished converting to mp3...");
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to execute lame");
                Console.WriteLine(mp3File);
                Console.WriteLine(outWavFile);
                Console.WriteLine(e.ToString());
            }
        }
    }
}
