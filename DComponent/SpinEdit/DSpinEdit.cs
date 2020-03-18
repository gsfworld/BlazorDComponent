using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DComponent
{
    public partial class DSpinEdit
    {
        [Parameter]
        public string Id { get; set; } = $"DC{Guid.NewGuid().ToString().Replace("-","")}";
        [Parameter]
        public decimal Inc { get; set; } = 1;
        [Parameter]
        public string Width { get; set; } = "100%";
        [Parameter]
        public bool Disable { get; set; } = false;
        [Parameter]
        public decimal MaxValue { get; set; } = 20000;
        [Parameter]
        public decimal MInValue { get; set; } = 0;
        [Parameter]
        public decimal CurrentValue
        {
            get => Convert.ToDecimal(_value);
            set
            {
                _value = SetRange(value).ToString();
            }
        }
        [Parameter]
        public EventCallback CurrentValueChanged { get; set; }
        private string _value { get; set; }

        private decimal SetFormate(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            if (Regex.Match(value, SpinValueType.sDecimal).Success)
                return Convert.ToDecimal(value);
            return 0;
        }

        private decimal SetRange(decimal value)
        {
            if (value < MInValue)
                return MInValue;
            if (value > MaxValue)
                return MaxValue;
            return value;
        }
    }
}
