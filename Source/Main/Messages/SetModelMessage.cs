using SRL.Commons.Model.Base;

namespace SRL.Main.Messages
{
    /// <summary>
    /// Passes a model object to registered classes.
    /// </summary>
    /// <typeparam name="T">Model type.</typeparam>
    internal class SetModelMessage<T> 
        where T : SvgSerializable
    {
        /// <summary>
        /// Model to set.
        /// </summary>
        public T Model { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetModelMessage{T}"/> class.
        /// </summary>
        /// <param name="model">Model to set.</param>
        public SetModelMessage(T model)
        {
            Model = model;
        } 
    }
}
