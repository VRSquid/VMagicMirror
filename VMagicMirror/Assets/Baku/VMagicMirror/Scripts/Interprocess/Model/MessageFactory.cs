using System.Runtime.CompilerServices;
using UnityEngine;

namespace Baku.VMagicMirror
{
    class MessageFactory
    {
        //Singleton
        private static MessageFactory _instance;
        public static MessageFactory Instance
            => _instance ?? (_instance = new MessageFactory());
        private MessageFactory() { }

        //コマンド名 = メソッド名になる(NoArgもWithArgも共通)
        private static Message NoArg([CallerMemberName]string command = "")
            => new Message(command);

        private static Message WithArg(string content, [CallerMemberName]string command = "")
            => new Message(command, content);

        public Message SetUnityProcessId(int id) => WithArg(id.ToString());

        public Message RequestShowError(string data) => WithArg(data);
        
        public Message CloseConfigWindow() => NoArg();

        public Message SetCalibrationFaceData(string data) => WithArg(data);

        public Message SetBlendShapeNames(string v) => WithArg(v);

        public Message AutoAdjustResults(AutoAdjustParameters parameters)
            => WithArg(JsonUtility.ToJson(parameters));

        //NOTE: 冗長なパラメータが入ってるが、冗長な部分はWPF側に捨てさせる(どうせ既定値しか入ってない)
        public Message AutoAdjustEyebrowResults(AutoAdjustParameters parameters)
            => WithArg(JsonUtility.ToJson(parameters));
        
        public Message UpdateDeviceLayout(DeviceLayoutsData data) => WithArg(JsonUtility.ToJson(data));

        public Message ExtraBlendShapeClipNames(string names) => WithArg(names);

        public Message MidiNoteOn(int noteNumber) => WithArg($"{noteNumber}");

        public Message ExTrackerCalibrateComplete(string data) => WithArg(data);
        
        public Message ExTrackerSetPerfectSyncMissedClipNames(string data) => WithArg(data);
        
        #region VRoid

        public Message VRoidModelLoadCompleted(string modelInfo) => WithArg(modelInfo);
        public Message VRoidModelLoadCanceled() => NoArg();

        #endregion
    }
}