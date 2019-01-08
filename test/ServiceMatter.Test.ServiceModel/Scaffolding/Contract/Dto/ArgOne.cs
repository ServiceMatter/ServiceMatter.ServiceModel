using System;
using System.Collections.Generic;

namespace Service.Matter.Test.ServiceModel.Scaffolding.Contract
{
    public class ArgOne : ArgValue
    {
    }

    public class ArgValue
    {
        public ArgValue()
        {
            Value = Guid.NewGuid();
        }

        Guid Value { get; set; }

        public override bool Equals(object obj)
        {
            var value = obj as ArgValue;
            return value != null &&
                   Value.Equals(value.Value);
        }

        public override int GetHashCode()
        {
            return -1937169414 + EqualityComparer<Guid>.Default.GetHashCode(Value);
        }
    }
}