//Encodes the streaming textures as mp4 video, using FFMPEG.exe
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Threading;

namespace Shubham.FFMPEG
{

    public delegate void FinishedEncoding(string message, bool success);
    public delegate void LogMessages(string message);

    public class Codec
    {
        public int _fps;
        public int _width, _height;
        public string _outVidLoc;
        public string _libLoc;

        private Queue<Byte[]> _frames;
        public int _frameCnt;
        private string _tempLoc;
        private bool _isJpeg;
        private Image _tempImage;
        //private bool startWriting;
        private bool _stopWriting;
        private Thread _writingThread;

        public event FinishedEncoding finishedEncoding;
        public event LogMessages logMessages;
        public int _bufferLen;

        public Codec(string outVidLoc, int fps, bool isJPEG, string libPath)
        {
            _fps = fps;
            _outVidLoc = outVidLoc;

            _frames = new Queue<byte[]>();
            _tempLoc = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TempFrames\\";
            _isJpeg = isJPEG;
            _libLoc = libPath;

            if (File.Exists(_outVidLoc))
            {
                if (logMessages != null)
                    logMessages(_outVidLoc + " Output video already exists.\nRemoving...");
                File.Delete(_outVidLoc);
            }
            

            _frameCnt = 0;
            if (Directory.Exists(_tempLoc))
                Directory.Delete(_tempLoc, true);

            Directory.CreateDirectory(_tempLoc);

            GC.AddMemoryPressure((long)35000);

            _stopWriting = false;
            _writingThread = new Thread(new ThreadStart(WritingThread));
            _writingThread.Start();
        }

        void WritingThread()
        {
            while(!_stopWriting)
            {
                if (_frames.Count > 0)
                {
                    if (_isJpeg)
                        DequeueJpgFrame();
                    else
                        DequeuePngFrame();
                }
                _bufferLen = _frames.Count;
            }
        }

        public void AddJpgFrame(byte[] frame)
        {
            _frames.Enqueue(frame);
        }

        void SaveJpgFrames()
        {

            //for(int i=0;i<20;i++)
            foreach(Byte[] b in _frames)
            {
                using (Image image = Image.FromStream(new MemoryStream(b)))
                {
                    image.Save(_tempLoc + "test_"+_frameCnt.ToString()+".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                _frameCnt++;
            }
            _frames.Clear();
        }

        void DequeueJpgFrame()
        {
            Byte[] b = _frames.Dequeue();
            using (Image image = Image.FromStream(new MemoryStream(b)))
            {
                image.Save(_tempLoc + "test_" + _frameCnt.ToString() + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            _frameCnt++;
            _frames.TrimExcess();
        }

        void SavePngFrames()
        {
            //_frameCnt = 0;
            //if (Directory.Exists(_tempLoc))
            //    Directory.Delete(_tempLoc, true);

            //Directory.CreateDirectory(_tempLoc);

            //for (int i = 0; i < 20; i++)
                foreach (Byte[] b in _frames)
                {
                    using (Image image = Image.FromStream(new MemoryStream(b)))
                    {
                        image.Save(_tempLoc + "test_" + _frameCnt.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    }
                    _frameCnt++;
                }
            _frames.Clear();
        }

        void DequeuePngFrame()
        {
            Byte[] b = _frames.Dequeue();
            using (Image image = Image.FromStream(new MemoryStream(b)))
            {
                image.Save(_tempLoc + "test_" + _frameCnt.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
            _frameCnt++;
            _frames.TrimExcess();

        }

        /// <summary>
        /// Would not be used most likely.
        /// </summary>
        /// <param name="frame">byte array of current frame</param>
        public void AddPngFrame(byte[] frame)
        {
            _frames.Enqueue(frame);
        }

        public void EncodeVideo()
        {
            _stopWriting = true;

            string startTime = DateTime.Now.ToString("hh:mm:ss");

            if (_isJpeg)
                SaveJpgFrames();
            else
                SavePngFrames();

            if (logMessages!=null)
                logMessages("Starting encoding:");

            try
            {
                Process proc = new Process();
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.RedirectStandardOutput = true;

                //proc.StartInfo.FileName = Directory.GetCurrentDirectory() + "\\ffmpeg.exe";
                proc.StartInfo.FileName = _libLoc + "\\ffmpeg.exe";
                if (_isJpeg)
                    proc.StartInfo.Arguments = "-framerate " + _fps.ToString() + " -i \"" + _tempLoc + "test_%d.jpg\" -c:v libx264 -pix_fmt yuv420p " + _outVidLoc;
                else
                    proc.StartInfo.Arguments = "-framerate " + _fps.ToString() + " -i \"" + _tempLoc + "test_%d.png\" -c:v libx264 -pix_fmt yuv420p " + _outVidLoc;
                //proc.StartInfo.Arguments = "-framerate 10 -i \"D:\\Shubham_Data\\Img\\img%d.jpg\" -c:v libx264 -pix_fmt yuv420p " + outLoc;
                //GLOB: ffmpeg -framerate 10 -pattern_type glob -i "*.png" out.mkv
                //Ind: ffmpeg -framerate 10 -start_number 100 -i 'img-%03d.jpeg' out.mkvprint

                proc.EnableRaisingEvents = true;

                proc.Start();
                proc.WaitForExit();

                //Console.WriteLine("Output from ffmpeg:\n" + proc.StandardOutput.ReadToEnd());
                if (finishedEncoding != null)
                    finishedEncoding("Finished encoding", true);
            }
            catch (Exception e)
            {
                //Console.WriteLine("Encode Error: " + e.Message);
                if (finishedEncoding != null)
                    finishedEncoding("Failed encoding: " + e.Message, false);
            }


            string endTime = DateTime.Now.ToString("hh:mm:ss");
            if (logMessages != null)
            {
                logMessages("Finished encoding:");
                logMessages("Frames: " + _frameCnt.ToString());
                logMessages("Duration: " + ((float)_frameCnt / (float)_fps).ToString() + "secs");
                logMessages("Encode duration: \nstart: " + startTime + "\nend: " + endTime + "\nTotal: " + DateTime.Parse(endTime).Subtract(DateTime.Parse(startTime)).ToString());
            }
            DiscardFrames();
        }

        public void DiscardFrames()
        {
            _frames.Clear();
            if (logMessages != null)
                logMessages("Flushed all frame data");
        }
    }
}
