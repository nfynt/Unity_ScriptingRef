using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Nfynt
{
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    public class WinFileBrowser : MonoBehaviour
    {
        public class OpenDialog
        {

            //Link to specify system function Open file dialog
            [DllImport( "Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto )]
            public static extern bool GetOpenFileName( [In, Out] OpenFileName ofn );

            public static bool GetOFN( [In, Out] OpenFileName ofn )
            {
                return GetOpenFileName( ofn );
            }

            //Link to specify system function and save as dialog box
            [DllImport( "Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto )]
            public static extern bool GetSaveFileName( [In, Out] OpenFileName ofn );

            public static bool GetSFN( [In, Out] OpenFileName ofn )
            {
                return GetSaveFileName( ofn );
            }
        }

        [StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto )]
        public class OpenFileName
        {
            public int structSize = 0;
            public IntPtr dlgOwner = IntPtr.Zero;
            public IntPtr instance = IntPtr.Zero;
            public String filter = null;
            public String customFilter = null;
            public int maxCustFilter = 0;
            public int filterIndex = 0;
            public String file = null;
            public int maxFile = 0;
            public String fileTitle = null;
            public int maxFileTitle = 0;
            public String initialDir = null;
            public String title = null;
            public int flags = 0;
            public short fileOffset = 0;
            public short fileExtension = 0;
            public String defExt = null;
            public IntPtr custData = IntPtr.Zero;
            public IntPtr hook = IntPtr.Zero;
            public String templateName = null;
            public IntPtr reservedPtr = IntPtr.Zero;
            public int reservedInt = 0;
            public int flagsEx = 0;
        }

        public enum FileType
        {
            None,
            Ifc
        }

        /// <summary>
        /// Open a window to select a file
        /// </summary>
        /// <param name="fileType">The file type to be selected</param>
        /// <param name="openPath">Set the default path for opening</param>
        /// <returns>Return to the path of the selected file</returns>
        public static string OpenFileDialog( FileType fileType, string openPath = null )
        {
            OpenFileName ofn = new OpenFileName();
            ofn.structSize = Marshal.SizeOf( ofn );
            string fliter = string.Empty;

            switch ( fileType )
            {
                case FileType.None:
                    fliter = "All Files(*.*)\0*.*\0\0";

                    break;

                case FileType.Ifc:
                    fliter = "IFC Files\0*.ifc;*.IFC\0";

                    break;
            }

            ofn.filter = fliter; //Set the type to be selected
            ofn.file = new string( new char[256] );
            ofn.maxFile = ofn.file.Length;
            ofn.fileTitle = new string( new char[64] );
            ofn.maxFileTitle = ofn.fileTitle.Length;

            if ( string.IsNullOrEmpty( openPath ) )
                ofn.initialDir = System.Environment.CurrentDirectory; //The default path opened is changed by itself
            else
                ofn.initialDir = openPath;

            ofn.title = "Select File"; //Title Customize and change by yourself

            //0x00000200 Set the user to select multiple files Do not use, temporarily did not find a solution to resolve multiple file paths
            //The specific meaning is to view the reference link given
            ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

            if ( OpenDialog.GetOFN( ofn ) )
            {
                Debug.Log( ofn.file );

                return ofn.file;
            }

            return null;
        }
    }
#endif
}