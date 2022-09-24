using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Core;
using HarmonyLib;
using System.Reflection;
using System.IO;
using UnityEngine.SceneManagement;

namespace WS_MapIDRec
{
    [BepInPlugin("com.nekoice.MapIDRecord", "MapIDRecord", "0.0.1")]
    public class WS_MapIDRec : BaseUnityPlugin
    {
        void Start()
        {
            Logger.LogInfo("Starting MapID Record");


            Logger.LogInfo("MapID Record is Enabled");
        }

        private void Trace_OnValueChange(object val)
        {
            Logger.LogInfo(val);
        }

        int lastmapidval = 0;
        int laststageval = 0;
        void Update()
        {
            if(StageControl.instance != null)
            {
                int val = StageControl.instance.MapID;
                int stageval = StageControl.instance.stageMapId;
                if (val != lastmapidval)
                {
                    Trace_OnValueChange(("MapID: ", val, "StagemapID: ", stageval, "SceneName:", SceneManager.GetActiveScene().name));
                }
                lastmapidval = val;
                laststageval = stageval;
            }
            
        }
        
    }
    public class ValueTrace<T>
    {
        public delegate void ValueChangeHandle(object val);
        public event ValueChangeHandle OnValueChange;
        object _target;
        string _valName;
        T _val;
        T _lastVal;
        public ValueTrace(object parentclass, string propertyname)
        {
            _target = parentclass;
            _valName = propertyname;
        }
        public void CheckValChange()
        {
            if (_target == null) return;
            var info = _target.GetType()
                              .GetRuntimeField(_valName);
            _val = (T)info.GetValue(_target);
            if (_val.GetHashCode() != _lastVal.GetHashCode())
            {
                OnValueChange(_val);
            }
            _lastVal = _val;
        }
    }
}
