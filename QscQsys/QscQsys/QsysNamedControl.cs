﻿using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysNamedControl
    {

        //Core
        QsysCore myCore;

        //Named Control
        private string controlName;
        public string ControlName { get { return this.controlName; } }
        private bool registered;
        public bool IsRegistered { get { return this.registered; } }
        private eControlType controlType;
        public eControlType ControlType { get { return this.ctrlType; } }

        //Internal Vars
        private double val = 0;
        public double Val { get { return val; } }
        private double valScaled = 0;
        public double ValScaled { get { return valScaled; } }
        private double lastSentVal = 0;
        private string sVal = "";
        public string S_Val { get { return sVal; } }
        private bool bVal = false;
        public bool b_Val { get { return bVal; } }
        private double max;
        private double min;
        private double rampTime;

        //Event
        public event EventHandler<QsysEventsArgs> QsysNamedControlEvent;


        /// <summary>
        /// Default constructor for a QsysNamedControl
        /// </summary>
        /// <param name="Name">The component name of the gain.</param>
        public QsysNamedControl(int _coreID, string _controlName, eControlType _controlType)
        {
            this.controlName = _controlName;
            this.ctrlType = _controlType;

            if (QsysCore.RegisterControl(cName))
            {
                QsysCore.Controls[cName].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Control_OnNewEvent);

                registered = true;
            }
            else
            {
                CrestronConsole.PrintLine("adding new control {0} failed..", Name);
            }
        }

        void Control_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            switch (ctrlType)
            {
                case eControlType.isValue:
                    val = e.Data;
                    valScaled = Math.Round(scale(val, min, max, 0, 65535));
                    sVal = e.SData;
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, cName, false, (int)val, "[[VAL]]"));
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, cName, false, (int)valScaled, "[[VAL-SCALED]]"));
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, cName, false, 0, sVal));
                    break;
                case eControlType.isButton:
                    bVal = Convert.ToBoolean(e.Data);
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, cName, bVal, 0, ""));
                    break;
                case eControlType.isTrigger:
                    bVal = false;
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, cName, false, 0, ""));
                    break;
                case eControlType.isString:
                    sVal = e.SData;
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, cName, false, 0, sVal));
                    break;
            }
        }


        public void SetValueScaled(double value)
        {
            if (ControlType != eControlType.isValue)
                return;
            double newRawVal = Math.Round(scale(value, 0, 65535, min, max), 2);
            if (newRawVal == lastSentVal) //avoid repeats
                return;
            lastSentVal = newRawVal;

            ControlSetDouble newValChange = new ControlSetDouble();
            newValChange.Params = new ControlSetValueDouble();
            newValChange.method = "Control.Set";
            newValChange.Params.Name = cName;
            newValChange.Params.Value = newRawVal;
            newValChange.Params.Ramp = rampTime;
            QsysCore.Enqueue(JsonConvert.SerializeObject(newValChange));
        }


        public void SetValueRaw(double value)
        {
            if (ControlType != eControlType.isValue)
                return;

            double newRawVal = value;
            if (newRawVal > max && value < min) //ensure within range
                return;
            if (lastSentVal == newRawVal) //avoid repeats
                return;
            lastSentVal = newRawVal;

            ControlSetDouble newValChange = new ControlSetDouble();
            newValChange.Params = new ControlSetValueDouble();
            newValChange.method = "Control.Set";
            newValChange.Params.Name = cName;
            newValChange.Params.Value = newRawVal;
            newValChange.Params.Ramp = rampTime;
            QsysCore.Enqueue(JsonConvert.SerializeObject(newValChange));
        }

        public void SetState(bool value)
        {
            if (ControlType != eControlType.isButton)
                return;

            ControlSetBool newStateChange = new ControlSetBool();
            newStateChange.Params = new ControlSetValueBool();
            newStateChange.method = "Control.Set";
            newStateChange.Params.Name = cName;
            newStateChange.Params.Value = value;
            QsysCore.Enqueue(JsonConvert.SerializeObject(newStateChange));
        }

        public void SetStateToggle()
        {
            if (ControlType != eControlType.isButton)
                return;

            bVal = !bVal;
            ControlSetBool newStateChange = new ControlSetBool();
            newStateChange.Params = new ControlSetValueBool();
            newStateChange.method = "Control.Set";
            newStateChange.Params.Name = cName;
            newStateChange.Params.Value = bVal;
            QsysCore.Enqueue(JsonConvert.SerializeObject(newStateChange));
        }

        public void Trigger()
        {
            if (ControlType != eControlType.isTrigger)
                return;

            ControlSetBool newTriggerChange = new ControlSetBool();
            newTriggerChange.Params = new ControlSetValueBool();
            newTriggerChange.method = "Control.Set";
            newTriggerChange.Params.Name = cName;
            newTriggerChange.Params.Value = true;
            QsysCore.Enqueue(JsonConvert.SerializeObject(newTriggerChange));
        }

        public void SetString(string value)
        {
            if (ControlType != eControlType.isString)
                return;

            ControlSetString newStringChange = new ControlSetString();
            newStringChange.Params = new ControlSetValueString();
            newStringChange.method = "Control.Set";
            newStringChange.Params.Name = cName;
            newStringChange.Params.Value = value;
            QsysCore.Enqueue(JsonConvert.SerializeObject(newStringChange));
        }

        /// <summary>
        /// Sets the QSys ramp time for the gain
        /// </summary>
        /// <param name="time"></param>
        public void RampTimeMS(double time)
        {
            rampTime = time / 1000; //ms to sec
        }

        public void SetMinMax(double newMin, double newMax)
        {
            min = newMin;
            max = newMax;
        }
        public void SetMinMaxViaString(string newMin, string newMax)
        {
            if (ControlType != eControlType.isValue)
                return;
            min = Convert.ToDouble(newMin);
            max = Convert.ToDouble(newMax);
        }

        private double scale(double A, double A1, double A2, double Min, double Max)
        {
            double percentage = (A - A1) / (A1 - A2);
            return (percentage) * (Min - Max) + Min;
        }

    }

    public enum eControlType
    {
        isValue = 0,
        isButton = 1,
        isTrigger = 2,
        isString = 3
    }
}