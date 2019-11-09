﻿using UnityEngine;
using UniRx;
using Zenject;

namespace Baku.VMagicMirror
{
    /// <summary> モーション関係で、操作ではなく設定値を受け取るレシーバクラス </summary>
    public class MotionSettingReceiver : MonoBehaviour
    {
        //Word to Motionの専用入力に使うデバイスを指定する定数値
        private const int DeviceTypeNone = 0;
        private const int DeviceTypeGamepad = 1;
        private const int DeviceTypeKeyboard = 2;

        [Inject] private ReceivedMessageHandler _handler = null;

        [SerializeField] private GamepadBasedBodyLean gamePadBasedBodyLean = null;
        [SerializeField] private SmallGamepadHandIKGenerator smallGamepadHandIk = null;

        [SerializeField] private HandIKIntegrator handIkIntegrator = null;

        [SerializeField] private HeadIkIntegrator headIkIntegrator = null;

        [SerializeField] private IkWeightCrossFade ikWeightCrossFade = null;
        
        [SerializeField] private StatefulXinputGamePad gamePad = null;

        private void Start()
        {
            _handler.Commands.Subscribe(message =>
            {
                switch (message.Command)
                {
                    case MessageCommandNames.EnableHidArmMotion:
                        handIkIntegrator.EnableHidArmMotion = message.ToBoolean();
                        ikWeightCrossFade.ForceStopHandIk = !message.ToBoolean();
                        break;
                    case MessageCommandNames.LengthFromWristToPalm:
                        SetLengthFromWristToPalm(message.ParseAsCentimeter());
                        break;
                    case MessageCommandNames.LengthFromWristToTip:
                        SetLengthFromWristToTip(message.ParseAsCentimeter());
                        break;
                    case MessageCommandNames.HandYOffsetBasic:
                        SetHandYOffsetBasic(message.ParseAsCentimeter());
                        break;
                    case MessageCommandNames.HandYOffsetAfterKeyDown:
                        SetHandYOffsetAfterKeyDown(message.ParseAsCentimeter());
                        break;
                    case MessageCommandNames.EnablePresenterMotion:
                        handIkIntegrator.EnablePresentationMode = message.ToBoolean();
                        break;
                    case MessageCommandNames.PresentationArmMotionScale:
                        handIkIntegrator.Presentation.PresentationArmMotionScale = message.ParseAsPercentage();
                        break;
                    case MessageCommandNames.PresentationArmRadiusMin:
                        handIkIntegrator.Presentation.PresentationArmRadiusMin = message.ParseAsCentimeter();
                        break;
                    case MessageCommandNames.LookAtStyle:
                        headIkIntegrator.SetLookAtStyle(message.Content);
                        break;
                    case MessageCommandNames.EnableGamepad:
                        gamePad.enabled = message.ToBoolean();
                        break;
                    case MessageCommandNames.GamepadLeanMode:
                        gamePadBasedBodyLean.SetGamepadLeanMode(message.Content);
                        smallGamepadHandIk.SetGamepadLeanMode(message.Content);
                        break;
                    case MessageCommandNames.GamepadLeanReverseHorizontal:
                        gamePadBasedBodyLean.ReverseGamepadStickLeanHorizontal = message.ToBoolean();
                        smallGamepadHandIk.ReverseGamepadStickLeanHorizontal = message.ToBoolean();
                        break;
                    case MessageCommandNames.GamepadLeanReverseVertical:
                        gamePadBasedBodyLean.ReverseGamepadStickLeanVertical = message.ToBoolean();
                        smallGamepadHandIk.ReverseGamepadStickLeanVertical = message.ToBoolean();
                        break;
                    case MessageCommandNames.SetDeviceTypeToStartWordToMotion:
                        SetDeviceTypeForWordToMotion(message.ToInt());
                        break;
                }
            });
        }

        private void SetDeviceTypeForWordToMotion(int deviceType)
        {
            switch (deviceType)
            {
                case DeviceTypeNone:
                    gamePadBasedBodyLean.UseGamepadForWordToMotion = false;
                    handIkIntegrator.UseGamepadForWordToMotion = false;
                    handIkIntegrator.UseKeyboardForWordToMotion = false;
                    break;
                case DeviceTypeGamepad:
                    gamePadBasedBodyLean.UseGamepadForWordToMotion = true;
                    handIkIntegrator.UseGamepadForWordToMotion = true;
                    handIkIntegrator.UseKeyboardForWordToMotion = false;
                    break;
                case DeviceTypeKeyboard:
                    gamePadBasedBodyLean.UseGamepadForWordToMotion = false;
                    handIkIntegrator.UseGamepadForWordToMotion = false;
                    handIkIntegrator.UseKeyboardForWordToMotion = true;
                    break;
                default:
                    break;
            }
        }

        //以下については適用先が1つじゃないことに注意

        private void SetLengthFromWristToTip(float v)
        {
            handIkIntegrator.Presentation.HandToTipLength = v;
            handIkIntegrator.Typing.HandToTipLength = v;
        }
        
        private void SetLengthFromWristToPalm(float v)
        {
            handIkIntegrator.MouseMove.HandToPalmLength = v;
        }
        
        private void SetHandYOffsetBasic(float offset)
        {
            handIkIntegrator.Typing.YOffsetAlways = offset;
            handIkIntegrator.MouseMove.YOffset = offset;
        }

        private void SetHandYOffsetAfterKeyDown(float offset)
        {
            handIkIntegrator.Typing.YOffsetAfterKeyDown = offset;
        }
    }
}
