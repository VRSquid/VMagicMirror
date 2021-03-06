﻿using System.Collections.Generic;
using UnityEngine;
using Zenject;
using VRM;
using Baku.VMagicMirror.ExternalTracker;

namespace Baku.VMagicMirror
{
    public class ExternalTrackerFaceSwitchApplier : MonoBehaviour
    {
        private bool _hasModel = false;
        private VRMBlendShapeProxy _proxy;
        private FaceControlConfiguration _config;
        private ExternalTrackerDataSource _externalTracker;
        private EyeBonePostProcess _eyeBoneResetter;

        //NOTE: 毎回Keyを生成するとGCAlloc警察に怒られるので、回数が減るように書いてます
        private string _latestClipName = "";
        private BlendShapeKey _latestKey = default;
        
        [Inject]
        public void Initialize(
            IVRMLoadable vrmLoadable, 
            FaceControlConfiguration config,
            ExternalTrackerDataSource externalTracker,
            EyeBonePostProcess eyeBoneResetter
            )
        {
            _config = config;
            _externalTracker = externalTracker;
            _eyeBoneResetter = eyeBoneResetter;

            vrmLoadable.VrmLoaded += info =>
            {
                _proxy = info.blendShape;
                _hasModel = true;
            };
            
            vrmLoadable.VrmDisposing += () =>
            {
                _hasModel = false;
                _proxy = null;
            };
        }

        //zenjectのDI対象にする？か、このスクリプトにもリセット系の処理を書くか。
        private BlendShapeInitializer _initializer = null;

        private void Start()
        {
            _initializer = FindObjectOfType<BlendShapeInitializer>();
        }
        
        private void LateUpdate()
        {
            if (!_hasModel ||
                string.IsNullOrEmpty(_externalTracker.FaceSwitchClipName) || 
                _config.WordToMotionExpressionActive
                )
            {
                return;
            }
            
            _proxy.Apply();
            _initializer.InitializeBlendShapes(_externalTracker.KeepLipSyncForFaceSwitch);

            if (_latestClipName != _externalTracker.FaceSwitchClipName)
            {
                _latestKey = CreateKey(_externalTracker.FaceSwitchClipName);
                _latestClipName = _externalTracker.FaceSwitchClipName;
            }

            //NOTE: 最終的な適用はWordToMotionBlendShapeがやる。ので、ここではその前処理だけやってればよい
            _proxy.AccumulateValue(_latestKey, 1.0f);
            //表情を適用した = 目ボーンは正面向きになってほしい
            _eyeBoneResetter.ReserveReset = true;
        }

        private static BlendShapeKey CreateKey(string name) => 
            _presets.ContainsKey(name)
                ? new BlendShapeKey(_presets[name])
                : new BlendShapeKey(name);

        private static readonly Dictionary<string, BlendShapePreset> _presets = new Dictionary<string, BlendShapePreset>()
        {
            ["Blink"] = BlendShapePreset.Blink,
            ["Blink_L"] = BlendShapePreset.Blink_L,
            ["Blink_R"] = BlendShapePreset.Blink_R,

            ["LookLeft"] = BlendShapePreset.LookLeft,
            ["LookRight"] = BlendShapePreset.LookRight,
            ["LookUp"] = BlendShapePreset.LookUp,
            ["LookDown"] = BlendShapePreset.LookDown,
            
            ["A"] = BlendShapePreset.A,
            ["I"] = BlendShapePreset.I,
            ["U"] = BlendShapePreset.U,
            ["E"] = BlendShapePreset.E,
            ["O"] = BlendShapePreset.O,

            ["Joy"] = BlendShapePreset.Joy,
            ["Angry"] = BlendShapePreset.Angry,
            ["Sorrow"] = BlendShapePreset.Sorrow,
            ["Fun"] = BlendShapePreset.Fun,
        };
    }
}
