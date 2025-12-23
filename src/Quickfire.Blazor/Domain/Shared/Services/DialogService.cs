using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quickfire.Blazor.Domain.Shared.Services
{
    public class SurefireDialogService
    {
        private readonly Dictionary<string, FluentDialog> _dialogRefs = new Dictionary<string, FluentDialog>();
        private readonly Dictionary<string, bool> _dialogStates = new Dictionary<string, bool>();
        private readonly Dictionary<string, object> _dialogData = new Dictionary<string, object>();

        public void RegisterDialog(string dialogId, FluentDialog dialogRef)
        {
            if (!_dialogRefs.ContainsKey(dialogId))
            {
                _dialogRefs.Add(dialogId, dialogRef);
                _dialogStates[dialogId] = true; // Hidden by default
            }
            else
            {
                _dialogRefs[dialogId] = dialogRef;
            }
        }

        public void SetDialogData<T>(string dialogId, T data)
        {
            _dialogData[dialogId] = data;
        }

        public T GetDialogData<T>(string dialogId)
        {
            if (_dialogData.TryGetValue(dialogId, out var data) && data is T typedData)
            {
                return typedData;
            }
            return default;
        }

        public bool IsDialogHidden(string dialogId)
        {
            return _dialogStates.TryGetValue(dialogId, out var hidden) && hidden;
        }

        public void ShowDialog(string dialogId)
        {
            if (_dialogStates.ContainsKey(dialogId))
            {
                _dialogStates[dialogId] = false;
            }
        }

        public void HideDialog(string dialogId)
        {
            if (_dialogStates.ContainsKey(dialogId))
            {
                _dialogStates[dialogId] = true;
            }
        }

        public bool GetDialogState(string dialogId)
        {
            return _dialogStates.TryGetValue(dialogId, out var state) ? state : true;
        }

        public void SetDialogState(string dialogId, bool hidden)
        {
            _dialogStates[dialogId] = hidden;
        }
    }
}
