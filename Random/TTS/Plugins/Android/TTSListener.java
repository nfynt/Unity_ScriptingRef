package com.unity3d.player;

import android.speech.tts.TextToSpeech;

public class TTSListener implements TextToSpeech.OnInitListener {
    private String unityObjName;

    public TTSListener(String objName){
        unityObjName = objName;
    }

    @Override
    public void onInit(int status){
        if(status == TextToSpeech.SUCCESS){
            UnityPlayer.UnitySendMessage(unityObjName,"OnTTSInitialized","SUCCESS");
        }else{
            String errorMessage = "FAILED: ";
            switch (status) {
                case TextToSpeech.ERROR:
                    errorMessage += "ERROR";
                    break;
                case TextToSpeech.ERROR_NETWORK:
                    errorMessage += "ERROR_NETWORK";
                    break;
                case TextToSpeech.ERROR_NETWORK_TIMEOUT:
                    errorMessage += "ERROR_NETWORK_TIMEOUT";
                    break;
                case TextToSpeech.ERROR_NOT_INSTALLED_YET:
                    errorMessage += "ERROR_NOT_INSTALLED_YET";
                    break;
                case TextToSpeech.ERROR_OUTPUT:
                    errorMessage += "ERROR_OUTPUT";
                    break;
                case TextToSpeech.ERROR_SERVICE:
                    errorMessage += "ERROR_SERVICE";
                    break;
                case TextToSpeech.ERROR_SYNTHESIS:
                    errorMessage += "ERROR_SYNTHESIS";
                    break;
                default:
                    errorMessage += "UNKNOWN_ERROR";
            }
            UnityPlayer.UnitySendMessage(unityObjName, "OnTTSInitialized", errorMessage);
        }
    }
}


/*
 __  _ _____   ____  _ _____  
|  \| | __\ `v' /  \| |_   _| 
| | ' | _| `. .'| | ' | | |   
|_|\__|_|   !_! |_|\__| |_|
 

*/