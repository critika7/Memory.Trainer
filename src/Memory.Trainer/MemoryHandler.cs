﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LegoCityUnderCover.Trainer.Models;
using Memory;

namespace LegoCityUnderCover.Trainer
{
    public class MemoryHandler
    {
        private readonly ProcessInfo _processInfo;
        private readonly Mem _mem = new Mem();
        private bool _isAttached = false;

        public EventHandler<bool> AttachedToProcess;

        private readonly Dictionary<MemAddress, string> _freezingMemory = new Dictionary<MemAddress, string>();

        public MemoryHandler(ProcessInfo processInfo)
        {
            _processInfo = processInfo ?? throw new ArgumentNullException(nameof(processInfo));
            CheckIfAttached();
            WriteFreezedValues();
        }

        private bool CheckIfAttached()
        {
                _isAttached = _mem.OpenProcess(_processInfo.ProcessName);
                AttachedToProcess?.Invoke(null, true);
                return _isAttached;
        }

        #region GetValue

        public string GetValue(string name, string dataType)
        {
            if (!_isAttached)
                if (!CheckIfAttached())
                    return default;

            var memAdress = _processInfo.MemoryAddresses.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (memAdress == null)
                return default;

            switch (dataType)
            {
                case "int":
                    return GetIntValue(memAdress).ToString().Replace("E+","E");
                case "string":
                    return GetStringValue(memAdress);
                case "float":
                    return GetFloatValue(memAdress).ToString();
                case "byte":
                    return GetByteValue(memAdress).ToString();
                case "double":
                    return GetDoubleValue(memAdress).ToString();
            }

            return default;
        }

        private double GetDoubleValue(MemAddress memAdress)
        {
            var value = 0d;
            foreach (var address in memAdress.Addresses)
            {
                value = _mem.ReadDouble(address);
                if (value > 0f)
                    break;
            }

            return value;
        }

        private float GetFloatValue(MemAddress memAdress)
        {
            var value = 0f;
            foreach (var address in memAdress.Addresses)
            {
                value = _mem.ReadFloat(address);
                if (value > 0f)
                    break;
            }

            return value;
        }
        private int GetByteValue(MemAddress memAdress)
        {
            var value = 0;
            foreach (var address in memAdress.Addresses)
            {
                value = _mem.ReadByte(address);
                if (value > 0)
                    break;
            }

            return value;
        }

        private string GetStringValue(MemAddress memAddress)
        {
            string value = null;

            foreach (var address in memAddress.Addresses)
            {
                value = _mem.ReadString(address);
                if (!string.IsNullOrEmpty(value))
                    break;
            }

            return value;
        }

        public int GetIntValue(MemAddress memAddress)
        {
            var value = 0;

            foreach (var address in memAddress.Addresses)
            {
                value = _mem.ReadInt(address);
                if (value > 0)
                    break;
            }

            return value;
        }
        #endregion

        #region WriteValue

        public bool WriteValue(string name, string dataType, string value, bool? freeze = null)
        {
            if (!_isAttached)
                return false;

            var memAddress = _processInfo.MemoryAddresses.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (memAddress == null)
                return false;

            if (freeze == true)
                _freezingMemory.Add(memAddress,value);
            else if (freeze == false)
                _freezingMemory.Remove(memAddress);

            switch (dataType)
            {
                case "int":
                    return WriteIntValue(memAddress, value);
                case "string":
                    return WriteStringValue(memAddress, value);
                case "float":
                    return WriteFloatValue(memAddress, value);
                case "byte":
                    return WriteByteValue(memAddress, value);
                case "double":
                    return WriteDoubleValue(memAddress, value);
            }

            return false;
        }


        private async Task WriteFreezedValues()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        foreach (var memory in _freezingMemory)
                        {
                            WriteValue(memory.Key.Name, memory.Key.Type, memory.Value);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    await Task.Delay(50);
                }
            });
        }
        private bool WriteDoubleValue(MemAddress memAddress, string value)
        {
            if (!double.TryParse(value, out var test))
                return false;

            foreach (var address in memAddress.Addresses)
            {
                if (_mem.WriteMemory(address, "double", value))
                    return true;
            }

            return false;
        }


        public bool WriteIntValue(MemAddress memAddress, string value)
        {
            if (!int.TryParse(value, out var test))
                return false;

            foreach (var address in memAddress.Addresses)
            {
                if (_mem.WriteMemory(address, "int", value))
                    return true;
            }

            return false;
        }
        public bool WriteStringValue(MemAddress memAddress, string value)
        {

            foreach (var address in memAddress.Addresses)
            {
                if (_mem.WriteMemory(address, "string", value))
                    return true;
            }

            return false;
        }
        private bool WriteByteValue(MemAddress memAddress, string value)
        {
            if (!byte.TryParse(value, out var test))
                return false;

            foreach (var address in memAddress.Addresses)
            {
                if (_mem.WriteMemory(address, "byte", value))
                    return true;
            }

            return false;
        }
        private bool WriteFloatValue(MemAddress memAddress, string value)
        {
            if (!float.TryParse(value, out var test))
                return false;

            foreach (var address in memAddress.Addresses)
            {
                if (_mem.WriteMemory(address, "float", value))
                    return true;
            }

            return false;
        }
        #endregion
    }
}
