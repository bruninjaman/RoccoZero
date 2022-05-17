// <copyright file="PriorityAttribute.cs" company="Ensage">
//    Copyright (c) 2018 Ensage.
// </copyright>

namespace Ensage.SDK.Menu.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class PriorityAttribute : Attribute
    {
        public PriorityAttribute(int orderNumber)
        {
            this.OrderNumber = orderNumber;
        }

        public int OrderNumber { get; }
    }
}
