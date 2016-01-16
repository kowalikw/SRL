using SRL.Commons.Model.Base;

namespace SRL.Main.Messages
{
    internal class SetModelMessage<T> 
        where T : SvgSerializable
    {
        public T Model { get; }

        public SetModelMessage(T model)
        {
            Model = model;
        } 
    }
}
