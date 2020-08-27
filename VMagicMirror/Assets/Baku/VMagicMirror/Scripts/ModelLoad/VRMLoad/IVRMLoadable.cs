﻿using System;

namespace Baku.VMagicMirror
{
    /// <summary> VRMロード元が提供すべきイベントの定義 </summary>
    public interface IVRMLoadable
    {
        /// <summary>VRMがロードされると呼び出されます。VrmLoadedより先に呼ばれます。</summary>
        event Action<VrmLoadedInfo> PreVrmLoaded;
        /// <summary>VRMがロードされると呼び出されます。</summary>
        event Action<VrmLoadedInfo> VrmLoaded;

        /// <summary>VRMがロードされると呼び出されます。VrmLoadedが完全に呼び終わったあとで呼ばれます。</summary>
        event Action<VrmLoadedInfo> PostVrmLoaded;
        
        /// <summary>VRMをアンロードするときに呼び出されます。</summary>
        event Action VrmDisposing;
    }
}
