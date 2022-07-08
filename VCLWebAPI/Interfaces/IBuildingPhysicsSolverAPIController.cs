namespace VCLWebAPI.Interfaces
{
    /// <summary>
    /// Defines the <see cref="IBuildingPhysicsSolverAPIController{T}" />.
    /// </summary>
    /// <typeparam name="T">.</typeparam>
    public interface IBuildingPhysicsSolverAPIController<T>
    {
        /// <summary>
        /// The Calculate.
        /// </summary>
        /// <param name="input">The input<see cref="T"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        string Calculate(T input);
    }
}