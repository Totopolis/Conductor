namespace Conductor.Domain.Abstractions;

public interface INumberService
{
    /// <summary>
    /// Generate next number in concrete generator.
    /// </summary>
    /// <param name="generatorType">Each generator have itself sequence.</param>
    /// <returns>Next sequence number.</returns>
    Task<int> GenerateNext(GeneratorType generatorType);

    public enum GeneratorType
    {
        Process,
        Deployment,
        Global
    }
}
