using System.Collections.Generic;

namespace Service.Matter.Test.ServiceModel.Scaffold.Contract
{
    public class OperationARequestDto
    {
        public string In { get; set; }

        public override bool Equals(object obj)
        {
            var dto = obj as OperationARequestDto;
            return dto != null &&
                   In == dto.In;
        }

        public override int GetHashCode()
        {
            return -855294866 + EqualityComparer<string>.Default.GetHashCode(In);
        }
    }
}