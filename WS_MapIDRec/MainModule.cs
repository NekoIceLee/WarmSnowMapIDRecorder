﻿using System;
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
        delegate void CheckValueChangeHandle();
        event CheckValueChangeHandle GoCheckValue;
        void Start()
        {
            Logger.LogInfo("Starting MapID Record");
            InstanceFieldTrace MapIDTrace = new InstanceFieldTrace(typeof(StageControl), nameof(StageControl.instance), nameof(StageControl.instance.MapID));
            MapIDTrace.OnValueChange += MapIDTrace_OnValueChange;
            GoCheckValue += MapIDTrace.CheckValChange;

            Logger.LogInfo("MapID Record is Enabled");
        }

        private void MapIDTrace_OnValueChange(object val)
        {
            
        }

        void Update()
        {
            GoCheckValue();
        }
        
    }
    public class InstanceFieldTrace<T> : InstanceFieldTrace
    {
        new public delegate void ValueChangeHandle(T val);
        new public event ValueChangeHandle OnValueChange;
        T _val;
        T _lastVal;
        public InstanceFieldTrace(Type instanceType, string instanceName, string fieldName) : base(instanceType, instanceName, fieldName)
        {
            _instanceName = instanceName;
            _target = instanceType;
            _fieldName = fieldName;
            var instance = _target.GetField(_instanceName)
                                  .GetValue(null);
            _val = (T)instance.GetType()
                              .GetRuntimeField(fieldName)
                              .GetValue(instance);
            _lastVal = _val;
        }
        new public async void CheckValChange()
        {
            await Task.Run(() =>
            {
                var instance = _target.GetField(_instanceName)
                                      .GetValue(null);
                if (instance == null) return;
                _val = (T)instance.GetType()
                                  .GetRuntimeField(_fieldName)
                                  .GetValue(instance);
                if (_val.GetHashCode() != _lastVal.GetHashCode())
                {
                    OnValueChange(_val);
                    _lastVal = _val;
                }
            });
        }
    }
    public class InstanceFieldTrace
    {
        public delegate void ValueChangeHandle(object val);
        public event ValueChangeHandle OnValueChange;
        protected Type _target;
        protected string _fieldName;
        protected string _instanceName;
        object _val;
        object _lastVal;
        public InstanceFieldTrace(Type instanceType, string instanceName, string fieldName)
        {
            _instanceName = instanceName;
            _target = instanceType;
            _fieldName = fieldName;
            var instance = _target.GetField(_instanceName)
                                  .GetValue(null);
            _val = instance.GetType()
                              .GetRuntimeField(fieldName)
                              .GetValue(instance);
            _lastVal = _val;
        }

        public async void CheckValChange()
        {
            await Task.Run(() =>
            {
                var instance = _target.GetField(_instanceName)
                                      .GetValue(null);
                if (instance == null) return;
                _val = instance.GetType()
                                  .GetRuntimeField(_fieldName)
                                  .GetValue(instance);
                if (_val.GetHashCode() != _lastVal.GetHashCode())
                {
                    OnValueChange(_val);
                    _lastVal = _val;
                }
            });
        }
    }
}
